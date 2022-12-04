using NoteEditor.Model;
using UniRx;
using System.Linq;
namespace NoteEditor.Presenter
{
    public class SettingMaxBlockSpinBoxPresenter : SpinBoxPresenterBase
    {
        protected override ReactiveProperty<int> GetReactiveProperty()
        {
            //var clearNoteList = editData.notes.Where(x => x.notes > EditData.MaxBlock.Value-1).ToList().Count;
            return EditData.MaxBlock;
        }
    }
}
