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
            /// LPB是啥小?
            /// </summary>
            public int LPB;
            public int num;
            /// /// <summary>
            /// block是第幾Lane的意思？
            /// </summary>
            public int block;
            
            public int type;
            public List<Note> notes;
        }
    }
}
