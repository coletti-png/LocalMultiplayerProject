using System;
using System.Collections;
using UnityEngine;
using FronkonGames.Artistic.Shockwave;

/// <summary> Artistic: Shockwave demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class ShockwaveDemo : MonoBehaviour
{
  [Header("This is just a demo, not for production environments ;)"), Space(20.0f)]

  [SerializeField, Tooltip("The key to trigger the shockwave (open or close)")]
  private KeyCode key = KeyCode.Space;

  [SerializeField, Tooltip("The time of the shockwave opening")]
  private float openTime = 1.0f;

  [SerializeField, Tooltip("The time of the shockwave closing")]
  private float closeTime = 1.0f;

  private Shockwave.Settings settings;

  private GUIStyle styleTitle;
  private GUIStyle styleLabel;
  private GUIStyle styleButton;
  private GUIStyle stylePanel;

  private IEnumerator currentCoroutine;
  private bool isOpen = false;
  private bool reset = true;

  private Vector2 scrollPosition;

  private IEnumerator ShockwaveCoroutine()
  {
    settings.radius = isOpen == true ? 1.0f : 0.0f;
    if (reset == true)
    {
      settings.ringWidthInner = isOpen == true ? 0.1f : 1.0f;
      settings.ringWidthOuter = isOpen == true ? 1.0f : 0.1f;
    }

    // Toggle the shockwave.
    float time = 0.0f;
    while (time < (isOpen == true ? openTime : closeTime))
    {
      settings.radius = isOpen == true ? 1.0f - (time / openTime) : time / closeTime;

      time += Time.deltaTime;

      yield return null;
    }

    isOpen = !isOpen;
    settings.radius = isOpen == true ? 1.0f : 0.0f;
    currentCoroutine = null;
  }

  private void ResetEffect()
  {
    openTime = closeTime = 2.0f;
    settings.ResetDefaultValues();
    settings.center = new(0.5f, 0.575f);
    settings.saturation = 0.0f;
    settings.intensity = 1.0f;
    reset = true;
  }

  private void Awake()
  {
    if (Shockwave.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    this.enabled = Shockwave.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = Shockwave.Instance.settings;
    styleTitle = styleLabel = styleButton = stylePanel = null;
    ResetEffect();
  }

  private void Update()
  {
    if (Input.GetKeyDown(key) == true && currentCoroutine == null)
      StartCoroutine(currentCoroutine = ShockwaveCoroutine());
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

    stylePanel ??= new GUIStyle(GUI.skin.box)
    {
      normal = { background = MakeTexture(new Color(0.0f, 0.0f, 0.0f, 0.85f)) }
    };

    GUILayout.BeginHorizontal();
    {
      GUILayout.BeginVertical(stylePanel, GUILayout.Width(500.0f), GUILayout.Height(Screen.height));
      {
        const float space = 10.0f;

        GUILayout.Space(space);

        GUILayout.Label("SHOCKWAVE DEMO", styleTitle);

        GUILayout.Space(space);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        {
          settings.intensity = SliderField("Intensity", settings.intensity, 0.0f, 1.0f);
          settings.radius = SliderField("Radius", settings.radius, 0.0f, 1.0f);
          closeTime = openTime = SliderField("Time", openTime, 0.0f, 5.0f);

          GUILayout.Space(10);

          // Shockwave core
          GUILayout.Label("Shockwave", styleTitle);
          settings.shockwaveColorBlend = EnumField("Shockwave Blend", settings.shockwaveColorBlend);
          settings.center.x = SliderField("Center X", settings.center.x, 0.0f, 1.0f);
          settings.center.y = SliderField("Center Y", settings.center.y, 0.0f, 1.0f);
          settings.width = SliderField("Width", settings.width, 0.01f, 0.75f);
          settings.colorStrength = Vector3Field("Color strength", settings.colorStrength, "R", "G", "B", 0.0f, 5.0f);
          settings.strength = SliderField("Strength", settings.strength, 0.0f, 5.0f);

          // Chromatic aberration per channel
          GUILayout.Label("Chromatic aberration", styleLabel);
          settings.chromaticAberration.x = SliderField("   R", settings.chromaticAberration.x, -10.0f, 10.0f);
          settings.chromaticAberration.y = SliderField("   G", settings.chromaticAberration.y, -10.0f, 10.0f);
          settings.chromaticAberration.z = SliderField("   B", settings.chromaticAberration.z, -10.0f, 10.0f);
          settings.insideTint = ColorField("Inside Tint", settings.insideTint, false);

          GUILayout.Space(10);

          // Flares
          GUILayout.Label("Flares", styleTitle);
          settings.flares = SliderField("Amount", settings.flares, 0.0f, 1.0f);
          settings.flaresColorBlend = EnumField("Blend", settings.flaresColorBlend);
          settings.flaresColor = ColorField("Color", settings.flaresColor, false);
          settings.flaresFrequency = SliderField("Frequency", settings.flaresFrequency, 0.0f, 64.0f);
          settings.flaresSpeed = SliderField("Speed", settings.flaresSpeed, 0.0f, 5.0f);
          settings.flaresThreshold = SliderField("Threshold", settings.flaresThreshold, 0.0f, 1.0f);
          settings.flaresSoftness = SliderField("Softness", settings.flaresSoftness, 0.0f, 10.0f);

          GUILayout.Space(10);

          // Global noise (flow)
          GUILayout.Label("Noise", styleTitle);
          settings.noise = SliderField("Amount", settings.noise, 0.0f, 1.0f);
          settings.noiseScale = SliderField("Scale", settings.noiseScale, 0.1f, 64.0f);
          settings.noiseSpeed = SliderField("Speed", settings.noiseSpeed, 0.0f, 5.0f);

          GUILayout.Space(10);

          // Edge
          GUILayout.Label("Edges", styleTitle);
          settings.edge = SliderField("Amount", settings.edge, 0.0f, 1.0f);
          settings.edgeColorBlend = EnumField("Blend", settings.edgeColorBlend);
          settings.edgeColor = ColorField("Color", settings.edgeColor, false);
          settings.edgeWidth = SliderField("Width", settings.edgeWidth, 0.1f, 5.0f);
          settings.edgeNoise = SliderField("Noise", settings.edgeNoise, 0.0f, 1.0f);
          settings.edgeNoiseScale = SliderField("Noise Scale", settings.edgeNoiseScale, 0.1f, 64.0f);
          settings.edgeNoiseSpeed = SliderField("Noise Speed", settings.edgeNoiseSpeed, 0.0f, 10.0f);
          settings.edgePlasma = SliderField("Plasma", settings.edgePlasma, 0.0f, 1.0f);
          settings.edgePlasmaScale = SliderField("Plasma Scale", settings.edgePlasmaScale, 0.01f, 64.0f);
          settings.edgePlasmaSpeed = SliderField("Plasma Speed", settings.edgePlasmaSpeed, 0.0f, 10.0f);

          GUILayout.Space(10);

          // Hue variation in interior
          GUILayout.Label("Hue variation", styleTitle);
          settings.hueVariation = SliderField("Amount", settings.hueVariation, 0.0f, 1.0f);
          settings.hueVariationScale = SliderField("Scale", settings.hueVariationScale, 0.0f, 2.0f);
          settings.hueVariationSpeed = SliderField("Speed", settings.hueVariationSpeed, 0.0f, 10.0f);

          GUILayout.Space(10);

          // Color grading (inside zone)
          GUILayout.Label("Color (inside)", styleTitle);
          settings.colorBlend = EnumField("Blend", settings.colorBlend);
          settings.contrast = SliderField("Contrast", settings.contrast, 0.0f, 2.0f);
          settings.brightness = SliderField("Brightness", settings.brightness, -1.0f, 1.0f);
          settings.gamma = SliderField("Gamma", settings.gamma, 0.1f, 10.0f);
          settings.hue = SliderField("Hue", settings.hue, 0.0f, 1.0f);
          settings.saturation = SliderField("Saturation", settings.saturation, 0.0f, 2.0f);
        }
        GUILayout.EndScrollView();

        GUILayout.Space(space * 2.0f);

        GUI.enabled = currentCoroutine == null;

        if (GUILayout.Button(isOpen == true ? "CLOSE" : "OPEN", styleButton) == true)
          StartCoroutine(currentCoroutine = ShockwaveCoroutine());

        GUI.enabled = true;

        GUILayout.Space(space * 2.0f);

        GUILayout.Label("Presets", styleTitle);
        GUILayout.BeginHorizontal();
        {
          if (GUILayout.Button("Subtle Warp", styleButton) == true) ApplyPresetSubtleWarp();
          if (GUILayout.Button("Time Tunnel", styleButton) == true) ApplyPresetTimeTunnel();
          if (GUILayout.Button("Anomaly Pulse", styleButton) == true) ApplyPresetAnomalyPulse();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
          if (GUILayout.Button("Explosive Ring", styleButton) == true) ApplyPresetExplosiveRing();
          if (GUILayout.Button("Monochrome Ripple", styleButton) == true) ApplyPresetMonochromeRipple();
          if (GUILayout.Button("Neon Vortex", styleButton) == true) ApplyPresetNeonVortex();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
          if (GUILayout.Button("Crystal Freeze", styleButton) == true) ApplyPresetCrystalFreeze();
          if (GUILayout.Button("Heat Haze", styleButton) == true) ApplyPresetHeatHaze();
          if (GUILayout.Button("Aurora Gate", styleButton) == true) ApplyPresetAuroraGate();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
          if (GUILayout.Button("Hyperdrive", styleButton) == true) ApplyPresetHyperdrive();
          if (GUILayout.Button("Singularity", styleButton) == true) ApplyPresetSingularity();
          if (GUILayout.Button("Shockfront Scan", styleButton) == true) ApplyPresetShockfrontScan();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        {
          if (GUILayout.Button("Retro Scanlines", styleButton) == true) ApplyPresetRetroScanlines();
          if (GUILayout.Button("Chromatic Bloom", styleButton) == true) ApplyPresetChromaticBloom();
        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          ResetEffect();
/*
        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);
*/
        GUILayout.Space(space * 2.0f);
      }
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();
    }
    GUILayout.EndHorizontal();
  }

  private void OnDestroy()
  {
    ResetEffect();
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

  private void ApplyPresetSubtleWarp()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.55f);
    settings.width = 0.40f;
    settings.ringWidthInner = 0.50f;
    settings.ringWidthOuter = 0.50f;
    settings.ringSharpness = 6.0f;
    settings.ringSkew = 0.0f;
    settings.strength = 0.8f;
    settings.chromaticAberration = new(-0.2f, 0.5f, 1.0f);
    settings.noise = 0.10f; settings.noiseScale = 6.0f; settings.noiseSpeed = 0.4f;
    settings.edge = 0.30f; settings.edgeWidth = 1.5f;
    settings.edgeNoise = 0.40f; settings.edgeNoiseScale = 6.0f; settings.edgeNoiseSpeed = 0.8f;
    settings.edgePlasma = 0.0f;
    settings.hueVariation = 0.15f; settings.hueVariationScale = 0.7f; settings.hueVariationRadial = 0.10f; settings.hueVariationRadialScale = 2.0f; settings.hueVariationSpeed = 0.8f;
    settings.flares = 0.05f; settings.flaresFrequency = 20.0f; settings.flaresSpeed = 1.0f; settings.flaresThreshold = 0.4f; settings.flaresSoftness = 6.0f;
    settings.saturation = 1.0f; settings.contrast = 1.0f; settings.brightness = 0.0f; settings.gamma = 1.0f;
  }

  private void ApplyPresetTimeTunnel()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.22f;
    settings.ringWidthInner = 0.35f; settings.ringWidthOuter = 0.55f;
    settings.ringSharpness = 12.0f; settings.ringSkew = 0.08f;
    settings.strength = 1.6f;
    settings.chromaticAberration = Shockwave.Settings.ChromaticAberrationDefault;
    settings.noise = 0.35f; settings.noiseScale = 8.0f; settings.noiseSpeed = 1.2f;
    settings.edge = 0.80f; settings.edgeWidth = 1.2f;
    settings.edgeNoise = 1.00f; settings.edgeNoiseScale = 8.0f; settings.edgeNoiseSpeed = 1.2f;
    settings.edgePlasma = 0.1f; settings.edgePlasmaScale = 5.0f; settings.edgePlasmaSpeed = 1.2f;
    settings.hueVariation = 0.35f; settings.hueVariationScale = 1.0f; settings.hueVariationRadial = 0.50f; settings.hueVariationRadialScale = 8.0f; settings.hueVariationSpeed = 2.0f;
    settings.flares = 0.20f; settings.flaresFrequency = 18.0f; settings.flaresSpeed = 2.0f; settings.flaresThreshold = 0.30f; settings.flaresSoftness = 6.0f;
    settings.saturation = 1.2f; settings.contrast = 1.05f;
  }

  private void ApplyPresetAnomalyPulse()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.52f);
    settings.width = 0.30f;
    settings.ringWidthInner = 0.40f; settings.ringWidthOuter = 0.60f;
    settings.ringSharpness = 8.0f; settings.ringSkew = -0.12f;
    settings.strength = 1.3f;
    settings.chromaticAberration = new(-0.5f, 1.0f, 2.0f);
    settings.noise = 0.50f; settings.noiseScale = 10.0f; settings.noiseSpeed = 1.5f;
    settings.edge = 1.00f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.80f; settings.edgeNoiseScale = 10.0f; settings.edgeNoiseSpeed = 1.6f;
    settings.edgePlasma = 0.3f; settings.edgePlasmaScale = 7.0f; settings.edgePlasmaSpeed = 1.5f;
    settings.hueVariation = 0.60f; settings.hueVariationScale = 1.0f; settings.hueVariationRadial = 0.30f; settings.hueVariationRadialScale = 6.0f; settings.hueVariationSpeed = 3.0f;
    settings.flares = 0.35f; settings.flaresFrequency = 22.0f; settings.flaresSpeed = 2.5f; settings.flaresThreshold = 0.32f; settings.flaresSoftness = 7.0f;
    settings.insideTint = new Color(0.80f, 0.95f, 1.0f);
    settings.contrast = 1.10f;
  }

  private void ApplyPresetExplosiveRing()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.12f;
    settings.ringWidthInner = 0.22f; settings.ringWidthOuter = 0.32f;
    settings.ringSharpness = 20.0f; settings.ringSkew = 0.25f;
    settings.strength = 2.5f;
    settings.chromaticAberration = Shockwave.Settings.ChromaticAberrationDefault;
    settings.noise = 0.25f; settings.noiseScale = 7.0f; settings.noiseSpeed = 1.0f;
    settings.edge = 1.00f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 1.00f; settings.edgeNoiseScale = 10.0f; settings.edgeNoiseSpeed = 2.0f;
    settings.edgePlasma = 0.40f; settings.edgePlasmaScale = 6.0f; settings.edgePlasmaSpeed = 2.5f;
    settings.hueVariation = 0.20f; settings.hueVariationScale = 1.0f; settings.hueVariationRadial = 0.10f; settings.hueVariationRadialScale = 4.0f; settings.hueVariationSpeed = 4.0f;
    settings.flares = 0.60f; settings.flaresFrequency = 28.0f; settings.flaresSpeed = 3.0f; settings.flaresThreshold = 0.25f; settings.flaresSoftness = 8.0f;
    settings.saturation = 1.5f; settings.contrast = 1.15f;
  }

  private void ApplyPresetMonochromeRipple()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.52f);
    settings.width = 0.34f;
    settings.ringWidthInner = 0.44f; settings.ringWidthOuter = 0.44f;
    settings.ringSharpness = 10.0f; settings.ringSkew = 0.0f;
    settings.strength = 0.9f;
    settings.chromaticAberration = Vector3.zero;
    settings.noise = 0.25f; settings.noiseScale = 6.0f; settings.noiseSpeed = 0.8f;
    settings.edge = 0.60f; settings.edgeWidth = 1.4f;
    settings.edgeColorBlend = ColorBlends.Solid; settings.edgeColor = Color.white;
    settings.edgeNoise = 0.50f; settings.edgeNoiseScale = 8.0f; settings.edgeNoiseSpeed = 1.0f;
    settings.edgePlasma = 0.0f;
    settings.hueVariation = 0.0f; settings.hueVariationRadial = 0.0f;
    settings.saturation = 0.0f; settings.contrast = 1.0f; settings.brightness = 0.0f;
    settings.flares = 0.0f;
  }

  private void ApplyPresetNeonVortex()
  {
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.20f;
    settings.ringWidthInner = 0.30f; settings.ringWidthOuter = 0.50f;
    settings.ringSharpness = 14.0f; settings.ringSkew = -0.05f;
    settings.strength = 1.7f;
    settings.chromaticAberration = new(-1.0f, 2.0f, 5.0f);
    settings.noise = 0.40f; settings.noiseScale = 12.0f; settings.noiseSpeed = 1.8f;
    settings.edge = 0.90f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.90f; settings.edgeNoiseScale = 12.0f; settings.edgeNoiseSpeed = 1.8f;
    settings.edgePlasma = 0.45f; settings.edgePlasmaScale = 5.5f; settings.edgePlasmaSpeed = 2.2f;
    settings.edgeColorBlend = ColorBlends.Hue; settings.edgeColor = new Color(1.0f, 0.0f, 0.8f);
    settings.hueVariation = 0.45f; settings.hueVariationScale = 1.2f; settings.hueVariationRadial = 0.25f; settings.hueVariationRadialScale = 10.0f; settings.hueVariationSpeed = 3.2f;
    settings.flares = 0.25f; settings.flaresFrequency = 24.0f; settings.flaresSpeed = 2.2f; settings.flaresThreshold = 0.34f; settings.flaresSoftness = 6.0f;
    settings.saturation = 1.3f;
  }

  private void ApplyPresetCrystalFreeze()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.53f);
    settings.width = 0.28f;
    settings.ringWidthInner = 0.48f; settings.ringWidthOuter = 0.52f;
    settings.ringSharpness = 10.0f; settings.ringSkew = 0.0f;
    settings.strength = 1.1f;
    settings.chromaticAberration = new(-0.4f, 0.8f, 1.6f);
    settings.noise = 0.20f; settings.noiseScale = 7.0f; settings.noiseSpeed = 0.6f;
    settings.edge = 0.9f; settings.edgeWidth = 1.2f;
    settings.edgeColorBlend = ColorBlends.Solid; settings.edgeColor = new Color(0.75f, 0.95f, 1.0f);
    settings.edgeNoise = 0.6f; settings.edgeNoiseScale = 9.0f; settings.edgeNoiseSpeed = 0.9f;
    settings.edgePlasma = 0.15f; settings.edgePlasmaScale = 5.0f; settings.edgePlasmaSpeed = 1.0f;
    settings.hueVariation = 0.10f; settings.hueVariationScale = 0.8f; settings.hueVariationRadial = 0.15f; settings.hueVariationRadialScale = 5.0f; settings.hueVariationSpeed = 0.6f;
    settings.insideTint = new Color(0.80f, 0.95f, 1.0f);
    settings.saturation = 0.6f; settings.contrast = 1.05f;
    settings.flares = 0.12f; settings.flaresFrequency = 20.0f; settings.flaresSpeed = 0.8f; settings.flaresThreshold = 0.4f; settings.flaresSoftness = 6.5f;
  }

  private void ApplyPresetHeatHaze()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.48f);
    settings.width = 0.36f;
    settings.ringWidthInner = 0.55f; settings.ringWidthOuter = 0.45f;
    settings.ringSharpness = 6.0f; settings.ringSkew = 0.10f;
    settings.strength = 0.9f;
    settings.chromaticAberration = new(0.2f, -0.4f, -0.8f);
    settings.noise = 0.55f; settings.noiseScale = 14.0f; settings.noiseSpeed = 2.2f;
    settings.edge = 0.4f; settings.edgeWidth = 1.3f;
    settings.edgeNoise = 0.9f; settings.edgeNoiseScale = 14.0f; settings.edgeNoiseSpeed = 2.0f;
    settings.edgePlasma = 0.20f; settings.edgePlasmaScale = 4.5f; settings.edgePlasmaSpeed = 1.8f;
    settings.hueVariation = 0.12f; settings.hueVariationScale = 0.5f; settings.hueVariationRadial = 0.08f; settings.hueVariationRadialScale = 3.0f; settings.hueVariationSpeed = 1.5f;
    settings.insideTint = new Color(1.0f, 0.95f, 0.85f);
    settings.saturation = 1.1f; settings.brightness = 0.05f;
  }

  private void ApplyPresetAuroraGate()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.26f;
    settings.ringWidthInner = 0.38f; settings.ringWidthOuter = 0.62f;
    settings.ringSharpness = 9.0f; settings.ringSkew = -0.06f;
    settings.strength = 1.4f;
    settings.chromaticAberration = new(-0.8f, 1.6f, 3.2f);
    settings.noise = 0.30f; settings.noiseScale = 9.0f; settings.noiseSpeed = 1.2f;
    settings.edge = 0.95f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.8f; settings.edgeNoiseScale = 11.0f; settings.edgeNoiseSpeed = 1.4f;
    settings.edgePlasma = 0.5f; settings.edgePlasmaScale = 5.0f; settings.edgePlasmaSpeed = 1.5f;
    settings.edgeColorBlend = ColorBlends.Hue; settings.edgeColor = new Color(0.2f, 1.0f, 0.8f);
    settings.hueVariation = 0.55f; settings.hueVariationScale = 1.4f; settings.hueVariationRadial = 0.35f; settings.hueVariationRadialScale = 12.0f; settings.hueVariationSpeed = 2.6f;
    settings.flares = 0.28f; settings.flaresFrequency = 26.0f; settings.flaresSpeed = 2.0f; settings.flaresThreshold = 0.33f; settings.flaresSoftness = 6.0f;
    settings.saturation = 1.2f;
  }

  private void ApplyPresetHyperdrive()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.16f;
    settings.ringWidthInner = 0.26f; settings.ringWidthOuter = 0.28f;
    settings.ringSharpness = 24.0f; settings.ringSkew = 0.20f;
    settings.strength = 2.8f;
    settings.chromaticAberration = Shockwave.Settings.ChromaticAberrationDefault;
    settings.noise = 0.20f; settings.noiseScale = 8.0f; settings.noiseSpeed = 1.0f;
    settings.edge = 1.0f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.7f; settings.edgeNoiseScale = 10.0f; settings.edgeNoiseSpeed = 1.6f;
    settings.edgePlasma = 0.35f; settings.edgePlasmaScale = 6.0f; settings.edgePlasmaSpeed = 2.8f;
    settings.hueVariation = 0.30f; settings.hueVariationScale = 1.0f; settings.hueVariationRadial = 0.20f; settings.hueVariationRadialScale = 6.0f; settings.hueVariationSpeed = 3.5f;
    settings.flares = 0.75f; settings.flaresFrequency = 30.0f; settings.flaresSpeed = 3.5f; settings.flaresThreshold = 0.22f; settings.flaresSoftness = 9.0f;
    settings.saturation = 1.4f; settings.contrast = 1.12f;
  }

  private void ApplyPresetSingularity()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.52f);
    settings.width = 0.18f;
    settings.ringWidthInner = 0.12f; settings.ringWidthOuter = 0.42f;
    settings.ringSharpness = 22.0f; settings.ringSkew = -0.30f;
    settings.strength = 2.0f;
    settings.chromaticAberration = new(-0.5f, 1.0f, 2.0f);
    settings.noise = 0.35f; settings.noiseScale = 9.0f; settings.noiseSpeed = 1.4f;
    settings.edge = 0.95f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.9f; settings.edgeNoiseScale = 12.0f; settings.edgeNoiseSpeed = 2.0f;
    settings.edgePlasma = 0.25f; settings.edgePlasmaScale = 6.5f; settings.edgePlasmaSpeed = 2.0f;
    settings.hueVariation = 0.20f; settings.hueVariationScale = 0.8f; settings.hueVariationRadial = 0.40f; settings.hueVariationRadialScale = 10.0f; settings.hueVariationSpeed = -2.5f;
    settings.flares = 0.15f; settings.flaresFrequency = 16.0f; settings.flaresSpeed = 1.6f; settings.flaresThreshold = 0.38f; settings.flaresSoftness = 6.0f;
    settings.saturation = 0.7f; settings.brightness = -0.02f;
  }

  private void ApplyPresetShockfrontScan()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.24f;
    settings.ringWidthInner = 0.34f; settings.ringWidthOuter = 0.34f;
    settings.ringSharpness = 12.0f; settings.ringSkew = 0.0f;
    settings.strength = 1.2f;
    settings.chromaticAberration = new(-0.2f, 0.4f, 0.8f);
    settings.noise = 0.20f; settings.noiseScale = 9.0f; settings.noiseSpeed = 1.2f;
    settings.edge = 0.85f; settings.edgeWidth = 1.5f;
    settings.edgeNoise = 0.30f; settings.edgeNoiseScale = 9.0f; settings.edgeNoiseSpeed = 1.0f;
    settings.edgePlasma = 0.10f; settings.edgePlasmaScale = 4.0f; settings.edgePlasmaSpeed = 1.0f;
    settings.hueVariation = 0.10f; settings.hueVariationScale = 0.6f; settings.hueVariationRadial = 0.05f; settings.hueVariationRadialScale = 3.0f; settings.hueVariationSpeed = 1.0f;
    settings.flares = 0.50f; settings.flaresFrequency = 36.0f; settings.flaresSpeed = 3.0f; settings.flaresThreshold = 0.42f; settings.flaresSoftness = 5.0f;
  }

  private void ApplyPresetRetroScanlines()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.32f;
    settings.ringWidthInner = 0.50f; settings.ringWidthOuter = 0.50f;
    settings.ringSharpness = 8.0f; settings.ringSkew = 0.0f;
    settings.strength = 0.7f;
    settings.chromaticAberration = Vector3.zero;
    settings.noise = 0.15f; settings.noiseScale = 64.0f; settings.noiseSpeed = 0.2f;
    settings.edge = 0.70f; settings.edgeWidth = 1.6f;
    settings.edgeColorBlend = ColorBlends.Solid; settings.edgeColor = Color.white;
    settings.edgeNoise = 0.10f; settings.edgeNoiseScale = 18.0f; settings.edgeNoiseSpeed = 0.3f;
    settings.edgePlasma = 0.0f;
    settings.hueVariation = 0.0f; settings.hueVariationRadial = 0.0f;
    settings.saturation = 0.0f; settings.contrast = 1.0f; settings.brightness = 0.0f;
    settings.flares = 0.0f;
  }

  private void ApplyPresetChromaticBloom()
  {
    reset = false;
    settings.intensity = 1.0f;
    settings.center = new(0.5f, 0.5f);
    settings.width = 0.20f;
    settings.ringWidthInner = 0.28f; settings.ringWidthOuter = 0.28f;
    settings.ringSharpness = 16.0f; settings.ringSkew = 0.0f;
    settings.strength = 1.8f;
    settings.chromaticAberration = new(-1.0f, 2.0f, 5.0f);
    settings.noise = 0.18f; settings.noiseScale = 8.0f; settings.noiseSpeed = 1.0f;
    settings.edge = 0.9f; settings.edgeWidth = 1.0f;
    settings.edgeNoise = 0.4f; settings.edgeNoiseScale = 10.0f; settings.edgeNoiseSpeed = 1.0f;
    settings.edgePlasma = 0.30f; settings.edgePlasmaScale = 6.0f; settings.edgePlasmaSpeed = 1.8f;
    settings.hueVariation = 0.25f; settings.hueVariationScale = 1.0f; settings.hueVariationRadial = 0.15f; settings.hueVariationRadialScale = 5.0f; settings.hueVariationSpeed = 2.4f;
    settings.flares = 0.45f; settings.flaresFrequency = 26.0f; settings.flaresSpeed = 2.4f; settings.flaresThreshold = 0.28f; settings.flaresSoftness = 8.0f;
    settings.saturation = 1.6f; settings.contrast = 1.1f;
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

  private Texture2D MakeTexture(Color color)
  {
    Texture2D texture = new(1, 1, TextureFormat.ARGB32, false);
    texture.SetPixel(0, 0, color);
    texture.Apply();
    return texture;
  }
}
