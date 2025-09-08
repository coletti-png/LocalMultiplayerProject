using System;
using UnityEngine;
using FronkonGames.Artistic.OilPaint;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(OilPaintDemo))]
public class OilPaintDemoWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI() =>
    EditorGUILayout.TextArea($"\nThis code is only for the demo\n\n<b>DO NOT USE</b> it in your projects\n\nIf you have any questions,\ncheck the <a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>,\n<b>thanks!</b>\n", Style);
}
#endif

/// <summary> Artistic: Oil Paint demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class OilPaintDemo : MonoBehaviour
{
  private enum Effects
  {
    KuwaharaBasic,
    KuwaharaGeneralized,
    KuwaharaDirectional,
    KuwaharaAnisotropic,
    TomitaTsuji,
    SymmetricNearestNeighbor
  }

  private Effects effect;

  private KuwaharaBasic kuwaharaBasic;
  private KuwaharaGeneralized kuwaharaGeneralized;
  private KuwaharaDirectional kuwaharaDirectional;
  private KuwaharaAnisotropic kuwaharaAnisotropic;
  private TomitaTsuji tomitaTsuji;
  private SymmetricNearestNeighbor symmetricNearestNeighbor;

  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;

  private void ResetEffects()
  {
    UpdateEffect(Effects.TomitaTsuji);

    kuwaharaBasic.settings.ResetDefaultValues();
    kuwaharaBasic.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    kuwaharaBasic.settings.detail = OilPaint.Detail.Sharpen;
    kuwaharaBasic.settings.detailStrength = 1.0f;
    kuwaharaBasic.settings.waterColor = 0.25f;

    kuwaharaGeneralized.settings.ResetDefaultValues();
    kuwaharaGeneralized.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    kuwaharaGeneralized.settings.detail = OilPaint.Detail.Sharpen;
    kuwaharaGeneralized.settings.detailStrength = 1.0f;
    kuwaharaGeneralized.settings.waterColor = 0.25f;

    kuwaharaDirectional.settings.ResetDefaultValues();
    kuwaharaDirectional.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    kuwaharaDirectional.settings.detail = OilPaint.Detail.Sharpen;
    kuwaharaDirectional.settings.detailStrength = 1.0f;
    kuwaharaDirectional.settings.waterColor = 0.25f;

    kuwaharaAnisotropic.settings.ResetDefaultValues();
    kuwaharaAnisotropic.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    kuwaharaAnisotropic.settings.detail = OilPaint.Detail.Sharpen;
    kuwaharaAnisotropic.settings.detailStrength = 1.0f;
    kuwaharaAnisotropic.settings.waterColor = 0.25f;

    tomitaTsuji.settings.ResetDefaultValues();
    tomitaTsuji.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    tomitaTsuji.settings.detail = OilPaint.Detail.Sharpen;
    tomitaTsuji.settings.detailStrength = 1.0f;
    tomitaTsuji.settings.waterColor = 0.25f;

    symmetricNearestNeighbor.settings.ResetDefaultValues();
    symmetricNearestNeighbor.settings.radius = (int)Mathf.Clamp(Screen.width / 3840.0f * 15.0f, 4.0f, 20.0f);
    symmetricNearestNeighbor.settings.detail = OilPaint.Detail.Sharpen;
    symmetricNearestNeighbor.settings.detailStrength = 1.0f;
    symmetricNearestNeighbor.settings.waterColor = 0.25f;
  }

  private void UpdateEffect(Effects effect)
  {
    this.effect = effect;

    kuwaharaBasic?.SetActive(false);
    kuwaharaGeneralized?.SetActive(false);
    kuwaharaDirectional?.SetActive(false);
    kuwaharaAnisotropic?.SetActive(false);
    tomitaTsuji?.SetActive(false);
    symmetricNearestNeighbor?.SetActive(false);

    switch (effect)
    {
      case Effects.KuwaharaBasic:            kuwaharaBasic?.SetActive(true); break;
      case Effects.KuwaharaGeneralized:      kuwaharaGeneralized?.SetActive(true); break;
      case Effects.KuwaharaDirectional:      kuwaharaDirectional?.SetActive(true); break;
      case Effects.KuwaharaAnisotropic:      kuwaharaAnisotropic?.SetActive(true); break;
      case Effects.TomitaTsuji:              tomitaTsuji?.SetActive(true); break;
      case Effects.SymmetricNearestNeighbor: symmetricNearestNeighbor?.SetActive(true); break;
    }
  }

  private void Awake()
  {
    kuwaharaBasic = KuwaharaBasic.Instance;
    kuwaharaGeneralized = KuwaharaGeneralized.Instance;
    kuwaharaDirectional = KuwaharaDirectional.Instance;
    kuwaharaAnisotropic = KuwaharaAnisotropic.Instance;
    tomitaTsuji = TomitaTsuji.Instance;
    symmetricNearestNeighbor = SymmetricNearestNeighbor.Instance;

    if (kuwaharaBasic == null ||
        kuwaharaGeneralized == null ||
        kuwaharaDirectional == null ||
        kuwaharaAnisotropic == null ||
        tomitaTsuji == null ||
        symmetricNearestNeighbor == null)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
      this.enabled = false;
    }
    else
      ResetEffects();
  }

  private void OnGUI()
  {
    styleTitle ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.LowerCenter,
      fontSize = 32,
      fontStyle = FontStyle.Bold
    };

    styleLabel ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontSize = 24
    };

    styleButton ??= new GUIStyle(GUI.skin.button)
    {
      fontSize = 24
    };

    GUILayout.BeginHorizontal();
    {
      GUILayout.BeginVertical("box", GUILayout.Width(450.0f), GUILayout.Height(Screen.height));
      {
        const float space = 10.0f;

        GUILayout.Space(space);

        GUILayout.Label("OIL PAINT DEMO", styleTitle);

        Effects newEffect = EnumField("", effect);
        if (newEffect != effect)
          UpdateEffect(newEffect);

        GUILayout.Space(space);

        switch (effect)
        {
          case Effects.KuwaharaBasic:
            kuwaharaBasic.settings.intensity = SliderField("Intensity", kuwaharaBasic.settings.intensity);
            kuwaharaBasic.settings.passes = SliderField("Passes", kuwaharaBasic.settings.passes, 1, 4);
            kuwaharaBasic.settings.radius = SliderField("Radius", kuwaharaBasic.settings.radius, 1, 20);
            break;
          case Effects.KuwaharaGeneralized:
            kuwaharaGeneralized.settings.intensity = SliderField("Intensity", kuwaharaGeneralized.settings.intensity);
            kuwaharaGeneralized.settings.passes = SliderField("Passes", kuwaharaGeneralized.settings.passes, 1, 4);
            kuwaharaGeneralized.settings.radius = SliderField("Radius", kuwaharaGeneralized.settings.radius, 1, 20);
            kuwaharaGeneralized.settings.sharpness = SliderField("Sharpness", kuwaharaGeneralized.settings.sharpness, 0.0f, 18.0f);
            kuwaharaGeneralized.settings.hardness = SliderField("Hardness", kuwaharaGeneralized.settings.hardness, 1.0f, 100.0f);
            break;
          case Effects.KuwaharaDirectional:
            kuwaharaDirectional.settings.intensity = SliderField("Intensity", kuwaharaDirectional.settings.intensity);
            kuwaharaDirectional.settings.passes = SliderField("Passes", kuwaharaDirectional.settings.passes, 1, 4);
            kuwaharaDirectional.settings.radius = SliderField("Radius", kuwaharaDirectional.settings.radius, 1, 20);
            break;
          case Effects.KuwaharaAnisotropic:
            kuwaharaAnisotropic.settings.intensity = SliderField("Intensity", kuwaharaAnisotropic.settings.intensity);
            kuwaharaAnisotropic.settings.passes = SliderField("Passes", kuwaharaAnisotropic.settings.passes, 1, 4);
            kuwaharaAnisotropic.settings.radius = SliderField("Radius", kuwaharaAnisotropic.settings.radius, 1, 20);
            kuwaharaAnisotropic.settings.sharpness = SliderField("Sharpness", kuwaharaAnisotropic.settings.sharpness, 0.0f, 18.0f);
            kuwaharaAnisotropic.settings.hardness = SliderField("Hardness", kuwaharaAnisotropic.settings.hardness, 1.0f, 100.0f);
            kuwaharaAnisotropic.settings.alpha = SliderField("Alpha", kuwaharaAnisotropic.settings.alpha, 0.01f, 2.0f);
            kuwaharaAnisotropic.settings.zeroCrossing = SliderField("Zero Crossing", kuwaharaAnisotropic.settings.zeroCrossing, 0.25f, 2.0f);
            break;
          case Effects.TomitaTsuji:
            tomitaTsuji.settings.intensity = SliderField("Intensity", tomitaTsuji.settings.intensity);
            tomitaTsuji.settings.passes = SliderField("Passes", tomitaTsuji.settings.passes, 1, 4);
            tomitaTsuji.settings.radius = SliderField("Radius", tomitaTsuji.settings.radius, 1, 20);
            break;
          case Effects.SymmetricNearestNeighbor:
            symmetricNearestNeighbor.settings.intensity = SliderField("Intensity", symmetricNearestNeighbor.settings.intensity);
            symmetricNearestNeighbor.settings.passes = SliderField("Passes", symmetricNearestNeighbor.settings.passes, 1, 4);
            symmetricNearestNeighbor.settings.radius = SliderField("Radius", symmetricNearestNeighbor.settings.radius, 1, 20);
            break;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          ResetEffects();

        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);

        GUILayout.Space(space * 2.0f);
      }
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();
    }
    GUILayout.EndHorizontal();
  }

  private void OnDestroy() => ResetEffects();

  private bool ToggleField(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.Toggle(value, string.Empty);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float SliderField(string label, float value, float min = 0.0f, float max = 1.0f)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private int SliderField(string label, int value, int min, int max)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      value = (int)GUILayout.HorizontalSlider(value, min, max);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Color ColorField(string label, Color value, bool alpha = true)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      float originalAlpha = value.a;

      UnityEngine.Color.RGBToHSV(value, out float h, out float s, out float v);
      h = GUILayout.HorizontalSlider(h, 0.0f, 1.0f);
      value = UnityEngine.Color.HSVToRGB(h, s, v);

      if (alpha == false)
        value.a = originalAlpha;
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private Vector3 Vector3Field(string label, Vector3 value, string x = "X", string y = "Y", string z = "Z", float min = 0.0f, float max = 1.0f)
  {
    GUILayout.Label(label, styleLabel);

    value.x = SliderField($"   {x}", value.x, min, max);
    value.y = SliderField($"   {y}", value.y, min, max);
    value.z = SliderField($"   {z}", value.z, min, max);

    return value;
  }

  private T EnumField<T>(string label, T value) where T : Enum
  {
    string[] names = System.Enum.GetNames(typeof(T));
    Array values = System.Enum.GetValues(typeof(T));
    int index = Array.IndexOf(values, value);

    GUILayout.BeginHorizontal();
    {
      GUILayout.Label(label, styleLabel);

      if (GUILayout.Button("<", styleButton) == true)
        index = index > 0 ? index - 1 : values.Length - 1;

      GUILayout.Label(names[index], styleLabel);

      if (GUILayout.Button(">", styleButton) == true)
        index = index < values.Length - 1 ? index + 1 : 0;
    }
    GUILayout.EndHorizontal();

    return (T)(object)index;
  }
}
