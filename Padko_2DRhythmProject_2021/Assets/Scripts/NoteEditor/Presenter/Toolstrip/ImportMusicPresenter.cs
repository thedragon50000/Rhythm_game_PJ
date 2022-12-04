using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SFB;
using System.IO;
using UnityEditor;
using ICSharpCode.SharpZipLib.Zip;
using zipLib;
using System.Linq;
using System;

namespace NoteEditor.Presenter
{
    public class ImportMusicPresenter : MonoBehaviour
    {
        public Button importButton;


        public List<string> waitingRemoveDirectorys = new List<string>();

        public List<string> retentionFolder = new List<string> { "Musics", "Notes", "test" };

        // Start is called before the first frame update
        void Start()
        {


            //Observable.EveryUpdate()
            //.Where(_ => waitingRemoveDirectorys.Count > 0)
            //.Subscribe(_ =>
            //{
            //    for (int i = 0; i < waitingRemoveDirectorys.Count; i++)
            //    {
            //        var item = waitingRemoveDirectorys[i];
            //        if (Directory.Exists(item))
            //        {
            //            Debug.Log("Count" + Directory.GetFiles(item).Count());
            //            if (Directory.GetFiles(item).Count() <= 0)
            //            {
            //                Directory.Delete(item,true);
            //                waitingRemoveDirectorys.RemoveAt(i);
            //            }
            //        }
            //    }
            //    Observable.Timer(TimeSpan.FromSeconds(1))
            //    .Subscribe(_ =>
            //    {
            //    #if UNITY_EDITOR
            //            AssetDatabase.Refresh();
            //     #endif
            //    }).AddTo(this) ;
            //}).AddTo(this);


            importButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ImportMusic();
            }); 


        }


        public static void ImportMusic(bool isRhythmMode = false)
        {
            List<ExtensionFilter> extensions = new List<ExtensionFilter>();

            //extensions = new List<> {
            //    new ExtensionFilter("Music Files", "mp3", "wav" ),
            //    new ExtensionFilter("Notes Files", "padko")
            //    };
            if (!isRhythmMode)
            {
                extensions.Add(new ExtensionFilter("Music Files", "mp3", "wav"));
                extensions.Add(new ExtensionFilter("Notes Files", "padko"));
            }
            else
            {
                extensions.Add(new ExtensionFilter("Notes Files", "padko"));
            }
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions.ToArray(), true);
            if (paths.Length > 0)
            {
                //Debug.Log("paths:" + paths[0]);
                for(int i=0 ;i<paths.Length; i++)
                { 
                    string musicPath = Path.Combine(Application.streamingAssetsPath, "Musics");
                    string ext = Path.GetExtension(paths[i]);

                    //Debug.Log("Extension:" + ext);
                    //判斷副檔名是 wav 或者 padko
                    if (ext == ".mp3" || ext == ".wav")
                    {
                        File.Copy(paths[i], Path.Combine(musicPath, Path.GetFileName(paths[i])), true);
                    }
                    else if (ext == ".padko")
                    {
                        string zipFilePath = paths[i];
                        string zipOutputPath = Application.streamingAssetsPath;
                        var unZipCB = new UnZipResult();
                        ZipHelper.UnzipFile(zipFilePath, zipOutputPath, null, unZipCB);
                        Observable.Timer(TimeSpan.FromSeconds(0))
                        .Subscribe(_ =>
                        {
                            string[] names = unZipCB.postEntrys.Where(name => name.Contains("MusicInfo.json")).ToArray();
                            if (names.Length > 0)
                            {
                                string fileName = default;
                                var musicInfoPath = Path.Combine(zipOutputPath, names[0]);
                                if (File.Exists(musicInfoPath))
                                {
                                    var infoJson = File.ReadAllText(musicInfoPath, System.Text.Encoding.UTF8);
                                    var musicInfojson = MusicInfoDataSerializer.Deserialize(infoJson);

                                    fileName = musicInfojson.name;
                                    var sourceMusic = Path.Combine(zipOutputPath, fileName);
                                    var distMusic = Path.Combine(zipOutputPath, "Musics", fileName);

                                    var jsonFold = Path.Combine(zipOutputPath, "Notes", Path.GetFileNameWithoutExtension(fileName));
                                    var sourceJsonPath = Path.Combine(zipOutputPath, Path.GetFileNameWithoutExtension(fileName), Path.ChangeExtension(fileName, "json"));
                                    var distJsonPath = Path.Combine(zipOutputPath, "Notes", Path.GetFileNameWithoutExtension(fileName), Path.ChangeExtension(fileName, "json"));

                                    var sourceInfoJsonPath = Path.Combine(zipOutputPath, Path.GetFileNameWithoutExtension(fileName), "MusicInfo.json");
                                    var distInfoJsonPath = Path.Combine(zipOutputPath, "Notes", Path.GetFileNameWithoutExtension(fileName), "MusicInfo.json");

                                    var sourceDirectory = Path.Combine(zipOutputPath, Path.GetFileNameWithoutExtension(fileName));
                                    var distDirectory = Path.Combine(zipOutputPath, "Notes", Path.GetFileNameWithoutExtension(fileName));
                                    if (File.Exists(distMusic))
                                        File.Delete(distMusic);
                                    File.Move(sourceMusic, distMusic);

                                        //File.Delete(Path.Combine(zipOutputPath, fileName));

                                    if (!Directory.Exists(jsonFold))
                                    {
                                        Directory.CreateDirectory(jsonFold);
                                    }

                                    if (File.Exists(distJsonPath))
                                        File.Delete(distJsonPath);
                                    File.Move(sourceJsonPath, distJsonPath);

                                    if (File.Exists(distInfoJsonPath))
                                        File.Delete(distInfoJsonPath);
                                    File.Move(sourceInfoJsonPath, distInfoJsonPath);
                                        //UnityTool.MoveDirectory(sourceDirectory, distDirectory);
                                        //if (!waitingRemoveDirectorys.Contains(sourceDirectory))
                                        //    waitingRemoveDirectorys.Add(sourceDirectory);
                                    if (Directory.Exists(sourceDirectory))
                                    {
                                        Directory.Delete(sourceDirectory, true);
                                    }
                                        //Directory.Move(sourceDirectory, distDirectory);
                                }
                            }
                        });
                    }
                }
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
    
}
