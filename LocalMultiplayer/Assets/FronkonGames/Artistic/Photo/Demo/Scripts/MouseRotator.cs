using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FronkonGames.Artistic.Photo
{
  /// <summary>
  /// Rotates the GameObject based on mouse movement when a specific key is held down.
  /// </summary>
  /// <remarks> This code is designed for demonstration purposes. </remarks>
  public sealed class MouseRotator : MonoBehaviour
  {
    [Header("Activation")]
    [Tooltip("The key that must be held down to enable rotation.")]
    [SerializeField]
    private KeyCode activationKey = KeyCode.Mouse1;

    [Header("Rotation Limits")]
    [Tooltip("Maximum rotation angle (degrees) around the local X (Pitch) and Y (Yaw) axes.")]
    [SerializeField]
    private Vector2 rotationRange = new(70.0f, 70.0f);

    [Header("Movement")]
    [Tooltip("How fast the object rotates based on mouse movement.")]
    [Range(1.0f, 20.0f)]
    [SerializeField]
    private float rotationSpeed = 10.0f;

    [Tooltip("Time in seconds to smoothly reach the target rotation. Lower values are faster/snappier.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float dampingTime = 0.15f;

    [Header("Events")]
    [Tooltip("Event fired when rotation starts/stops.")]
    public UnityEvent<bool> OnRotatingChanged;

    // Gizmo settings made readonly constants as they don't need Inspector exposure based on the previous version
    private const float GizmoRadius = 1.0f;
    private static readonly Color YawGizmoColor = Color.green;
    private static readonly Color PitchGizmoColor = Color.red;

    private Vector3 targetAngles;        // Desired rotation based on input
    private Vector3 followAngles;        // Smoothed rotation applied to the object
    private Vector3 followVelocity;      // Velocity used by SmoothDamp
    private Quaternion originalRotation; // Initial rotation of the object
    private bool isRotating = false;     // Flag to track if rotation is active

    public void ResetRotation(Transform transform)
    {
      originalRotation = transform.rotation;
      followVelocity = followAngles = targetAngles = Vector3.zero;
      isRotating = false;
      this.gameObject.transform.rotation = transform.rotation;
    }

    private void Awake()
    {
      // Store the initial local rotation when the script wakes up
      originalRotation = transform.localRotation;

      // Initialize angles to zero to avoid potential issues with uninitialized values
      targetAngles = Vector3.zero;
      followAngles = Vector3.zero;
      followVelocity = Vector3.zero;
    }

    private void Update()
    {
      HandleActivationInput();

      // Only process new input if rotating
      if (isRotating == true)
        ProcessRotationInput();

      // Always apply smoothing and rotation to allow smooth stopping
      ApplySmoothingAndRotation();
    }

    private void HandleActivationInput()
    {
      if (Input.GetKeyDown(activationKey) == true && isRotating == false)
      {
        isRotating = true;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to center
        Cursor.visible = false;                   // Hide cursor
        OnRotatingChanged?.Invoke(isRotating);
      }

      if (Input.GetKeyUp(activationKey) == true && isRotating == true)
      {
        isRotating = false;
        Cursor.lockState = CursorLockMode.None;   // Unlock cursor
        Cursor.visible = true;                    // Show cursor
        OnRotatingChanged?.Invoke(isRotating);
      }
    }

    private void ProcessRotationInput()
    {
      // Read mouse input axes
      float inputH = Input.GetAxis("Mouse X");
      float inputV = Input.GetAxis("Mouse Y");

      // Angle Wrapping (prevents target angle from accumulating large values)

      if (targetAngles.y > 180.0f)  targetAngles.y -= 360.0f;
      if (targetAngles.y < -180.0f) targetAngles.y += 360.0f;
      if (targetAngles.x > 180.0f)  targetAngles.x -= 360.0f;
      if (targetAngles.x < -180.0f) targetAngles.x += 360.0f;

      // Update target angles based on mouse input and speed
      targetAngles.y += inputH * rotationSpeed;
      targetAngles.x += inputV * rotationSpeed; // Input V is typically inverted for pitch

      // Clamp target angles to the defined range
      // Note: We use half the range because we're rotating relative to the center (original rotation)
      targetAngles.y = Mathf.Clamp(targetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
      targetAngles.x = Mathf.Clamp(targetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);
    }

    private void ApplySmoothingAndRotation()
    {
      // Smoothly interpolate from the current follow angles towards the target angles
      followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, dampingTime);

      // Calculate the final rotation: Start from the original rotation and apply the smoothed delta rotation
      // Note the negative sign on followAngles.x for standard pitch control (mouse up = look up)
      transform.localRotation = originalRotation * Quaternion.Euler(-followAngles.x, followAngles.y, 0.0f);
    }

#if UNITY_EDITOR
    // Draw Gizmos in the Scene view when the object is selected
    private void OnDrawGizmosSelected()
    {
      // Use the current rotation as the base for drawing gizmos if not playing,
      // or the original rotation if playing (as that's our reference point).
      Quaternion baseRotation = Application.isPlaying ? originalRotation : transform.localRotation;
      Vector3 position = transform.position;
      // Ensure gizmos respect parent rotation if applicable
      Quaternion worldRotation = transform.parent ? transform.parent.rotation * baseRotation : baseRotation;

      // Get rotation axes in world space based on the base rotation
      Vector3 up = worldRotation * Vector3.up;
      Vector3 right = worldRotation * Vector3.right;
      Vector3 forward = worldRotation * Vector3.forward;

      float halfYaw = rotationRange.y * 0.5f;
      float halfPitch = rotationRange.x * 0.5f;

      // Draw Yaw Limits (Rotation around Y axis)

      Handles.color = YawGizmoColor;
      // Draw arc in the object's local XZ plane (around its local Y axis)
      Handles.DrawWireArc(position, up, Quaternion.AngleAxis(-halfYaw, up) * forward, rotationRange.y, GizmoRadius);
      // Draw lines indicating the yaw limits
      Handles.DrawLine(position, position + Quaternion.AngleAxis(-halfYaw, up) * forward * GizmoRadius);
      Handles.DrawLine(position, position + Quaternion.AngleAxis(halfYaw, up) * forward * GizmoRadius);

      // Draw Pitch Limits (Rotation around X axis)

      Handles.color = PitchGizmoColor;
      // Draw arc in the object's local YZ plane (around its local X axis)
      // Note the negative angle range (-rotationRange.x) because DrawWireArc expects a positive sweep angle.
      // We start at the "upper" limit (halfPitch) and sweep downwards.
      Handles.DrawWireArc(position, right, Quaternion.AngleAxis(halfPitch, right) * forward, -rotationRange.x, GizmoRadius);
      // Draw lines indicating the pitch limits
      Handles.DrawLine(position, position + Quaternion.AngleAxis(halfPitch, right) * forward * GizmoRadius); // Upper limit
      Handles.DrawLine(position, position + Quaternion.AngleAxis(-halfPitch, right) * forward * GizmoRadius); // Lower limit

      // Reset color
      Handles.color = Color.white;
    }
#endif // UNITY_EDITOR
  }
}