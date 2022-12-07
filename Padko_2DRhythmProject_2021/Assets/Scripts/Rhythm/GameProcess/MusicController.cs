using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NoteEditor.Utility;
using UniRx;
using UniRx.Triggers;

namespace Game.Process
{
    /// <summary>
    /// 管理音乐的播放、切换、暂停等
    /// </summary>
    public class MusicController : SingletonMonoBehaviour<MusicController>
    {
        [SerializeField] private AudioSource music;

        [SerializeField] private int samples;

        public float volume;

        public bool isPause;


        Slider processBar;
        public Slider gameProcessBar;
        public Slider editProcessBar;
        public ReactiveProperty<float> process;


        //public Text musicName, musicTime;

        //public bool playing;

        void Start()
        {
            volume = PlayerSettings.Instance.musicVolume;
            music.volume = volume;
            //float musicLength =

            if(!NotesController.Instance.isEditMode)
            {
                processBar = gameProcessBar;
                editProcessBar.gameObject.SetActive(false);
            }
            else
            {
                //processBar = editProcessBar;
                //gameProcessBar.gameObject.SetActive(false);
                processBar = gameProcessBar;
                editProcessBar.gameObject.SetActive(false);
            }

            bool isSetProcessBar = false;
            process.Value = 0;
            process.Subscribe(x =>
            {
                processBar.value = process.Value;
            });

            #region 目前尚未完成 自行調整Silder自由滑動譜面的功能
            //if (NotesController.Instance.isEditMode)
            //{ 
            //    //processBar.
            //    processBar.OnValueChangedAsObservable().Subscribe(x=>{
            //        //music.time = processBar.value;
            //        music.time = processBar.value;
            //        process.Value = music.time;
            //    });
            

            
            //    processBar.OnPointerUpAsObservable().Where(_ => NotesController.Instance.playing).Subscribe(x =>
            //    {

            //        music.time = processBar.value;
            //        process.Value = music.time;
            //        PlayController.Instance.laneNotes = new List<Queue<NoteObject>>(PlayController.Instance.editLaneNotes);
            //        //將所有陣列內的內容重設為初始值

            //        for (int i = 0; i < PlayController.Instance.laneCount; i++)
            //        {
            //            var noteList = PlayController.Instance.laneNotes[i].Where(note => note.clicked).ToList();
            //            foreach(var item in noteList)
            //            {
            //                item.reActive();
            //            }
            //        }

            //        var maxNoteDoPosY = NotesController.Instance.maxNoteDoPosY;
            //        var musicPercentage = music.time / music.clip.length;


            //        var posY = maxNoteDoPosY * musicPercentage;

            //        //Debug.Log("musicPercentage:" + musicPercentage);
            //        //Debug.Log("posY:" + posY);
            //        //Debug.Log("noteDoPosY:" + NotesController.Instance.noteDoPosY);

            //        var length = NotesController.Instance.length;
            //       // NotesController.Instance.SetNoteFieldsPos(posY);
            //        NotesController.Instance.SetDoNoteMove(maxNoteDoPosY, length, posY);
            //        //在edit狀態Miss不會SetActive(false)

            //        //如果會卡的話 就改為調整完後放開 才進行Miss相關判定

            //    });
            //}
            #endregion

            //processBar.OnValueChangedAsObservable().Subscribe(x =>
            //{
            //    music.time = processBar.value;



            //    PlayController.Instance.laneNotes = new List<Queue<NoteObject>>(PlayController.Instance.editLaneNotes);

            //    //在edit狀態Miss不會SetActive(false)

            //    //如果會卡的話 就改為調整完後放開 才進行Miss相關判定

            //});



            this.UpdateAsObservable().Where(_=> NotesController.Instance.ready).Subscribe(_ =>
            {
                if(!isSetProcessBar)
                {
                    processBar.minValue = 0;
                    processBar.maxValue = music.clip.length;
                    isSetProcessBar = true;
                }
                process.Value = music.time;   
                //process.Value = music.time / music.clip.length;
                //Debug.Log("music.clip.length:" + music.clip.length);
                //Debug.Log("musicTime:" + music.time);

            }).AddTo(this);
            

        }

        public void SetVolume()
        {
            volume = PlayerSettings.Instance.musicVolume;
            music.volume = volume;
        }
        public bool IsPlaying()
        {
            return music.isPlaying;
        }

        public void PlayMusic()
        {
            music.Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            music.clip = clip;
            music.Play();
        }

        public void SetMusic(AudioClip clip)
        {
            
            music.clip = clip;
            music.time = 0;
            
            music.Stop();
            
            //musicName.text = music.clip.name;
        }

        public void PauseMusic(bool isPause=true)
        {
            if(isPause)
                music.Pause();
            else
                music.UnPause();
            this.isPause = isPause;
        }


        public AudioClip GetMusic()
        {
            return music.clip;
        }

        /// <summary>
        /// PCM 樣本中的播放位置。
        /// Playback position in PCM samples.
        /// 用以讀取當前播放時間或在樣本中尋找新的播放時間。
        /// </summary>
        /// <returns></returns>
        public int GetSamples()
        {
            return music.timeSamples;
        }

        public float GetTime()
        {
            return music.time;
        }

        void Update()
        {
            samples = GetSamples();

            //ShowMusicTime();
        }

        public float SampleToTime(float s)
        {
            if(music.clip == null) return 0;

            return ConvertUtils.SamplesToTime(s, music.clip.frequency);
        }

        public float TimeToSample(float time)
        {
            if (music.clip == null) return 0;

            return time * music.clip.frequency;
        }

        /*
        private void ShowMusicTime()
        {
            //musicName.text = music.clip.name;
            if (music == null || music.clip == null)
                return;
            musicTime.text = ComputeUtility.FormatTwoTime((int)music.time) + ":" + ComputeUtility.FormatTwoTime((int)music.clip.length);
        }
        */
    }
}


