using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


namespace Game.MainMenu
{
    public class SettingMenu : IBaseUISystem
    {
        public Slider seSlider, musicSlider;
        public Text seText, musicText;
        public Slider offsetSlider;
        public Text offsetText;
        public KeyCodeButton key0, key1, key2, key3, key4;
        private KeyCodeButton[] buttons;
        public Toggle clap;
        public Slider speedSlider;
        public Text speedText;
        public Dropdown resolution;
        public Button applyButton;
        public Button switchButton;
        public GameObject page0, page1;
        private GameObject[] pages;
        private int pageIndex = 0;
        public ReactiveProperty<int> offset = new ReactiveProperty<int>();
        public ReactiveProperty<int> speed = new ReactiveProperty<int>();
        public Button fourKeysBindButton;
        //public ReactiveProperty<string> fourKeysStr = new ReactiveProperty<string>();
        public Text fourKeysButtonText;
        public Text fourKeysBindText;
        public Button fiveKeysBindButton;
        public Text fiveKeysButtonText;
        // public ReactiveProperty<string> fiveKeysStr = new ReactiveProperty<string>();
        public Text fiveKeysBindText;
        public Button sixKeysBindButton;
        public Text sixKeysButtonText;
        // public ReactiveProperty<string> sixKeysStr = new ReactiveProperty<string>();
        public Text sixKeysBindText;//顯示綁定的按鍵
        public bool isInputing;//正在綁定按鍵
        public E_RhythmKeysType keyType;
        int keyMaxCount;
        int keyCount;
        List<KeyCode> kcList = new List<KeyCode>();
        void OnEnable()
        {
            PlayerSettings setting = PlayerSettings.Instance;

            seSlider.value = setting.seVolume;
            seText.text = (int)(100 * seSlider.value) + "%";

            musicSlider.value = setting.musicVolume;
            musicText.text = (int)(100 * musicSlider.value) + "%";

            buttons = new KeyCodeButton[]{key0, key1, key2, key3, key4};
            for(int i = 0;i<5;i++){
                buttons[i].SetKeyCode(setting.GetKeyCode(i));
            }
            //InputKey(fourKeysBindText, RhythmKeysType.four);
            if(fourKeysBindText != null)
            {
                InitKeyBindTextShow(fourKeysButtonText, fourKeysBindText, E_RhythmKeysType.four);
                InitKeyBindTextShow(fiveKeysButtonText, fiveKeysBindText, E_RhythmKeysType.five);
                InitKeyBindTextShow(sixKeysButtonText, sixKeysBindText, E_RhythmKeysType.six);

            }
            clap.isOn = (setting.clap == 1);
            if (setting.playerOffset != 0)
            {
                offset.Value = setting.playerOffset / 100;
                offsetSlider.value = setting.playerOffset / 100;
            }
            else
            {
                offset.Value = 0;
            }
            //offsetText.text = setting.playerOffset + "";
            speed.Value = setting.speed;
            speedSlider.value = setting.speed;
            //speedText.text = setting.speed + "";
            
            pages = new GameObject[] { page0, page1 };
            Switch(0);
        }
        public virtual void Start()
        {
            applyButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Apply();
            });
            switchButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Switch();
            });
            speed.Subscribe(x =>
            {
                speedSlider.value = x;
            }).AddTo(this);
            offset.Subscribe(x =>
            {
                offsetSlider.value = x;
            }).AddTo(this);
            speedSlider.OnValueChangedAsObservable().Subscribe(x =>
            {
                speed.Value = (int)x;
            }).AddTo(this);
            offsetSlider.OnValueChangedAsObservable().Subscribe(x =>
            {
                offset.Value = (int)x;
            }).AddTo(this);

            fourKeysBindButton.OnClickAsObservable().Subscribe(_ =>
            {
                if(!isInputing)
                {
                    InputKeyStart(fourKeysButtonText, fourKeysBindText, E_RhythmKeysType.four);
                }
                else //取消的話
                {
                    InitKeyBindTextShow(fourKeysButtonText, fourKeysBindText, E_RhythmKeysType.four);
                }
                

            }).AddTo(this);
            fiveKeysBindButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (!isInputing)
                {
                    InputKeyStart(fiveKeysButtonText, fiveKeysBindText, E_RhythmKeysType.five);
                }
                else //取消的話
                {
                    InitKeyBindTextShow(fiveKeysButtonText, fiveKeysBindText, E_RhythmKeysType.five);
                }
                //InputKeyUpdate(fiveKeysButtonText, fiveKeysBindText, RhythmKeysType.five);
            }).AddTo(this);
            sixKeysBindButton.OnClickAsObservable().Subscribe(_ =>
            {
                if (!isInputing)
                {
                    InputKeyStart(sixKeysButtonText, sixKeysBindText, E_RhythmKeysType.six);
                }
                else //取消的話
                {
                    InitKeyBindTextShow(sixKeysButtonText, sixKeysBindText, E_RhythmKeysType.six);
                }
            }).AddTo(this);
        }



        public void Update()
        {
            if (isActiveAndEnabled)
            {
                seText.text = (int)(100 * seSlider.value) + "%";
                musicText.text = (int)(100 * musicSlider.value) + "%";
                //offsetText.text = (int)offsetSlider.value + "%";
                //speedText.text = (int)speedSlider.value + "%";
            }
            InputKeyUpdate();

        }

        public void Switch()
        {
            Debug.Log("page:" + (1 - pageIndex));
            Switch(1 - pageIndex);
        }

        public void Switch(int i)
        {
            pageIndex = i;
            pages[i].SetActive(true);
            pages[1 - i].SetActive(false);
            if (fourKeysBindText != null)
            {
                InitKeyBindTextShow(fourKeysButtonText, fourKeysBindText, E_RhythmKeysType.four);
                InitKeyBindTextShow(fiveKeysButtonText, fiveKeysBindText, E_RhythmKeysType.five);
                InitKeyBindTextShow(sixKeysButtonText, sixKeysBindText, E_RhythmKeysType.six);
            }

        }
        public virtual void Apply()
        {
            PlayerSettings setting = PlayerSettings.Instance;
            //setting.SetFiveKeyCodes(key0.keyCode, key1.keyCode, key2.keyCode, key3.keyCode, key4.keyCode);
            setting.SetSettings(musicSlider.value, seSlider.value, clap.isOn ? 1 : 0, (int)offsetSlider.value * 100, (int)speedSlider.value);
            setting.SaveJsonData();
            CloseUI();
        }

        public void InitKeyBindTextShow(Text buttonText, Text keyText, E_RhythmKeysType keysType = E_RhythmKeysType.five)
        {
            var setting = PlayerSettings.Instance;
            isInputing = false;
            buttonText.text = "自定義";
            keyText.text = "";
            int keyMaxCount = 4 + (int)keysType;
            for (int i = 0; i < keyMaxCount; i++)
            {
                keyText.text += PlayerSettings.Instance.GetKeyCode(i, keysType).ToString();
                if(i != keyMaxCount-1)
                    keyText.text += ",";
            }
        }

        void InputKeyStart(Text buttonText, Text keyText, E_RhythmKeysType keysType = E_RhythmKeysType.five)
        {
            isInputing = true;
            buttonText.text = "綁定中";
            keyMaxCount = 4 + (int)keysType;
            keyText.text = "";
            this.keyType = keysType;
            kcList.Clear();
        }
        void InputKeyEnd(Text buttonText, Text keyText, E_RhythmKeysType keysType = E_RhythmKeysType.five)
        {
            isInputing = false;
            buttonText.text = "自定義";
            keyCount = 0;
            //keyText.text = "";
            PlayerSettings.Instance.SetKeyCodes(kcList.ToArray());
        }
        void InputKeyUpdate()
        {
            Text buttonText = default; 
            Text keyText = default;
            if (keyType == E_RhythmKeysType.four)
            {
                buttonText = fourKeysButtonText;
                keyText = fourKeysBindText;
            }
            else if(keyType == E_RhythmKeysType.five)
            {
                buttonText = fiveKeysButtonText;
                keyText = fiveKeysBindText;
            }
            else if (keyType == E_RhythmKeysType.six)
            {
                buttonText = sixKeysButtonText;
                keyText = sixKeysBindText;
            }
                //List<KeyCode> kcList = new List<KeyCode>();
            if (isInputing)
            {
                if (Input.anyKeyDown)
                {
                    for (KeyCode kc = KeyCode.Space; kc < KeyCode.Insert; kc++)
                    {
                        if (Input.GetKeyDown(kc))
                        {
                            kcList.Add(kc);
                            keyText.text += kc.ToString();
                            Debug.Log("kc:" + kc.ToString());
                            if (kcList.Count != keyMaxCount)
                                keyText.text += ",";
                            if(kcList.Count == keyMaxCount)
                            {
                                InputKeyEnd(buttonText, keyText, keyType);
                            }
                            break;
                        }
                    }
                }
            }
        }


        public void SetKeyCode(KeyCode kc)
        {
            //keyCode = kc;
            //keyText.text = keyCode.ToString();
        }
    }
}


