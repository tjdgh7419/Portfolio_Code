using Manager;
using UnityEngine;

public class EffectSound : PoolAble
{
    AudioSource audioSource;
    private float timer = 0;
   
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (audioSource.clip != null && timer > audioSource.clip.length)
        {
            ReleaseObject();
            timer = 0;
        }
    }

    public void Initialized(Data.SoundType soundType, bool posCheck)
    {   
        audioSource.clip = Resources.Load<AudioClip>(Managers.Sound.GetSoundType(soundType));     
        if (Managers.Sound.isOnOffEffectSound && Managers.Sound.isOnOffMasterSound)
        {
            audioSource.volume = PreferencesManager.GetEffectVolume() * PreferencesManager.GetMasterVolume();
        }
        else
        {
            audioSource.volume = 0;
        }
    
        if (posCheck)
        {
            audioSource.spatialBlend = 1f;           
        }        
        else
        {
            audioSource.spatialBlend = 0f;
        }

        audioSource.Play();
    }
}
