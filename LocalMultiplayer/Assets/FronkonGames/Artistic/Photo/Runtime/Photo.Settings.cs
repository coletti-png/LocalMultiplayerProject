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

namespace FronkonGames.Artistic.Photo
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Photo
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Photo settings.

      /// <summary> Center of the photo. </summary>
      public Vector2 center = Vector2.one * 0.5f;

      /// <summary> Image focus [-1, 1]. Default 0. </summary>
      public float focus = 0.0f;

      /// <summary> Image shift at loss of focus [0, 1]. Default 0.25. </summary>
      public float focusOffset = 0.25f;

      /// <summary> Rings intensity [0, 1]. Default 0.5. </summary>
      public float rings = 0.5f;

      /// <summary> Rings color. Default black. </summary>
      public Color ringsColor = Color.black;

      /// <summary> Rings color blend mode. Default Solid. </summary>
      public ColorBlends ringsColorBlend = ColorBlends.Solid;

      /// <summary> Rings thickness [0, 5]. Default 1. </summary>
      public float ringsThickness = 1.0f;

      /// <summary> Rings sharpness [0, 5]. Default 1. </summary>
      public float ringsSharpness = 1.0f;

      /// <summary> Ring 2 scale [0, 2]. Default 1. </summary>
      public float ring3Scale = 1.0f;

      /// <summary> Ring 1 scale [0, 2]. Default 1. </summary>
      public float ring2Scale = 1.0f;

      /// <summary> Ring 0 scale [0, 2]. Default 1. </summary>
      public float ring1Scale = 1.0f;

      /// <summary> Split ring scale [0, 2]. Default 1. </summary>
      public float ringSplitScale = 1.0f;

      /// <summary> Points grid intensity [0, 1]. Default 0.5. </summary>
      public float grid = 0.5f;

      /// <summary> Grid color. Default black. </summary>
      public Color gridColor = Color.black;

      /// <summary> Grid color blend mode. Default Solid. </summary>
      public ColorBlends gridColorBlend = ColorBlends.Solid;

      /// <summary> Frost ring intensity [0, 2]. Default 1. </summary>
      public float frost = 1.0f;

      /// <summary> Frost color. Default white. </summary>
      public Color frostColor = Color.white;

      /// <summary> Frost color blend mode. Default Solid. </summary>
      public ColorBlends frostColorBlend = ColorBlends.Solid;

      /// <summary> Chromatic aberration effect [0, 1]. Default 0.2. </summary>
      public float aberration = 0.2f;

      /// <summary> Aberration factor per color channel. </summary>
      public Vector3 aberrationChannels = DefaultAberrationChannels;

      /// <summary> Vignette type. </summary>
      public Vignettes vignette = Vignettes.Rectangular;

      /// <summary> Vignette size [0, 2]. Default 0.7. </summary>
      public float vignetteSize = 0.7f;

      /// <summary> Vignette smoothness [0, 1]. Default 0.25. </summary>
      public float vignetteSmoothness = 0.25f;

      /// <summary> Vignette aspect ratio [0, 1], 1 for square. Default 0. </summary>
      public float vignetteAspect = 0.0f;

      /// <summary> Aperture size [0, 1]. Default 1. </summary>
      public float apertureSize = 1.0f;

      /// <summary> Number of aperture blades [3, 12]. Default 5. </summary>
      public int apertureBlades = 5;

      /// <summary> Film stock. </summary>
      public Films film = Films.None;

      /// <summary> Years out of date of the film [0, 30]. Default 0. </summary>
      public float expiredYears = 0.0f;

      /// <summary> Blur strength [0, 10]. Default 2. </summary>
      public int blur = 2;

      /// <summary> Grain intensity [0, 1]. Default 0.2. </summary>
      public float grain = 0.2f;

      /// <summary> Halation effect for Cinestill [0, 1]. Default 0.25. </summary>
      public float halation = 0.25f;

      /// <summary> Chromatic fringing in high contrast areas [0, 5]. Default 0. </summary>
      public float chromaticFringing = 0.0f;

      /// <summary> Dust amount [0, 1]. Default 0. </summary>
      public float dust = 0.0f;
            
      /// <summary> Dust size [0.1, 2]. Default 1. </summary>
      public float dustSize = 1.0f;

      // <summary> Controls the intensity of light leaks [0, 1]. Default 0. </summary> 
      public float lightLeak = 0.0f;

      // <summary> Controls the animation speed of light leaks [0, 2]. Default 1. </summary>
      public float lightLeakSpeed = 1.0f;

      // <summary> Controls the amount of color bleeding [0, 1]. Default 0. </summary> 
      public float colorBleed = 0.0f;

      // <summary> Controls the amount of color bleeding [0, 10]. Default 2. </summary>
      public float colorBleedAmount = 2.0f;

      public static Vector3 DefaultAberrationChannels = new(0.6f, 0.8f, 1.0f);

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10.0]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>
      public float saturation = 1.0f;
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
        center = Vector2.one * 0.5f;
        focus = 0.0f;
        focusOffset = 0.25f;
        rings = 0.5f;
        ringsColor = Color.black;
        ringsColorBlend = ColorBlends.Solid;
        ringsThickness = 1.0f;
        ringsSharpness = 1.0f;
        ring3Scale = ring2Scale = ring1Scale = ringSplitScale = 1.0f;
        grid = 0.5f;
        gridColor = Color.black;
        gridColorBlend = ColorBlends.Solid;
        frost = 1.0f;
        frostColor = Color.white;
        frostColorBlend = ColorBlends.Solid;
        aberration = 0.2f;
        aberrationChannels = DefaultAberrationChannels;
        vignette = Vignettes.Rectangular;
        vignetteSize = 0.7f;
        vignetteSmoothness = 0.25f;
        vignetteAspect = 0.0f;
        film = Films.None;
        expiredYears = 0.0f;
        blur = 2;
        grain = 0.2f;
        halation = 0.25f;
        chromaticFringing = 0.0f;
        dust = 0.0f;
        dustSize = 1.0f;
        lightLeak = 0.0f;
        lightLeakSpeed = 1.0f;
        colorBleed = 0.0f;
        colorBleedAmount = 2.0f;
        apertureSize = 1.0f;
        apertureBlades = 5;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

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
