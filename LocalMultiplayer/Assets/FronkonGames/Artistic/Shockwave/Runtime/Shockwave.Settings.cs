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
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Artistic.Shockwave
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Shockwave
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Common settings.

      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Shockwave settings.

      /// <summary> Controls the radius of the shockwave [0, 1]. Default 0, no shockwave. 1 shockwave outside the screen. </summary>
      public float radius = 0.0f;

      /// <summary> The center of the shockwave. </summary>
      public Vector2 center = new(0.5f, 0.5f);

      /// <summary> The strength of the shockwave. </summary>
      public Vector3 colorStrength = new(1.0f, 1.0f, 1.0f);

      /// <summary> The color blend of the shockwave. </summary>
      public ColorBlends shockwaveColorBlend = ColorBlends.Solid;

      /// <summary> Controls the strength of the shockwave [0, 5]. Default 1. </summary>
      public float strength = 1.0f;

      /// <summary> The width of the shockwave [0.01, 0.75]. Default 0.25. </summary>
      public float width = 0.25f;

       /// <summary> Inner ring width [0.01, 1.0]. Default 0.5. </summary>
       public float ringWidthInner = 0.5f;

       /// <summary> Outer ring width [0.01, 1.0]. Default 0.5. </summary>
       public float ringWidthOuter = 0.5f;

       /// <summary> Ring sharpness exponent [1.0, 32.0]. Default 8. </summary>
       public float ringSharpness = 8.0f;

       /// <summary> Ring skew [-1.0, 1.0] (pushes thickness outward/inward). Default 0. </summary>
       public float ringSkew = 0.0f;

      /// <summary> The chromatic aberration of the shockwave per color channel. </summary>
      public Vector3 chromaticAberration = ChromaticAberrationDefault;

      /// <summary> The inside zone tint color. </summary>
      public Color insideTint = InsideTintDefault;

      /// <summary> The flares amount of the shockwave [0, 1]. Default 0.2. </summary>
      public float flares = 0.2f;

      /// <summary> The flares color blend of the shockwave. </summary>
      public ColorBlends flaresColorBlend = ColorBlends.Solid;

      /// <summary> The flares color of the shockwave. </summary>
      public Color flaresColor = InsideTintDefault;

      /// <summary> The flares frequency of the shockwave [0, 64]. Default 24. </summary>
      public float flaresFrequency = 24.0f;

      /// <summary> The flares speed of the shockwave [0, 5]. Default 1.5. </summary>
      public float flaresSpeed = 1.5f;

      /// <summary> The flares threshold of the shockwave [0, 1]. Default 0.35. </summary>
      public float flaresThreshold = 0.35f;

      /// <summary> The flares softness of the shockwave [0, 10]. Default 6.0. </summary>
      public float flaresSoftness = 6.0f;

      /// <summary> The noise amount of the shockwave [0, 1]. Default 0.2. </summary>
      public float noise = 0.2f;

      /// <summary> The noise scale of the shockwave [0.1, 64]. Default 8. </summary>
      public float noiseScale = 8.0f;

      /// <summary> The noise speed of the shockwave [0, 5]. Default 0.2. </summary>
      public float noiseSpeed = 0.2f;

      /// <summary> Edge highlight amount [0, 1]. Default 1.0. </summary>
      public float edge = 1.0f;

      /// <summary> Edge color blend mode. </summary>
      public ColorBlends edgeColorBlend = ColorBlends.Hue;

      /// <summary> Edge color/tint. Default cyan. </summary>
      public Color edgeColor = Color.cyan;

      /// <summary> Edge sampling width in texels [0.1, 5]. Default 1. </summary>
      public float edgeWidth = 1.0f;

      /// <summary> Edge noise amount [0, 1]. Default 1. </summary>
      public float edgeNoise = 1.0f;

      /// <summary> Edge noise scale [0.1, 64]. Default 8. </summary>
      public float edgeNoiseScale = 8.0f;

      /// <summary> Edge noise speed [0, 10]. Default 1.0. </summary>
      public float edgeNoiseSpeed = 1.0f;

      /// <summary> Edge plasma amount [0, 1]. Default 0.1. </summary>
      public float edgePlasma = 0.1f;

      /// <summary> Edge plasma scale [0.01, 64]. Default 5.0. </summary>
      public float edgePlasmaScale = 5.0f;

      /// <summary> Edge plasma speed [0, 10]. Default 1.0. </summary>
      public float edgePlasmaSpeed = 1.0f;

      /// <summary> Hue variation amount [0, 1]. Default 0 (disabled). </summary>
      public float hueVariation = 0.0f;

      /// <summary> Hue variation angular scale (bands around center) [0, 2]. Default 1. </summary>
      public float hueVariationScale = 1.0f;

      /// <summary> Hue variation speed [-10, 10]. Default 1.0. </summary>
      public float hueVariationSpeed = 1.0f;

      /// <summary> Hue variation radial amount [0, 1]. Default 0 (disabled). </summary>
      public float hueVariationRadial = 0.0f;

       /// <summary> Hue variation radial scale [0, 64]. Default 4. </summary>
       public float hueVariationRadialScale = 4.0f;

      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 2.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10.0]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1.0. </summary>
      public float saturation = 1.0f;

      /// <summary> The color blend of the shockwave. </summary>
      public ColorBlends colorBlend = ColorBlends.Solid;

      public static readonly Color InsideTintDefault = new(0.725f, 1.0f, 1.0f);

      public static readonly Vector3 ChromaticAberrationDefault = new(-1.0f, 2.0f, 5.0f);

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

#if !UNITY_6000_0_OR_NEWER
      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif
      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        radius = 0.0f;
        center = new(0.5f, 0.5f);
        colorStrength = new(1.0f, 1.0f, 1.0f);
        shockwaveColorBlend = ColorBlends.Solid;
        strength = 1.0f;
        width = 0.25f;
        ringWidthInner = 0.5f;
        ringWidthOuter = 0.5f;
        ringSharpness = 8.0f;
        ringSkew = 0.0f;
        chromaticAberration = ChromaticAberrationDefault;
        insideTint = InsideTintDefault;
        flares = 0.2f;
        flaresColorBlend = ColorBlends.Solid;
        flaresColor = InsideTintDefault;
        flaresFrequency = 24.0f;
        flaresSpeed = 1.5f;
        flaresThreshold = 0.35f;
        flaresSoftness = 6.0f;
        noise = 0.2f;
        noiseScale = 8.0f;
        noiseSpeed = 0.2f;
        edge = 1.0f;
        edgeColorBlend = ColorBlends.Hue;
        edgeColor = Color.cyan;
        edgeWidth = 1.0f;
        edgeNoise = 1.0f;
        edgeNoiseScale = 8.0f;
        edgeNoiseSpeed = 1.0f;
        edgePlasma = 0.1f;
        edgePlasmaScale = 5.0f;
        edgePlasmaSpeed = 1.0f;
        edgePlasma = 0.1f;
        edgePlasmaScale = 5.0f;
        edgePlasmaSpeed = 1.0f;
        hueVariation = 0.0f;
         hueVariationScale = 1.0f;
         hueVariationSpeed = 1.0f;
         hueVariationRadial = 0.0f;
         hueVariationRadialScale = 4.0f;
        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;
        colorBlend = ColorBlends.Solid;

        affectSceneView = false;
#if !UNITY_6000_0_OR_NEWER
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }
    }
  }
}
