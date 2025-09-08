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
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Kuwahara Basic URP"
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
      Name "Fronkon Games Artistic Oil Paint: Kuwahara Basic"

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

      uint _Radius;

      float4 SampleQuadrant(float2 uv, int x1, int x2, int y1, int y2, float n)
      {
        float luminanceSum = 0.0;
        float luminanceSum2 = 0.0;
        float3 colSum = 0.0;

        UNITY_LOOP
        for (int x = x1; x <= x2; ++x)
        {
          UNITY_LOOP
          for (int y = y1; y <= y2; ++y)
          {
            half3 c = SAMPLE_MAIN_LOD(uv + float2(x, y) * TEXEL_SIZE.xy).rgb;
            float l = Luminance(c);
            luminanceSum += l;
            luminanceSum2 += l * l;
            colSum += saturate(c);
          }
        }

        float mean = luminanceSum / n;

        return float4(colSum / n, abs(luminanceSum2 / n - mean * mean));
      }

      half4 KuwaharaBasic(float2 uv, float depth)
      {
        int radius = _Radius / 2.0;
        half4 pixel = 0.0;

#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        radius = CalculateRadius(radius, depth);
#endif

        float windowSize = 2.0 * radius + 1;
        int quadrantSize = int(ceil(windowSize / 2.0));
        int numSamples = quadrantSize * quadrantSize;

        float4 q1 = SampleQuadrant(uv, -radius, 0, -radius, 0, numSamples);
        float4 q2 = SampleQuadrant(uv, 0, radius, -radius, 0, numSamples);
        float4 q3 = SampleQuadrant(uv, 0, radius, 0, radius, numSamples);
        float4 q4 = SampleQuadrant(uv, -radius, 0, 0, radius, numSamples);

        float minstd = min(q1.a, min(q2.a, min(q3.a, q4.a)));
        int4 q = float4(q1.a, q2.a, q3.a, q4.a) == minstd;

        UNITY_BRANCH
        if (dot(q, 1) > 1)
          pixel = saturate(half4((q1.rgb + q2.rgb + q3.rgb + q4.rgb) / 4.0, 1.0));
        else
          pixel = saturate(half4(q1.rgb * q.x + q2.rgb * q.y + q3.rgb * q.z + q4.rgb * q.w, 1.0));

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
          pixel = KuwaharaBasic(uv, depth);
        else
          pixel = depth < 0.98 ? KuwaharaBasic(uv, depth) : color;
#else
        pixel = KuwaharaBasic(uv, 0.0);
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
