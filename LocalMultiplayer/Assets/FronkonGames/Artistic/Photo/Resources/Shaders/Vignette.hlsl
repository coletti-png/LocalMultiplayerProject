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

#pragma multi_compile _ VIGNETTE_RECTANGULAR VIGNETTE_CIRCULAR

float _VignetteSize;
float _VignetteSmoothness;
float _VignetteAperture;
float _VignetteAspect;

float ApplyVignette(float2 uv)
{
#if VIGNETTE_RECTANGULAR
  uv = (uv - 0.5) * 2.0;
  uv.x *= lerp(1.0, _ScreenParams.x / _ScreenParams.y, _VignetteAspect);

  float d = max(abs(uv.x), abs(uv.y));
  return 1.0 - smoothstep(_VignetteSize, _VignetteSize + _VignetteSmoothness, d);
#elif VIGNETTE_CIRCULAR
  uv = (uv - 0.5) * 2.0;
  uv.x *= lerp(1.0, _ScreenParams.x / _ScreenParams.y, _VignetteAspect);

  float d = length(uv);
  return 1.0 - smoothstep(_VignetteSize, _VignetteSize + _VignetteSmoothness, d);
#else
  return 1.0;
#endif  
}