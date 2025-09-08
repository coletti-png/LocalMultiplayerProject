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
using UnityEngine;
using UnityEditor;
using static FronkonGames.Artistic.Spark.Inspector;

namespace FronkonGames.Artistic.Spark.Editor
{
  /// <summary> Artistic Spark inspector. </summary>
  [CustomPropertyDrawer(typeof(Spark.Settings))]
  public class SparkFeatureSettingsDrawer : Drawer
  {
    private Spark.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Spark.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Spark.
      /////////////////////////////////////////////////
      Separator();

      settings.rays = Slider("Rays", "How many points each spark has [1, 20]. Default 4.", settings.rays, 1, 20, 4);
      IndentLevel++;
      settings.size = Slider("Size", "Size of the stars [1, 400]. Default 50.", settings.size, 1.0f, 400.0f, 50.0f);
      settings.gain = Slider("Gain", "Overall brightness of stars [0, 500]. Default 40.", settings.gain, 0.0f, 500.0f, 40.0f);
      settings.blend = (ColorBlends)EnumPopup("Blend", "Color blend.", settings.blend, ColorBlends.Screen);
      settings.tint = ColorField("Tint", "Size of the stars [1, 400]. Default 50.", settings.tint, Spark.Settings.DefaultTint);
      settings.twirl = Slider("Twirl", "Bend the arms of the sparks [-1, 1]. Default 0.", settings.twirl, -1.0f, 1.0f, 0.0f);
      settings.spin = Slider("Spin", "Sparks rotation [-360, 360]. Default 48.", settings.spin, -360.0f, 360.0f, 48.0f);
      settings.falloff = Slider("Falloff", "Dissolves away ends of rays [-2, 4]. Default 1.9.", settings.falloff, -2.0f, 4.0f, 1.9f);
      settings.aspect = Slider("Aspect", "Stretch stars for anamorphic sparks [0.01, 20]. Default 1.", settings.aspect, 0.01f, 20.0f, 1.0f);
      IndentLevel--;

      settings.barrel = Slider("Barrel", "Bend sparks around center of frame [-10, 10]. Default 0.", settings.barrel, -10.0f, 10.0f, 0.0f);
      IndentLevel++;
      settings.barrelBend = Slider("Bend", "Bendiness of barrel distortion [1, 20]. Default 2.", settings.barrelBend, 1.0f, 20.0f, 2.0f);
      IndentLevel--;

      settings.dispersion = Slider("Dispersion", "How much the sparks change color along their arms [0, 10]. Default 0.25.", settings.dispersion, 0.0f, 10.0f, 0.25f);
      IndentLevel++;
      settings.dispersionCycles = Slider("Cycles", "How many complete rainbows fit along each spark [0.01, 30]. Default 1.", settings.dispersionCycles, 0.01f, 30.0f, 1.0f);
      settings.dispersionOffset = Slider("Offset", "Hue-clock the dispersion rainbows [-360, 360]. Default -45.", settings.dispersionOffset, -360.0f, 360.0f, -45.0f);
      IndentLevel--;

      settings.blur = Slider("Blur", "Blur size [0, 100]. Default 2.", settings.blur, 0.0f, 100.0f, 2.0f);
      IndentLevel++;
      settings.blurWeights.x = Slider("Red", "Multiplier red channel [0, 10].", settings.blurWeights.x, 0.0f, 10.0f, 1.0f);
      settings.blurWeights.y = Slider("Green", "Multiplier green channel [0, 10].", settings.blurWeights.y, 0.0f, 10.0f, 1.0f);
      settings.blurWeights.z = Slider("Blue", "Multiplier blue channel [0, 10].", settings.blurWeights.z, 0.0f, 10.0f, 1.0f);
      IndentLevel--;

      settings.dirt = Slider("Dirt", "Noise [0, 1]. Default 0.1.", settings.dirt, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.dirtFreq = Slider("Frequency", "Noise frequency [0, 1000]. Default 5.", settings.dirtFreq, 0.0f, 1000.0f, 5.0f);
      IndentLevel--;

      settings.threshold = Slider("Threshold", "Highlights darker than this are ignored [0, 3]. Default 1.25.", settings.threshold, 0.0f, 3.0f, 1.25f);
      IndentLevel++;
      settings.thresholdClamp = Slider("Clamp", "Highlights brighter than this don't increase the glint further, to avoid hot pixels creating huge sparks [0, 20]. Default 5.", settings.thresholdClamp, 0.0f, 20.0f, 5.0f);
      IndentLevel--;

      settings.downSample = (Spark.DownSamples)EnumPopup("Down sample", "Down sample resolution.", settings.downSample, Spark.DownSamples.Eighth);

      settings.artifacts = Slider("Artifacts", "Increase to avoid stepping artifacts [0.1, 16]. Default 1.4.", settings.artifacts, 0.1f, 16.0f, 1.4f);

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

#if !UNITY_6000_0_OR_NEWER
        settings.filterMode = (FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, FilterMode.Bilinear);
#endif
        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingTransparents.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents);
#if !UNITY_6000_0_OR_NEWER
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);
#endif

        IndentLevel--;
      }
    }
  }
}
