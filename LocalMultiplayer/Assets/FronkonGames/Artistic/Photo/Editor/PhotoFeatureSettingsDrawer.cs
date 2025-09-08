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
using static FronkonGames.Artistic.Photo.Inspector;
using UnityEngine.Assertions.Must;

namespace FronkonGames.Artistic.Photo.Editor
{
  /// <summary> Artistic Photo Isolation inspector. </summary>
  [CustomPropertyDrawer(typeof(Photo.Settings))]
  public class PhotoFeatureSettingsDrawer : Drawer
  {
    private Photo.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Photo.Settings>();

      /////////////////////////////////////////////////
      // Photo.
      /////////////////////////////////////////////////
      Separator();

      settings.center = Vector2Field("Center", "", settings.center, Vector2.one * 0.5f);

      settings.focus = Slider("Focus", "Image focus [-1, 1]. Default 0.", settings.focus, -1.0f, 1.0f, 0.0f);
      IndentLevel++;
      settings.focusOffset = Slider("Offset", "Image shift at loss of focus [0, 1]. Default 0.25.", settings.focusOffset, 0.0f, 1.0f, 0.25f);
      settings.blur = Slider("Blur", "Blur strength [0, 10]. Default 2.", settings.blur, 0, 10, 2);
      IndentLevel--;

      settings.grid = Slider("Grid", "Points grid intensity [0, 1]. Default 0.5", settings.grid, 0.0f, 1.0f, 0.5f);
      IndentLevel++;
      settings.gridColor = ColorField("Color", "Grid color. Default black.", settings.gridColor, Color.black);
      settings.gridColorBlend = (ColorBlends)EnumPopup("Blend op", "Grid color blend mode. Default Solid.", settings.gridColorBlend, ColorBlends.Solid);
      IndentLevel--;

      settings.rings = Slider("Rings", "Black rings intensity [0, 1]. Default 0.5.", settings.rings, 0.0f, 1.0f, 0.5f);
      IndentLevel++;
      settings.ringsColor = ColorField("Color", "Rings color. Default black.", settings.ringsColor, Color.black);
      settings.ringsColorBlend = (ColorBlends)EnumPopup("Blend op", "Rings color blend mode. Default Solid.", settings.ringsColorBlend, ColorBlends.Solid);
      settings.ringsThickness = Slider("Thickness", "Black rings thickness [0, 5]. Default 1.", settings.ringsThickness, 0.0f, 5.0f, 1.0f);
      settings.ringsSharpness = Slider("Sharpness", "Rings sharpness [0, 5]. Default 1.", settings.ringsSharpness, 0.0f, 5.0f, 1.0f);
      settings.ring1Scale = Slider("Ring #1", "Ring 1 scale [0, 2]. Default 1.", settings.ring1Scale, 0.0f, 2.0f, 1.0f);
      settings.ring2Scale = Slider("Ring #2", "Ring 2 scale [0, 2]. Default 1.", settings.ring2Scale, 0.0f, 2.0f, 1.0f);
      settings.ring3Scale = Slider("Ring #3", "Ring 3 scale [0, 2]. Default 1.", settings.ring3Scale, 0.0f, 2.0f, 1.0f);
      settings.ringSplitScale = Slider("Ring split", "Ring split scale [0, 2]. Default 1.", settings.ringSplitScale, 0.0f, 2.0f, 1.0f);
      IndentLevel--;

      settings.frost = Slider("Frost", "Frost ring intensity [0, 2]. Default 1.", settings.frost, 0.0f, 2.0f, 1.0f);
      IndentLevel++;
      settings.frostColor = ColorField("Color", "Frost color. Default white.", settings.frostColor, Color.white);
      settings.frostColorBlend = (ColorBlends)EnumPopup("Blend op", "Frost color blend mode. Default Solid.", settings.frostColorBlend, ColorBlends.Solid);
      IndentLevel--;

      settings.aberration = Slider("Aberration", "Chromatic aberration effect [0, 1]. Default 0.2.", settings.aberration, 0.0f, 1.0f, 0.2f);
      IndentLevel++;
      settings.aberrationChannels = Vector3Field("Channels", "Aberration factor per color channel.", settings.aberrationChannels, Photo.Settings.DefaultAberrationChannels);
      IndentLevel--;

      settings.vignette = (Vignettes)EnumPopup("Vignette", "Vignette type.", settings.vignette, Films.None);
      if (settings.vignette != Vignettes.None)
      {
        IndentLevel++;
        settings.vignetteSize = Slider("Size", "", settings.vignetteSize, 0.0f, 2.0f, 0.7f);
        settings.vignetteSmoothness = Slider("Smoothness", "", settings.vignetteSmoothness, 0.0f, 1.0f, 0.25f);
        settings.vignetteAspect = Slider("Aspect", "", settings.vignetteAspect, 0.0f, 2.0f, 0.0f);
        IndentLevel--;
      }

      settings.grain = Slider("Grain", "", settings.grain, 0.0f, 1.0f, 0.2f);

      settings.apertureSize = Slider("Aperture", "Aperture size [0, 1]. Default 1.", settings.apertureSize, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.apertureBlades = Slider("Blades", "Number of aperture blades [3, 12]. Default 5.", settings.apertureBlades, 3, 12, 5);
      IndentLevel--;

      settings.film = (Films)EnumPopup("Film", "Film stock.", settings.film, Films.None);
      if (settings.film == Films.Cinestill_800T)
      {
        IndentLevel++;
        settings.halation = Slider("Halation", "Halation effect for Cinestill [0, 1]. Default 0.25.", settings.halation, 0.0f, 1.0f, 0.25f);
        IndentLevel--;
      }

      EditorGUILayout.BeginHorizontal();
      {
        FlexibleSpace();

        string description = string.Empty;
        switch (settings.film)
        {
          case Films.Agfa_Vista_400:       description = "cool color balance, strong blue/green response, budget film look"; break;
          case Films.Cinestill_800T:       description = "red halation, cinematic look"; break;
          case Films.Fuji_Velvia_50:       description = "high saturation, punchy contrast, enhanced red/blue response"; break;
          case Films.Fuji_C200:            description = "cool tones, blue emphasis"; break;
          case Films.Fuji_Pro_400H:        description = "soft, natural skin tones and neutral gray balance"; break;
          case Films.Lomography_Color_800: description = "heavy grain, cyan shift, crushed shadows"; break;
          case Films.ORWO_UT18:            description = "cinema film stock, green/magenta bias, soft highlight shoulder"; break;
          case Films.Kodak_Ektar_100:      description = "ultra-vivid colors, high contrast, infrared-ready base"; break;
          case Films.Kodak_Portra_400:     description = "soft highlight rolloff, enhanced greens for skin tones"; break;
          case Films.Kodak_Gold_200:       description = "golden hues, green boost"; break;
          case Films.Polaroid_600:         description = "warm tones, strong vignette"; break;
          case Films.Ilford_HP5_BW:        description = "black and white"; break;
        }

        GUILayout.Label(description, EditorStyles.miniLabel);
        GUILayout.Space(22);
      }
      EditorGUILayout.EndHorizontal();

      if (Foldout("Glitches") == true)
      {
        IndentLevel++;

        settings.chromaticFringing = Slider("Chromatic fringing", "Chromatic fringing in high contrast areas [0, 5]. Default 0.", settings.chromaticFringing, 0.0f, 5.0f, 0.0f);
        settings.dust = Slider("Dust", "Dust amount [0, 1]. Default 0.", settings.dust, 0.0f, 1.0f, 0.0f);
        IndentLevel++;
        settings.dustSize = Slider("Size", "Dust size [0.1, 2]. Default 1.", settings.dustSize, 0.1f, 2.0f, 1.0f);
        IndentLevel--;
        settings.lightLeak = Slider("Light leak", "Controls the intensity of light leaks [0, 1]. Default 0.", settings.lightLeak, 0.0f, 1.0f, 0.0f);
        IndentLevel++;
        settings.lightLeakSpeed = Slider("Speed", "Controls the animation speed of light leaks [0, 2]. Default 1.", settings.lightLeakSpeed, 0.0f, 2.0f, 1.0f);
        IndentLevel--;
        settings.colorBleed = Slider("Color bleed", "Controls the amount of color bleeding [0, 1]. Default 0.", settings.colorBleed, 0.0f, 1.0f, 0.0f);
        IndentLevel++;
        settings.colorBleedAmount = Slider("Amount", "Controls the amount of color bleeding [0, 10]. Default 2.", settings.colorBleedAmount, 0.0f, 10.0f, 2.0f);
        IndentLevel--;
        settings.expiredYears = Slider("Expired years", "Years out of date of the film [0, 30]. Default 0.", settings.expiredYears, 0.0f, 30.0f, 0.0f);

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
