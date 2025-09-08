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

#pragma multi_compile ___ FILM_AFGA_VISTA_400       \
                          FILM_POLAROID_600         \
                          FILM_KODAK_GOLD_200       \
                          FILM_KODAK_PORTRA_400     \
                          FILM_KODAK_EKTAR_100      \
                          FILM_FUJI_C200            \
                          FILM_FUJI_VELVIA_50       \
                          FILM_FUJI_PRO_400H        \
                          FILM_CINESTILL_800T       \
                          FILM_LOMOGRAPHY_COLOR_800 \
                          FILM_ORWO_UT18            \
                          FILM_ILFORD_HP5_BW

float _Halation;
float _ExpiredYears;

half3 ExpiredFilm(half3 color, float expirationYears)
{
  float shift = expirationYears * 0.01;
  return half3(color.r * (0.9 + shift),
               color.g * (0.8 - shift),
               color.b * (1.0 + shift)) + half3(shift * 0.2, -shift * 0.1, shift * 0.3);
}

// Film stock color matrices
half3 ApplyFilm(half3 color, float2 uv)
{
#if FILM_AFGA_VISTA_400
  color = mul(float3x3(0.95,  0.15, -0.10,
                      -0.05,  0.90,  0.15,
                       0.10, -0.05,  1.00), color);
  color.b += 0.03;              // Cool blue bias
  color.g = pow(abs(color.g), 0.95); // Flat greens
#elif FILM_POLAROID_600
  color = mul(float3x3(1.30, -0.08, -0.12,
                      -0.15,  1.20, -0.10,
                       0.05, -0.20,  1.10), color);

  // Polaroid's flat contrast
  color = pow(color, 1.1);
  color *= 1.05;
#elif FILM_KODAK_GOLD_200
  color = mul(float3x3(1.15, 0.05, -0.10,
                      -0.05, 1.10, -0.05,
                      -0.10, 0.00,  0.95), color);
  color.g *= 1.05; // Boost greens
#elif FILM_KODAK_PORTRA_400
  color = mul(float3x3(1.05, 0.02, -0.07,
                      -0.10, 1.15, -0.05,
                      -0.03, -0.02, 0.98), color);
  color = lerp(color, pow(color, 0.9), 0.7); // Soft highlight rolloff
  color.g *= 1.02;                           // Enhanced greens for skin tones  
#elif FILM_KODAK_EKTAR_100
  color = mul(float3x3(1.20, -0.10, -0.10,
                      -0.15,  1.10, -0.05,
                      -0.05,  0.00,  0.95), color);
  color = pow(color, 1.1); // High contrast
  color.r *= 1.05;         // Red boost
  color.b *= 0.97;         // Blue reduction
#elif FILM_FUJI_C200
  color = mul(float3x3(0.90, 0.10, 0.05,
                       0.00, 1.05, 0.00,
                      -0.05, 0.00, 1.10), color);
#elif FILM_FUJI_VELVIA_50
  color = mul(float3x3(0.85,  0.25, 0.10,
                       0.00,  1.10, 0.00,
                       0.15, -0.20, 1.05), color);
  color = pow(color, 1.2); // Punchy contrast
  color.rb *= 1.05;        // Boost red/blue
#elif FILM_FUJI_PRO_400H
  color = mul(float3x3(0.90,  0.10,  0.00,
                      -0.05,  1.05, -0.05,
                       0.05, -0.05,  1.05), color);
#elif FILM_LOMOGRAPHY_COLOR_800
  color = mul(float3x3(1.40, -0.20, -0.20,
                      -0.30,  1.30, -0.20,
                      -0.10, -0.10,  1.20), color);
  color = saturate(color * 1.2 - 0.02); // Crushed shadows
  color.rb += 0.025;                    // Cyan shift
#elif FILM_ORWO_UT18
  color = mul(float3x3(0.80,  0.20, 0.00,
                       0.10,  0.90, 0.00,
                       0.10, -0.10, 1.00), color);
  color.g *= 0.9;              // Green reduction
  color.r = pow(color.r, 0.8); // Film-like shoulder
#elif FILM_CINESTILL_800T
  color = mul(float3x3(1.10, 0.20, -0.30,
                       0.00, 0.95,  0.00,
                       0.20, -0.15, 1.20), color);
  // Halation bleed
  float2 redOffset = float2(0.02 * _Halation, 0.0);
  float redBlur = SAMPLE_MAIN(uv + redOffset).r;
  color = lerp(color, float3(redBlur, 0.0, 0.0), _Halation * 0.5);
#elif FILM_ILFORD_HP5_BW
  color = mul(float3x3(0.30, 0.59, 0.11,
                       0.30, 0.59, 0.11,
                       0.30, 0.59, 0.11), color);
#endif

  if (_ExpiredYears > 0.0)
    color.rgb = ExpiredFilm(color.rgb, _ExpiredYears);

  return color;
}

float ApplyGrain(float2 uv)
{
  float grain = 0.0;
  
  #if FILM_KODAK_PORTRA_400
  float speed = _Time.y * 10.0;
  grain = frac(sin(dot(uv, float2(12.9898, 78.233) + speed)) * 43758.5453);
#elif FILM_LOMOGRAPHY_COLOR_800
  float speed = _Time.y * 10.0;
  float2 scaledUV = uv * _ScreenParams.xy / 12.0;
  grain = step(0.9, frac(scaledUV.x + scaledUV.y + speed * 0.5));
#else
  grain = Rand(uv * _ScreenParams.xy / 64.0) * 2.0 - 1.0;
#endif

  return grain;
}
