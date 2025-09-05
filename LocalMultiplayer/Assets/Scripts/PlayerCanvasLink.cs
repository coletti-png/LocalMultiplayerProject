using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PlayerCanvasLink : MonoBehaviour
{
    private void Start()
    {
        var cam = GetComponentInParent<SplitScreenCamera>().GetComponent<Camera>();
        var canvas = GetComponent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = cam;
        }
    }
}
