using NoteEditor.DTO;
using NoteEditor.Notes;
using NoteEditor.Presenter;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoteEditor.Model
{
    public class EditDataSerializer
    {
        public static string Serialize()
        {
            var dto = new MusicDTO.EditData();
            dto.BPM = EditData.BPM.Value;
            dto.maxBlock = EditData.MaxBlock.Value;
            dto.offset = EditData.OffsetSamples.Value;
            dto.name = Path.GetFileNameWithoutExtension(EditData.Name.Value);

            var sortedNoteObjects = EditData.Notes.Values
                .Where(note => !((note.note.type == NoteTypes.Long || note.note.type == NoteTypes.Consecutive )&& EditData.Notes.ContainsKey(note.note.prev)))
                .OrderBy(note => note.note.position.ToSamples(Audio.Source.clip.frequency, EditData.BPM.Value));

            dto.listEndingNotes = new List<MusicDTO.Note>();

            foreach (var noteObject in sortedNoteObjects)
            {
                if (noteObject.note.type == NoteTypes.Single)
                {
                    dto.listEndingNotes.Add(ToDTO(noteObject));
                }
                else if (noteObject.note.type == NoteTypes.Long || noteObject.note.type == NoteTypes.Consecutive)
                {
                    var current = noteObject;
                    var note = ToDTO(noteObject);

                    while (EditData.Notes.ContainsKey(current.note.next))
                    {
                        var nextObj = EditData.Notes[current.note.next];
                        note.listEndingNotes.Add(ToDTO(nextObj));
                        current = nextObj;
                    }

                    dto.listEndingNotes.Add(note);
                }
            }

            return UnityEngine.JsonUtility.ToJson(dto);
        }




        public static void Deserialize(string json)
        {
            var editData = UnityEngine.JsonUtility.FromJson<MusicDTO.EditData>(json);
            var notePresenter = EditNotesPresenter.Instance;

            EditData.BPM.Value = editData.BPM;
            EditData.MaxBlock.Value = editData.maxBlock;
            EditData.OffsetSamples.Value = editData.offset;

            foreach (var note in editData.listEndingNotes)
            {
                if (note.type == 1)
                {
                    notePresenter.AddNote(ToNoteObject(note));
                    continue;
                }

                var longNoteObjects = new[] { note }.Concat(note.listEndingNotes)
                    .Select(note_ =>
                    {
                        notePresenter.AddNote(ToNoteObject(note_));
                        return EditData.Notes[ToNoteObject(note_).position];
                    })
                    .ToList();

                for (int i = 1; i < longNoteObjects.Count; i++)
                {
                    longNoteObjects[i].note.prev = longNoteObjects[i - 1].note.position;
                    longNoteObjects[i - 1].note.next = longNoteObjects[i].note.position;
                }

                EditState.LongNoteTailPosition.Value = NotePosition.None;
            }
        }

        static MusicDTO.Note ToDTO(NoteObject noteObject)
        {
            var note = new MusicDTO.Note();
            note.num = noteObject.note.position.num;
            note.block = noteObject.note.position.block;
            note.LPB = noteObject.note.position.LPB;
            if(noteObject.note.type == NoteTypes.Single)
                note.type = 1;
            else if (noteObject.note.type == NoteTypes.Long)
                note.type = 2;
            else if (noteObject.note.type == NoteTypes.Consecutive)
                note.type = 3;
            //note.type = noteObject.note.type == NoteTypes.Long ? 2 : 1;
            note.listEndingNotes = new List<MusicDTO.Note>();
            return note;
        }

        public static Note ToNoteObject(MusicDTO.Note musicNote)
        {
            NoteTypes noteType = NoteTypes.Single;
            if (musicNote.type == 1)
                noteType = NoteTypes.Single;
            else if(musicNote.type == 2)
                noteType = NoteTypes.Long;
            else if(musicNote.type == 3)
                noteType = NoteTypes.Consecutive;
            return new Note(
                //new NotePosition(musicNote.LPB, musicNote.num, musicNote.block),
                //musicNote.type == 1 ? NoteTypes.Single : NoteTypes.Long
                new NotePosition(musicNote.LPB, musicNote.num, musicNote.block),
                noteType
                );
        }
    }
}
