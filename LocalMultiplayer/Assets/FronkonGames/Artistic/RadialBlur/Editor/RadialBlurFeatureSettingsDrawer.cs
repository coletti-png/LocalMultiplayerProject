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
using static FronkonGames.Artistic.RadialBlur.Inspector;

namespace FronkonGames.Artistic.RadialBlur.Editor
{
  /// <summary> Artistic Radial Blur inspector. </summary>
  [CustomPropertyDrawer(typeof(RadialBlur.Settings))]
  public class RadialBlurFeatureSettingsDrawer : Drawer
  {
    private RadialBlur.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<RadialBlur.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Radial Blur.
      /////////////////////////////////////////////////
      Separator();

      settings.center = Vector2Field("Center", "Center of effect", settings.center, Vector2.zero);
      settings.samples = Slider("Samples", "Number of samples used to calculate the blur effect [2, 20]. Default 8.", settings.samples, 2, 20, 8);
      settings.density = Slider("Density", "Effect density or distance in blur layers [0, 1]. Default 0.75.", settings.density, 0.0f, 1.0f, 0.75f);
      settings.falloff = Slider("Falloff", "Blur falloff [0, 10]. Default 3.", settings.falloff, 0.0f, 10.0f, 3.0f);

      settings.channelsOffset.x = Slider("Offset red", "Shifting of color red channel by effect intensity.", settings.channelsOffset.x, -10.0f, 10.0f, RadialBlur.Settings.DefaultChannelsOffset.x);
      IndentLevel++;
      settings.channelsOffset.y = Slider("Green", "Shifting of color green channel by effect intensity.", settings.channelsOffset.y, -10.0f, 10.0f, RadialBlur.Settings.DefaultChannelsOffset.y);
      settings.channelsOffset.z = Slider("Blue", "Shifting of color blue channel by effect intensity.", settings.channelsOffset.z, -10.0f, 10.0f, RadialBlur.Settings.DefaultChannelsOffset.z);
      IndentLevel--;

      settings.fishEye = Slider("Fisheye", "Screen deformation[-2, 2]. Default -0.1.", settings.fishEye, -2.0f, 2.0f, -0.1f);

      settings.gradientPower = Slider("Gradient power", "Relationship between the exterior and interior zone [0.1, 10]. Default 1.5.", settings.gradientPower, 0.1f, 10.0f, 1.5f);
      IndentLevel++;
      MinMaxSlider("Range", "Range for calculating the inner / outer zone [0, 2].", ref settings.gradientRangeMin, ref settings.gradientRangeMax, 0.0f, 2.0f, 0.0f, 1.0f);

      settings.innerColor = ColorField("Inner", "Color of the inner zone.", settings.innerColor, Color.white);
      IndentLevel++;
      settings.innerBrightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.innerBrightness, -1.0f, 1.0f, 0.0f);
      settings.innerContrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.innerContrast, 0.0f, 10.0f, 1.0f);
      settings.innerGamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.innerGamma, 0.01f, 10.0f, 1.0f);
      settings.innerHue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.innerHue, 0.0f, 1.0f, 0.0f);
      settings.innerSaturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.innerSaturation, 0.0f, 2.0f, 1.0f);
      IndentLevel--;

      settings.outerColor = ColorField("Outer", "Color of the outer zone.", settings.outerColor, Color.white);
      IndentLevel++;
      settings.outerBrightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.outerBrightness, -1.0f, 1.0f, 0.0f);
      settings.outerContrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.outerContrast, 0.0f, 10.0f, 1.0f);
      settings.outerGamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.outerGamma, 0.01f, 10.0f, 1.0f);
      settings.outerHue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.outerHue, 0.0f, 1.0f, 0.0f);
      settings.outerSaturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.outerSaturation, 0.0f, 2.0f, 1.0f);
      IndentLevel--;
      IndentLevel--;

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
          "Render pass injection. Default BeforeRenderingPostProcessing.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing);
#if !UNITY_6000_0_OR_NEWER
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);
#endif

        IndentLevel--;
      }
    }
  }
}
