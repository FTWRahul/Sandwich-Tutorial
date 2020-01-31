using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
   public AudioSource source;
   public static AudioManager instance;

   private Object[] _swipeClips;
   private Object[] _biteClips;
   private AudioClip _winSound;
   
   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(this);
      }
      _swipeClips = Resources.LoadAll("Sounds/Swipe", typeof(AudioClip));
      _biteClips = Resources.LoadAll("Sounds/Bite", typeof(AudioClip));
      _winSound = (AudioClip)Resources.Load("Sounds/SFX/Win");
   }

   public void PlaySwipeSound()
   {
      AudioClip audioClip = (AudioClip) _swipeClips[Random.Range(0, _swipeClips.Length)];
      PlayAudio(audioClip);
   }
   public void PlayBiteSound()
   {
      AudioClip audioClip = (AudioClip) _biteClips[Random.Range(0, _biteClips.Length)];
      PlayAudio(audioClip);
   }
   
   private void PlayAudio(AudioClip audioClip)
   {
      source.clip = audioClip;
      source.Play();
   }
   
   public void PlayWinSound()
   {
      source.clip = _winSound;
      source.Play();
   }
}
