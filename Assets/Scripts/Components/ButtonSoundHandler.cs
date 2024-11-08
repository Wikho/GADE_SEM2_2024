using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonSoundHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sound Settings")]
    [Tooltip("Sound to play when the button is pressed.")]
    public AudioClip pressSound;

    [Tooltip("Sound to play when the button is released.")]
    public AudioClip releaseSound;

    private AudioSource audioSource;


    private void Awake()
    {
        // Retrieve or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Called when the pointer is pressed down on the button.
    /// </summary>
    /// <param name="eventData">Data associated with the event.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        PlaySound(pressSound);
    }

    /// <summary>
    /// Called when the pointer is released from the button.
    /// </summary>
    /// <param name="eventData">Data associated with the event.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        PlaySound(releaseSound);
    }

    /// <summary>
    /// Plays the specified audio clip if it's assigned.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
