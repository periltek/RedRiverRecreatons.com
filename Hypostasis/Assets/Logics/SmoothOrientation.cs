using UnityEngine;

public class SmoothOrientation : MonoBehaviour
{
    public Vector3 portraitPosition;
    public Vector3 portraitScale;
    public Vector3 landscapePosition;
    public Vector3 landscapeScale;
    public float transitionDuration = 1.0f;

    private Vector3 targetPosition;
    private Vector3 targetScale;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private float transitionTimer;
    private bool isTransitioning;

    void Start()
    {
        UpdateTargets();
        transform.position = targetPosition;
        transform.localScale = targetScale;
    }

    void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.Portrait)
        {
            UpdateTargets();
        }

        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(transitionTimer / transitionDuration);

            transform.position = Vector3.Lerp(initialPosition, targetPosition, progress);
            transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

            if (progress >= 1.0f)
            {
                isTransitioning = false;
            }
        }
    }

    void UpdateTargets()
    {
        Vector3 newTargetPosition;
        Vector3 newTargetScale;

        if (Screen.width > Screen.height) // Landscape
        {
            newTargetPosition = landscapePosition;
            newTargetScale = landscapeScale;
        }
        else // Portrait
        {
            newTargetPosition = portraitPosition;
            newTargetScale = portraitScale;
        }

        if (newTargetPosition != targetPosition || newTargetScale != targetScale)
        {
            StartTransition(newTargetPosition, newTargetScale);
        }
    }

    void StartTransition(Vector3 newTargetPosition, Vector3 newTargetScale)
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        targetPosition = newTargetPosition;
        targetScale = newTargetScale;

        transitionTimer = 0.0f;
        isTransitioning = true;
    }
}
