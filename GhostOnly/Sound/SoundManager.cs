using Data;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    AudioSource[] _audioSource = new AudioSource[(int)Define.Sound.Max];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    public bool isOnOffEffectSound;
    public bool isOnOffMasterSound;
    public bool isOnOffBgmSound;
    // MP3 Player       -> AudioSource
    // MP3 음원          -> AudioClip
    // 관객(귀)          -> AudioListener

    private void Awake()
    {      
        DontDestroyOnLoad(this);  
    }
    private void Start()
    {
        Managers.GameManager.OnAppSettingChanged += ChangeBgm;

        isOnOffEffectSound = PreferencesManager.IsEffectVolumeOn();
        isOnOffMasterSound = PreferencesManager.IsMasterVolumeOn();
        isOnOffBgmSound = PreferencesManager.IsBgmVolumeOn();
    }

    private void OnDestroy()
    {
        Managers.GameManager.OnAppSettingChanged -= ChangeBgm;
    }
    private void ChangeBgm(AppSetting setting)
    {
        AudioSource audioSource = GetCurrent();
        isOnOffEffectSound = setting.IsEffectVolumeOn;
        isOnOffMasterSound = setting.IsMasterVolumeOn;
        isOnOffBgmSound = setting.IsBgmVolumeOn;

        if (audioSource != null)
        {
            if (setting.IsBgmVolumeOn && setting.IsMasterVolumeOn)
            {
                audioSource.volume = setting.BGMVolume * setting.MasterVolume;
            }
            else
            {
                audioSource.volume = 0;
            }
        }
        else
        {
            return;
        }
    }

    public string GetSoundType(Data.SoundType soundType)
    {
        return Managers.Data.SoundDic[soundType.ToString()].soundPath;
    }

    public void PlaySound(Data.SoundType soundType, Vector2 pos = default, bool posCheck = false)
    {
        GameObject sound = ObjectPoolManager.Instance.GetGo(PoolType.EffectSound);
        sound.transform.position = pos;
     
        if (sound.TryGetComponent(out EffectSound effectSound))
        {
            effectSound.Initialized(soundType, posCheck);
        }
    }
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            GameObject.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));

            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSource[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSource[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public AudioSource GetCurrent()
    {
        return _audioSource[(int)Define.Sound.Bgm];
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSource)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(Data.SoundType path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {          
        AudioClip audioClip = GetOrAddAudioClip(Managers.Sound.GetSoundType(path), type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;
        // BGM
        if (type == Define.Sound.Bgm)
        {

            AudioSource audioSource = _audioSource[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            if (isOnOffBgmSound && isOnOffMasterSound)
            {
                audioSource.volume = PreferencesManager.GetBgmVolume() * PreferencesManager.GetMasterVolume();
            }
            else
            {
                audioSource.volume = 0;
            }
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    // 캐싱
    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("SoundData/") == false)
            path = $"SoundData/{path}";

        AudioClip audioClip = null;

        // BGM
        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        // 효과음
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }
        if (audioClip == null)
            Debug.Log($"AudioClip Missing {path}");

        return audioClip;
    }

   
}