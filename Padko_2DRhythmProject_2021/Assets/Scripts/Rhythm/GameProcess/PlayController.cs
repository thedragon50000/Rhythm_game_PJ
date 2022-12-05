using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NoteEditor.Utility;
using Game.MusicSelect;
using UniRx;
using NoteEditor.Notes;

namespace Game.Process
{
    /// <summary>
    /// 管理键盘输入和音符判定
    /// </summary>
    public class PlayController : SingletonMonoBehaviour<PlayController>
    {
        [HideInInspector] public int laneCount;
        public List<Queue<NoteObject>> laneNotes = new List<Queue<NoteObject>>();
        public List<Queue<NoteObject>> editLaneNotes = new List<Queue<NoteObject>>();
        public bool[] laneHolding;
        Queue<int> tmpLaneHold = new Queue<int>();
        public ReactiveCollection<int> keyStates; //1按一下 2按住
        private int offset;
        private int BPM, LPB;
        public GameObject player;

        private Judgement judgement;
        // private bool autoplay;

        public void Init(int c)
        {
            laneCount = c;

            if (laneNotes.Count == laneCount)
            {
                for (int i = 0; i < laneCount; i++)
                {
                    laneNotes[i].Clear();
                }
            }
            else
            {
                laneNotes.Clear();
                for (int i = 0; i < laneCount; i++)
                {
                    laneNotes.Add(new Queue<NoteObject>());
                }
            }

            if (NotesController.Instance.isEditMode)
            {
                //editLaneNotes = new List<Queue<NoteObject>>(laneNotes);
            }

            laneHolding = new bool[laneCount];
            //keyStates = new int[laneCount];
            for (int i = 0; i < laneCount; i++)
            {
                laneHolding[i] = false;
                keyStates.Add((int)KeyState.none);
                //keyStates[i] = (int)KeyState.none;
            }


            offset = NotesController.Instance.offset;
            BPM = NotesController.Instance.BPM;

            judgement = new Judgement();
            judgement.SetSampleRange(0.05f, 0.10f, 0.125f, 0.15f);
            judgement.SetEarly(-0.15f);
            //judgement.SetEarly(-0.05f);
        }

        public void Quit()
        {
            MainMenuFacade.Instance.sceneChangeFlag = SceneChangeFlag.MainMenuScene;
            //SceneManager.LoadScene("MusicSelect");
        }

        void Update()
        {
            for (int i = 0; i < laneCount; i++)
            {
                //ClickLane(i);

                TestMiss(i);
                if (keyStates[i] == (int)KeyState.hold || keyStates[i] == (int)KeyState.tap)
                {
                    LaneKeyDown(i, isConsecutive: true);
                }

                if (Input.GetKeyDown(PlayerSettings.Instance.GetKeyCode(i, NotesController.Instance.keyType)))
                {
                    keyStates[i] = (int)KeyState.tap;
                    LaneKeyDown(i);
                }

                if (Input.GetKeyUp(PlayerSettings.Instance.GetKeyCode(i, NotesController.Instance.keyType)))
                {
                    keyStates[i] = (int)KeyState.none;
                    LaneKeyUp(i);
                }
            }
        }

        public void NoteEnqueue(NoteObject gn)
        {
            if (gn == null)
                Debug.Log("Note object is null!");
            else if (gn.clicked)
                Debug.Log("Note object is already clicked!");
            else
            {
                var lane = gn.Block();
                if (lane < 0 || lane >= laneCount)
                    Debug.Log("Note's block is false!");
                else
                {
                    laneNotes[gn.Block()].Enqueue(gn);
                    //editLaneNotes[gn.Block()].Enqueue(gn);
                }
            }
        }

        //测试当前最近的音符是否miss
        private void TestMiss(int lane)
        {
            NoteObject note;
            if (laneNotes[lane].Count > 0)
                note = laneNotes[lane].Peek();
            else return;

            //如果该音符已经被错过，直接增加一个miss
            bool isOut = judgement.Out(note);
            if (isOut)
            {
                if (note.isReActive)
                {
                    laneNotes[lane].Dequeue().Miss();
                }

                int missSkill = PlayerController.Instance.OutMissSkill(isOut);
                //if (laneHolding[lane])
                //    laneHolding[lane] = false;
                if (missSkill != ComboPresenter.MISS && note.Type() != 2)
                {
                    ComboPresenter.Instance.Combo(missSkill, note.Block());
                    laneNotes[lane].Dequeue().Click();
                    //if (PlayerSettings.Instance.clap == 1) SEPool.Instance.PlayClap();
                    return;
                }

                ComboPresenter.Instance.Combo(-1, note.Block());
                laneNotes[lane].Dequeue().Miss();

                //如果该音符是长押的第一个音，则第二个音符也miss
                if (note.Type() == 2)
                {
                    laneHolding[lane] = false;

                    var cn = note.GetChainedNote();
                    if (cn != null)
                    {
                        if (cn == laneNotes[lane].Peek())
                        {
                            ComboPresenter.Instance.Combo(-1, note.Block());
                            laneNotes[lane].Dequeue().Miss();
                        }
                    }
                }
            }
        }

        private void LaneAutoPlay(int lane)
        {
            NoteObject gn;
            if (laneNotes[lane].Count > 0)
                gn = laneNotes[lane].Peek();
            else return;

            if (Mathf.Abs(GetDeltaTime(gn)) < MusicController.Instance.TimeToSample(0.1f))
            {
                var type = gn.Type();

                switch (type)
                {
                    case 1:
                        LaneKeyDown(lane);
                        break;
                    case 2:
                        if (laneHolding[lane])
                            LaneKeyUp(lane);
                        else
                            LaneKeyDown(lane);
                        break;
                }
            }
        }

        private void LaneKeyUp(int lane)
        {
            if (MusicController.Instance.GetSamples() <= 0) return;

            if (laneHolding[lane])
            {
                laneHolding[lane] = false;
                //获取最近的仍在判定区的音符
                NoteObject gn;
                if (laneNotes[lane].Count > 0)
                    gn = laneNotes[lane].Peek();
                else
                {
                    return;
                }

                //判定
                var result = judgement.Judge(gn);
                if (gn.Type() != 2)
                    result = PlayerController.Instance.JudgeChangeSkill(result);
                //过早则不予以判定
                if (result == -2 || result == -3)
                {
                    if (gn.GetHoldingBar() != null)
                    {
                        gn.GetHoldingBar().SetColor(Color.gray);
                        laneNotes[lane].Dequeue().Miss();
                        ComboPresenter.Instance.Combo(-1, gn.Block());
                        Debug.Log("BarMiss");
                    }
                    //把下一條刪掉了??

                    //if (gn.GetHoldingBar() != null && tmpLaneHold.Count > 0)
                    //    laneNotes[tmpLaneHold.Dequeue()].Dequeue().Miss();//因為觸發了這個所以無法觸發下面的Click
                    return;
                }

                if (gn.Type() == 1)
                {
                    ComboPresenter.Instance.Combo(result, gn.Block());
                }
                else
                {
                    ComboPresenter.Instance.Combo(ComboPresenter.PERFECT, gn.Block());
                }

                //ComboPresenter.Instance.Combo(result, gn.Block());

                if (result == -1)
                {
                    if (gn.GetHoldingBar() != null)
                        gn.GetHoldingBar().SetColor(Color.gray);
                    laneNotes[lane].Dequeue().Miss();

                    //holdingNoteList.Dequeue().Miss();
                }
                else
                {
                    laneNotes[lane].Dequeue().Click();
                }
            }
        }

        private void LaneKeyDown(int lane, bool isConsecutive = false)
        {
            if (!isConsecutive)
            {
                if (PlayerSettings.Instance.clap == 1) SEPool.Instance.PlayClap();
                PlayerController.Instance.DoAnimation(lane);
            }

            if (MusicController.Instance.GetSamples() <= 0) return;
            //获取最近的仍在判定区的音符
            NoteObject gn;
            if (laneNotes[lane].Count > 0)
                gn = laneNotes[lane].Peek();
            else return;

            //判定
            var result = judgement.Judge(gn);
            //过早则不予以判定
            if (result == -2 || result == -3)
                return;
            var type = gn.Type();
            if (type != 2)
                result = PlayerController.Instance.JudgeChangeSkill(result);
            //Debug.Log("result:" + result);
            if (isConsecutive)
            {
                if (type == 3)
                {
                    result = PlayerController.Instance.JudgeChangeSkill(ComboPresenter.PERFECT);
                    ComboPresenter.Instance.Combo(result, gn.Block());
                    if (result == -1)
                        laneNotes[lane].Dequeue().Miss();
                    else
                        laneNotes[lane].Dequeue().Click();
                    if (PlayerSettings.Instance.clap == 1) SEPool.Instance.PlayClap();
                }
            }
            else
            {
                if (result == -1)
                {
                    if (type == 1 || type == 3)
                    {
                        laneNotes[lane].Dequeue().Miss();
                    }

                    //如果该音符是长押的第一个音，则第二个音符也miss
                    if (type == 2)
                    {
                        laneHolding[lane] = false;
                        var cn = gn.GetChainedNote();
                        if (cn != null)
                        {
                            if (cn == laneNotes[lane].Peek())
                            {
                                ComboPresenter.Instance.Combo(-1, gn.Block());
                                laneNotes[lane].Dequeue().Miss();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    laneNotes[lane].Dequeue().Click();

                    if (type == 2)
                    {
                        laneHolding[lane] = true;
                        tmpLaneHold.Enqueue(lane);
                        //holdingNoteList.Enqueue(gn);
                    }
                }

                if (type == 1 || type == 2)
                {
                    ComboPresenter.Instance.Combo(result, gn.Block());
                }
                else
                {
                    ComboPresenter.Instance.Combo(ComboPresenter.PERFECT, gn.Block());
                }
            }
        }

        private float GetDeltaTime(NoteObject gn)
        {
            //Debug.Log(gn.num + ", " + ConvertUtils.NoteToSamples(gn.note, 1, BPM)+","+playtime);
            float d = (MusicController.Instance.GetSamples() - (gn.time + offset));
            //Debug.Log(gn.name + d);
            //Debug.Log("delta: "+d);
            return d;
        }
    }

    class Judgement
    {
        float[] sampleRange;

        float early;

        MusicController mc;
        float offset;
        int BPM;

        public Judgement()
        {
            mc = MusicController.Instance;
            offset = NotesController.Instance.offset;
            BPM = NotesController.Instance.BPM;
        }

        public void SetSampleRange(float perfect, float great, float good, float bad)
        {
            sampleRange = new float[]
            {
                mc.TimeToSample(perfect),
                mc.TimeToSample(great),
                mc.TimeToSample(good),
                mc.TimeToSample(bad),
            };
        }

        public void SetEarly(float e)
        {
            early = mc.TimeToSample(e);
        }

        public float GetDeltaSample(NoteObject gn)
        {
            float d = (mc.GetSamples() - (gn.time + offset));
            return d;
        }

        ///<summary>
        ///对音符进行判定
        ///<returns>0: perfect; 1: great; 2: good; 3: bad; -1: miss; -2: early, no judgement</returns>
        ///</summary>
        public int Judge(NoteObject gn)
        {
            var delta = GetDeltaSample(gn);

            //if (delta < early)
            //    return -2;

            //if (delta < sampleRange[0])
            //    return -2;
            //Debug.Log("delta: " + delta + ",early:" + early + ",perfect:" + sampleRange[0] + ",great:" +  sampleRange[1]);


            delta = Mathf.Abs(delta);

            //if(delta < sampleRange[0]) //perfect 0.05
            //{

            //}
            //if (delta < 0)
            //    return -2;
            //Debug.Log("delta:" + delta);
            //Debug.Log("perfect:"+ sampleRange[0]);
            //Debug.Log("great:" + sampleRange[1]);

            for (int i = 0; i < 3; i++)
            {
                if (delta <= sampleRange[i]) return i;
                //if (delta >= sampleRange[i])
                //{
                //    return i;
                //}
            }

            return -2;
        }

        public bool Out(NoteObject gn)
        {
            //就是沒按按鍵導致的MISS
            var delta = GetDeltaSample(gn);
            return delta > sampleRange[3];
        }

        public bool IsMissed(NoteObject gn)
        {
            return Judge(gn) == -1;
        }
    }
}