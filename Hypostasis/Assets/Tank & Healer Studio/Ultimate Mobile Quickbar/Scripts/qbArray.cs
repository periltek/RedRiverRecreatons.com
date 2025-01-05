using UnityEngine;
using UnityEngine.Events;

public class qbArray : MonoBehaviour
{
    [Header("Button Array")]
    public qbButtonToggle[] buttonArray; // Array of qbButtonToggle components

    [Header("Control Options")]
    public bool toggleAllOn; // Whether to toggle all buttons on or off
    public UnityEvent onArrayActivated; // Event to trigger when activating all buttons
    public UnityEvent onArrayDeactivated; // Event to trigger when deactivating all buttons

    // Start is called before the first frame update
    private void Start()
    {
        // Optionally register to initial events if needed
    }

    // Toggle all buttons in the array on or off
    public void ToggleAllButtons(bool state)
    {
        foreach (qbButtonToggle button in buttonArray)
        {
            if (button != null)
            {
                button.Toggle(); // Call the existing Toggle() method instead of SpecifiedToggle
            }
        }

        // Optionally trigger events for all buttons activated or deactivated
        if (state)
        {
            onArrayActivated.Invoke();
        }
        else
        {
            onArrayDeactivated.Invoke();
        }
    }

    // Toggle all buttons on
    public void ActivateAllButtons()
    {
        ToggleAllButtons(true);
    }

    // Toggle all buttons off
    public void DeactivateAllButtons()
    {
        ToggleAllButtons(false);
    }

    // Play audio on all buttons in the array
    public void PlayAudioOnAllButtons(AudioClip clip)
    {
        foreach (qbButtonToggle button in buttonArray)
        {
            if (button != null)
            {
                button.PlayAudio(clip);
            }
        }
    }

    // Play the same animation on all buttons in the array
    public void PlayAnimationOnAllButtons(AnimationClip clip)
    {
        foreach (qbButtonToggle button in buttonArray)
        {
            if (button != null)
            {
                button.SetAnimationClip(clip);
                button.PlayAnimation();
            }
        }
    }

    // Access and control a specific button in the array by index
    public void ToggleButtonByIndex(int index, bool state)
    {
        if (index >= 0 && index < buttonArray.Length)
        {
            buttonArray[index].Toggle(); // Use the Toggle method here as well
        }
    }

    // Play audio on a specific button in the array by index
    public void PlayAudioOnButtonByIndex(int index, AudioClip clip)
    {
        if (index >= 0 && index < buttonArray.Length)
        {
            buttonArray[index].PlayAudio(clip);
        }
    }

    // Set animation for a specific button in the array by index
    public void SetAnimationOnButtonByIndex(int index, AnimationClip clip)
    {
        if (index >= 0 && index < buttonArray.Length)
        {
            buttonArray[index].SetAnimationClip(clip);
        }
    }

    // Example function to show how to update button visuals (Icon)
    public void UpdateButtonIconByIndex(int index, Sprite newIcon)
    {
        if (index >= 0 && index < buttonArray.Length)
        {
            buttonArray[index].qInfo.UpdateIcon(newIcon);
        }
    }
}
