using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEditor;
using UnityEngine;


namespace zipLib
{


    public static class ZipHelper
    {
        //解壓Zip包
        public static bool UnzipFile(string pZipFilePath,       //zip文件路徑
                                     string pOutPath,           //解壓路徑
                                     string pPassword = null,   //密碼
                                     UnzipCallback pCB = null)  //回調
        {
            if (string.IsNullOrEmpty(pZipFilePath) || string.IsNullOrEmpty(pOutPath))
            {
                pCB?.OnFinished(false);
                return false;
            }

            return UnzipFile(File.OpenRead(pZipFilePath), pOutPath, pPassword, pCB);
        }

        //解壓Zip包
        public static bool UnzipFile(byte[] pFileBytes,         //zip文件二進制數據
                                     string pOutPath,           //解壓目錄
                                     string pPassword = null,   //密碼
                                     UnzipCallback pCB = null)  //回調
        {
            if (null == pFileBytes || string.IsNullOrEmpty(pOutPath))
            {
                pCB?.OnFinished(false);
                return false;
            }

            return UnzipFile(new MemoryStream(pFileBytes), pOutPath, pPassword, pCB);
        }

        //解壓Zip
        public static bool UnzipFile(Stream pInputStream,       //Zip文件流
                                     string pOutPath,           //解壓路徑
                                     string pPassword = null,   //密碼
                                     UnzipCallback pCB = null)  //回調
        {
            if (null == pInputStream || string.IsNullOrEmpty(pOutPath))
            {
                pCB?.OnFinished(false);
                return false;
            }

            // 創建文件目錄
            if (!Directory.Exists(pOutPath))
                Directory.CreateDirectory(pOutPath);

            // 解壓Zip包
            using (var zipInputStream = new ZipInputStream(pInputStream))
            {
                if (!string.IsNullOrEmpty(pPassword))
                    zipInputStream.Password = pPassword;

                ZipEntry entry = null;
                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    
                    //if(!entry.IsUnicodeText)
                    //    entry.IsUnicodeText = true;
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    if (null != pCB && !pCB.OnPreUnzip(entry))
                        continue; // 過濾

                    var filePathName = Path.Combine(pOutPath, entry.Name);

                    // 創建文件目錄
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    // 寫入文件
                    try
                    {
                        using (var fileStream = File.Create(filePathName))
                        {
                            var bytes = new byte[2048];
                            while (true)
                            {
                                var count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                {
                                    fileStream.Write(bytes, 0, count);
                                }
                                else
                                {
                                    pCB?.OnPostUnzip(entry);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        Debug.LogError("[UnzipFile]: " + _e);
                        GC.Collect();
                        pCB?.OnFinished(false);
                        return false;
                    }
                }
            }

            GC.Collect();
            pCB?.OnFinished(true);
            return true;
        }

        //壓縮文件和文件夾
        public static bool Zip(string[] pFileOrDirArray,    //需要壓縮的文件和文件夾
                               string pZipFilePath,         //輸出的zip文件完整路徑
                               string pPassword = null,     //密碼
                               ZipCallback pCB = null,      //回調
                               int pZipLevel = 6)           //壓縮等級
        {
            if (null == pFileOrDirArray || string.IsNullOrEmpty(pZipFilePath))
            {
                pCB?.OnFinished(false);
                return false;
            }

            var zipOutputStream = new ZipOutputStream(File.Create(pZipFilePath));
            zipOutputStream.SetLevel(pZipLevel); // 6 壓縮品質跟壓縮速度的平衡點
            zipOutputStream.Password = pPassword;

            foreach (string fileOrDirectory in pFileOrDirArray)
            {
                var result = false;

                if (Directory.Exists(fileOrDirectory))
                    result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, pCB);
                else if (File.Exists(fileOrDirectory))
                    result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, pCB);

                if (!result)
                {
                    GC.Collect();
                    pCB?.OnFinished(false);
                    return false;
                }
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();
            zipOutputStream = null;

            GC.Collect();
            pCB?.OnFinished(true);
            return true;
        }

        //压缩文件
        private static bool ZipFile(string pFileName,                   //需要壓縮的文件名
                                    string pParentPath,                 //相對路徑
                                    ZipOutputStream pZipOutputStream,   //壓縮輸出流
                                    ZipCallback pCB = null)             //回調
        {
            ZipEntry entry = null;
            FileStream fileStream = null;
            try
            {
                string path = pParentPath + Path.GetFileName(pFileName);
                entry = new ZipEntry(path) { DateTime = DateTime.Now };

                if (null != pCB && !pCB.OnPreZip(entry))
                    return true; // 過濾

                fileStream = File.OpenRead(pFileName);
                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                entry.Size = buffer.Length;

                pZipOutputStream.PutNextEntry(entry);
                pZipOutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception _e)
            {
                Debug.LogError("[ZipUtility.ZipFile]: " + _e);
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

            pCB?.OnPostZip(entry);

            return true;
        }

        //壓縮文件夾
        private static bool ZipDirectory(string pDirPath,                   //文件夾路徑
                                         string pParentPath,                //相對路徑
                                         ZipOutputStream pZipOutputStream,  //壓縮輸出流
                                         ZipCallback pCB = null)            //回調
        {
            ZipEntry entry = null;
            string path = Path.Combine(pParentPath, GetDirName(pDirPath));
            try
            {
                entry = new ZipEntry(path)
                {
                    DateTime = DateTime.Now,
                    Size = 0
                };

                if (null != pCB && !pCB.OnPreZip(entry))
                    return true; // 過濾

                pZipOutputStream.PutNextEntry(entry);
                pZipOutputStream.Flush();

                var files = Directory.GetFiles(pDirPath);
                foreach (string file in files)
                    ZipFile(file, Path.Combine(pParentPath, GetDirName(pDirPath)), pZipOutputStream, pCB);
            }
            catch (Exception _e)
            {
                Debug.LogError("[ZipDirectory]: " + _e);
                return false;
            }

            var directories = Directory.GetDirectories(pDirPath);
            foreach (string dir in directories)
                if (!ZipDirectory(dir, Path.Combine(pParentPath, GetDirName(pDirPath)), pZipOutputStream, pCB))
                    return false;

            pCB?.OnPostZip(entry);

            return true;
        }

        private static string GetDirName(string pPath)
        {
            if (!Directory.Exists(pPath))
                return string.Empty;

            pPath = pPath.Replace("\\", "/");
            var _Ss = pPath.Split('/');
            if (string.IsNullOrEmpty(_Ss[_Ss.Length - 1]))
                return _Ss[_Ss.Length - 2] + "/";
            return _Ss[_Ss.Length - 1] + "/";
        }

        //壓縮回調接口
        public interface ZipCallback
        {
            bool OnPreZip(ZipEntry _entry); //true表示繼續執行
            void OnPostZip(ZipEntry _entry);
            void OnFinished(bool _result);
        }

        //解壓縮接口
        public interface UnzipCallback
        {
            bool OnPreUnzip(ZipEntry _entry); //true表示繼續執行
            void OnPostUnzip(ZipEntry _entry);
            void OnFinished(bool _result);
        }
    }

    public class ZipResult : ZipHelper.ZipCallback
    {
        public bool OnPreZip(ZipEntry _entry)
        {
            if (_entry.IsFile)
            {
                //Debug.Log(_entry.Name);
                if (GetFileSuffix(_entry.Name) == "meta")
                    return false;
            }
           // _entry.IsUnicodeText = true;
            return true;
        }

        public void OnPostZip(ZipEntry _entry)
        {

        }

        public void OnFinished(bool _result)
        {
            Debug.Log("Zip Finished : " + (Time.realtimeSinceStartup - ZipTest.sTime));
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public string GetFileSuffix(string path)
        {
            int _index = path.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return path.Substring(_index, path.Length - _index);
        }
    }

    public class UnZipResult : ZipHelper.UnzipCallback
    {
        public List<string> postEntrys = new List<string>();
        public bool OnPreUnzip(ZipEntry _entry)
        {
            //Debug.Log("_entry.Name:"+ _entry.Name);
            
            return true;
        }

        public void OnPostUnzip(ZipEntry _entry)
        {
            //Debug.Log(_entry.Name);
            postEntrys.Add(_entry.Name);
        }

        public void OnFinished(bool _result)
        {
            Debug.Log("UnZip Finished : " + (Time.realtimeSinceStartup - ZipTest.sTime));
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }



}