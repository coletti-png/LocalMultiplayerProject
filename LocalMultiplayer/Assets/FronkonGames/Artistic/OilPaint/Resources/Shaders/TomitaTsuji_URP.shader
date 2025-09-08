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
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Tomita-Tsuji URP"
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
      Name "Fronkon Games Artistic Oil Paint: Tomita-Tsuji"

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

      half4 TomitaTsuji(float2 uv, float depth)
      {
        float3 m[5] =
        {
          { 0.0, 0.0, 0.0 },
          { 0.0, 0.0, 0.0 },
          { 0.0, 0.0, 0.0 },
          { 0.0, 0.0, 0.0 },
          { 0.0, 0.0, 0.0 }
        };
        float3 s[5] = m;

        half3 c;
        int u, v, i, j;

        int radius = _Radius;
#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        radius = CalculateRadius(radius, depth);
#endif

        UNITY_LOOP
        for (j = -radius; j <= 0; ++j)
          for (i = -radius; i <= 0; ++i)
          {
            c = SAMPLE_MAIN_LOD(uv + float2(i, j) * TEXEL_SIZE.xy).rgb;
            m[0] += c;
            s[0] += c * c;
          }

        UNITY_LOOP
        for (j = -radius; j <= 0; ++j)
          for (i = 0; i <= radius; ++i)
          {
            c = SAMPLE_MAIN_LOD(uv + float2(i, j) * TEXEL_SIZE.xy).rgb;
            m[1] += c;
            s[1] += c * c;
          }

        UNITY_LOOP
        for (j = 0; j <= radius; ++j)
          for (i = 0; i <= radius; ++i)
          {
            c = SAMPLE_MAIN_LOD(uv + half2(i, j) * TEXEL_SIZE.xy).rgb;
            m[2] += c;
            s[2] += c * c;
          }

        UNITY_LOOP
        for (j = 0; j <= radius; ++j)
          for (i = -radius; i <= 0; ++i)
          {
            c = SAMPLE_MAIN_LOD(uv + float2(i, j) * TEXEL_SIZE.xy).rgb;
            m[3] += c;
            s[3] += c * c;
          }

        half radiusTT = radius / 2.0;
        UNITY_LOOP
        for (j = -radiusTT; j <= radiusTT; ++j)
          for (i = -radiusTT; i <= radiusTT; ++i)
          {
            c = saturate(SAMPLE_MAIN_LOD(uv + float2(i, j) * TEXEL_SIZE.xy).rgb);
            m[4] += c;
            s[4] += c * c;
          }

        half3 pixel = half3(0.0, 0.0, 0.0);
        half minSigma2 = 1e+2;

        half n = (half)radius + 1;
        n *= n;

        m[0] /= n;
        s[0] = abs(s[0] / n - m[0] * m[0]);

        half sigma2 = s[0].r + s[0].g + s[0].b;

        UNITY_BRANCH
        if (sigma2 < minSigma2)
        {
          minSigma2 = sigma2;
          pixel = m[0];
        }

        m[1] /= n;
        s[1] = abs(s[1] / n - m[1] * m[1]);

        sigma2 = s[1].r + s[1].g + s[1].b;
        UNITY_BRANCH
        if (sigma2 < minSigma2)
        {
          minSigma2 = sigma2;
          pixel = m[1];
        }

        m[2] /= n;
        s[2] = abs(s[2] / n - m[2] * m[2]);

        sigma2 = s[2].r + s[2].g + s[2].b;
        UNITY_BRANCH
        if (sigma2 < minSigma2)
        {
          minSigma2 = sigma2;
          pixel = m[2];
        }

        m[3] /= n;
        s[3] = abs(s[3] / n - m[3] * m[3]);

        sigma2 = s[3].r + s[3].g + s[3].b;
        UNITY_BRANCH
        if (sigma2 < minSigma2)
        {
          minSigma2 = sigma2;
          pixel = m[3];
        }
        
        m[4] /= n;
        s[4] = abs(s[4] / n - m[4] * m[4]);

        sigma2 = s[4].r + s[4].g + s[4].b;
        UNITY_BRANCH
        if (sigma2 < minSigma2)
        {
          minSigma2 = sigma2;
          pixel = m[4];
        }

        return half4(pixel, 1.0);
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
          pixel = TomitaTsuji(uv, depth);
        else
          pixel = depth < 0.98 ? TomitaTsuji(uv, depth) : color;
#else
        pixel = TomitaTsuji(uv, 0.0);
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
