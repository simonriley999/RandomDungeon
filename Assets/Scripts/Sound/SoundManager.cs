using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public Sound[] sounds;
    public AudioMixer audioMixer;

    private void Awake() {
        foreach(Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.outputAudioMixerGroup = s.audioMixerGroup;
            
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.loop = s.loop;
        }
    }
    // Start is called before the first frame update
    private void Start() {
        Play("Explore");
        Play("Battle");
        findWithName("Battle").audioSource.volume = 0f;
    }
    Sound findWithName(string _name)
    {
        return Array.Find(sounds,s =>s.name == _name);
    }
    public void Play(string _name)
    {
        Sound s = findWithName(_name);
        s.audioSource.Play();
    }

    public void Pause(string _name)
    {
        Sound s = findWithName(_name);
        s.audioSource.Pause();
    }

    public void SetVolume(string _name,float _volume)
    {
        Sound s = findWithName(_name);
        StartCoroutine(GradualChangeVolume(s.audioSource,_volume));
    }

    IEnumerator GradualChangeVolume(AudioSource _source,float _volume)
    {
        float changeVolume = _volume - _source.volume;
        float percent = 0;
        while (percent < 1)
        {
            _source.volume += Time.deltaTime * changeVolume;
            percent += Time.deltaTime;
            yield return null;
        }
    }
}
