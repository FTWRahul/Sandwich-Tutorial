using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// singleton audio manager for all sound related things
/// </summary>
public class AudioManager : MonoBehaviour
{
   public AudioSource source;
   public static AudioManager instance;

   private Object[] _swipeClips;
   private Object[] _biteClips;
   private AudioClip _winSound;
   
   private void Awake()
   {
      //singleton implementation
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(this);
      }
      DontDestroyOnLoad(gameObject);
      
      //Load audio clips from the resources folder
      _swipeClips = Resources.LoadAll("Sounds/Swipe", typeof(AudioClip));
      _biteClips = Resources.LoadAll("Sounds/Bite", typeof(AudioClip));
      _winSound = (AudioClip)Resources.Load("Sounds/SFX/Win");
   }

   /// <summary>
   /// Plays a random sounds from the swipe pool
   /// </summary>
   public void PlaySwipeSound()
   {
      AudioClip audioClip = (AudioClip) _swipeClips[Random.Range(0, _swipeClips.Length)];
      PlayAudio(audioClip);
   }
   /// <summary>
   /// Plays a random sound from the bite pool
   /// </summary>
   public void PlayBiteSound()
   {
      AudioClip audioClip = (AudioClip) _biteClips[Random.Range(0, _biteClips.Length)];
      PlayAudio(audioClip);
   }
   
   /// <summary>
   /// Plays the provided audio clip
   /// </summary>
   /// <param name="audioClip"></param>
   private void PlayAudio(AudioClip audioClip)
   {
      source.clip = audioClip;
      source.Play();
   }
   
   /// <summary>
   /// Plays the victory sound
   /// </summary>
   public void PlayWinSound()
   {
      source.clip = _winSound;
      source.Play();
   }
}
