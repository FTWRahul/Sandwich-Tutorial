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

   private Object[] clips;
   
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
      clips = Resources.LoadAll("Sounds", typeof(AudioClip));
   }

   public void PlaySound()
   {
      AudioClip audioClip = (AudioClip) clips[Random.Range(0, clips.Length)];
      source.clip = audioClip;
      source.Play();
   }
}
