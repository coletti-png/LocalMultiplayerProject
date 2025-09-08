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
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Symmetric Nearest Neighbor URP"
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
      Name "Fronkon Games Artistic Oil Paint: Symmetric Nearest Neighbor"

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

      inline float CalcDistance(half3 c1, half3 c2)
      {
        float3 c = c1 - c2;
        float y = c.r * 0.2124681075446384 + c.g * 0.4169973963260294 + c.b * 0.08137907133969426;
        float i = c.r * 0.3258860837850668 - c.g * 0.14992193838645426 - c.b * 0.17596414539861255;
        float q = c.r * 0.0935501584120867 - c.g * 0.23119531908149002 + c.b * 0.13764516066940333;

        return y * y + i * i + q * q;
      }

      half4 SymmetricNearestNeighbor(half3 pixel, float depth, float2 uv)
      {
        float4 sum = (float4)0.0;
        float2 invSize = 1.0 / _ScreenParams.xy;

        int radius = _Radius;
#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        radius = CalculateRadius(radius, depth);
#endif

        UNITY_LOOP
        for (int i = 0; i <= radius; ++i)
        {
          half3 c1 = SAMPLE_MAIN_LOD(uv + float2( i, 0.0) * invSize).rgb;
          half3 c2 = SAMPLE_MAIN_LOD(uv + float2(-i, 0.0) * invSize).rgb;

          float d1 = CalcDistance(c1, pixel);
          float d2 = CalcDistance(c2, pixel);

          sum.rgb += d1 < d2 ? c1 : c2;
          
          sum.a += 1.0;
        }

        UNITY_LOOP
        for (int j = 1; j <= radius; ++j)
        {
          UNITY_LOOP
          for (int i = -radius; i <= radius; ++i)
          {
            half3 c1 = SAMPLE_MAIN_LOD(uv + float2( i,  j) * invSize).rgb;
            half3 c2 = SAMPLE_MAIN_LOD(uv + float2(-i, -j) * invSize).rgb;

            float d1 = CalcDistance(c1, pixel);
            float d2 = CalcDistance(c2, pixel);

            sum.rgb += d1 < d2 ? c1 : c2;

            sum.a += 1.0;
          }
        }

        return half4(sum.rgb / sum.a, 1.0);
      }

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

#if PROCESS_DEPTH
        const float depth = SampleDepthCurve(SampleLinear01Depth(uv));

        UNITY_BRANCH
        if (_SampleSky == 1)
          pixel = SymmetricNearestNeighbor(pixel.rgb, depth, uv);
        else
          pixel = depth < 0.98 ? SymmetricNearestNeighbor(pixel.rgb, depth, uv) : color;
#else
        pixel = SymmetricNearestNeighbor(pixel.rgb, 0.0, uv);
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
      #pragma multi_compile ___ PROCESS_DEPTH
      #pragma multi_compile ___ VIEW_DEPTH
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
