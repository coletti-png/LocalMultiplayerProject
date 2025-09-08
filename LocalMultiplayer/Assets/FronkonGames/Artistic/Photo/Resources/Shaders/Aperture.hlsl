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

#pragma multi_compile _ APERTURE

float _ApertureSize;
int _ApertureBlades;

inline float Saw(float t)
{
  const float clamping = 0.2;
  return clamp(abs(frac(t) * 2.0 - 1.0) * (1.0 + 2.0 * clamping) - clamping, 0.0, 1.0);
}

inline float2x2 Rotation(float angle)
{
  float cosAngle = cos(angle);
  float sinAngle = sin(angle);
  return float2x2(cosAngle, sinAngle, -sinAngle, cosAngle);
}

inline float SMin(float a, float b, float k)
{
  float h = clamp(0.5 + 0.5 * (a - b) / k, 0.0, 1.0);
  return lerp(a, b, h) - k * h * (1.0 - h);
}

half3 ApplyAperture(half3 color, float2 uv)
{
  float2 splitUV = uv * 2.0;
  splitUV.x = frac(splitUV.x);
  splitUV.y -= 0.5;
  
  float2 renderUV = splitUV;
  renderUV.x = frac(renderUV.x) * 2.0 - 1.0;
  renderUV.y -= 0.5;
  renderUV.y *= 2.0 * _ScreenParams.y / _ScreenParams.x;
  renderUV *= _ScreenParams.x / _ScreenParams.y;

  float mask = 0.0;
  for (int i = 0; i < _ApertureBlades; ++i)
  {
      const float pivotAngle = 2.0 * PI * float(i) / float(_ApertureBlades);
      const float2 pivotUV = mul(Rotation(pivotAngle), uv) - float2(1.05, 0.0);

      const float bladeAngle = PI / 3.0 * (1.05 - _ApertureSize);
      const float2 bladeUV = mul(pivotUV, Rotation(bladeAngle));

      const float d = length(bladeUV - float2(-1.05, 0.0));
      const float dd = fwidth(d);
      float blade = smoothstep(-dd, dd, d - 1.05);
      mask = max(mask, blade);
  }

  return 1.0 - mask;  
}