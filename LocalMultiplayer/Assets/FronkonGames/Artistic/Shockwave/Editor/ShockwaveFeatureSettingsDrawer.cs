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
using static FronkonGames.Artistic.Shockwave.Inspector;

namespace FronkonGames.Artistic.Shockwave.Editor
{
  /// <summary> Artistic Shockwave inspector. </summary>
  [CustomPropertyDrawer(typeof(Shockwave.Settings))]
  public class ShockwaveFeatureSettingsDrawer : Drawer
  {
    private Shockwave.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Shockwave.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Shockwave.
      /////////////////////////////////////////////////
      Separator();

      Label("Shockwave");
      IndentLevel++;
      settings.shockwaveColorBlend = (ColorBlends)EnumPopup("Blend", "The color blend of the shockwave. Default Solid.", settings.shockwaveColorBlend, ColorBlends.Solid);
      settings.radius = Slider("Radius", "Controls the radius of the shockwave [0, 1]. Default 0, no shockwave. 1 shockwave outside the screen.", settings.radius, 0.0f, 1.0f, 0.0f);
      settings.center = Vector2Field("Center", "Controls the center of the shockwave [0, 1]. Default (0.5, 0.5)", settings.center, Vector2.one * 0.5f);
      settings.strength = Slider("Strength", "Controls the strength of the shockwave [0, 5]. Default 1.", settings.strength, 0.0f, 5.0f, 1.0f);
      settings.width = Slider("Width", "The width of the shockwave [0.01, 0.75]. Default 0.25.", settings.width, 0.01f, 0.75f, 0.25f);
      IndentLevel++;
      settings.ringWidthInner = Slider("Inner width", "Inner ring width [0.01, 1.0]. Default 0.5.", settings.ringWidthInner, 0.01f, 1.0f, 0.5f);
      settings.ringWidthOuter = Slider("Outer width", "Outer ring width [0.01, 1.0]. Default 0.5.", settings.ringWidthOuter, 0.01f, 1.0f, 0.5f);
      IndentLevel--;
      settings.ringSharpness = Slider("Sharpness", "Ring sharpness exponent [1.0, 32.0].", settings.ringSharpness, 1.0f, 32.0f, 8.0f);
      settings.ringSkew = Slider("Skew", "Ring skew [-1.0, 1.0] (pushes thickness outward/inward).", settings.ringSkew, -1.0f, 1.0f, 0.0f);
      settings.colorStrength = Vector3Field("Color strength", "The color strength of the shockwave per color channel. Default (1.0, 1.0, 1.0)", settings.colorStrength, Vector3.one);
      settings.chromaticAberration = Vector3Field("Chromatic aberration", "The chromatic aberration of the shockwave per color channel. Default (-1.0, 2.0, 5.0)", settings.chromaticAberration, Shockwave.Settings.ChromaticAberrationDefault);
      settings.noise = Slider("Noise", "The noise amount of the shockwave [0, 1]. Default 0.2.", settings.noise, 0.0f, 1.0f, 0.2f);
      IndentLevel++;
      settings.noiseScale = Slider("Scale", "The noise scale of the shockwave [0.1, 64]. Default 8.", settings.noiseScale, 0.1f, 64.0f, 8.0f);
      settings.noiseSpeed = Slider("Speed", "The noise speed of the shockwave [0, 5]. Default 0.2.", settings.noiseSpeed, 0.0f, 5.0f, 0.2f);
      IndentLevel--;
      IndentLevel--;

      settings.flares = Slider("Flares", "The flares amount of the shockwave [0, 1]. Default 0.2.", settings.flares, 0.0f, 1.0f, 0.2f);
      IndentLevel++;
      settings.flaresColorBlend = (ColorBlends)EnumPopup("Blend", "The color blend of the flares. Default Solid.", settings.flaresColorBlend, ColorBlends.Solid);
      settings.flaresColor = ColorField("Color", "The color of the flares. Default light cyan.", settings.flaresColor, Shockwave.Settings.InsideTintDefault);
      settings.flaresFrequency = Slider("Frequency", "The flares frequency of the shockwave [0, 64]. Default 24.", settings.flaresFrequency, 0.0f, 64.0f, 24.0f);
      settings.flaresSpeed = Slider("Speed", "The flares speed of the shockwave [0, 5]. Default 1.5.", settings.flaresSpeed, 0.0f, 5.0f, 1.5f);
      settings.flaresThreshold = Slider("Threshold", "The flares threshold of the shockwave [0, 1]. Default 0.35.", settings.flaresThreshold, 0.0f, 1.0f, 0.35f);
      settings.flaresSoftness = Slider("Softness", "The flares softness of the shockwave [0, 10]. Default 6.", settings.flaresSoftness, 0.0f, 10.0f, 6.0f);
      IndentLevel--;

      Label("Inside zone");
      IndentLevel++;

      Label("Color");
      IndentLevel++;
      settings.colorBlend = (ColorBlends)EnumPopup("Blend", "The color blend of the shockwave. Default Solid.", settings.colorBlend, ColorBlends.Solid);
      settings.insideTint = ColorField("Tint", "The inside zone tint color. Default light cyan.", settings.insideTint, Shockwave.Settings.InsideTintDefault);
      settings.contrast = Slider("Contrast", "Contrast [0.0, 2.0]. Default 1.", settings.contrast, 0.0f, 2.0f, 1.0f);
      settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
      settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
      settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
      settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.0.", settings.saturation, 0.0f, 2.0f, 1.0f);
      IndentLevel--;

      settings.edge = Slider("Edges", "Edge highlight amount [0, 1]. Default 1.0", settings.edge, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.edgeColorBlend = (ColorBlends)EnumPopup("Blend", "Edge color blend mode. Default Hue.", settings.edgeColorBlend, ColorBlends.Hue);
      settings.edgeColor = ColorField("Color", "Edge color/tint. Default Cyan.", settings.edgeColor, Color.cyan);
      settings.edgeWidth = Slider("Width", "Edge sampling width in texels [0.1, 5]. Default 1.0.", settings.edgeWidth, 0.1f, 5.0f, 1.0f);
      settings.edgeNoise = Slider("Noise", "Edge noise amount [0, 1]. Default 1.", settings.edgeNoise, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.edgeNoiseScale = Slider("Scale", "Edge noise scale [0.1, 64]. Default 8.", settings.edgeNoiseScale, 0.1f, 64.0f, 8.0f);
      settings.edgeNoiseSpeed = Slider("Speed", "Edge noise speed [0, 10]. Default 1.0.", settings.edgeNoiseSpeed, 0.0f, 10.0f, 1.0f);
      IndentLevel--;
      settings.edgePlasma = Slider("Plasma", "Edge plasma amount [0, 1]. Default 0.1", settings.edgePlasma, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.edgePlasmaScale = Slider("Scale", "Edge plasma scale [0.01, 64]. Default 5.", settings.edgePlasmaScale, 0.01f, 64.0f, 5.0f);
      settings.edgePlasmaSpeed = Slider("Speed", "Edge plasma speed [0, 10]. Default 1.0.", settings.edgePlasmaSpeed, 0.0f, 10.0f, 1.0f);
      IndentLevel--;
      settings.hueVariation = Slider("Hue variation", "Hue variation amount [0, 1]. Default 0 (disabled).", settings.hueVariation, 0.0f, 1.0f, 0.0f);
      IndentLevel++;
      settings.hueVariationScale = Slider("Angular scale", "Angular scale (bands) [0, 2]. Default 1.", settings.hueVariationScale, 0.0f, 2.0f, 1.0f);
      settings.hueVariationRadial = Slider("Radial amount", "Hue variation radial amount [0, 1]. Default 0 (disabled).", settings.hueVariationRadial, 0.0f, 1.0f, 0.0f);
      settings.hueVariationRadialScale = Slider("Radial scale", "Hue variation radial scale [0, 64]. Default 4.", settings.hueVariationRadialScale, 0.0f, 64.0f, 4.0f);
      settings.hueVariationSpeed = Slider("Speed", "Hue variation speed [-10, 10]. Default 1.0.", settings.hueVariationSpeed, -10.0f, 10.0f, 1.0f);
      IndentLevel--;
      IndentLevel--;

      IndentLevel--;

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
