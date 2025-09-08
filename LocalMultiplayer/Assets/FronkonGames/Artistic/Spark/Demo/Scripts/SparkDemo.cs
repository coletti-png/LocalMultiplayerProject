using System;
using UnityEngine;
using FronkonGames.Artistic.Spark;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(SparkDemo))]
public class SparkDemoWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI() =>
    EditorGUILayout.TextArea($"\nThis code is only for the demo\n\n<b>DO NOT USE</b> it in your projects\n\nIf you have any questions,\ncheck the <a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>,\n<b>thanks!</b>\n", Style);
}
#endif

/// <summary> Artistic: Spark demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class SparkDemo : MonoBehaviour
{
  private Spark.Settings settings;

  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;

  private void ResetEffect()
  {
    settings.ResetDefaultValues();
    settings.intensity = 1.0f;

    settings.size = 120.0f;
    settings.dispersion = 1.0f;
    settings.threshold = 0.5f;
  }

  private void Awake()
  {
    if (Spark.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
    }

    this.enabled = Spark.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = Spark.Instance.settings;
    ResetEffect();
  }

  private void Update()
  {
    settings.spin += Time.deltaTime * 10.0f;
    if (settings.spin > 360.0f)
      settings.spin = -360.0f;
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

        GUILayout.Label("SPARK DEMO", styleTitle);

        GUILayout.Space(space);

        settings.intensity = SliderField("Intensity", settings.intensity);

        settings.rays = SliderField("Rays", settings.rays, 1, 20);
        settings.gain = SliderField("  Gain", settings.gain, 0.0f, 500.0f);
        settings.twirl = SliderField("  Twirl", settings.twirl, -1.0f, 1.0f);
        settings.spin = SliderField("  Spin", settings.spin, -360.0f, 360.0f);
        settings.falloff = SliderField("  Falloff", settings.falloff, -2.0f, 4.0f);
        settings.aspect = SliderField("  Aspect", settings.aspect, 0.01f, 20.0f);

        GUILayout.Space(space);

        settings.barrel = SliderField("Barrel", settings.barrel, -10.0f, 10.0f);
        settings.barrelBend = SliderField("  Bend", settings.barrelBend, 1.0f, 20.0f);

        GUILayout.Space(space);

        settings.dispersion = SliderField("Distortion", settings.dispersion, 0.0f, 10.0f);
        settings.dispersionCycles = SliderField("  Cycles", settings.dispersionCycles, 0.01f, 30.0f);
        settings.dispersionOffset = SliderField("  Offset", settings.dispersionOffset, -360.0f, 360.0f);

        GUILayout.Space(space);

        settings.blur = SliderField("Blur", settings.blur, 0.0f, 100.0f);

        GUILayout.Space(space);

        settings.threshold = SliderField("Threshold", settings.threshold, 0.0f, 3.0f);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          ResetEffect();

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

  private void OnDestroy()
  {
    settings?.ResetDefaultValues();
  }

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
