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

namespace FronkonGames.Artistic.RadialBlur
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class RadialBlur
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
      #region Radial Blur settings.

      /// <summary> Center of effect. </summary>
      public Vector2 center = Vector2.zero;

      /// <summary> Number of samples used to calculate the blur effect [2, 20]. Default 8. </summary>
      /// <remarks> The higher, the better the quality but the worse the performance. </remarks>
      public int samples = 8;

      /// <summary> Effect density or distance in blur layers [0, 1]. Default 0.75. </summary>
      public float density = 0.75f;

      /// <summary> Blur falloff [0, 10]. Default 3. </summary>
      public float falloff = 3.0f;

      /// <summary> Shifting of color channels by effect intensity. </summary>
      public Vector3 channelsOffset = DefaultChannelsOffset;

      /// <summary> Screen deformation[-2, 2]. Default -0.1. </summary>
      public float fishEye = -0.1f;

      /// <summary> Relationship between the exterior and interior zone [0.1, 10]. Default 1.5. </summary>
      public float gradientPower = 1.5f;

      /// <summary> Minimum range for calculating the inner zone [0, 2]. Default 0. </summary>
      public float gradientRangeMin = 0.0f;

      /// <summary> Maximum range for calculating the outer zone [0, 2]. Default 1. </summary>
      public float gradientRangeMax = 1.0f;

      /// <summary> Color of the inner zone. </summary>
      public Color innerColor = Color.white;

      /// <summary> Inner brightness [-1, 1]. Default 0. </summary>
      public float innerBrightness = 0.0f;

      /// <summary> Inner contrast [0, 10]. Default 1. </summary>
      public float innerContrast = 1.0f;

      /// <summary> Inner gamma [0.1, 10]. Default 1. </summary>
      public float innerGamma = 1.0f;

      /// <summary> The inner color wheel [0, 1]. Default 0. </summary>
      public float innerHue = 0.0f;

      /// <summary> Inner intensity of a colors [0, 2]. Default 1. </summary>
      public float innerSaturation = 1.0f;

      /// <summary> Color of the exterior area. </summary>
      public Color outerColor = Color.white;

      /// <summary> Outer brightness [-1, 1]. Default 0. </summary>
      public float outerBrightness = 0.0f;

      /// <summary> Outer contrast [0, 10]. Default 1. </summary>
      public float outerContrast = 1.0f;

      /// <summary> Outer gamma [0.1, 10]. Default 1. </summary>
      public float outerGamma = 1.0f;

      /// <summary> The outer color wheel [0, 1]. Default 0. </summary>
      public float outerHue = 0.0f;

      /// <summary> Outer intensity of a colors [0, 2]. Default 1. </summary>
      public float outerSaturation = 1.0f;

      public static readonly Vector3 DefaultChannelsOffset = new(1.0f, 2.0f, 4.0f);

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1, 1]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0, 10]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0, 1]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
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
        intensity = 1.0f;

        center = Vector2.zero;
        samples = 8;
        density = 0.75f;
        falloff = 3.0f;
        channelsOffset = DefaultChannelsOffset;
        fishEye = -0.1f;
        gradientPower = 1.5f;
        gradientRangeMin = 0.0f;
        gradientRangeMax = 1.0f;
        innerColor = Color.white;
        innerBrightness = 0.0f;
        innerContrast = 1.0f;
        innerGamma = 1.0f;
        innerHue = 0.0f;
        innerSaturation = 1.0f;
        outerColor = Color.white;
        outerBrightness = 0.0f;
        outerContrast = 1.0f;
        outerGamma = 1.0f;
        outerHue = 0.0f;
        outerSaturation = 1.0f;

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
