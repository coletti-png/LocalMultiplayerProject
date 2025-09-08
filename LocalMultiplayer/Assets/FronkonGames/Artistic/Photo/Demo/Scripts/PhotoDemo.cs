using System;
using UnityEngine;
using FronkonGames.Artistic.Photo;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PhotoDemo))]
public class PhotoWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI()
  {
    this.DrawDefaultInspector();
    GUILayout.TextArea("This code is only for the demo\n\nIf you have any questions, check the\n<a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>", Style);
  }
}
#endif

/// <summary> Artistic: Color Isolation demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class PhotoDemo : MonoBehaviour
{
  private Photo.Settings settings;

  private int preset = -1;

  private GUIStyle styleLabel;
  private GUIStyle styleButton;

  private bool hideUI;
  private float originalRings;
  private float originalGrid;

  [Header("Focus Effect Settings")]
  [SerializeField] private float focusIntensity = 0.5f;
  [SerializeField] private float focusSmoothing = 5.0f;
  [SerializeField] private float focusReturnDelay = 0.2f;
  [SerializeField] private float focusReturnSpeed = 3.0f;
  [SerializeField] private float zoomVelocityMultiplier = 20.0f;

  [Header("Sound Settings")]
  [SerializeField] private AudioClip motorSound;
  [SerializeField] private float motorVolume = 0.5f;
  [SerializeField] private float motorFadeSpeed = 3.0f;
  [SerializeField] private float minMotorPitch = 0.8f;
  [SerializeField] private float maxMotorPitch = 1.2f;

  private float lastZoomValue = 0.0f;
  private bool isZooming = false;
  private float focusReturnTimer = 0.0f;
  private float currentFocusTarget = 0.0f;
  private float currentFocusValue = 0.0f;
  private AudioSource motorAudioSource;
  private float currentMotorVolume = 0.0f;

  private readonly float height = 22.0f;

  private void Awake()
  {
    if (Photo.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
      this.enabled = false;
    }
    
    motorAudioSource = gameObject.AddComponent<AudioSource>();
    motorAudioSource.clip = motorSound;
    motorAudioSource.loop = true;
    motorAudioSource.volume = 0;
    motorAudioSource.playOnAwake = false;
  }

  public void OnZooming(float zoom)
  {
    isZooming = true;
    focusReturnTimer = 0.0f;
    
    float zoomVelocity = Mathf.Abs(zoom - lastZoomValue) * zoomVelocityMultiplier;
    lastZoomValue = zoom;
    
    float defocusAmount = -focusIntensity * Mathf.Sin(zoom * Mathf.PI) * (1.0f + zoomVelocity);
    currentFocusTarget = Mathf.Clamp(defocusAmount, -1.0f, 1.0f);
    
    if (!motorAudioSource.isPlaying && motorSound != null)
    {
      motorAudioSource.volume = Mathf.Min(currentMotorVolume, 0.05f);
      motorAudioSource.Play();
    }
    
    float pitchFactor = Mathf.Clamp01(zoomVelocity * 0.1f);
    motorAudioSource.pitch = Mathf.Lerp(minMotorPitch, maxMotorPitch, pitchFactor);
  }
  
  public void OnZoomEnd() => isZooming = false;

  public void OnStartTakePhoto()
  {
    originalRings = settings.rings;
    originalGrid = settings.grid;
    settings.rings = 0.0f;
    settings.grid = 0.0f;

    hideUI = true;
  }

  public void OnEndTakePhoto()
  {
    hideUI = false;
    settings.rings = originalRings;
    settings.grid = originalGrid;
  }

  private readonly string[] Presets = new string[] { "Classic", "Vintage", "Cinematic", "Retro", "Nature", "Noir", "Polaroid" };

  private void SetPreset(int preset)
  {
    if (this.preset != preset)
    {
      this.preset = preset;

      settings.ResetDefaultValues();

      switch (preset)
      {
        case 0: // Classic
          settings.film = Films.Kodak_Portra_400;
          settings.vignetteSize = 0.85f;
          settings.vignetteSmoothness = 0.1f;
          settings.grain = 0.2f;
          settings.rings = 0.3f;
          settings.ringsThickness = 3.0f;
          settings.ring1Scale = 1.2f;
          settings.ring2Scale = 1.5f;
          settings.ring3Scale = 1.8f;
          settings.dust = 0.05f;
          break;
          
        case 1: // Vintage
          settings.film = Films.Agfa_Vista_400;
          settings.blur = 1;
          settings.ring1Scale = 1.5f;
          settings.ring2Scale = 2.0f;
          settings.ring3Scale = 2.0f;
          settings.ringsThickness = 5;
          settings.vignetteSize = 0.8f;
          settings.vignetteSmoothness = 0.02f;
          settings.vignetteAspect = 0.2f;
          settings.grain = 0.4f;
          settings.grid = 0.0f;
          settings.dust = 0.25f;
          settings.colorBleed = 0.2f;
          settings.lightLeak = 0.15f;
          break;
          
        case 2: // Cinematic
          settings.film = Films.Cinestill_800T;
          settings.halation = 0.5f;
          settings.ringsColor = settings.frostColor = new Color(1.0f, 1.0f, 1.0f, 0.02f);
          settings.ringsColorBlend = settings.frostColorBlend = ColorBlends.Lighten;
          settings.focusOffset = 0.5f;
          settings.ring1Scale = 1.25f;
          settings.ring2Scale = 1.25f;
          settings.ring3Scale = 1.25f;
          settings.grid = 0.0f;
          settings.vignette = Vignettes.Circular;
          settings.vignetteAspect = 1.0f;
          settings.grain = 0.0f;
          settings.lightLeak = 0.2f;
          settings.lightLeakSpeed = 1.25f;
          settings.chromaticFringing = 0.3f;
          settings.blur = 1;
          break;
          
        case 3: // Retro
          settings.film = Films.Fuji_C200;
          settings.grid = 2.0f;
          settings.ringsThickness = 5.0f;
          settings.vignette = Vignettes.None;
          settings.grain = 1.0f;
          settings.colorBleed = 0.5f;
          settings.dust = 0.3f;
          settings.dustSize = 0.6f;
          settings.halation = 0.3f;
          settings.chromaticFringing = 0.4f;
          settings.lightLeak = 0.3f;
          settings.lightLeakSpeed = 0.5f;
          break;
          
        case 4: // Nature
          settings.film = Films.Fuji_Velvia_50;
          settings.vignette = Vignettes.Rectangular;
          settings.vignetteSize = 0.8f;
          settings.vignetteAspect = 0.5f;
          settings.vignetteSmoothness = 0.0f;
          settings.chromaticFringing = 2.0f;
          settings.dust = 0.05f;
          settings.dustSize = 0.4f;
          settings.rings = 0.5f;
          settings.ringsThickness = 2.0f;
          settings.ring1Scale = 1.3f;
          settings.ring2Scale = 1.6f;
          settings.ring3Scale = 1.9f;
          settings.grain = 0.15f;
          settings.colorBleed = 0.1f;
          break;

        case 5: // Noir
          settings.film = Films.Ilford_HP5_BW;
          settings.contrast = 1.02f;
          settings.vignette = Vignettes.Circular;
          settings.vignetteSize = 0.7f;
          settings.vignetteSmoothness = 0.1f;
          settings.grain = 0.6f;
          settings.rings = 0.4f;
          settings.ringsThickness = 4.0f;
          settings.dust = 0.15f;
          settings.dustSize = 0.5f;
          settings.blur = 1;
          settings.lightLeak = 0.0f;
          settings.halation = 0.2f;
          break;
          
        case 6: // Polaroid
          settings.film = Films.Kodak_Gold_200;
          settings.brightness = 0.1f;
          settings.contrast = 1.2f;
          settings.saturation = 1.2f;
          settings.vignette = Vignettes.Rectangular;
          settings.vignetteSize = 0.85f;
          settings.vignetteSmoothness = 0.15f;
          settings.vignetteAspect = 0.9f; // More square-like for Polaroid feel
          settings.grain = 0.3f;
          settings.dust = 0.2f;
          settings.dustSize = 0.7f;
          settings.colorBleed = 0.3f;
          settings.lightLeak = 0.25f;
          settings.lightLeakSpeed = 0.0f; // Static light leak
          settings.chromaticFringing = 0.5f;
          settings.rings = 0.0f;
          settings.grid = 0.0f;
          settings.frost = 1.5f;
          settings.frostColor = new Color(1.0f, 0.95f, 0.8f, 0.2f); // Warm frost
          settings.frostColorBlend = ColorBlends.Screen;
          break;
      }
    }
  }

  private void OnEnable()
  {
    settings = Photo.Instance.settings;
    styleLabel = styleButton = null;

    SetPreset(0);
  }

  private void Update()
  {
    currentFocusValue = Mathf.Lerp(currentFocusValue, currentFocusTarget, Time.deltaTime * focusSmoothing);
    settings.focus = currentFocusValue;
    
    if (isZooming == false)
    {
      focusReturnTimer += Time.deltaTime;
      if (focusReturnTimer > focusReturnDelay)
        currentFocusTarget = Mathf.Lerp(currentFocusTarget, 0.0f, Time.deltaTime * focusReturnSpeed);
    }
    
    if (isZooming)
    {
      if (!motorAudioSource.isPlaying && motorSound != null)
        motorAudioSource.Play();
      
      float fadeInT = Time.deltaTime * motorFadeSpeed;
      currentMotorVolume = Mathf.Lerp(currentMotorVolume, motorVolume, fadeInT);
    }
    else
    {
      float fadeOutT = Time.deltaTime * motorFadeSpeed * 0.8f; // Slightly slower fade out for natural feel
      currentMotorVolume = Mathf.Lerp(currentMotorVolume, 0f, fadeOutT);
      
      if (currentMotorVolume < 0.01f && motorAudioSource.isPlaying)
      {
        motorAudioSource.Stop();
        currentMotorVolume = 0.0f;
      }
    }
    
    if (motorAudioSource != null)
      motorAudioSource.volume = currentMotorVolume;
  }

  private void OnGUI()
  {
    if (hideUI == true)
      return;

    float screenWidth = Screen.width;
    float screenHeight = Screen.height;
    float scaleFactor = Mathf.Min(screenWidth / 1920.0f, screenHeight / 1080.0f) * 3.0f;
    Matrix4x4 originalMatrix = GUI.matrix;
    
    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleFactor, scaleFactor, 1.0f));

    styleLabel ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontSize = Mathf.RoundToInt(24 / scaleFactor),
      fontStyle = FontStyle.Bold
    };

    styleButton ??= new GUIStyle(GUI.skin.button)
    {
      fontSize = Mathf.RoundToInt(24 / scaleFactor),
      fontStyle = FontStyle.Bold
    };

    float scaledWidth = screenWidth / scaleFactor;
    float scaledHeight = screenHeight / scaleFactor;
    GUILayout.BeginVertical(GUILayout.Width(scaledWidth), GUILayout.Height(scaledHeight));
    {
      GUILayout.FlexibleSpace();

      GUILayout.BeginHorizontal(GUILayout.Height(height));
      {
        GUILayout.Space(20.0f);

        GUILayout.Label("LEFT:SHOOT  RIGHT:ROTATE  WHEEL:ZOOM", styleLabel);

        GUILayout.FlexibleSpace();

        GUILayout.Label("PRESETS ", styleLabel);

        SetPreset(GUILayout.SelectionGrid(preset, Presets, Presets.Length, styleButton));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("WEB", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(20.0f);
      }
      GUILayout.EndHorizontal();
    }
    GUILayout.EndVertical();

    GUI.matrix = originalMatrix;
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

      Color.RGBToHSV(value, out float h, out float s, out float v);
      h = GUILayout.HorizontalSlider(h, 0.0f, 1.0f);
      value = Color.HSVToRGB(h, s, v);

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
    string[] names = Enum.GetNames(typeof(T));
    Array values = Enum.GetValues(typeof(T));
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
