
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class ZipWrapper : MonoBehaviour
{
    #region ZipCallback
    public abstract class ZipCallback
    {
        /// <summary>
        /// ????文件或文件?前?行的回?
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns>如果返回true，???文件或文件?，反之?不??文件或文件?</returns>
        public virtual bool OnPreZip(ZipEntry _entry)
        {
            return true;
        }

        /// <summary>
        /// ????文件或文件?后?行的回?
        /// </summary>
        /// <param name="_entry"></param>
        public virtual void OnPostZip(ZipEntry _entry) { }

        /// <summary>
        /// ???行完?后的回?
        /// </summary>
        /// <param name="_result">true表示??成功，false表示??失?</param>
        public virtual void OnFinished(bool _result) { }
    }
    #endregion

    #region UnzipCallback
    public abstract class UnzipCallback
    {
        /// <summary>
        /// 解???文件或文件?前?行的回?
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns>如果返回true，???文件或文件?，反之?不??文件或文件?</returns>
        public virtual bool OnPreUnzip(ZipEntry _entry)
        {
            return true;
        }

        /// <summary>
        /// 解???文件或文件?后?行的回?
        /// </summary>
        /// <param name="_entry"></param>
        public virtual void OnPostUnzip(ZipEntry _entry) { }

        /// <summary>
        /// 解??行完?后的回?
        /// </summary>
        /// <param name="_result">true表示解?成功，false表示解?失?</param>
        public virtual void OnFinished(bool _result) { }
    }
    #endregion

    /// <summary>
    /// ??文件和文件?
    /// </summary>
    /// <param name="_fileOrDirectoryArray">文件?路?和文件名</param>
    /// <param name="_outputPathName">??后的?出路?文件名</param>
    /// <param name="_password">??密?</param>
    /// <param name="_zipCallback">ZipCallback?象，??回?</param>
    /// <returns></returns>
    public static bool Zip(string[] _fileOrDirectoryArray, string _outputPathName, string _password = null, ZipCallback _zipCallback = null)
    {
        if ((null == _fileOrDirectoryArray) || string.IsNullOrEmpty(_outputPathName))
        {
            if (null != _zipCallback)
                _zipCallback.OnFinished(false);

            return false;
        }

        ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(_outputPathName));
        zipOutputStream.SetLevel(6);    // ???量和??速度的平衡?
        if (!string.IsNullOrEmpty(_password))
            zipOutputStream.Password = _password;

        for (int index = 0; index < _fileOrDirectoryArray.Length; ++index)
        {
            bool result = false;
            string fileOrDirectory = _fileOrDirectoryArray[index];
            if (Directory.Exists(fileOrDirectory))
                result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);
            else if (File.Exists(fileOrDirectory))
                result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, _zipCallback);

            if (!result)
            {
                if (null != _zipCallback)
                    _zipCallback.OnFinished(false);

                return false;
            }
        }

        zipOutputStream.Finish();
        zipOutputStream.Close();

        if (null != _zipCallback)
            _zipCallback.OnFinished(true);

        return true;
    }

    /// <summary>
    /// 解?Zip包
    /// </summary>
    /// <param name="_filePathName">Zip包的文件路?名</param>
    /// <param name="_outputPath">解??出路?</param>
    /// <param name="_password">解?密?</param>
    /// <param name="_unzipCallback">UnzipCallback?象，??回?</param>
    /// <returns></returns>
    public static bool UnzipFile(string _filePathName, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        try
        {
            return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password, _unzipCallback);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }
    }

    /// <summary>
    /// 解?Zip包
    /// </summary>
    /// <param name="_fileBytes">Zip包字???</param>
    /// <param name="_outputPath">解??出路?</param>
    /// <param name="_password">解?密?</param>
    /// <param name="_unzipCallback">UnzipCallback?象，??回?</param>
    /// <returns></returns>
    public static bool UnzipFile(byte[] _fileBytes, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if ((null == _fileBytes) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        bool result = UnzipFile(new MemoryStream(_fileBytes), _outputPath, _password, _unzipCallback);
        if (!result)
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);
        }

        return result;
    }

    /// <summary>
    /// 解?Zip包
    /// </summary>
    /// <param name="_inputStream">Zip包?入流</param>
    /// <param name="_outputPath">解??出路?</param>
    /// <param name="_password">解?密?</param>
    /// <param name="_unzipCallback">UnzipCallback?象，??回?</param>
    /// <returns></returns>
    public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        // ?建文件目?
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);

        // 解?Zip包
        ZipEntry entry = null;
        using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
        {
            if (!string.IsNullOrEmpty(_password))
                zipInputStream.Password = _password;

            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                if ((null != _unzipCallback) && !_unzipCallback.OnPreUnzip(entry))
                    continue;   // ??

                string filePathName = Path.Combine(_outputPath, entry.Name);

                // ?建文件目?
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(filePathName);
                    continue;
                }

                // ?入文件
                try
                {
                    using (FileStream fileStream = File.Create(filePathName))
                    {
                        byte[] bytes = new byte[2048];
                        while (true)
                        {
                            int count = zipInputStream.Read(bytes, 0, bytes.Length);
                            if (count > 0)
                                fileStream.Write(bytes, 0, count);
                            else
                            {
                                if (null != _unzipCallback)
                                    _unzipCallback.OnPostUnzip(entry);

                                break;
                            }
                        }
                    }
                }
                catch (System.Exception _e)
                {
                    Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

                    if (null != _unzipCallback)
                        _unzipCallback.OnFinished(false);

                    return false;
                }
            }
        }

        if (null != _unzipCallback)
            _unzipCallback.OnFinished(true);

        return true;
    }

    /// <summary>
    /// ??文件
    /// </summary>
    /// <param name="_filePathName">文件路?名</param>
    /// <param name="_parentRelPath">要??的文件的父相?文件?</param>
    /// <param name="_zipOutputStream">???出流</param>
    /// <param name="_zipCallback">ZipCallback?象，??回?</param>
    /// <returns></returns>
    private static bool ZipFile(string _filePathName, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null)
    {

        //Crc32 crc32 = new Crc32();
        ZipEntry entry = null;
        FileStream fileStream = null;
        try
        {
            string entryName = _parentRelPath + '/' + Path.GetFileName(_filePathName);
            entry = new ZipEntry(entryName);
            entry.DateTime = System.DateTime.Now;

            if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                return true;    // ??

            fileStream = File.OpenRead(_filePathName);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Close();

            entry.Size = buffer.Length;

            //crc32.Reset();
            //crc32.Update(buffer);
            //entry.Crc = crc32.Value;

            _zipOutputStream.PutNextEntry(entry);
            _zipOutputStream.Write(buffer, 0, buffer.Length);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipUtility.ZipFile]: " + _e.ToString());
            return false;
        }
        finally
        {
            if (null != fileStream)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        if (null != _zipCallback)
            _zipCallback.OnPostZip(entry);

        return true;
    }

    /// <summary>
    /// ??文件?
    /// </summary>
    /// <param name="_path">要??的文件?</param>
    /// <param name="_parentRelPath">要??的文件?的父相?文件?</param>
    /// <param name="_zipOutputStream">???出流</param>
    /// <param name="_zipCallback">ZipCallback?象，??回?</param>
    /// <returns></returns>
    private static bool ZipDirectory(string _path, string _parentRelPath, ZipOutputStream _zipOutputStream, ZipCallback _zipCallback = null)
    {
        ZipEntry entry = null;
        try
        {
            string entryName = Path.Combine(_parentRelPath, Path.GetFileName(_path) + '/');
            entry = new ZipEntry(entryName);
            entry.DateTime = System.DateTime.Now;
            entry.Size = 0;

            if ((null != _zipCallback) && !_zipCallback.OnPreZip(entry))
                return true;    // ??

            _zipOutputStream.PutNextEntry(entry);
            _zipOutputStream.Flush();

            string[] files = Directory.GetFiles(_path);
            for (int index = 0; index < files.Length; ++index)
            {
                // 排除Unity中可能的 .meta 文件
                if (files[index].EndsWith(".meta") == true)
                {
                    Debug.LogWarning(files[index] + " not to zip");
                    continue;
                }

                ZipFile(files[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback);
            }
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipUtility.ZipDirectory]: " + _e.ToString());
            return false;
        }

        string[] directories = Directory.GetDirectories(_path);
        for (int index = 0; index < directories.Length; ++index)
        {
            if (!ZipDirectory(directories[index], Path.Combine(_parentRelPath, Path.GetFileName(_path)), _zipOutputStream, _zipCallback))
            {
                return false;
            }
        }

        if (null != _zipCallback)
            _zipCallback.OnPostZip(entry);

        return true;
    }



}
public class ZipCallback : ZipWrapper.ZipCallback
{
    public override bool OnPreZip(ZipEntry _entry)
    {
        //Debug.Log("OnPreZip Name： " + _entry.Name);
        //Debug.Log("OnPreZip IsFile：" + _entry.IsFile);
        return base.OnPreZip(_entry);
    }

    public override void OnPostZip(ZipEntry _entry)
    {
        //Debug.Log("OnPostZip Name： " + _entry.Name);
    }

    public override void OnFinished(bool _result)
    {
        //Debug.Log("OnZipFinished _result： " + _result);
    }
}

public class UnzipCallback : ZipWrapper.UnzipCallback
{
    public List<string> postEntrys = new List<string>();

    public override bool OnPreUnzip(ZipEntry _entry)
    {
        //Debug.Log("OnPreUnzip Name： " + _entry.Name);
        //Debug.Log("OnPreUnzip IsFile：" + _entry.IsFile);
        _entry.IsUnicodeText = true;
        return base.OnPreUnzip(_entry);
    }

    public override void OnPostUnzip(ZipEntry _entry)
    {
        //Debug.Log("OnPostUnzip Name： " + _entry.Name);
        postEntrys.Add(_entry.Name);
        base.OnPostUnzip(_entry);
    }

    public override void OnFinished(bool _result)
    {
        Debug.Log("OnUnZipFinished _result： " + _result);
        base.OnFinished(_result);
    }
}