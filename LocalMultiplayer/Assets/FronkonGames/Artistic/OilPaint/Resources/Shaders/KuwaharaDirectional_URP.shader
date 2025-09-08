////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Kuwahara Directional URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }
    LOD 100
    ZTest Always ZWrite Off Cull Off

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Kuwahara Directional"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ PROCESS_DEPTH
      #pragma multi_compile ___ VIEW_DEPTH
      #pragma multi_compile ___ DETAIL_PASS

      #include "Artistic.hlsl"
#if PROCESS_DEPTH
      #include "ProcessDepth.hlsl"
#endif

      int _Radius;

      half4 KuwaharaDirectional(float2 uv, float depth)
      {
#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(_Radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        _Radius = CalculateRadius(_Radius, depth);
#endif

        const float KernelX[9] = { -1.0, -2.0, -1.0,  0.0, 0.0, 0.0,  1.0, 2.0, 1.0 };
        const float KernelY[9] = { -1.0,  0.0,  1.0, -2.0, 0.0, 2.0, -1.0, 0.0, 1.0 };

        int i = 0;
        float Gx = 0.0;
        float Gy = 0.0;

        UNITY_LOOP
        for (int x = -1; x <= 1; ++x)
        {
          UNITY_LOOP
          for (int y = -1; y <= 1; ++y)
          {
            if (i == 4)
            {
              i++;
              continue;
            }

            float2 offset = float2(x, y) * TEXEL_SIZE.xy;
            half3 c = SAMPLE_MAIN_LOD(uv + offset).rgb;

            float l = dot(c, float3(0.2125, 0.7152, 0.0722));

            Gx += l * KernelX[i];
            Gy += l * KernelY[i];

            i++;
          }
        }

        float angle = 0.0;
        if (abs(Gx) > 0.001)
          angle = atan(Gy / Gx);

        float s = sin(angle);
        float c = cos(angle);

        float3 mean[4] = { (float3)0.0, (float3)0.0, (float3)0.0, (float3)0.0 };
        float3 sigma[4] = { (float3)0.0, (float3)0.0, (float3)0.0, (float3)0.0 };

        float2 offsets[4] =
        {
          float2(-_Radius, -_Radius),
          float2(-_Radius,  0.0),
          float2(0.0,      -_Radius),
          float2(0.0,       0.0)
        };

        UNITY_LOOP
        for (i = 0; i < 4; ++i)
        {
          UNITY_LOOP
          for (int j = 0; j <= _Radius; ++j)
          {
            UNITY_LOOP
            for (int k = 0; k <= _Radius; ++k)
            {
              float2 pos = float2(j, k) + offsets[i];
              float2 offs = pos * TEXEL_SIZE.xy;
              offs = float2(offs.x * c - offs.y * s, offs.x * s + offs.y * c);
              float2 uvpos = uv + offs;

              half3 c = saturate(SAMPLE_MAIN_LOD(uvpos).rgb);

              mean[i] += c;
              sigma[i] += c * c;
            }
          }
        }

        float n = SafePositivePow_float(float(_Radius + 1), 2.0);

        float sigma_f;
        float min = 1.0;

        half4 pixel = 0.0;
        UNITY_LOOP
        for (i = 0; i < 4; ++i)
        {
          mean[i] /= n;
          sigma[i] = abs(sigma[i] / n - mean[i] * mean[i]);
          sigma_f = sigma[i].b + sigma[i].y + sigma[i].z;
          
          UNITY_BRANCH
          if (sigma_f < min)
          {
            min = sigma_f;
            pixel = half4(mean[i], 1.0);
          }
        }

        return pixel;
      }

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = (half4)0.0;

#if PROCESS_DEPTH
        const float depth = SampleDepthCurve(SampleLinear01Depth(uv));

        UNITY_BRANCH
        if (_SampleSky == 1)
          pixel = KuwaharaDirectional(uv, depth);
        else
          pixel = depth < 0.98 ? KuwaharaDirectional(uv, depth) : color;
#else
        pixel = KuwaharaDirectional(uv, 0.0);
#endif

#ifndef DETAIL_PASS
        pixel.rgb = ColorAdjust(pixel.rgb);
#endif

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Detail"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ DETAIL_SHARPEN DETAIL_EMBOSS
      #pragma multi_compile ___ WATER_COLOR
      #pragma multi_compile ___ DETAIL_PASS

      #include "Artistic.hlsl"
#if PROCESS_DEPTH
      #include "ProcessDepth.hlsl"
#endif
#if DETAIL_SHARPEN || DETAIL_EMBOSS
      #include "Detail.hlsl"
#endif
#if WATER_COLOR
      #include "WaterColor.hlsl"
#endif

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

#if PROCESS_DEPTH
        const float depth = SampleDepthCurve(SampleLinear01Depth(uv));
#else
        const float depth = 0.0;
#endif

#ifdef DETAIL_SHARPEN
        pixel.rgb = Sharpen(uv, pixel.rgb, depth);
#elif DETAIL_EMBOSS
        pixel.rgb = Emboss(uv, pixel.rgb, depth);
#endif

#if WATER_COLOR
        pixel.rgb = WaterColor(color.rgb, pixel.rgb, uv);
#endif

#ifdef DETAIL_PASS
        pixel.rgb = ColorAdjust(pixel.rgb);
#endif

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }    
  }
  
  FallBack "Diffuse"
}
