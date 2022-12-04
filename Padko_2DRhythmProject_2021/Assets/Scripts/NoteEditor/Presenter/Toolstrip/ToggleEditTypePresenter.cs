using NoteEditor.Notes;
using NoteEditor.Model;
using NoteEditor.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace NoteEditor.Presenter
{
    public class ToggleEditTypePresenter : MonoBehaviour
    {
        [SerializeField]
        Button editTypeToggleButton = default;
        [SerializeField]
        Sprite iconLongNotes = default;
        [SerializeField]
        Sprite iconSingleNotes = default;
        [SerializeField]
        Color longTypeStateButtonColor = default;
        [SerializeField]
        Color consecutiveTypeStateButtonColor = default;
        [SerializeField]
        Color singleTypeStateButtonColor = default;

        NoteTypes noteType = NoteTypes.Single;

        void Awake()
        {
            var altKey = this.UpdateAsObservable().Where(_ => KeyInput.AltKeyDown());
            altKey.Subscribe(_ =>
                noteType = NoteTypes.Long
            );
            var tabKey = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Tab));
            tabKey.Subscribe(_ =>
                noteType = NoteTypes.Consecutive
            );
            editTypeToggleButton.OnClickAsObservable()
                .Merge(altKey)
                .Merge(tabKey)
                .Subscribe(editType => EditState.NoteType.Value = noteType);

            var buttonImage = editTypeToggleButton.GetComponent<Image>();

            EditState.NoteType.Select(_ => EditState.NoteType.Value)
                .Subscribe(isLongType =>
                {
                    if(isLongType == NoteTypes.Single)
                    {
                        buttonImage.sprite = iconSingleNotes;
                        buttonImage.color = singleTypeStateButtonColor;
                    }
                    else if(isLongType == NoteTypes.Long)
                    {
                        buttonImage.sprite = iconLongNotes;
                        buttonImage.color = longTypeStateButtonColor;
                    }
                    else if(isLongType == NoteTypes.Consecutive )
                    {
                        buttonImage.sprite = iconLongNotes;
                        buttonImage.color = consecutiveTypeStateButtonColor;
                    }
                    //buttonImage.sprite = isLongType ? iconLongNotes : iconSingleNotes;
                    //buttonImage.color = isLongType ? longTypeStateButtonColor : singleTypeStateButtonColor;
                });
        }
    }
}
