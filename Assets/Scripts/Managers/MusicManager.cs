using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource _source; // Using the one off the main camera since its always going to be at full volume and not subject to distance based volume scaling.
    [SerializeField] private AudioSource _heartbeatSource; // for a heartbeat sfx
    [SerializeField] private AudioSource _earRingingSource; // for a ear ringing sfx
    [SerializeField] private AudioClip _introClip; // clip to start before the looping section starts
    [SerializeField] private AudioClip _loopClip; // clip to use for procedural modification
    [SerializeField] private float _fullHealthPitch; // pitch scale when player tower is at full health
    [SerializeField] private float _lowHealthPitch; // pitch scale when player tower is at 0.
    
    [Header("Tower")]
    [SerializeField] private HealthComponent _mainTowerHealth;

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
        _heartbeatSource.Play();
        _earRingingSource.Play();
    }

    IEnumerator GetComponent()
    {
        // Delay so that initialization of terrain is complete before looking for the component 
        yield return new WaitForSeconds(0.1f);
        _mainTowerHealth = FindAnyObjectByType<MainTower>().gameObject.GetComponent<HealthComponent>();
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            _source.Play();
        }
        // Setting pitch dynamically based on main tower defense
        _source.pitch = Mathf.Lerp(_lowHealthPitch, _fullHealthPitch, _mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth); 
        
        // So TLDR of whats going on here is that our ears dont hear volume linearly, so an exponential curve is used here to make it sound more perceptually linear.
        //  This same tactic is commonly used in guitars for their volume knobs interestingly enough.
        //  Also Worth noting, we do not need to do this with pitch, as pitch is not perceived the same as volume, and a more linear scaling sounds more perceptually linear.
        //  We're using x^2 since its a fairly good exponential curve for this.
        //  Paste "x^{2}\left\{0\le x\le1\right\}" into https://www.desmos.com/calculator for a visualization of the curve
        _source.volume = Mathf.Lerp(0.0f, 0.6f, (_mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth) * (_mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth)); 
        // Lerp inverted so and from 0..2 so it reaches max volume sooner (volume is always clamped 0..1 anyways)
        _heartbeatSource.volume = Mathf.Lerp(2.0f, 0.0f, Mathf.Sqrt(_mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth)); 
        _heartbeatSource.pitch = Mathf.Lerp(0.5f, 0.75f, _mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth); 
        // theres some math involved here, you can map this in Desmos again to see it. basically the ringing will fade in at much lower health than the heartbeat
        _earRingingSource.volume = Mathf.Lerp(1, -1, Mathf.Sqrt(_mainTowerHealth.currentHealth / _mainTowerHealth.maxHealth)); 
        
    }
}
