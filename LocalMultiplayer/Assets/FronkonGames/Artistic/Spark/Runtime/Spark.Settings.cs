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

namespace FronkonGames.Artistic.Spark
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Spark
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
      #region Spark settings.

      /// <summary> How many points each spark has [1, 20]. Default 4. </summary>
      public int rays = 4;

      /// <summary> Downsample. </summary>
      public DownSamples downSample = DownSamples.Eighth;

      /// <summary> Color blend. </summary>
      public ColorBlends blend = ColorBlends.Screen;

      /// <summary> Overall brightness of sparks [0, 500]. Default 40. </summary>
      public float gain = 40.0f;

      /// <summary> Size of the sparks [1, 400]. Default 50. </summary>
      public float size = 50.0f;

      /// <summary> Highlights darker than this are ignored [0, 3]. Default 1.25. </summary>
      public float threshold = 1.25f;

      /// <summary> Highlights brighter than this don't increase the glint further, to avoid hot pixels creating huge sparks [0, 20]. Default 5. </summary>
      public float thresholdClamp = 5.0f;

      /// <summary> Bend the arms of the sparks [-1, 1]. Default 0. </summary>
      public float twirl = 0.0f;

      /// <summary> Sparks rotation [-360, 360]. Default 48. </summary>
      public float spin = 48.0f;

      /// <summary> Bend sparks around center of frame [-10, 10]. Default 0. </summary>
      public float barrel = 0.0f;

      /// <summary> Bendiness of barrel distortion [1, 20]. Default 2. </summary>
      public float barrelBend = 2.0f;

      /// <summary> Dissolves away ends of sparks [-2, 4]. Default 1.9. </summary>
      public float falloff = 1.9f;

      /// <summary> Stretch stars for anamorphic sparks [0.01, 20]. Default 1. </summary>
      public float aspect = 1.0f;

      /// <summary> How much the sparks change color along their arms [0, 10]. Default 0.25. </summary>
      public float dispersion = 0.25f;

      /// <summary> How many complete rainbows fit along each spark [0.01, 30]. Default 1. </summary>
      public float dispersionCycles = 1.0f;

      /// <summary> Hue-clock the dispersion rainbows [-360, 360]. Default -45. </summary>
      public float dispersionOffset = -45.0f;

      /// <summary> Noise [0, 1]. Default 0.1. </summary>
      public float dirt = 0.1f;

      /// <summary> Noise frequency [0, 1000]. Default 5. </summary>
      public float dirtFreq = 5.0f;

      /// <summary> Increase to avoid stepping artifacts [0.1, 16]. Default 1.4. </summary>
      public float artifacts = 1.4f;

      /// <summary> Tint the sparks towards this color. </summary>
      public Color tint = DefaultTint;

      /// <summary> Blur size [0, 100]. Default 2. </summary>
      public float blur = 2.0f;

      /// <summary> Multiplier per color channel [0, 10]. </summary>
      public Vector3 blurWeights = Vector3.one;

      public static readonly Color DefaultTint = new(1.0f, 1.0f, 1.0f, 0.7f);

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

        rays = 4;
        downSample = DownSamples.Eighth;
        blend = ColorBlends.Screen;
        gain = 40.0f;
        size = 50.0f;
        threshold = 1.25f;
        thresholdClamp = 5.0f;
        barrel = 0.0f;
        barrelBend = 2.0f;
        tint = DefaultTint;
        blur = 2.0f;
        blurWeights = Vector3.one;
        spin = 48.0f;
        artifacts = 1.4f;
        twirl = 0.0f;
        falloff = 1.9f;
        dispersion = 0.25f;
        dispersionCycles = 1.0f;
        dispersionOffset = -45.0f;
        dirt = 0.1f;
        dirtFreq = 5.0f;
        aspect = 1.0f;

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
