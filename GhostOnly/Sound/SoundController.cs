using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip audioData1;
    public AudioClip audioData2;
    private void Start()
    {     
        Managers.Sound.Play(audioData1, Define.Sound.Bgm);
    }

}
