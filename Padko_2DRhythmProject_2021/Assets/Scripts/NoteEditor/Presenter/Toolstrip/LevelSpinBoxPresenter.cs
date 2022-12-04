using NoteEditor.Model;
using UniRx;

namespace NoteEditor.Presenter
{
    public class LevelSpinBoxPresenter : SpinBoxPresenterBase
    {
        protected override ReactiveProperty<int> GetReactiveProperty()
        {
            return ExportMusicPresenter.Instance._level;
        }
    }
}
