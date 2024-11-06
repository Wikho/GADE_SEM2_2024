using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _introClip; // clip to start before the looping section starts
    [SerializeField] private AudioClip _loopClip; // clip to use for procedural modification
    [SerializeField] private float fullHealthPitch;
    [SerializeField] private float lowHealthPitch;
    
    [Header("Tower")]
    [SerializeField] private HealthComponent _mainTowerHealth;
    [Header("Debug")]
    [Range(1, 1.2f)][SerializeField] private float manualSpeedModifier = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine(GetComponent());

        _source.PlayOneShot(_introClip);
        _source.clip = _loopClip;
    }

    IEnumerator GetComponent() // Jank but works
    {
        yield return new WaitForSeconds(0.1f);
        _mainTowerHealth = FindAnyObjectByType<MainTower>().gameObject.GetComponent<HealthComponent>();
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            _source.Play();
        }
        _source.pitch = Mathf.Lerp(lowHealthPitch, fullHealthPitch, _mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth) * manualSpeedModifier; // Setting pitch dynamically based on main tower defense
        // TODO: Modify the pitch of the song based on gameplay
    }
}
