
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public class Test_ZipWrapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Test_Zip();

        Test_UnZip();
    }

    #region Test_Zip


    void Test_Zip()
    {

        string[] zipFilePaths = new string[] {
            Application.dataPath+"/SharpZipLibWrapper/Test_Folder/Folder",
            Application.dataPath+"/SharpZipLibWrapper/Test_Folder/TestFile.txt",
        };

        string zipOutputPath = Application.dataPath + "/zipTest.zip";

        ZipWrapper.Zip(zipFilePaths, zipOutputPath, "password", new ZipCallback());
    }



    public class ZipCallback : ZipWrapper.ZipCallback
    {
        public override bool OnPreZip(ZipEntry _entry)
        {
            //Debug.Log("OnPreZip Name¡G " + _entry.Name);
            //Debug.Log("OnPreZip IsFile¡G" + _entry.IsFile);
            return base.OnPreZip(_entry);
        }

        public override void OnPostZip(ZipEntry _entry)
        {
            //Debug.Log("OnPostZip Name¡G " + _entry.Name);
        }

        public override void OnFinished(bool _result)
        {
            //Debug.Log("OnZipFinished _result¡G " + _result);
        }
    }

    #endregion

    #region Test_UnZip

    void Test_UnZip()
    {

        string zipFilePath = Application.dataPath + "/zipTest.zip";
        string zipOutputPath = Application.dataPath + "/UnZip";

        ZipWrapper.UnzipFile(zipFilePath, zipOutputPath, "password", new UnzipCallback());
    }

    public class UnzipCallback : ZipWrapper.UnzipCallback
    {
        public override bool OnPreUnzip(ZipEntry _entry)
        {
            Debug.Log("OnPreUnzip Name¡G " + _entry.Name);
            Debug.Log("OnPreUnzip IsFile¡G" + _entry.IsFile);
            return base.OnPreUnzip(_entry);
        }

        public override void OnPostUnzip(ZipEntry _entry)
        {
            Debug.Log("OnPostUnzip Name¡G " + _entry.Name);
            base.OnPostUnzip(_entry);
        }

        public override void OnFinished(bool _result)
        {
            Debug.Log("OnUnZipFinished _result¡G " + _result);
            base.OnFinished(_result);
        }
    }

    #endregion
}
