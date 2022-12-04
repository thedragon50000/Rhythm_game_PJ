using NoteEditor.Model;
using NoteEditor.Utility;
using System.IO;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoteEditor.Presenter
{
    public class SavePresenter : SingletonMonoBehaviour<SavePresenter>
    {
        [SerializeField]
        Button saveButton = default;
        [SerializeField]
        Text messageText = default;
        [SerializeField]
        Color unsavedStateButtonColor = default;
        [SerializeField]
        Color savedStateButtonColor = Color.white;

        [SerializeField]
        GameObject saveDialog = default;
        [SerializeField]
        Button dialogSaveButton = default;
        [SerializeField]
        Button dialogDoNotSaveButton = default;
        [SerializeField]
        Button dialogCancelButton = default;
        [SerializeField]
        Text dialogMessageText = default;

        ReactiveProperty<bool> mustBeSaved = new ReactiveProperty<bool>();

        void Awake()
        {
            var editPresenter = EditNotesPresenter.Instance;

            //this.UpdateAsObservable()
            //    .Where(_ => Input.GetKeyDown(KeyCode.Escape))
            //    .Subscribe(_ => Application.Quit());

            var saveActionObservable = this.UpdateAsObservable()
                .Where(_ => KeyInput.CtrlPlus(KeyCode.S))
                .Merge(saveButton.OnClickAsObservable());

            mustBeSaved = (ReactiveProperty<bool>)Observable.Merge(
                    EditData.BPM.Select(_ => true),
                    EditData.OffsetSamples.Select(_ => true),
                    EditData.MaxBlock.Select(_ => true),
                    editPresenter.RequestForEditNote.Select(_ => true),
                    editPresenter.RequestForAddNote.Select(_ => true),
                    editPresenter.RequestForRemoveNote.Select(_ => true),
                    editPresenter.RequestForChangeNoteStatus.Select(_ => true),
                    Audio.OnLoad.Select(_ => false),
                    saveActionObservable.Select(_ => false))
                .SkipUntil(Audio.OnLoad.DelayFrame(1))
                .Do(unsaved => saveButton.GetComponent<Image>().color = unsaved ? unsavedStateButtonColor : savedStateButtonColor)
                .ToReactiveProperty();

            mustBeSaved.Where(_ => Audio.Source.clip != null).SubscribeToText(messageText, unsaved => unsaved ? "貼心提示:記得保存" : "");

            saveActionObservable.Subscribe(_ => ExportMusicPresenter.Instance.setExportSetting(ExportType.save));//Save());

            dialogSaveButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    mustBeSaved.Value = false;
                    saveDialog.SetActive(false);
                    ExportMusicPresenter.Instance.setExportSetting(ExportType.save);
                    //Save();
                    Application.Quit();
                });

            dialogDoNotSaveButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    mustBeSaved.Value = false;
                    saveDialog.SetActive(false);
                    Application.Quit();
                });

            dialogCancelButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    saveDialog.SetActive(false);
                });

            Application.wantsToQuit += ApplicationQuit;
        }

        bool ApplicationQuit()
        {
            if (mustBeSaved.Value)
            {
                dialogMessageText.text = "Do you want to save the changes you made in the note '"
                    + EditData.Name.Value + "' ?" + System.Environment.NewLine
                    + "Your changes will be lost if you don't save them.";
                saveDialog.SetActive(true);
                return false;
            }

            return true;
        }

        public void Save()
        {
            var fileName = Path.ChangeExtension(EditData.Name.Value, "json");
            var fileDirectoryName = Path.GetFileNameWithoutExtension(EditData.Name.Value);
            var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes/"+ fileDirectoryName);
            // Notes/音樂名稱/json
            //var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), fileDirectoryName);
            var filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var json = EditDataSerializer.Serialize();
            File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
            messageText.text = filePath + " 保存完畢";

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}
