
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
        /// ????���Τ��?�e?�檺�^?
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns>�p�G��^true�A???���Τ��?�A�Ϥ�?��??���Τ��?</returns>
        public virtual bool OnPreZip(ZipEntry _entry)
        {
            return true;
        }

        /// <summary>
        /// ????���Τ��?�Z?�檺�^?
        /// </summary>
        /// <param name="_entry"></param>
        public virtual void OnPostZip(ZipEntry _entry) { }

        /// <summary>
        /// ???�槹?�Z���^?
        /// </summary>
        /// <param name="_result">true���??���\�Afalse���??��?</param>
        public virtual void OnFinished(bool _result) { }
    }
    #endregion

    #region UnzipCallback
    public abstract class UnzipCallback
    {
        /// <summary>
        /// ��???���Τ��?�e?�檺�^?
        /// </summary>
        /// <param name="_entry"></param>
        /// <returns>�p�G��^true�A???���Τ��?�A�Ϥ�?��??���Τ��?</returns>
        public virtual bool OnPreUnzip(ZipEntry _entry)
        {
            return true;
        }

        /// <summary>
        /// ��???���Τ��?�Z?�檺�^?
        /// </summary>
        /// <param name="_entry"></param>
        public virtual void OnPostUnzip(ZipEntry _entry) { }

        /// <summary>
        /// ��??�槹?�Z���^?
        /// </summary>
        /// <param name="_result">true��ܸ�?���\�Afalse��ܸ�?��?</param>
        public virtual void OnFinished(bool _result) { }
    }
    #endregion

    /// <summary>
    /// ??���M���?
    /// </summary>
    /// <param name="_fileOrDirectoryArray">���?��?�M���W</param>
    /// <param name="_outputPathName">??�Z��?�X��?���W</param>
    /// <param name="_password">??�K?</param>
    /// <param name="_zipCallback">ZipCallback?�H�A??�^?</param>
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
        zipOutputStream.SetLevel(6);    // ???�q�M??�t�ת�����?
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
    /// ��?Zip�]
    /// </summary>
    /// <param name="_filePathName">Zip�]������?�W</param>
    /// <param name="_outputPath">��??�X��?</param>
    /// <param name="_password">��?�K?</param>
    /// <param name="_unzipCallback">UnzipCallback?�H�A??�^?</param>
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
    /// ��?Zip�]
    /// </summary>
    /// <param name="_fileBytes">Zip�]�r???</param>
    /// <param name="_outputPath">��??�X��?</param>
    /// <param name="_password">��?�K?</param>
    /// <param name="_unzipCallback">UnzipCallback?�H�A??�^?</param>
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
    /// ��?Zip�]
    /// </summary>
    /// <param name="_inputStream">Zip�]?�J�y</param>
    /// <param name="_outputPath">��??�X��?</param>
    /// <param name="_password">��?�K?</param>
    /// <param name="_unzipCallback">UnzipCallback?�H�A??�^?</param>
    /// <returns></returns>
    public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        // ?�ؤ���?
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);

        // ��?Zip�]
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

                // ?�ؤ���?
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(filePathName);
                    continue;
                }

                // ?�J���
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
    /// ??���
    /// </summary>
    /// <param name="_filePathName">����?�W</param>
    /// <param name="_parentRelPath">�n??����󪺤���?���?</param>
    /// <param name="_zipOutputStream">???�X�y</param>
    /// <param name="_zipCallback">ZipCallback?�H�A??�^?</param>
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
    /// ??���?
    /// </summary>
    /// <param name="_path">�n??�����?</param>
    /// <param name="_parentRelPath">�n??�����?������?���?</param>
    /// <param name="_zipOutputStream">???�X�y</param>
    /// <param name="_zipCallback">ZipCallback?�H�A??�^?</param>
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
                // �ư�Unity���i�઺ .meta ���
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
        //Debug.Log("OnPreZip Name�G " + _entry.Name);
        //Debug.Log("OnPreZip IsFile�G" + _entry.IsFile);
        return base.OnPreZip(_entry);
    }

    public override void OnPostZip(ZipEntry _entry)
    {
        //Debug.Log("OnPostZip Name�G " + _entry.Name);
    }

    public override void OnFinished(bool _result)
    {
        //Debug.Log("OnZipFinished _result�G " + _result);
    }
}

public class UnzipCallback : ZipWrapper.UnzipCallback
{
    public List<string> postEntrys = new List<string>();

    public override bool OnPreUnzip(ZipEntry _entry)
    {
        //Debug.Log("OnPreUnzip Name�G " + _entry.Name);
        //Debug.Log("OnPreUnzip IsFile�G" + _entry.IsFile);
        _entry.IsUnicodeText = true;
        return base.OnPreUnzip(_entry);
    }

    public override void OnPostUnzip(ZipEntry _entry)
    {
        //Debug.Log("OnPostUnzip Name�G " + _entry.Name);
        postEntrys.Add(_entry.Name);
        base.OnPostUnzip(_entry);
    }

    public override void OnFinished(bool _result)
    {
        Debug.Log("OnUnZipFinished _result�G " + _result);
        base.OnFinished(_result);
    }
}