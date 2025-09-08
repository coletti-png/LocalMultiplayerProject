// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Hidden/Fronkon Games/Artistic/Sharpen URP"
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
      Name "Fronkon Games Artistic Color Sharpen Luma"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ LUMA_FAST LUMA_NORMAL LUMA_WIDER LUMA_PYRAMID
      #pragma multi_compile ___ DEBUG_VIEW

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _SharpClamp;
      float _OffsetBias;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float3 luma = float3(0.2126, 0.7152, 0.0722) * _Sharpness;
#if LUMA_FAST
        pixel.rgb = SAMPLE_MAIN(uv + (TEXEL_SIZE.xy / 3.0) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + (-TEXEL_SIZE.xy / 3.0) * _OffsetBias).rgb;

        pixel.rgb *= 0.5;
        luma *= 1.5;
#elif LUMA_NORMAL
        pixel.rgb  = SAMPLE_MAIN(uv + float2(TEXEL_SIZE.x, -TEXEL_SIZE.y) * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - TEXEL_SIZE.xy * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + TEXEL_SIZE.xy * 0.5 * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - float2(TEXEL_SIZE.x, -TEXEL_SIZE.y) * 0.5 * _OffsetBias).rgb;

        pixel.rgb *= 0.25;
#elif LUMA_WIDER
        pixel.rgb  = SAMPLE_MAIN(uv + TEXEL_SIZE.xy * float2(0.4, -1.2) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - TEXEL_SIZE.xy * float2(1.2, 0.4) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + TEXEL_SIZE.xy * float2(1.2, 0.4) * _OffsetBias).rgb;
        pixel.rgb += SAMPLE_MAIN(uv - TEXEL_SIZE.xy * float2(0.4, -1.2) * _OffsetBias).rgb;

        pixel.rgb *= 0.25;
        luma *= 0.51;
#else
        pixel.rgb  = SAMPLE_MAIN(uv + float2(0.5 * TEXEL_SIZE.x, -TEXEL_SIZE.y * _OffsetBias)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(_OffsetBias * -TEXEL_SIZE.x, 0.5 * -TEXEL_SIZE.y)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(_OffsetBias * TEXEL_SIZE.x, 0.5 * TEXEL_SIZE.y)).rgb;
        pixel.rgb += SAMPLE_MAIN(uv + float2(0.5 * -TEXEL_SIZE.x, TEXEL_SIZE.y * _OffsetBias)).rgb;

        pixel.rgb *= 0.25;
        luma *= 0.666;
#endif
        half3 sharp = color.rgb - pixel.rgb;

        float4 sharpClamp = float4(luma * (0.5 / _SharpClamp), 0.5);

        float sharpLuma = saturate(dot(float4(sharp, 1.0), sharpClamp));
        sharpLuma = (_SharpClamp * 2.0) * sharpLuma - _SharpClamp;

        pixel.rgb += sharpLuma;
        pixel.rgb = saturate(pixel.rgb);

#if DEBUG_VIEW
        return color * saturate(sharpLuma * 5.0);
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Laplacian"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ LAPLACIAN_3X3 LAPLACIAN_5X5 LAPLACIAN_7X7
      #pragma multi_compile ___ DEBUG_VIEW

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _OffsetBias;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

#if LAPLACIAN_3X3
        // 3x3 Laplacian kernel
        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float3 laplacian = -4.0 * color.rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb;
#elif LAPLACIAN_5X5
        // 5x5 Laplacian kernel
        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float3 laplacian = -20.0 * color.rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(2.0 * off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-2.0 * off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, 2.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -2.0 * off.y)).rgb;
        laplacian *= 0.05; // Normalize
#else
        // 7x7 Laplacian kernel
        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float3 laplacian = -48.0 * color.rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb * 6.0;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb * 6.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb * 6.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb * 6.0;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(2.0 * off.x, 0.0)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(-2.0 * off.x, 0.0)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, 2.0 * off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -2.0 * off.y)).rgb * 4.0;
        laplacian += SAMPLE_MAIN(uv + float2(2.0 * off.x, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-2.0 * off.x, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(2.0 * off.x, -off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-2.0 * off.x, -off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, 2.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, 2.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, -2.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, -2.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(3.0 * off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-3.0 * off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, 3.0 * off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -3.0 * off.y)).rgb;
        laplacian *= 0.020833; // Normalize
#endif

        pixel.rgb = color.rgb - laplacian * _Sharpness * 0.1;
        pixel.rgb = saturate(pixel.rgb);

#if DEBUG_VIEW
        return color * saturate(abs(laplacian) * 5.0);
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Edge Aware"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ EDGE_SOBEL EDGE_CANNY EDGE_LAPLACIAN
      #pragma multi_compile ___ DEBUG_VIEW

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _OffsetBias;
      float _EdgeThreshold;
      float _EdgeWidth;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float edgeStrength = 0.0;

#if EDGE_SOBEL
        // Sobel edge detection
        float3 gx = SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb + SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb;
        gx -= SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb + SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb;
        
        float3 gy = SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb + SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb;
        gy -= SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb + SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb;
        
        edgeStrength = length(gx) + length(gy);
#elif EDGE_CANNY
        // Simplified Canny edge detection
        float3 gx = SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb + SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb;
        gx -= SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb + SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb;
        
        float3 gy = SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb + SAMPLE_MAIN(uv + float2(off.x, off.y)).rgb;
        gy -= SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb + 2.0 * SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb + SAMPLE_MAIN(uv + float2(off.x, -off.y)).rgb;
        
        float magnitude = length(gx) + length(gy);
        float direction = atan2(length(gy), length(gx));
        
        // Non-maximum suppression approximation
        edgeStrength = magnitude * smoothstep(_EdgeThreshold, _EdgeThreshold + 0.1, magnitude);
#else
        // Laplacian edge detection
        float3 laplacian = -4.0 * color.rgb;
        laplacian += SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb;
        laplacian += SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb;
        
        edgeStrength = length(laplacian);
#endif

        // Apply sharpening only where edges are detected
        float sharpeningFactor = smoothstep(_EdgeThreshold, _EdgeThreshold + _EdgeWidth, edgeStrength);
        
        // Simple unsharp mask for edge areas
        float3 blur = SAMPLE_MAIN(uv + float2(off.x, 0.0)).rgb;
        blur += SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb;
        blur += SAMPLE_MAIN(uv + float2(0.0, off.y)).rgb;
        blur += SAMPLE_MAIN(uv + float2(0.0, -off.y)).rgb;
        blur *= 0.25;
        
        float3 sharp = color.rgb - blur;
        pixel.rgb = color.rgb + sharp * _Sharpness * sharpeningFactor;
        pixel.rgb = saturate(pixel.rgb);

#if DEBUG_VIEW
        return color * sharpeningFactor;
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Bilateral"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ DEBUG_VIEW

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _OffsetBias;
      float _SpatialSigma;
      float _RangeSigma;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float3 bilateralSum = 0.0;
        float weightSum = 0.0;
        
        // Bilateral filtering with sharpening
        for (int i = -2; i <= 2; i++)
        {
          for (int j = -2; j <= 2; j++)
          {
            float2 sampleUV = uv + float2(i, j) * off;
            float3 sampleColor = SAMPLE_MAIN(sampleUV).rgb;
            
            // Spatial weight (Gaussian)
            float spatialDist = length(float2(i, j));
            float spatialWeight = exp(-spatialDist * spatialDist / (2.0 * _SpatialSigma * _SpatialSigma));
            
            // Range weight (color similarity)
            float colorDiff = length(color.rgb - sampleColor);
            float rangeWeight = exp(-colorDiff * colorDiff / (2.0 * _RangeSigma * _RangeSigma));
            
            float weight = spatialWeight * rangeWeight;
            bilateralSum += sampleColor * weight;
            weightSum += weight;
          }
        }
        
        float3 bilateralBlur = bilateralSum / weightSum;
        float3 sharp = color.rgb - bilateralBlur;
        
        pixel.rgb = color.rgb + sharp * _Sharpness * 0.5;
        pixel.rgb = saturate(pixel.rgb);

#if DEBUG_VIEW
        return color * saturate(abs(sharp) * 5.0);
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Color Sharpen Contrast Adaptive"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ DEBUG_VIEW

      #include "Artistic.hlsl"
      #include "Vibrance.hlsl"

      float _Sharpness;
      float _OffsetBias;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float2 off = TEXEL_SIZE.xy * _OffsetBias;
        float3 a = SAMPLE_MAIN(uv + float2(-off.x, -off.y)).rgb;
        float3 b = SAMPLE_MAIN(uv + float2(0.0,    -off.y)).rgb;
        float3 c = SAMPLE_MAIN(uv + float2(off.x,  -off.y)).rgb;
        float3 d = SAMPLE_MAIN(uv + float2(-off.x, 0.0)).rgb;
        float3 f = SAMPLE_MAIN(uv + float2(off.x,  0.0)).rgb;
        float3 g = SAMPLE_MAIN(uv + float2(-off.x, off.y)).rgb;
        float3 h = SAMPLE_MAIN(uv + float2(0.0,    off.y)).rgb;
        float3 i = SAMPLE_MAIN(uv + float2(off.x,  off.y)).rgb;

        float3 mnRGB = min(min(min(d, color.rgb), min(f, b)), h);
        float3 mnRGB2 = min(mnRGB, min(min(a, c), min(g, i)));
        mnRGB += mnRGB2;

        float3 mxRGB = max(max(max(d, color.rgb), max(f, b)), h);
        float3 mxRGB2 = max(mxRGB, max(max(a, c), max(g, i)));
        mxRGB += mxRGB2;

        float3 rcpMRGB = rcp(mxRGB);
        float3 ampRGB = saturate(min(mnRGB, 2.0 - mxRGB) * rcpMRGB);    

        ampRGB = rsqrt(ampRGB);

        float peak = -3.0 * _Sharpness + 8.0;
        float3 wRGB = -rcp(ampRGB * peak);

        float3 rcpWeightRGB = rcp(4.0 * wRGB + 1.0);

        float3 window = (b + d) + (f + h);
        pixel.rgb = saturate((window * wRGB + color.rgb) * rcpWeightRGB);

#if DEBUG_VIEW
        return color * sqrt((color.r - pixel.r) + (color.g - pixel.g) + (color.b - pixel.b));
#endif
        pixel.rgb = Vibrance(pixel.rgb);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

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
