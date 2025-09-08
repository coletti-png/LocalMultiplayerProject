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
Shader "Hidden/Fronkon Games/Artistic/Radial Blur URP"
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
      Name "Fronkon Games Artistic Radial Blur Pass"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL

      #include "Artistic.hlsl"

      float2 _Center;
      int _Samples;
      float _Distance;
      float _Falloff;
      float3 _ChannelsOffset;
      float _Fisheye;
      float _GradientPower;
      float _GradientRangeMin;
      float _GradientRangeMax;

      float3 _InnerColor;
      float _InnerBrightness;
      float _InnerContrast;
      float _InnerGamma;
      float _InnerHue;
      float _InnerSaturation;

      float3 _OuterColor;
      float _OuterBrightness;
      float _OuterContrast;
      float _OuterGamma;
      float _OuterHue;
      float _OuterSaturation;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);

        half4 pixel = 0.0;

        float2 direction = normalize(uv - _Center);
        float2 velocity = direction * _Distance * SafePositivePow_float(length(uv - 0.5), _Falloff);
        float fSamples = float(_Samples);
        float invSamples = 1.0 / fSamples;

        float3x2 increments = float3x2(velocity * _ChannelsOffset.x * invSamples,
                                       velocity * _ChannelsOffset.y * invSamples,
                                       velocity * _ChannelsOffset.z * invSamples);

        float3x2 offsets = (float3x2)0.0;

        float2 m = float2(0.5, 0.5) + _Center * 0.2;
        float2 d = uv - m;
        float gradient = sqrt(dot(d, d));

        float prop = _ScreenParams.y / _ScreenParams.x;
        float bind;

        UNITY_BRANCH
        if (_Fisheye > 0.0)
          bind = sqrt(dot(m, m));
        else
          bind = prop < 1.0 ? m.x : m.y;

        UNITY_BRANCH
        if (_Fisheye > 0.0)
		      uv = m + normalize(d) * tan(gradient * _Fisheye) * bind / tan( bind * _Fisheye);
        else if (_Fisheye < 0.0)
		      uv = m + normalize(d) * atan(gradient * -_Fisheye * 10.0) * bind / atan(-_Fisheye * bind * 10.0);

        UNITY_LOOP
        for (int i = 0; i < _Samples; ++i)
        {
          pixel.r += SAMPLE_MAIN(uv + offsets[0]).r;
          pixel.g += SAMPLE_MAIN(uv + offsets[1]).g;
          pixel.b += SAMPLE_MAIN(uv + offsets[2]).b;

          offsets -= increments;
        }

        gradient = SafePositivePow_float(gradient, _GradientPower);
        gradient = RemapValue(gradient, 0.0, 1.0, _GradientRangeMin, _GradientRangeMax);

        half3 inner = lerp(pixel.rgb, pixel.rgb * _InnerColor, 1.0 - gradient);
        inner = ColorAdjust(inner, _InnerContrast, _InnerBrightness, _InnerHue, _InnerGamma, _InnerSaturation);
        
        half3 outer = lerp(pixel.rgb, pixel.rgb * _OuterColor, gradient);
        outer = ColorAdjust(outer, _OuterContrast, _OuterBrightness, _OuterHue, _OuterGamma, _OuterSaturation);

        pixel.rgb = lerp(inner, outer, gradient);

        pixel.rgb /= fSamples;

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
