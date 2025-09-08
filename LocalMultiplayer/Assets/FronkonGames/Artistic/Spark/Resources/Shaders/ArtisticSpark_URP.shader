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
Shader "Hidden/Fronkon Games/Artistic/Spark URP"
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
      Name "Fronkon Games Artistic Spark Pass 0"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"

      float _Rays;
      float _Gain;
      float _Size;
      float _Threshold;
      float _ThresholdClamp;
      float4 _Tint;
      float _Spin;
      float _Twirl;
      float _Barrel;
      float _BarrelBend;
      float _Artifacts;
      float _Falloff;
      float _Dispersion;
      float _DispersionCycles;
      float _DispersionOffset;
      float _Dirt;
      float _DirtFreq;
      float _Aspect;

      inline float3 ToYUV(float3 rgb) { return mul(float3x3(0.2215, -0.1145, 0.5016, 0.7154, -0.3855, -0.4556, 0.0721, 0.5, -0.0459), rgb); }
      inline float3 ToRGB(float3 yuv) { return mul(float3x3(1.0, 1.0, 1.0, 0.0, -0.1870, 1.8556, 1.5701, -0.4664, 0.0), yuv); }

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        half4 pixel = 0.0;

        float angle;
        float2 offset;
        float samples = _Size * _Artifacts;
        float3 sparks = 0.0;

        UNITY_LOOP
        for (float ray = 0.0; ray < _Rays; ray++)
        {
          angle = ray * PI2 / _Rays;
          angle -= _Spin / 360.0 * PI2;

          UNITY_LOOP
          for (float i = _Size / samples; i < _Size; i += _Size / samples)
          {
            angle -= (_Twirl / samples * i / _Size) / 360.0 * PI2;

            offset = i / _ScreenParams.xy * float2(cos(angle), sin(angle));
            offset.x *= _Aspect;
            offset -= SafePositivePow_float(i / _Size, _BarrelBend) * 0.1 * _Barrel * (-uv + float2(0.5, 0.5));

            sparks = SAMPLE_MAIN_LOD(uv + offset).rgb;
            sparks = min(sparks, _ThresholdClamp);
            sparks *= max(sparks - _Threshold, 0.0);
            sparks *= max(0.0, _Falloff > 1.0 ? lerp(1.0, -_Falloff + 2.0, i / _Size) : lerp(_Falloff, 1.0, i / _Size));

            float3 sampley = ToYUV(sparks);
            sampley.gb *= _Tint.a;
            
            float hue = _DispersionCycles * PI2 * -i / _Size;
            hue -= _DispersionOffset / 360.0 * PI2;
            
            float2 rainbow = float2(cos(hue), sin(hue)) * sampley.r;
            sampley.gb = lerp(sampley.gb, rainbow, _Dispersion * i / _Size);
            sparks = ToRGB(sampley);
            
            float noise = Rand(float2(42.1, 12.4) + 0.01 * (float2)(_DirtFreq / 100.0) * offset) - 0.5;
            noise += Rand(float2(4.1, 1.4) + 0.01 * (float2)(_DirtFreq / 1000.0) * offset) - 0.5;
            noise += Rand(float2(2.1, 2.4) + 0.01 * (float2)(_DirtFreq / 10000.0) * offset) - 0.5;
            sparks *= lerp(1.0, clamp(10.0 * noise, 0.0, 99.0), _Dirt);

            pixel.rgb += sparks;
          }
        }

        pixel.rgb /= floor(_Rays) * samples;
        pixel.rgb *= _Gain;

        float3 glinty = ToYUV(pixel.rgb);
        float3 tinty = ToYUV(_Tint.rgb);
        tinty.gb *= glinty.r;
        glinty.gb = lerp(glinty.gb, tinty.gb, 4.0 * length(tinty.gb));
        pixel.rgb *= ToRGB(glinty);

        return pixel;
      }

      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Spark Pass 1"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"
      #include "GaussianBlur.hlsl"

      float _Blur;
      float3 _BlurWeights;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        return GaussianBlur(uv * _ScreenParams.xy,
                            _ScreenParams.xy,
                            _Blur * _BlurWeights.r,
                            _Blur * _BlurWeights.g,
                            _Blur * _BlurWeights.b, 0.0, float2(1.0, 0.0));
      }

      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Spark Pass 2"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"
      #include "GaussianBlur.hlsl"

      TEXTURE2D(_SourceTex);
      int _Blend;

      float _Blur;
      float3 _BlurWeights;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const half4 color = SAMPLE_TEXTURE2D_X(_SourceTex, sampler_LinearClamp, uv);
        half4 pixel = SAMPLE_MAIN(uv);

        pixel = GaussianBlur(uv * _ScreenParams.xy,
                             _ScreenParams.xy,
                             _Blur * _BlurWeights.r,
                             _Blur * _BlurWeights.g,
                             _Blur * _BlurWeights.b, 0.0, float2(0.0, 1.0));

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        pixel.rgb = ColorBlend(_Blend, color.rgb, pixel.rgb);

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
