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
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Kuwahara Generalized URP"
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
      Name "Fronkon Games Artistic Oil Paint: Kuwahara Generalized"

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
      float _Sharpness;
      float _Hardness;

      half4 KuwaharaGeneralized(float2 uv, float depth)
      {
#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(_Radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        _Radius = CalculateRadius(_Radius, depth);
#endif
        const float zeta = 2.0 / float(_Radius);

        float4 m[8];
        float3 s[8];
        for (int k = 0; k < 8; ++k)
        {
          m[k] = 0.0;
          s[k] = 0.0;
        }

        UNITY_LOOP
        for (int y = -_Radius; y <= _Radius; ++y)
        {
          UNITY_LOOP
          for (int x = -_Radius; x <= _Radius; ++x)
          {
            float2 v = float2(x, y) / float(_Radius);
            half3 c = SAMPLE_MAIN_LOD(uv + float2(x, y) * TEXEL_SIZE.xy).rgb;
            c = clamp(c, 0.0, 1.0);
            float sum = 0.0;
            float w[8];
            float z, vxx, vyy;

            vxx = vyy = zeta;
            z = max(0.0, v.y + vxx); 
            w[0] = z * z;
            sum += w[0];
            z = max(0.0, -v.x + vyy); 

            w[2] = z * z;
            sum += w[2];
            z = max(0.0, -v.y + vxx); 
            w[4] = z * z;
            sum += w[4];
            z = max(0.0, v.x + vyy); 
            w[6] = z * z;
            sum += w[6];
            v = 1.4142 / 2.0 * float2(v.x - v.y, v.x + v.y);
            vxx = vyy = zeta;
            z = max(0.0, v.y + vxx); 
            w[1] = z * z;
            sum += w[1];
            z = max(0.0, -v.x + vyy); 
            w[3] = z * z;
            sum += w[3];
            z = max(0.0, -v.y + vxx); 
            w[5] = z * z;
            sum += w[5];
            z = max(0.0, v.x + vyy); 
            w[7] = z * z;
            sum += w[7];

            const float g = exp(-3.125 * dot(v, v)) / sum;
            for (int k = 0; k < 8; ++k)
            {
              float wk = w[k] * g;
              m[k] += float4(c * wk, wk);
              s[k] += c * c * wk;
            }
          }
        }

        half4 pixel = 0.0;
        for (k = 0; k < 8; ++k)
        {
          m[k].rgb /= m[k].w;
          s[k] = abs(s[k] / m[k].w - m[k].rgb * m[k].rgb);

          float sigma2 = s[k].r + s[k].g + s[k].b;
          float w = 1.0 / (1.0 + SafePositivePow_float(_Hardness * 1000.0 * sigma2, 0.5 * _Sharpness));

          pixel += float4(m[k].rgb * w, w);
        }

        return clamp((pixel / pixel.w), 0.0, 1.0);
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
          pixel = KuwaharaGeneralized(uv, depth);
        else
          pixel = depth < 0.98 ? KuwaharaGeneralized(uv, depth) : color;
#else
        pixel = KuwaharaGeneralized(uv, 0.0);
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
