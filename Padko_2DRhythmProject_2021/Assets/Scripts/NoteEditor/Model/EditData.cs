using NoteEditor.Notes;
using NoteEditor.Utility;
using System.Collections.Generic;
using UniRx;

namespace NoteEditor.Model
{
    public class EditData : SingletonMonoBehaviour<EditData>
    {
        ReactiveProperty<string> name_ = new();
        ReactiveProperty<int> maxBlock_ = new(5);
        ReactiveProperty<int> LPB_ = new(4);
        ReactiveProperty<int> BPM_ = new(120);
        ReactiveCollection<int> BPMs_ = new();
        ReactiveProperty<int> offsetSamples_ = new(0);
        Dictionary<NotePosition, NoteObject> notes_ = new();

        public static ReactiveProperty<string> Name => Instance.name_;
        public static ReactiveProperty<int> MaxBlock => Instance.maxBlock_;
        public static ReactiveProperty<int> LPB => Instance.LPB_;
        public static ReactiveProperty<int> BPM => Instance.BPM_;

        public static ReactiveCollection<int> BPMs => Instance.BPMs_;
        //把BPM改成List，來讓BPM可以中途變更

        public static ReactiveProperty<int> OffsetSamples => Instance.offsetSamples_;

        public static Dictionary<NotePosition, NoteObject> Notes => Instance.notes_;
    }
}
