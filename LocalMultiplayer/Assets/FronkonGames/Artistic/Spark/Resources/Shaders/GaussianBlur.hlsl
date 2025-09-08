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
#ifndef GAUSSIANBLUR_INCLUDE
#define GAUSSIANBLUR_INCLUDE

float4 GaussianBlur(float2 xy, float2 res, float sizered, float sizegreen, float sizeblue, float sizealpha, float2 dir)
{
  float4 sigmas = float4(sizered, sizegreen, sizeblue, sizealpha);

  float4 gx = 0.0;
  float4 gy = 0.0;
  float4 gz = 0.0;

  gx = 1.0 / max(sqrt(2.0 * 3.141592653589793238) * sigmas, EPSILON);
  gy = exp(-0.5 / max(sigmas * sigmas, EPSILON));
  gz = gy * gy;

  float4 a = 0.0;
  float4 centre = 0.0;
  float4 sample1 = 0.0;
  float4 sample2 = 0.0;

  centre = SAMPLE_MAIN(xy / res);
  a += gx * centre;
  float4 energy = gx;
  gx *= gy;
  gy *= gz;

  float support = max(max(max(sigmas.r, sigmas.g), sigmas.b), sigmas.a) * 3.0;

  UNITY_LOOP
  for (float i = 1.0; i <= support; i++)
  {
    sample1 = SAMPLE_MAIN_LOD((xy - i * dir) / res);
    sample2 = SAMPLE_MAIN_LOD((xy + i * dir) / res);
    a += gx * sample1;
    a += gx * sample2;
    energy += 2.0 * gx;
    gx *= gy;
    gy *= gz;
  }

  a /= energy;

  UNITY_BRANCH
  if (sizered < 0.1)
    a.r = centre.r;

  UNITY_BRANCH
  if (sizegreen < 0.1)
    a.g = centre.g;

  UNITY_BRANCH
  if (sizeblue < 0.1)
    a.b = centre.b;

  return a; 
}

#endif // GAUSSIANBLUR_INCLUDE