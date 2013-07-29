using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Collections;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace OldSkull.Utils
{
    public static class Sounds
    {
        static public bool Loaded;

        static public Dictionary<string,SFX> sounds;
        static public string CurrentMusic;

        static public void Load(string[] soundList)
        {
            sounds = new Dictionary<string, SFX>();

            for (int i = 0; i < soundList.Length; i++)
            {
                sounds.Add(soundList[i].ToUpper(),new SFX(soundList[i]));
            }
        }

        static public SFX Play(string soundName,float volume=1)
        {
            SFX sfx = sounds[soundName.ToUpper()];
            sfx.Play(160,volume);
            return sfx;
        }

        static public void PlayMusic(string musicName)
        {
            if (CurrentMusic != musicName)
            {
                
                string path = Path.Combine(Engine.Instance.Content.RootDirectory, @"Music\", musicName + ".mp3");
                MediaPlayer.Play(Song.FromUri(musicName, new Uri(path, UriKind.Relative)));
                CurrentMusic = musicName;
            }
        }

        static public void Init()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Stop();
        }

    }
}
