using System.Collections.Generic;
using UnityEngine;
using System;
using static SoundClipByName;
using System.Linq;

[CreateAssetMenu(fileName = "DataSound", menuName = "CreateSound/Clip", order = 0)]
[Serializable]
public class DataSound : ScriptableObject
{
    public List<SoundClipByName> clips = new List<SoundClipByName>();
    public AudioClip GetClipByType(SoundType type)
    {
        return clips.FirstOrDefault(x => x.type == type).clip;
        //foreach (SoundClipByName soundData in clips)
        //{
        //    if(soundData.type == type)
        //    {
        //        return soundData.clip;
        //    }    
        //}    
    }
}

[Serializable]
public class SoundClipByName
{
    [Serializable]
    public enum SoundType
    {
        BGM,
        Complete,
        Move,
        ClickBtn,
        Select,
        Destroy,
        ComleteGame
    }
    public SoundType type;
    public AudioClip clip;
}