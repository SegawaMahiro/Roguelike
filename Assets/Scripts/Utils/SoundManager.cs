using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] Sound[] _se;

    private Dictionary<string, AudioClip> _sounds = new();
    private AudioSource _audioSource;

    public override void OnAwake() {
        TryGetComponent(out _audioSource);
        foreach(var s in _se) {
            _sounds.Add(s.Key, s.Clip);
        }
    }
    public void PlaySE(string key) {
        _audioSource.PlayOneShot(_sounds[key]);
    }
}

[Serializable]
public class Sound
{
    [SerializeField] string _key;
    [SerializeField] AudioClip _clip;
    public AudioClip Clip => _clip;
    public string Key => _key;
}