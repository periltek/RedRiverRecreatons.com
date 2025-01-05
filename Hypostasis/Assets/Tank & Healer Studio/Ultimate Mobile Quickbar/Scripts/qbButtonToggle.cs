using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class qbButtonToggle : MonoBehaviour
{
    [Header("Button Details")]
    public string buttonTitle;
    public UltimateMobileQuickbarButtonInfo qInfo;
    public Sprite IconForButton;
    
    [Header("Quick Bar Setup")]
    [Range(0, 20)] public int qbSetCatalog;
    [Range(0, 6)] public int qbButtonCatalog;

    [Header("Button Functionality")]
    public UnityEvent onPressEvent;  // Custom event when button is pressed
    public UnityEvent onReleaseEvent; // Custom event when button is released
    public GameObject loader; //Game object with loading animations that play on enable

    [Header("Visual and Audio Settings")]
    public bool on = false;
    public GameObject[] targets; // Objects to toggle on/off
    public AudioSource audio;
    public Animation anim;

    [Header("Animation Settings")]
    public AnimationClip defaultAnimationClip; // Default animation when toggled

    // Cooldown state to prevent double toggling in quick succession
    private bool canToggle = true;
    private float toggleCooldown = 0.5f; // Delay in seconds between presses

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(buttonTitle))
        {
            this.name = buttonTitle;
        }
    }

    private void Start()
    {
        foreach (GameObject obj in targets)
        {
            if (on)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
        // Register button with quick bar
        RegisterButton();
    }

    private void OnEnable()
    {
        // Register button when enabled
        RegisterButton();
    }

    // Register the button to the quickbar
    private void RegisterButton()
    {
        UltimateMobileQuickbar.RegisterToQuickbar("MainMenu", Toggle, qInfo, qbSetCatalog, qbButtonCatalog);
        qInfo.UpdateIcon(IconForButton);
    }

    // Toggle function triggered by button press
    public void Toggle()
    {
        if (!canToggle) return;  // Ignore if toggle is in cooldown state

        canToggle = false;  // Disable further toggling until cooldown ends

        on = !on;  // Toggle state

        if (on)
        {
            ActivateTargets(true);
            Debug.Log("Toggling... Current state: True");
            onPressEvent.Invoke();
        }
        else
        {
            ActivateTargets(false);
            Debug.Log("Deactivating targets");
            onReleaseEvent.Invoke();
        }

        // Re-enable the toggle after cooldown
        StartCoroutine(EnableToggleAfterDelay(toggleCooldown));
    }

    // Activate or deactivate targets
    private void ActivateTargets(bool state)
    {
        if (targets != null)
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(state);
                Debug.Log($"Target: {target.name} set to {state}");
            }
        }
    }

    // Coroutine to re-enable toggle after the cooldown period
    private IEnumerator EnableToggleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canToggle = true;  // Re-enable toggling after cooldown
    }

    // Play audio with the specified clip
    public void PlayAudio(AudioClip clip)
    {
        if (audio && !audio.isPlaying && clip != null)
        {
            if (audio.clip != null)
            {
                AudioClip originalClip = audio.clip;
                audio.clip = clip;
                StartCoroutine(ResetAudioClipAfterPlay(originalClip));
            }
            audio.clip = clip;
            audio.Play();
        }
    }

    // Reset to the original audio clip after playing
    private IEnumerator ResetAudioClipAfterPlay(AudioClip originalClip)
    {
        audio.Play();
        while (audio.isPlaying)
        {
            yield return null;
        }
        audio.clip = originalClip;
    }

    // Set custom animation clip
    public void SetAnimationClip(AnimationClip newClip)
    {
        if (anim != null)
        {
            anim.clip = newClip;
        }
    }

    // Play the assigned animation clip
    public void PlayAnimation()
    {
        if (anim != null && anim.clip != null)
        {
            anim.Play();
        }
        else if (defaultAnimationClip != null) // Use default if no specific clip is set
        {
            anim.Play(defaultAnimationClip.name);
        }
    }


    public void SetLoader(bool status)
    {
        //Activates the loading object with loading animation
        loader.SetActive(status);
    }
}
