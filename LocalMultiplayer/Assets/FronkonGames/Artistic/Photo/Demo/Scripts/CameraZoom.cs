using UnityEngine;
using UnityEngine.Events;

namespace FronkonGames.Artistic.Photo
{
  /// <summary>
  /// Controls Camera Field of View (FOV) using the mouse scroll wheel for zooming.
  /// </summary>
  /// <remarks> This code is designed for demonstration purposes. </remarks>
  [RequireComponent(typeof(Camera))]
  public sealed class CameraZoom : MonoBehaviour
  {
    [Header("Zoom Settings")]
    [Tooltip("The target Camera component. If null, it will try to get it from this GameObject.")]
    [SerializeField] private Camera targetCamera = null;

    [Tooltip("Minimum Field of View (maximum zoom).")]
    [Range(1.0f, 179.0f)]
    [SerializeField] private float minFOV = 20.0f;

    [Tooltip("Sensitivity of the zoom based on mouse wheel input. Higher values zoom faster.")]
    [Range(1.0f, 100.0f)]
    [SerializeField] private float zoomSensitivity = 30.0f;

    [Tooltip("Time in seconds to smoothly reach the target FOV. Lower values are faster/snappier.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Events")]
    [Tooltip("Event fired once when the zoom action starts (scroll wheel is moved).")]
    public UnityEvent OnZoomStart;

    [Tooltip("Event fired every frame while zooming, passing the raw requested FOV change for this frame.")]
    public UnityEvent<float> OnZooming;

    [Tooltip("Event fired once when the zoom action ends (scroll wheel stops moving and smoothing finishes).")]
    public UnityEvent OnZoomEnd;

    private float targetFOV;              // The desired FOV based on input
    private float currentFOVVelocity;     // Reference variable for SmoothDamp
    private bool isZooming = false;       // Tracks if a zoom action is currently in progress
    private float lastScrollInput = 0.0f; // Store previous frame's input to detect start/stop
    private float maxFOV = 60.0f;

    private const float ScrollInputThreshold = 0.005f;
    private const float SmoothingFinishedThreshold = 0.01f;

    private void Awake()
    {
      if (targetCamera == null)
        targetCamera = GetComponent<Camera>();

      if (targetCamera == null)
      {
        Debug.LogError("CameraZoom: No Camera component found on this GameObject or assigned in the inspector.", this);
        this.enabled = false;
        return;
      }

      targetFOV = maxFOV = targetCamera.fieldOfView;
      if (minFOV > maxFOV)
      {
        Debug.LogWarning("CameraZoom: minFOV cannot be greater than maxFOV. Swapping values.", this);
        (minFOV, maxFOV) = (maxFOV, minFOV); // Swap values
      }
    }

    private void OnValidate()
    {
      ValidateFOVRanges();
    }

    private void ValidateFOVRanges()
    {
      if (minFOV > maxFOV)
      {
        Debug.LogWarning($"CameraZoom ({gameObject.name}): minFOV ({minFOV}) cannot be greater than maxFOV ({maxFOV}). Clamping minFOV to maxFOV.", this);
        minFOV = maxFOV;
      }

      minFOV = Mathf.Clamp(minFOV, 1.0f, 179.0f);
      maxFOV = Mathf.Clamp(maxFOV, 1.0f, 179.0f);
    }

    private void Update()
    {
      if (targetCamera == null) return;

      float scrollInput = Input.GetAxis("Mouse ScrollWheel");

      bool significantScroll = Mathf.Abs(scrollInput) > ScrollInputThreshold;
      if (significantScroll)
      {
        targetFOV -= scrollInput * zoomSensitivity;
        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
      }

      // Apply smooth damping to the camera's FOV
      float previousFOV = targetCamera.fieldOfView;
      targetCamera.fieldOfView = Mathf.SmoothDamp(targetCamera.fieldOfView, targetFOV, ref currentFOVVelocity, smoothTime);
      
      // Check if FOV is changing significantly (either from mouse or programmatic zoom)
      bool fovChanging = Mathf.Abs(targetCamera.fieldOfView - previousFOV) > SmoothingFinishedThreshold;
      
      if (!significantScroll && Mathf.Abs(currentFOVVelocity) < SmoothingFinishedThreshold)
      {
        if (Mathf.Approximately(targetFOV, minFOV))
        {
          if (!Mathf.Approximately(targetCamera.fieldOfView, minFOV))
          {
            targetCamera.fieldOfView = minFOV;
            currentFOVVelocity = 0.0f;
          }
        }
        else if (Mathf.Approximately(targetFOV, maxFOV))
        {
          if (!Mathf.Approximately(targetCamera.fieldOfView, maxFOV))
          {
            targetCamera.fieldOfView = maxFOV;
            currentFOVVelocity = 0.0f;
          }
        }
      }

      bool justStartedScrolling = significantScroll && Mathf.Abs(lastScrollInput) < ScrollInputThreshold;
      bool justStoppedScrolling = !significantScroll && Mathf.Abs(lastScrollInput) > ScrollInputThreshold;
      bool isSmoothingFinished = Mathf.Abs(currentFOVVelocity) < SmoothingFinishedThreshold && Mathf.Abs(targetCamera.fieldOfView - targetFOV) < SmoothingFinishedThreshold;

      // Start zooming if mouse scroll started OR if FOV is changing programmatically
      if ((justStartedScrolling || (fovChanging && !isZooming)) && !isZooming)
      {
        isZooming = true;
        OnZoomStart?.Invoke();
      }

      // Trigger OnZooming during any FOV change (mouse or programmatic)
      if (significantScroll || fovChanging)
      {
        float normalizedZoom = 1.0f - (targetCamera.fieldOfView - minFOV) / (maxFOV - minFOV);
        OnZooming?.Invoke(normalizedZoom);
      }

      if (isZooming && justStoppedScrolling)
      {
        // Scroll input stopped this frame. Don't end zoom immediately, wait for smoothing or snap.
      }
      else if (isZooming && !significantScroll && isSmoothingFinished)
      {
        isZooming = false;
        OnZoomEnd?.Invoke();
      }

      lastScrollInput = scrollInput;
    }

    /// <summary>
    /// Immediately sets the target FOV and snaps the camera to it (ignoring smoothing).
    /// </summary>
    /// <param name="fov">The desired Field of View.</param>
    public void SetFOVImmediate(float fov)
    {
      if (targetCamera == null) return;

      targetFOV = Mathf.Clamp(fov, minFOV, maxFOV);
      targetCamera.fieldOfView = targetFOV;
      currentFOVVelocity = 0.0f;
    }

    /// <summary>
    /// Sets the target FOV, allowing the camera to smoothly transition to it.
    /// </summary>
    /// <param name="fov">The desired Field of View.</param>
    public void SetFOVSmooth(float fov, float time)
    {
      if (targetCamera == null) return;

      targetFOV = Mathf.Clamp(fov, minFOV, maxFOV);
      smoothTime = 1.0f / time;
    }
  }
}