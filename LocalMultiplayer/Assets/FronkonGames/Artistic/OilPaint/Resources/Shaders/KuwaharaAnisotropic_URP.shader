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
Shader "Hidden/Fronkon Games/Artistic/Oil Paint/Kuwahara Anisotropic URP"
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
      Name "Fronkon Games Artistic Oil Paint: Kuwahara Anisotropic"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ PROCESS_DEPTH
      #pragma multi_compile ___ VIEW_DEPTH
      #pragma multi_compile ___ DETAIL_PASS

      #include "Artistic.hlsl"
#if PROCESS_DEPTH
      #include "ProcessDepth.hlsl"
#endif

      uint _Radius;
      TEXTURE2D_X(_TensorTex);

      float _Sharpness;
      float _Hardness;
      float _Alpha;
      float _ZeroCrossing;

      half4 KuwaharaAnisotropic(float2 uv, float depth)
      {
        const half4 tensor = SAMPLE_TEXTURE2D_X(_TensorTex, sampler_LinearClamp, uv);

        uint radius = _Radius / 2;
#if PROCESS_DEPTH
#if VIEW_DEPTH
        return SafePositivePow_float(ViewRadius(radius, depth), 2.0) * SAMPLE_MAIN(uv);
#endif
        radius = CalculateRadius(radius, depth);
#endif

        float a = float(radius) * clamp((_Alpha + tensor.w) / _Alpha, 0.1, 2.0);
        float b = float(radius) * clamp(_Alpha / (_Alpha + tensor.w), 0.1, 2.0);

        float cos_phi = cos(tensor.z);
        float sin_phi = sin(tensor.z);

        float2x2 R = { cos_phi, -sin_phi, sin_phi, cos_phi};
        float2x2 S = { 0.5 / a, 0.0, 0.0, 0.5 / b};

        float2x2 SR = mul(S, R);

        int max_x = int(sqrt(a * a * cos_phi * cos_phi + b * b * sin_phi * sin_phi));
        int max_y = int(sqrt(a * a * sin_phi * sin_phi + b * b * cos_phi * cos_phi));

        float sinZeroCross = sin(_ZeroCrossing);
        float eta = (0.01 + cos(_ZeroCrossing)) / (sinZeroCross * sinZeroCross);
        int k;
        float4 m[8];
        float3 s[8];

        for (k = 0; k < 8; ++k)
        {
          m[k] = 0.0;
          s[k] = 0.0;
        }

        [loop]
        for (int y = -max_y; y <= max_y; ++y)
        {
          [loop]
          for (int x = -max_x; x <= max_x; ++x)
          {
            float2 v = mul(SR, float2(x, y));
            if (dot(v, v) <= 0.25f)
            {
              half3 c = SAMPLE_MAIN_LOD(uv + float2(x, y) * TEXEL_SIZE.xy).rgb;
              c = saturate(c);
              float sum = 0;
              float w[8];
              float z, vxx, vyy;

              vxx = 0.01 - eta * v.x * v.x;
              vyy = 0.01 - eta * v.y * v.y;

              z = max(0, v.y + vxx); 
              w[0] = z * z;
              sum += w[0];

              z = max(0, -v.x + vyy); 
              w[2] = z * z;
              sum += w[2];

              z = max(0, -v.y + vxx); 
              w[4] = z * z;
              sum += w[4];

              z = max(0, v.x + vyy); 
              w[6] = z * z;
              sum += w[6];
              v = sqrt(2.0) / 2.0 * float2(v.x - v.y, v.x + v.y);
              vxx = 0.01 - eta * v.x * v.x;
              vyy = 0.01 - eta * v.y * v.y;

              z = max(0, v.y + vxx); 
              w[1] = z * z;
              sum += w[1];

              z = max(0, -v.x + vyy); 
              w[3] = z * z;
              sum += w[3];

              z = max(0, -v.y + vxx); 
              w[5] = z * z;
              sum += w[5];
              
              z = max(0, v.x + vyy); 
              w[7] = z * z;
              sum += w[7];

              float g = exp(-3.125 * dot(v,v)) / sum;

              for (int k = 0; k < 8; ++k)
              {
                float wk = w[k] * g;
                m[k] += float4(c * wk, wk);
                s[k] += c * c * wk;
              }
            }
          }
        }

        half4 pixel = (float4)0.0;
        [loop]
        for (k = 0; k < 8; ++k)
        {
          m[k].rgb /= m[k].w;
          s[k] = abs(s[k] / m[k].w - m[k].rgb * m[k].rgb);

          float sigma2 = s[k].r + s[k].g + s[k].b;
          float w = 1.0 / (1.0 + SafePositivePow_float(_Hardness * 1000.0 * sigma2, 0.5 * _Sharpness));

          pixel += half4(m[k].rgb * w, w);
        }

        pixel.rgb = saturate(pixel.rgb / pixel.w);

        return pixel;
      }

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = (half4)0.0;

#if PROCESS_DEPTH
        const float depth = SampleDepthCurve(SampleLinear01Depth(uv));

        UNITY_BRANCH
        if (_SampleSky == 1)
          pixel = KuwaharaAnisotropic(uv, depth);
        else
          pixel = depth < 0.98 ? KuwaharaAnisotropic(uv, depth) : color;
#else
        pixel = KuwaharaAnisotropic(uv, 0.0);
#endif

#ifndef DETAIL_PASS
        pixel.rgb = ColorAdjust(pixel.rgb);
#endif

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Detail"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile ___ DETAIL_SHARPEN DETAIL_EMBOSS
      #pragma multi_compile ___ WATER_COLOR
      #pragma multi_compile ___ DETAIL_PASS

      #include "Artistic.hlsl"
#if PROCESS_DEPTH
      #include "ProcessDepth.hlsl"
#endif
#if DETAIL_SHARPEN || DETAIL_EMBOSS
      #include "Detail.hlsl"
#endif
#if WATER_COLOR
      #include "WaterColor.hlsl"
#endif

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;

#if PROCESS_DEPTH
        const float depth = SampleDepthCurve(SampleLinear01Depth(uv));
#else
        const float depth = 0.0;
#endif

#ifdef DETAIL_SHARPEN
        pixel.rgb = Sharpen(uv, pixel.rgb, depth);
#elif DETAIL_EMBOSS
        pixel.rgb = Emboss(uv, pixel.rgb, depth);
#endif

#if WATER_COLOR
        pixel.rgb = WaterColor(color.rgb, pixel.rgb, uv);
#endif

#ifdef DETAIL_PASS
        pixel.rgb = ColorAdjust(pixel.rgb);
#endif

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Tensor"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash

      #include "Artistic.hlsl"

      half4 CalculateTensors(const float2 uv)
      {
        half3 x = (1.0 * SAMPLE_MAIN(uv + float2(-TEXEL_SIZE.x, -TEXEL_SIZE.y)).rgb +
                   2.0 * SAMPLE_MAIN(uv + float2(-TEXEL_SIZE.x,  0.0)).rgb +
                   1.0 * SAMPLE_MAIN(uv + float2(-TEXEL_SIZE.x,  TEXEL_SIZE.y)).rgb +
                  -1.0 * SAMPLE_MAIN(uv + float2( TEXEL_SIZE.x, -TEXEL_SIZE.y)).rgb +
                  -2.0 * SAMPLE_MAIN(uv + float2( TEXEL_SIZE.x,  0.0)).rgb +
                  -1.0 * SAMPLE_MAIN(uv + float2( TEXEL_SIZE.x,  TEXEL_SIZE.y)).rgb) / 4.0;

        half3 y = (1.0 * SAMPLE_MAIN(uv + float2(-TEXEL_SIZE.x, -TEXEL_SIZE.y)).rgb +
                   2.0 * SAMPLE_MAIN(uv + float2( 0.0,          -TEXEL_SIZE.y)).rgb +
                   1.0 * SAMPLE_MAIN(uv + float2( TEXEL_SIZE.x, -TEXEL_SIZE.y)).rgb +
                  -1.0 * SAMPLE_MAIN(uv + float2(-TEXEL_SIZE.x,  TEXEL_SIZE.y)).rgb +
                  -2.0 * SAMPLE_MAIN(uv + float2( 0.0,           TEXEL_SIZE.y)).rgb +
                  -1.0 * SAMPLE_MAIN(uv + float2( TEXEL_SIZE.x,  TEXEL_SIZE.y)).rgb) / 4.0;

        return half4(dot(x, x), dot(y, y), dot(x, y), 1.0);
      }

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        return CalculateTensors(uv);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Blur Horizontal"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash

      #include "Artistic.hlsl"

      int _Blur;

      inline float Gaussian(float sigma, float pos)
      {
        return (1.0 / sqrt(2.0 * PI * sigma * sigma)) * exp(-(pos * pos) / (2.0 * sigma * sigma));
      }

      half4 BlurHorizontal(const float2 uv)
      {
        half3 pixel = (half3)0.0;
        float kernelSum = 0.0;

        UNITY_LOOP
        for (int x = -_Blur; x <= _Blur; ++x)
        {
          half3 color = SAMPLE_MAIN(uv + float2(x * TEXEL_SIZE.x, 0.0)).rgb;
          const float gauss = Gaussian(2.0, x);

          pixel += color * gauss;
          kernelSum += gauss;
        }

        return half4(pixel / kernelSum, 1.0);
      }      

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        return BlurHorizontal(uv);
      }
      ENDHLSL
    }

    Pass
    {
      Name "Fronkon Games Artistic Oil Paint: Blur Vertical & Anisotropy"

      HLSLPROGRAM
      #pragma vertex ArtisticVert
      #pragma fragment ArtisticFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash

      #include "Artistic.hlsl"

      int _Blur;

      inline float Gaussian(float sigma, float pos)
      {
        return (1.0 / sqrt(2.0 * PI * sigma * sigma)) * exp(-(pos * pos) / (2.0 * sigma * sigma));
      }

      half3 BlurVertical(const float2 uv)
      {
        half3 pixel = (half3)0.0;
        float kernelSum = 0.0;

        UNITY_LOOP
        for (int y = -_Blur; y <= _Blur; ++y)
        {
          half3 color = SAMPLE_MAIN(uv + float2(0.0, y * TEXEL_SIZE.y)).rgb;
          const float gauss = Gaussian(2.0, y);

          pixel += color * gauss;
          kernelSum += gauss;
        }

        return pixel / kernelSum;
      }

      half4 CalculateAnisotropy(const half3 pixel)
      {
        float lambda1 = 0.5 * (pixel.y + pixel.x + sqrt(pixel.y * pixel.y - 2.0 * pixel.x * pixel.y + pixel.x * pixel.x + 4.0 * pixel.z * pixel.z));
        float lambda2 = 0.5 * (pixel.y + pixel.x - sqrt(pixel.y * pixel.y - 2.0 * pixel.x * pixel.y + pixel.x * pixel.x + 4.0 * pixel.z * pixel.z));

        float2 v = float2(lambda1 - pixel.x, -pixel.z);
        float2 t = length(v) > 0.0 ? normalize(v) : float2(0.0, 1.0);
        float phi = -atan2(t.y, t.x);
        float A = (lambda1 + lambda2 > 0.0) ? (lambda1 - lambda2) / (lambda1 + lambda2) : 0.0;

        return half4(t, phi, A);
      }      

      half4 ArtisticFrag(const ArtisticVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;

        half4 pixel = (half4)0.0;
        float kernelSum = 0.0;

        pixel.rgb = BlurVertical(uv);

        return CalculateAnisotropy(pixel.rgb);
      }
      ENDHLSL      
    }
  }
  
  FallBack "Diffuse"
}
