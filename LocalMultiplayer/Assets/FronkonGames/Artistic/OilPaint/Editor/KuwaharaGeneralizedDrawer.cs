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
using UnityEditor;
using static FronkonGames.Artistic.OilPaint.Inspector;

namespace FronkonGames.Artistic.OilPaint.Editor
{
  /// <summary> Kuwahara Generalized inspector. </summary>
  [CustomPropertyDrawer(typeof(KuwaharaGeneralized.Settings))]
  public class KuwaharaGeneralizedDrawer : Drawer
  {
    private KuwaharaGeneralized.Settings settings;

    protected override string EffectName => "Kuwahara Generalized";

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<KuwaharaGeneralized.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Kuwahara Generalized.
      /////////////////////////////////////////////////
      Separator();

      settings.passes = Slider("Passes", "[1, 4]. Default 1.", settings.passes, 1, 4, 1);
      settings.radius = Slider("Radius", "Size of the Kuwahara filter kernel [2, 20]. Default 10.", settings.radius, 2, 20, 10);
      settings.sharpness = Slider("Sharpness", "Adjusts sharpness of the color segments [0, 18]. Default 8.", settings.sharpness, 0.0f, 18.0f, 8.0f);
      settings.hardness = Slider("Hardness", "Adjusts hardness of the color segments [1, 100]. Default 8.", settings.hardness, 1.0f, 100.0f, 8.0f);
      settings.renderSize = Slider("Render size", "Final render size, if it is less than 1 it will be faster but more blurred.", settings.renderSize, 0.0f, 1.0f, 1.0f);

      settings.detail = (OilPaint.Detail)EnumPopup("Improve details", "Detail algorithm.", settings.detail, OilPaint.Detail.None);
      if (settings.detail != OilPaint.Detail.None)
      {
        IndentLevel++;
        settings.detailStrength = Slider("Strength", "Strength of the detail [0, 1]. Default 1. Only valid if Detail it not None.", settings.detailStrength, 0.0f, 1.0f, 1.0f);

        if (settings.detail == OilPaint.Detail.Emboss)
        {
          settings.embossStrength = Slider("Emboss", "Strength of the emboss effect [0, 20]. Default 5. Only valid if Detail it Emboss.", settings.embossStrength, 0.0f, 20.0f, 5.0f);
          settings.embossAngle = Slider("Angle", "Angle of the emboss effect [0, 360]. Default 0. Only valid if Detail it Emboss.", settings.embossAngle, 0.0f, 360.0f, 0.0f);
        }

        IndentLevel--;
      }

      settings.waterColor = Slider("Water color", "Water Color effect.", settings.waterColor, 0.0f, 1.0f, 0.0f);

      settings.processDepth = Toggle("Process depth", "Activate the use of the depth buffer to improve the effect. Default False.", settings.processDepth, false);
      if (settings.processDepth == true)
      {
        IndentLevel++;

        settings.depthCurve = CurveField("Depth curve", "Change rate at which kernel sizes change between depths. Only valid if process depth is on.", settings.depthCurve, OilPaint.DefaultDepthCurve);
        settings.depthPower = Slider("Depth power", "Factor to concentrate the depth curve. Default 1.", settings.depthPower, 0.0f, 1.0f, 1.0f);
        settings.sampleSky = Toggle("Sample sky", "Affect the sky? Default True.", settings.sampleSky, true);
        settings.viewDepthCurve = Toggle("View depth curve", "To view the depth curve in the Editor.", settings.viewDepthCurve, false);

        IndentLevel--;
      }

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
        settings.filterMode = (UnityEngine.FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, UnityEngine.FilterMode.Bilinear);
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

    protected override void InspectorChanged()
    {
      settings ??= GetSettings<KuwaharaGeneralized.Settings>();
      if (settings.processDepth == true)
        settings.forceCurveTextureUpdate = true;
    }
  }
}
