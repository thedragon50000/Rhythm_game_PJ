using System.Collections.Generic;

namespace NoteEditor.DTO
{
    public class MusicDTO
    {
        [System.Serializable]
        public class EditData
        {
            public string name;
            public int maxBlock;
            public int BPM;
            public int offset;
            public List<Note> notes;
        }

        [System.Serializable]
        public class Note
        {
            /// <summary>
            /// 用於指定在一個節拍中播放多少條跟踪線。
            /// LPB 在大多數情況下會設置為 4
            /// 也就是一拍對應4個按鈕，當然也可以增加
            /// Lines Per Beat, and is used to specify how many tracker lines to play in one beat.
            /// In trackers, this value is often important since it sets the edit-resolution.
            /// LPB would in most situations be set to 4, and not changed afterwards.
            /// </summary>
            public int LPB;
            
            /// <summary>
            /// 該音符在所有節拍中的順序編號
            /// </summary>
            public int num;
            
            /// <summary>
            /// 第幾Lane
            /// </summary>
            public int block;
            
            /// <summary>
            /// 先分type再決定方式
            /// 用什麼方式打這個音符
            /// </summary>
            public int type;
            
            /// <summary>
            /// todo:?
            /// </summary>
            public List<Note> notes;
        }
    }
}
