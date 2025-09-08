// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
Shader "Hidden/Fronkon Games/Artistic/Shockwave URP"
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
      Name "Fronkon Games Artistic Shockwave Pass"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile __ NOISE_ON
      #pragma multi_compile __ CHROMATIC_ABERRATION_ON
      #pragma multi_compile __ FLARES_ON
      #pragma multi_compile __ EDGE_ON
      #pragma multi_compile __ EDGE_NOISE_ON
      #pragma multi_compile __ EDGE_PLASMA_ON
      #pragma multi_compile __ HUE_VARIATION_ON

      #include "Artistic.hlsl"
      #include "ColorBlend.hlsl"

      static const half3 LUMA = half3(0.299, 0.587, 0.114);

      float _Radius;
      float2 _Center;
      float _Strength;
      float _Width;
      float3 _ChromaticAberration;
      int _ColorBlend;
      int _ShockwaveColorBlend;
      float3 _ColorStrength;
      float _Flares;
      int _FlaresColorBlend;
      float3 _FlaresColor;
      float _FlaresFrequency;
      float _FlaresSpeed;
      float _FlaresThreshold;
      float _FlaresSoftness;
      float _Noise;
      float _NoiseScale;
      float _NoiseSpeed;
      float _Edge;
      int _EdgeColorBlend;
      float3 _EdgeColor;
      float _EdgeWidth;
      float _EdgeNoise;
      float _EdgeNoiseScale;
      float _EdgeNoiseSpeed;
      float _EdgePlasma;
      float _EdgePlasmaScale;
      float _EdgePlasmaSpeed;
      float _HueVar;
      float _HueVarSpeed;
      float _HueVarScale;
      float _HueVarRadial;
      float _HueVarRadialScale;
      float3 _InsideTint;
      float _RingWidthInner;
      float _RingWidthOuter;
      float _RingSharpness;
      float _RingSkew;

      half4 ArtisticFrag(ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

        float radius = sqrt(2.0) * _Radius;
        float aspect = _ScreenParams.x / _ScreenParams.y;
        float2 delta = _Center - uv;
        float circle = radius - length(delta * float2(aspect, 1.0));

        float tB = saturate(1.0 - abs(circle));
        float tB2 = tB * tB;
        float tB4 = tB2 * tB2;
        float tB8 = tB4 * tB4;
        float tB16 = tB8 * tB8;
        float baseFalloff = tB16 * tB4; // tB^20

        float useAsym = (_RingWidthInner > 0.0 || _RingWidthOuter > 0.0) ? 1.0 : 0.0;
        float wSide = (circle >= 0.0) ? _RingWidthInner : _RingWidthOuter;
        float wEff = lerp(_Width, wSide, useAsym);
        float circleSkew = circle - _RingSkew * wEff * ((circle >= 0.0) ? 1.0 : -1.0);
        float tW = saturate(1.0 - abs(circleSkew) / max(wEff, 1e-4));
        float widthFalloff = pow(tW, max(_RingSharpness, 1.0));
        float ring = (wEff > 0.0) ? widthFalloff : baseFalloff;
        float amplitude = _Strength * sin(_Radius * PI) * ring;

#if NOISE_ON
        float2 noiseUV = (_Center - uv) * float2(aspect, 1.0) * _NoiseScale + float2(_Time.y * _NoiseSpeed, 0.0);
        float noise = snoise(noiseUV) * 0.5 + 0.5;
        amplitude *= lerp(1.0 - _Noise, 1.0 + _Noise, noise);
#endif
        
        half3 outside = pixel.rgb;
        float2 dirN = normalize(delta * float2(aspect, 1.0));
        float2 dirUV = float2(dirN.x / aspect, dirN.y);
        float2 offset = amplitude * dirUV;

#if CHROMATIC_ABERRATION_ON
        outside.r = SAMPLE_MAIN(uv + (_ChromaticAberration.r * TEXEL_SIZE.xy) * offset).r;
        outside.g = SAMPLE_MAIN(uv + (_ChromaticAberration.g * TEXEL_SIZE.xy) * offset).g;
        outside.b = SAMPLE_MAIN(uv + (_ChromaticAberration.b * TEXEL_SIZE.xy) * offset).b;
#else
        outside = SAMPLE_MAIN(uv + TEXEL_SIZE.xy * offset * 100.0).rgb;
#endif

        half3 inside = ColorBlend(_ColorBlend, outside, ColorAdjust(color.rgb));

        inside *= _InsideTint;

#if EDGE_ON
        if (_Edge > 0.0)
        {
          float2 texel = TEXEL_SIZE.xy * max(_EdgeWidth, 0.1);

#if EDGE_NOISE_ON
          float2 eNoiseUV = (uv - _Center) * float2(aspect, 1.0) * _EdgeNoiseScale + float2(_Time.y * _EdgeNoiseSpeed, 0.0);
          float nA = snoise(eNoiseUV);
          float nB = snoise(eNoiseUV.yx + 37.0);
          float2 uvJitter = float2(nA, nB) * _EdgeNoise * texel;
#else
          float2 uvJitter = 0.0;
#endif

          float l00 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2(-1.0, -1.0)).rgb, LUMA);
          float l10 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2(-1.0,  0.0)).rgb, LUMA);
          float l20 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2(-1.0,  1.0)).rgb, LUMA);
          float l01 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 0.0, -1.0)).rgb, LUMA);
          float l11 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 0.0,  0.0)).rgb, LUMA);
          float l21 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 0.0,  1.0)).rgb, LUMA);
          float l02 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 1.0, -1.0)).rgb, LUMA);
          float l12 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 1.0,  0.0)).rgb, LUMA);
          float l22 = dot(SAMPLE_MAIN(uv + uvJitter + texel * float2( 1.0,  1.0)).rgb, LUMA);

          float gx = (-l00 - 2.0 * l10 - l20) + (l02 + 2.0 * l12 + l22);
          float gy = (-l00 - 2.0 * l01 - l02) + (l20 + 2.0 * l21 + l22);
          float edge = saturate(length(float2(gx, gy)));

          float interiorMask = saturate(smoothstep(0.0, max(_Width, 0.02), circle));
          half3 edgeBlend = ColorBlend(_EdgeColorBlend, color.rgb, _EdgeColor);

#if EDGE_PLASMA_ON
          if (_EdgePlasma > 0.0)
          {
            float t = _Time.y * _EdgePlasmaSpeed;
            float2 uvp = (uv - _Center) * float2(aspect, 1.0);
            uvp *= max(_EdgePlasmaScale, 0.01);

            float2 baseUV = uvp - t * 0.013;
            float fireA = snoise(baseUV);
            float fireB = snoise(baseUV * 2.1) * 0.6;
            float fireC = snoise(baseUV * 5.4) * 0.42;
            float q = (fireA + fireB + fireC) * 0.5;

            float r0A = snoise(uvp + q * 0.5 + t - uvp.x - uvp.y);
            float r0B = snoise((uvp + q * 0.5 + t - uvp.x - uvp.y) * 2.1) * 0.6;
            float r0C = snoise((uvp + q * 0.5 + t - uvp.x - uvp.y) * 5.4) * 0.42;
            float r1A = snoise(uvp + q - t);
            float r1B = snoise((uvp + q - t) * 2.1) * 0.6;
            float r1C = snoise((uvp + q - t) * 5.4) * 0.42;
            float r1 = r1A + r1B + r1C;
            
            float r1x2 = r1 * 2.0;
            float grad = r1x2 * r1x2;
            grad *= grad;
            half3 rampA = half3(1.0 - grad * 1.4, 0.2, 1.05) / max(grad, 1e-3);
            half3 rampB = half3(0.3 * (1.0 - grad) * 2.0, 0.2, 1.05) / max(grad, 1e-3);
            half3 plasma = (grad <= 0.5) ? rampA : rampB;
            plasma = plasma / (1.50 + max(half3(0.0, 0.0, 0.0), plasma));

            edgeBlend = lerp(edgeBlend, plasma, saturate(_EdgePlasma));
          }
#endif

#if HUE_VARIATION_ON
          if (_HueVar > 0.0 || _HueVarRadial > 0.0)
          {
            float2 p = (uv - _Center) * float2(aspect, 1.0);
            float theta = atan2(p.y, p.x);
            float r = length(p);
            float bandsAng = sin(theta * max(_HueVarScale * 0.1, 0.0) + _Time.y * _HueVarSpeed);
            float bandsRad = sin(r * max(_HueVarRadialScale, 0.0) + _Time.y * _HueVarSpeed);
            float hueDelta = 0.5 * (_HueVar * bandsAng + _HueVarRadial * bandsRad);
            float3 hsv = RgbToHsv(edgeBlend);
            hsv.x = frac(hsv.x + hueDelta);
            edgeBlend = HsvToRgb(hsv);
          }
  #endif
  
          inside = lerp(inside, edgeBlend, saturate(_Edge) * edge * interiorMask);
        }
#endif

        pixel.rgb = lerp(outside, inside, clamp(circle * 25.0, 0.0, 1.0));

        pixel.rgb = lerp(pixel.rgb, ColorBlend(_ShockwaveColorBlend, pixel.rgb, pixel.rgb * _ColorStrength), amplitude * amplitude);

#if FLARES_ON
        float2 deltaAspect2 = delta * float2(aspect, 1.0);
        float theta = atan2(deltaAspect2.y, deltaAspect2.x);
        float angle01 = frac((theta + PI) / (2.0 * PI));
        float timePhase = _Time.y * _FlaresSpeed;
        float nA = snoise(float2(angle01 * _FlaresFrequency, timePhase));
        float nB = snoise(float2(angle01 * (_FlaresFrequency * 2.7), timePhase * 1.7));
        float n = 0.7 * nA + 0.3 * nB;

        float w = (_Width > 0.0) ? _Width : 0.02;
        float ringMask = 1.0 - smoothstep(w, w * 2.0, abs(circle));
        float flaresMask = saturate((n - _FlaresThreshold) * _FlaresSoftness) * ringMask;
        pixel.rgb += _Flares * amplitude * flaresMask * ColorBlend(_FlaresColorBlend, half3(1.0, 0.95, 0.9), _FlaresColor);
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
