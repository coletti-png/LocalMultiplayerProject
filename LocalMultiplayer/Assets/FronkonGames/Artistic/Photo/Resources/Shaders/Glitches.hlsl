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
#pragma once

#pragma multi_compile _ GLITCH_CHROMATIC_FRINGING
#pragma multi_compile _ GLITCH_DUST
#pragma multi_compile _ GLITCH_LIGHT_LEAK
#pragma multi_compile _ GLITCH_COLOR_BLEED

#if GLITCH_CHROMATIC_FRINGING
float _ChromaticFringing;

half3 ApplyChromaticFringing(half3 color, float2 uv)
{
  float2 texelSize = TEXEL_SIZE.xy * _ChromaticFringing;
  float lumC = Luminance(color.rgb);
  float lumL = Luminance(SAMPLE_MAIN(uv - float2(texelSize.x, 0.0)).rgb);
  float lumR = Luminance(SAMPLE_MAIN(uv + float2(texelSize.x, 0.0)).rgb);
  float lumT = Luminance(SAMPLE_MAIN(uv - float2(0.0, texelSize.y)).rgb);
  float lumB = Luminance(SAMPLE_MAIN(uv + float2(0.0, texelSize.y)).rgb);
  
  float2 grad = float2(lumR - lumL, lumB - lumT);
  float gradMag = length(grad);
  
  if (gradMag > 0.1)
  {
    float2 dir = normalize(grad);
    float3 rSample = SAMPLE_MAIN(uv + dir * texelSize * 1.5).rgb;
    float3 bSample = SAMPLE_MAIN(uv - dir * texelSize * 1.5).rgb;
    
    float fringingStrength = min(gradMag * 2.0, 1.0) * _ChromaticFringing;
    color.r = lerp(color.r, rSample.r, fringingStrength * 0.5);
    color.b = lerp(color.b, bSample.b, fringingStrength * 0.5);
  }

  return color;
}
#endif

#if GLITCH_DUST
float _Dust;
float _DustSize;

half3 ApplyDust(half3 color, float2 uv)
{
  float2 dustUV = uv * _ScreenParams.xy / (_DustSize * 10.0);
  float noise1 = snoise(dustUV * 0.5);
  float noise2 = snoise(dustUV * 1.0);
  float noise3 = snoise(dustUV * 2.0);

  float dustPattern = saturate((noise1 * 0.5 + noise2 * 0.3 + noise3 * 0.2) * 0.5 + 0.5);
  dustPattern = pow(dustPattern, 10.0); // Sharpen dust spots
  

  float dustIntensity = dustPattern * _Dust;
  float3 dustColor = float3(0.8, 0.7, 0.6) * dustIntensity; // Yellowish dust color
  

  float2 largeUV = uv * _ScreenParams.xy / (_DustSize * 30.0);
  float largeNoise = snoise(largeUV);
  float largeSpot = pow(saturate(largeNoise * 0.5 + 0.5), 20.0) * _Dust * 2.0;


  color = lerp(color, color * (1.0 - dustIntensity * 0.5) + dustColor, dustIntensity);
  color = lerp(color, float3(1.0, 0.9, 0.8), largeSpot * 0.3);

  return color;
}
#endif

#if GLITCH_LIGHT_LEAK
float _LightLeak;
float _LightLeakSpeed;

half3 ApplyLightLeak(half3 color, float2 uv)
{
  float2 leakUV1 = uv * 0.8 + float2(_Time.y * _LightLeakSpeed * 0.01, 0.0);
  float2 leakUV2 = uv * 1.2 + float2(0.0, _Time.y * _LightLeakSpeed * 0.5);

  float leak1 = pow(max(0.0, sin(leakUV1.x * PI)), 4.0);
  float leak2 = pow(max(0.0, cos(leakUV2.y * PI * 0.5)), 4.0);
  float leak3 = pow(max(0.0, sin((uv.x + uv.y) * PI)), 8.0);

  float3 leakColor1 = float3(1.0, 0.6, 0.3) * leak1; // Orange/yellow leak
  float3 leakColor2 = float3(0.9, 0.2, 0.1) * leak2; // Red leak
  float3 leakColor3 = float3(0.7, 0.7, 1.0) * leak3; // Blue leak

  float3 finalLeak = leakColor1 + leakColor2 + leakColor3;
  float edgeMask = 1.0 - smoothstep(0.0, 0.5, distance(uv, float2(0.5, 0.5)));
  finalLeak *= edgeMask;

  return color + finalLeak * _LightLeak;  
}
#endif

#if GLITCH_COLOR_BLEED
float _ColorBleed;
float _ColorBleedAmount;

half3 ApplyColorBleed(half3 color, float2 uv)
{
  float2 bleedOffset = float2(TEXEL_SIZE.x * 2.0 * _ColorBleedAmount, 0.0);
  float3 bleedColor;
  bleedColor.r = SAMPLE_MAIN(uv + bleedOffset).r;
  bleedColor.g = color.g;
  bleedColor.b = SAMPLE_MAIN(uv - bleedOffset).b;

  return lerp(color, bleedColor, _ColorBleed);
}
#endif