// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Hidden/Fronkon Games/Artistic/Photo URP"
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
      Name "Fronkon Games Artistic Photo (Vertical Blur)"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"

      int _Blur;
      float _Focus;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = 0.0;

        int samples = 2 * _Blur + 1;
        samples = lerp(1, samples, 3.4 * sign(_Focus) * _Focus);

        UNITY_LOOP
        for (float y = -samples * 0.5; y < samples * 0.5; y++)
          pixel.rgb += SAMPLE_MAIN_LOD(uv + float2(0.0, y * _Blur) * TEXEL_SIZE.xy).rgb;
        pixel.rgb /= samples;
        
        return pixel;
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Photo (Horizontal Blur)"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"

      int _Blur;
      float _Focus;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = 0.0;

        int samples = 2 * _Blur + 1;
        samples = lerp(1, samples, 3.4 * sign(_Focus) * _Focus);

        UNITY_LOOP
        for (float x = -samples * 0.5; x < samples * 0.5; x++)
          pixel.rgb += SAMPLE_MAIN_LOD(uv + float2(x * _Blur, 0.0) * TEXEL_SIZE.xy).rgb;
        pixel.rgb /= samples;

        return pixel;
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Photo"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"
      #include_with_pragmas "Films.hlsl"
      #include_with_pragmas "Glitches.hlsl"
      #include_with_pragmas "Aperture.hlsl"
      #include_with_pragmas "Vignette.hlsl"

      TEXTURE2D_X(_BlurTex);

      float2 _Center;
      float _Focus;
      float _FocusOffset;
      float _Grid;
      half4 _GridColor;
      int _GridColorBlend;
      float _Rings;
      half4 _RingsColor;
      int _RingsColorBlend;
      float _RingsThickness;
      float _RingsSharpness;
      float _Ring0Scale;
      float _Ring1Scale;
      float _Ring2Scale;
      float _RingSplitScale;
      float _Frost;
      half4 _FrostColor;
      int _FrostColorBlend;
      float _Aberration;
      float3 _AberrationChannels;
      float _Vignette;
      float _VignetteSharpness;
      float _Grain;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        const float2 aspect = float2(_ScreenParams.x / _ScreenParams.y, 1.0);
        const float2 center = aspect * _Center;

        float2 delta = center - (aspect * uv);
        float dist = length(delta);
        float2 middle = floor(-delta.yx + float2(1.0, 1.0));

        #define CIRCLE(size, dist, sharpness) pow(clamp((size) * (dist), 0.0, 1.0), sharpness)

        float blackRings = CIRCLE(4.0, dist * _Ring2Scale, _RingsSharpness);
        blackRings += 0.9 - 0.9 * CIRCLE(4.02 + _RingsThickness, dist * _Ring2Scale, _RingsSharpness * 2.0);
        
        blackRings *= CIRCLE(8.0, dist * _Ring1Scale, _RingsSharpness * 2.0);
        blackRings += 1.0 - CIRCLE(8.01 + _RingsThickness, dist * _Ring1Scale, _RingsSharpness * 2.0);

        blackRings *= CIRCLE(9.0, dist * _Ring0Scale, _RingsSharpness * 2.0);
        blackRings += 1.0 - CIRCLE(9.03 + _RingsThickness, dist * _Ring0Scale, _RingsSharpness * 2.0);

        blackRings = pow(abs(blackRings), 10.0 * _Rings);

        float frostRing;
        frostRing = CIRCLE(18.0, dist * _RingSplitScale, _RingsSharpness * 4.0);
        frostRing *= 1.0 - CIRCLE(9.00 + _RingsThickness, dist * _Ring0Scale, _RingsSharpness * 4.0);
        frostRing *= _Frost * abs(_Focus * 1.25);

        float2 grid = mod(delta * 10.0, 1.0);
        grid = clamp(grid, 0.0, 1.0);
        grid.x = pow(grid.x, 25.0);
        grid.y = pow(grid.y, 25.0);
        grid.x *= grid.y * 1.0;
        grid.x = pow(1.0 - grid.x, 2.5 * _Grid);

        float2 pattern = delta * 45;
        pattern = mod(pattern, 1.0);	
        pattern = clamp(pattern, 0.0, 1.0);
        frostRing *= pattern.x * pattern.y;
        
        float splitRing = (1.0 - CIRCLE(18.1, dist * _RingSplitScale, 256.0));
        frostRing += splitRing;
        splitRing *= lerp(_Focus, -_Focus, middle.x);
        float splitOffset = splitRing;

        float vignette = ApplyVignette(uv);
        float2 shift = -(1.0 - vignette) * delta;
        float3 aberration = TEXEL_SIZE.x * _AberrationChannels * _Aberration;
        
        uv.x += splitOffset * _FocusOffset * TEXEL_SIZE.x;
        uv += TEXEL_SIZE.xy * _Focus * frostRing * 10.0;

        half3 blurColor = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_LinearClamp, uv + shift * aberration.x).rgb;
        blurColor.g = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_LinearClamp, uv + shift * aberration.y).g;
        blurColor.b = SAMPLE_TEXTURE2D_X(_BlurTex, sampler_LinearClamp, uv + shift * aberration.z).b;

        half3 sharpColor = SAMPLE_MAIN(uv + shift * aberration.x).rgb;

        pixel.rgb = lerp(blurColor, lerp(sharpColor, ColorBlend(_FrostColorBlend, sharpColor, sharpColor * _FrostColor.rgb), frostRing), frostRing * _FrostColor.a);
        pixel.rgb = lerp(pixel.rgb, ColorBlend(_GridColorBlend, pixel.rgb, _GridColor.rgb), (1.0 - grid.x) * _GridColor.a);
        pixel.rgb = lerp(pixel.rgb, ColorBlend(_RingsColorBlend, pixel.rgb, _RingsColor.rgb), (1.0 - blackRings) * _RingsColor.a);
        pixel.rgb = ApplyFilm(pixel.rgb, uv);
        pixel.rgb += ApplyGrain(uv) * _Grain * 0.01;

#if GLITCH_CHROMATIC_FRINGING
        pixel.rgb = ApplyChromaticFringing(pixel.rgb, uv);
#endif

#if GLITCH_COLOR_BLEED
        pixel.rgb = ApplyColorBleed(pixel.rgb, uv);
#endif

#if GLITCH_DUST
        pixel.rgb = ApplyDust(pixel.rgb, uv);
#endif

#if GLITCH_LIGHT_LEAK
        pixel.rgb = ApplyLightLeak(pixel.rgb, uv);
#endif
        pixel.rgb *= vignette;

#if APERTURE
        pixel.rgb *= ApplyAperture(pixel.rgb, delta);
#endif
        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);
        
        return pixel;
      }
      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
