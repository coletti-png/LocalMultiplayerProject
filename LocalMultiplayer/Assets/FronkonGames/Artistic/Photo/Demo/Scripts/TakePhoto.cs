using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using FronkonGames.Artistic.Photo;

/// <summary> Takes a photo when a key is pressed and displays it on screen with animation. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class TakePhoto : MonoBehaviour
{
  [Header("Photo Settings")]
  [SerializeField] private float animationDuration = 1.5f;
  [SerializeField] private Vector2 finalPosition = new(0.85f, 0.15f);
  [SerializeField] private float finalScale = 0.25f;
  [SerializeField] private float shutterDuration = 0.3f;

  [Header("Audio Settings")]
  [SerializeField] public AudioClip servoSound;
  [SerializeField] public float servoVolume = 1.0f;
  [SerializeField] public AudioClip shutterSound;
  [SerializeField] public float shutterVolume = 1.0f;

  [Header("Events")]
  [SerializeField] public UnityEvent OnTakePhotoStart = new();
  [SerializeField] public UnityEvent OnTakePhotoEnd = new();

  public bool Trigger { get; set; } = true;

  private Photo.Settings settings;
  private Texture2D photoTexture;
  private bool displayingPhoto = false;
  private float animationTime = 0.0f;
  private bool takingPhoto = false;
  private float shutterTime = 0.0f;
  private AudioSource audioSource;

  private void Awake() => this.enabled = Photo.IsInRenderFeatures();

  private void Start()
  {
    settings = Photo.Instance.settings;

    audioSource = this.gameObject.GetComponent<AudioSource>();
    if (audioSource == null)
      audioSource = this.gameObject.AddComponent<AudioSource>();
  }

  private void Update()
  {
    if (Input.mousePosition.y > Screen.height * 0.1f &&
        Input.GetMouseButton(0) &&
        (!displayingPhoto || animationTime >= animationDuration) && !takingPhoto && Trigger)
      Shoot();

    if (takingPhoto == true)
    {
      shutterTime += Time.deltaTime;
      float normalizedTime = shutterTime / shutterDuration;
      
      if (normalizedTime < 0.5f)
      {
        float t = normalizedTime * 2.0f;
        settings.apertureSize = Mathf.Lerp(1.0f, 0.0f, t);
      }
      else if (normalizedTime < 1.0f)
      {
        float t = (normalizedTime - 0.5f) * 2.0f;
        settings.apertureSize = Mathf.Lerp(0.0f, 1.0f, t);
      }
      else
      {
        takingPhoto = false;
        settings.apertureSize = 1.0f;
        
        displayingPhoto = false;
        StartCoroutine(CapturePhoto());
      }
    }
  }
  
  /// <summary>
  /// Takes a photo programmatically, with the same behavior as when triggered by user input.
  /// </summary>
  public void Shoot()
  {
    if ((!displayingPhoto || animationTime >= animationDuration) && !takingPhoto)
    {
      OnTakePhotoStart?.Invoke();

      if (photoTexture != null)
      {
        Destroy(photoTexture);
        photoTexture = null;
      }

      takingPhoto = true;
      shutterTime = 0.0f;

      if (shutterSound != null && audioSource.isPlaying == false)
        audioSource.PlayOneShot(shutterSound, shutterVolume);
    }
  }

  private IEnumerator CapturePhoto()
  {
    float originalGrid = settings.grid;
    float originalRings = settings.rings;
    settings.grid = settings.rings = 0.0f;

    yield return new WaitForEndOfFrame();

    int width = Screen.width;
    int height = Screen.height;
    RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);

    ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);

    photoTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
    RenderTexture previousActive = RenderTexture.active;
    RenderTexture.active = renderTexture;
    photoTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    photoTexture.Apply();

    RenderTexture.active = previousActive;
    RenderTexture.ReleaseTemporary(renderTexture);

    displayingPhoto = true;
    animationTime = 0.0f;

    settings.grid = originalGrid;
    settings.rings = originalRings;

    OnTakePhotoEnd?.Invoke();
  }

  private void OnGUI()
  {
    if (displayingPhoto && photoTexture != null)
    {
      animationTime += Time.deltaTime;
      float t = Mathf.Clamp01(animationTime / animationDuration);

      float smoothT = 1.0f - Mathf.Pow(1f - t, 3.0f);
      float currentScale = Mathf.Lerp(1f, finalScale, smoothT);

      float posX = Mathf.Lerp(0.5f, finalPosition.x, smoothT);
      float posY = Mathf.Lerp(0.5f, finalPosition.y, smoothT);

      int width = (int)(Screen.width * currentScale);
      int height = (int)(Screen.height * currentScale);

      int x = (int)(Screen.width * posX) - width / 2;
      int y = (int)(Screen.height * posY) - height / 2;

#if UNITY_EDITOR
      GUI.DrawTextureWithTexCoords(new Rect(x, y, width, height), photoTexture, new Rect(0.0f, 0.0f, 1.0f, -1.0f));
#else
      GUI.DrawTextureWithTexCoords(new Rect(x, y, width, height), photoTexture, new Rect(0.0f, 0.0f, 1.0f, 1.0f));
#endif
    }
  }

  private void OnDestroy()
  {
    settings?.ResetDefaultValues();
    
    if (photoTexture != null)
      Destroy(photoTexture);
  }
}
