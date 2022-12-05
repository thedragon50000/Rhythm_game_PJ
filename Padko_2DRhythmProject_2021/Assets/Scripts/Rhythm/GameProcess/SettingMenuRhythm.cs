using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.MainMenu;
using UniRx;
using UnityEngine.UI;
using Game.MainMenu;
using UniRx.Triggers;



namespace Game.Process
{
    public class SettingMenuRhythm : SettingMenu
    {
        public Button reStartButton;
        public Sprite editBackImage;//回到演奏模式 並讓演奏模式選擇某首歌的模式
        public Button exitButton;



        public override void Start()
        {
            Init();
            var keyType = NotesController.Instance.keyType;
            var judgmentPointText = NotesController.Instance.noteTracks[(int)keyType].noteFieldsText;

            for (int i=0; i<judgmentPointText.Count; i++)
            {
                judgmentPointText[i].text = PlayerSettings.Instance.GetKeyCode(i, NotesController.Instance.keyType).ToString();
            }
            if(NotesController.Instance.isEditMode)
            {
                mOpenButton.image.sprite = editBackImage;
            }

            //按ESC也要能觸發
            Observable.EveryUpdate().Where(_ => !PlayerController.Instance.isDead)
                .Subscribe(_ =>
                {
                    if(Input.GetKeyDown(KeyCode.F4))
                    {
                        RhythmFacade.Instance.ExitScene();
                    }
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (!MusicController.Instance.isPause)
                        {
                            OpenUI();
                        }
                        else
                        {
                            Apply();
                        }
                    }
                }).AddTo(this);



            applyButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Apply();
                //Pause(false);
            }).AddTo(this);
            switchButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Switch();
            }).AddTo(this);


            #region 重新開始

            var reStartButtonStream = reStartButton.OnClickAsObservable();

            var reStartKeyCodeStream = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.F5));

            Observable.Merge(reStartButtonStream, reStartKeyCodeStream)
                .Subscribe(_ =>
                {
                    Apply();
                    MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.RhythmScene;
                    // do something
                }).AddTo(this);

            #endregion



            exitButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Apply();
                //Pause(false);
                RhythmFacade.Instance.ExitScene();
                //MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MusicSelect;
            }).AddTo(this);
        }
        public void Pause(bool isPause=true)
        {
            if(isPause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
            MusicController.Instance.PauseMusic(isPause);
            Debug.Log("Time.timeScale:" + Time.timeScale);
        }
        public override void OpenUI()
        {
            if (PlayerController.Instance.isDead)
            {
                return;
            }

            OpenPauseUI();

        }
        void OpenPauseUI()
        {
            base.OpenUI();
            if (MainMenuFacade.Instance.isMenuOpen)
            {
                Pause();
            }
        }

        public override void CloseUI()
        {
            base.CloseUI();
            if (!MainMenuFacade.Instance.isMenuOpen)
            {
                Pause(false);
            }
        }
        public void SetRhythmSetting()//讓設定影響音遊的譜面
        {
            //music
            //sound
            //offset
            //speed
            //clap
            MusicController.Instance.SetVolume();
            //MusicController.Instance.volume = PlayerSettings.Instance.musicVolume;
            SEPool.Instance.volume = PlayerSettings.Instance.seVolume;
            NotesController.Instance.playerOffset = PlayerSettings.Instance.playerOffset;
        }
        public override void Apply()
        {
            base.Apply();
            SetRhythmSetting();
        }
    }
}