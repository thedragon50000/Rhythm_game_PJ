using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSettingsAttr : ISettingsAttr
{
    public string selectJsonStr;
    public string selectAudioStr;
    public AudioClip selectAudio;
    public float trackSpeed = 0.5f;
    public float bgmVolume = 0.5f;
    public float keyAudioVolume = 0.15f;
    public bool isSkipAllRhythm;
}
