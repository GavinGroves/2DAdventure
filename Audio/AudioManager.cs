using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")] public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;
    [Header("组件")]
    public AudioSource bgmSource;
    public AudioSource fxSource;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
    }

    private void OnBGMEvent(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        fxSource.clip = clip;
        fxSource.Play();
    }
}
