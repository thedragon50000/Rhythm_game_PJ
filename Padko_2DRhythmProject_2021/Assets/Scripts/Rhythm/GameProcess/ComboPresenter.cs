using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using NoteEditor.Utility;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using Game.MusicSelect;


namespace Game.Process
{
    public class ComboPresenter : SingletonMonoBehaviour<ComboPresenter>
    {
        public Text clickText, comboText;

        // public float showTime = 0.2f;
        public float leftTime = 0;
        public const int MISS = -1, PERFECT = 0, GREAT = 1, GOOD = 2, BAD = 3;
        public int miss, perfect, great, good, bad;
        float perfectAddition = 1, greatAddition = 0.5f, goodAddition = 0.2f;
        public GameObject missObj, perfectObj, greatObj, goodObj, badObj;
        public Text missText, perfectText, greatText, goodText, badText;

        public float accuracy; //改以準確率來決定評價 最大為100%
        public Text AccuracyText;

        public ReactiveProperty<int> combo = new ReactiveProperty<int>();
        public ReactiveProperty<int> maxCombo = new ReactiveProperty<int>();
        public Text maxComboText;

        public Button nextButton;
        public GameObject resultMenu;
        public GameObject particleEffectObj;
        public Text resultScoreText;
        public Text scoreText;
        public Text resultGradeText;
        public Text resultNameText;
        public Text resultLevelText;

        bool isFC;
        bool isAP;

        public GameObject resultEffectObj;
        public Text resultEffectText;
        public GameObject FCAPObj;
        public Text FCAPText;
        int notesCount;
        public ReactiveProperty<double> totalScore = new ReactiveProperty<double>();
        float maxScore = 1000000;
        public double judgementScore;
        ResultGrade grade;
        public Transform clickEffectParent;

        void Start()
        {
            combo.Subscribe(combo =>
            {
                comboText.text = combo + " x COMBO";
                //Debug.Log(combo);
            });
            totalScore.Subscribe(score => { scoreText.text = "Super Chat: " + Mathf.Round((float) score); });
            nextButton.OnClickAsObservable().Subscribe(_ => { RhythmFacade.Instance.ExitScene(); });
        }


        void Update()
        {
            if (leftTime <= 0)
            {
                clickText.text = "";
            }
            else
            {
                leftTime -= Time.deltaTime;
            }
        }

        public void SetNoteScore(int count)
        {
            notesCount = count;
            judgementScore = maxScore / notesCount;
            //Debug.Log("notesCount:" + notesCount);
        }

        // public void ShowClick(string c)
        // {
        //     clickText.text = c;
        //     leftTime = showTime;
        // }

        public void
            ShowClickEffect(GameObject obj, int noteBlock, bool isEffect = false) //顯示 Perfect Great Good Miss的特效之類的
        {
            var spawnObj = IAssetFactory.SpawnGameObject(obj: obj,
                pos: NotesController.Instance.noteTracks[(int) NotesController.Instance.keyType].targetLines[noteBlock]
                    .transform.position, parent: clickEffectParent, destoryTime: 0.2f);
            if (isEffect)
            {
                var particleEffect = IAssetFactory.SpawnGameObject(obj: particleEffectObj,
                    pos: NotesController.Instance.noteTracks[(int) NotesController.Instance.keyType]
                        .targetLines[noteBlock]
                        .transform.position, destoryTime: 0.2f);
                particleEffect.GetComponent<ParticleSystem>().Play();
            }
        }

        public ResultGrade ShowTotalScore()
        {
            totalScore.Value = Mathf.Round((float) totalScore.Value);
            accuracy = (float) Mathf.Round((float) (perfect + great * greatAddition + good * goodAddition) /
                NotesController.Instance.notesCount * 10000) / 100;
            resultScoreText.text = "Super Chat: " + totalScore.Value;
            AccuracyText.text = "Accuracy: " + accuracy + "%";
            int addCoin = 100;
            if (accuracy >= 100)
            {
                grade = ResultGrade.EX;
                addCoin *= 10;
            }
            else if (accuracy >= 95)
            {
                grade = ResultGrade.S;
                addCoin *= 5;
            }
            else if (accuracy >= 90)
            {
                grade = ResultGrade.A;
                addCoin *= 3;
            }
            else if (accuracy >= 80)
            {
                grade = ResultGrade.B;
            }
            else if (accuracy >= 70)
            {
                grade = ResultGrade.C;
            }
            else if (accuracy >= 50)
            {
                grade = ResultGrade.D;
            }
            else
            {
                grade = ResultGrade.E;
            }

            PlayerSettings.Instance.coin += addCoin;
            resultGradeText.text = grade.ToString();
            return grade;
        }

        public void ShowResultEffect(bool isAct = true)
        {
            resultEffectObj.SetActive(isAct);
            if (!isAct)
                return;
            // if (maxCombo.Value < combo.Value) maxCombo = combo;

            if (notesCount <= maxCombo.Value)
            {
                isFC = true;
                resultEffectText.text = "FullCombo";
            }

            if (notesCount <= perfect)
            {
                isAP = true;
                resultEffectText.text = "AllPerfect";
            }

            if (!isFC && !isAP)
                resultEffectText.text = "LiveClear";
        }

        public void ShowResult()
        {
            if (resultMenu.activeSelf)
            {
                resultMenu.SetActive(false);
                //return;
            }

            EndCombo();
            //先判定FC跟AP 並進行相關特效
            ShowResultEffect(isAct: true);

            //然後過幾秒才顯示全部分數
            Observable.Timer(TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    ShowResultEffect(isAct: false);
                    clickText.gameObject.SetActive(false);

                    resultMenu.SetActive(true);

                    missText.text = "" + (miss + bad);
                    perfectText.text = "" + perfect;
                    greatText.text = "" + great;
                    goodText.text = "" + good;


                    resultNameText.text = "" + Path.GetFileNameWithoutExtension(NotesController.Instance.musicName);
                    resultLevelText.text = "" + NotesController.Instance.musicLevel;
                    resultLevelText.color = NoteProp.GetLevelColor(NotesController.Instance.musicLevel);
                    if (isFC)
                    {
                        FCAPObj.SetActive(true);
                        FCAPText.text = "FC";
                    }
                    else
                    {
                        FCAPObj.SetActive(false);
                    }

                    if (isAP)
                        FCAPText.text = "AP";

                    ResultGrade grade = ShowTotalScore();

                    maxComboText.text = "Max Combo: " + maxCombo.Value;

                    //最佳成績要存進json裡面

                    var saveData = PlayerSettings.Instance.saveData;
                    bool isFind = false;
                    if (saveData != null)
                    {
                        //有找到存檔的話就修改 沒找到就新增
                        foreach (var item in saveData.listHighScoreDatas)
                        {
                            if (item.name == NotesController.Instance.musicName)
                            {
                                //分數和maxCombo以及isFCAP都是只要比他大就寫入進去
                                if (item.accuracy < accuracy)
                                {
                                    item.accuracy = accuracy;
                                }

                                if (item.score < totalScore.Value)
                                {
                                    item.score = totalScore.Value;
                                }

                                if (isFC)
                                {
                                    item.isFC = isFC;
                                }

                                if (isAP)
                                {
                                    item.isAP = isAP;
                                }

                                if ((int) item.grade < (int) grade)
                                {
                                    item.grade = grade;
                                }

                                if (item.maxCombo < maxCombo.Value)
                                {
                                    item.maxCombo = maxCombo.Value;
                                }

                                isFind = true;
                                break;
                            }
                        }
                    }

                    if (!isFind)
                    {
                        MaxScoreData maxScore = new MaxScoreData();
                        maxScore.name = NotesController.Instance.musicName;
                        maxScore.score = totalScore.Value;
                        maxScore.isFC = isFC;
                        maxScore.isAP = isAP;
                        maxScore.grade = grade;
                        maxScore.accuracy = accuracy;
                        maxScore.maxCombo = maxCombo.Value;
                        saveData.listHighScoreDatas.Add(maxScore);
                    }

                    PlayerSettings.Instance.SaveJsonData();
                }).AddTo(this);
        }


        public void Combo(int i, int noteBlock)
        {
            var judgeScore = PlayerController.Instance.ScoreSkill(judgementScore);
            PlayerController.Instance.JudgeSkill(i);
            switch (i)
            {
                case MISS:
                    miss++;
                    ShowClickEffect(missObj, noteBlock);
                    PlayerController.Instance.SetHP(-10);
                    //ShowClick("Miss");
                    EndCombo();
                    break;
                case PERFECT:
                    perfect++;
                    ShowClickEffect(perfectObj, noteBlock, isEffect: true);
                    totalScore.Value += judgeScore;
                    //ShowClick("Perfect");
                    combo.Value++;
                    break;
                case GREAT:
                    great++;
                    ShowClickEffect(greatObj, noteBlock, isEffect: true);
                    totalScore.Value += judgeScore * greatAddition; //0.5f
                    //ShowClick("Great");
                    combo.Value++;
                    break;
                case GOOD:
                    good++;
                    ShowClickEffect(goodObj, noteBlock, isEffect: true);
                    totalScore.Value += judgeScore * goodAddition; //0.2f
                    //ShowClick("Good");
                    combo.Value++;
                    //EndCombo();
                    break;
                case BAD:
                    bad++;
                    ShowClickEffect(missObj, noteBlock);
                    PlayerController.Instance.SetHP(-10);
                    //ShowClick("Bad");
                    EndCombo();
                    break;

                default:
                    break;
            }

            PlayerController.Instance.ComboSkill(combo.Value);
        }

        private void EndCombo()
        {
            if (combo.Value > maxCombo.Value) maxCombo.Value = combo.Value;
            combo.Value = 0;
        }
    }
}

public class MaxScoreData
{
    public string name;

    // public int miss, perfect, great, good;
    public bool isFC, isAP;
    public int maxCombo;
    public double score;
    public float accuracy;
    public ResultGrade grade;
}

public enum ResultGrade
{
    E,
    D,
    C,
    B,
    A,
    S,
    EX,
    MAX
}