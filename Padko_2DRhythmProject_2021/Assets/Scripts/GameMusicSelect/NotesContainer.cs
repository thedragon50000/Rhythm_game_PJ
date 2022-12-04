using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteEditor.Utility;
using NoteEditor.Model;

namespace Game.MusicSelect
{
    /// <summary>
    /// 存储所选择乐谱的信息
    /// </summary>
    public class NotesContainer : SingletonMonoBehaviour<NotesContainer>
    {
        public AudioClip music;
        public string json;
        public string musicNameExtension;
        public int level;
        public int notesCount;
        public bool isEditMode;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void SetContainer(AudioClip music, string json, string musicNameExtension, int level, int notesCount, bool isEditMode = false)
        {
            this.music = music;
            this.json = json;
            this.musicNameExtension = musicNameExtension;
            this.level = level;
            this.notesCount = notesCount;
            this.isEditMode = isEditMode;
        }


        public void DestroySelf(){
            Destroy(gameObject);
        }

    }
}
