using Manager;
using UnityEngine;

public class EffectSound : PoolAble
{
    private AudioSource _audioSource;
    private float _timer = 0;
   
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_audioSource.clip != null && _timer > _audioSource.clip.length)
        {
            ReleaseObject();
            _timer = 0;
        }
    }

    public void Initialized(Data.SoundType soundType, bool posCheck)
    {   
        _audioSource.clip = Resources.Load<AudioClip>(Managers.Sound.GetSoundType(soundType));     
        if (Managers.Sound.isOnOffEffectSound && Managers.Sound.isOnOffMasterSound)
        {
            _audioSource.volume = PreferencesManager.GetEffectVolume() * PreferencesManager.GetMasterVolume();
        }
        else
        {
            _audioSource.volume = 0;
        }
    
        if (posCheck)
        {
            _audioSource.spatialBlend = 1f;           
        }        
        else
        {
            _audioSource.spatialBlend = 0f;
        }

        _audioSource.Play();
    }
}
