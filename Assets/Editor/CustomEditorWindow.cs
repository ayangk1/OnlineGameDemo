using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using FtpWebRequest = System.Net.FtpWebRequest;

namespace Editor
{
    public class CustomEditorWindow : EditorWindow
    {
        private const float windowWidth = 320;
        private const float windowHeight = 240;
        private const float upMargin = 4;
    
        [MenuItem("ABPackage/OpenWindow")]
        public static void OpenWindow()
        {
            CustomEditorWindow window = GetWindowWithRect(typeof(CustomEditorWindow), new Rect(0, 0, windowWidth, windowHeight)) as CustomEditorWindow;
            if (window != null) window.Show();
        }
    
        private static string serverIP = "http://1.14.18.29/";
        private void OnGUI()
        {
            GUI.Label(new Rect(10,upMargin + 7,150,15),"IP:");
            serverIP = GUI.TextField(new Rect(30, upMargin + 5, 290, 20), serverIP);
            if (GUI.Button(new Rect(0, upMargin + 25, windowWidth, 20), "指定文件夹生成版本对比文件"))
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(path) && !Path.HasExtension(path))
                    CreateABCompareFile(path);
            }
            if (GUI.Button(new Rect(0, upMargin + 45, windowWidth, 20), "移动资源文件到streamingAssetsPath"))
            {
                
            }
            if (GUI.Button(new Rect(0, upMargin + 65, windowWidth, 20), "上传文件到服务器"))
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(path) && !Path.HasExtension(path))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(path);
                    FileInfo[] fileInfos = directory.GetFiles();
                    foreach (FileInfo info in fileInfos)
                    {
                        if (info.Extension == "" || info.Extension == ".lua")
                            UploadFtp(info);
                    }
                }
            }
        }
        
        /// <summary>
        /// 上传文件到ftp服务器
        /// </summary>
        private void UploadFtp(FileInfo info)
        {
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://1.14.18.29:39001/UnityHotfix/AB/" + info.Name));
            NetworkCredential n = new NetworkCredential("hFSRZjX9BfJG","cnt4Z9KytT9d");
            req.Credentials = n;
            req.Proxy = null;
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.UseBinary = true;
            
            Stream uploadStream = req.GetRequestStream();
            using (FileStream file = File.OpenRead(info.FullName))
            {
                byte[] bytes = new byte[2048];
                int contentLength = file.Read(bytes, 0, bytes.Length);

                while (contentLength != 0)
                {
                    uploadStream.Write(bytes,0,contentLength);
                    contentLength = file.Read(bytes, 0, bytes.Length);
                }
                
                file.Close();
                uploadStream.Close();
            }
            
            Debug.Log("上传FTP服务器成功");
        }
        

        private void UploadData(FileInfo info)
        {
            HttpWebRequest req = WebRequest.CreateHttp("http://1.14.18.29:8123/uploadData");
            //HttpWebRequest req = WebRequest.CreateHttp("http://127.0.0.1:8123/uploadData");
            req.KeepAlive = false;
            req.Method = "PUT";
            req.Headers = new WebHeaderCollection();
            req.Headers.Add("name",info.Name);

            Stream uploadStream = req.GetRequestStream();
            using (FileStream file = File.OpenRead(info.FullName))
            {
                byte[] bytes = new byte[2048];
                int contentLength = file.Read(bytes, 0, bytes.Length);

                while (contentLength != 0)
                {
                    uploadStream.Write(bytes,0,contentLength);
                    contentLength = file.Read(bytes, 0, bytes.Length);
                }
                
                file.Close();
                uploadStream.Close();
            }
            
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            try
            {
                using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                {
                    string data = reader.ReadToEnd();
                    Debug.Log(data);
                }
            }
            finally
            {
                res.Dispose();
            }
        }
        
        

        public void MoveToAntherFloder()
        {
        
        }
    
        public void CreateABCompareFile(string floaderPath)
        {
            if (File.Exists(Path.Combine(floaderPath + "/ABCompareInfo.txt")))
                File.Delete(Path.Combine(floaderPath + "/ABCompareInfo.txt"));
        
            DirectoryInfo directory = Directory.CreateDirectory(floaderPath);
            FileInfo[] fileInfos = directory.GetFiles();
            string abCompareInfo = "";
            foreach (FileInfo info in fileInfos)
            {
                if (info.Extension == "" || info.Extension == ".lua")
                {
                    //Debug.Log("filename:" + info.Name + "filesize:" + info.Length);
                    abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                    abCompareInfo += '|';
                }
            }
            abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);
            File.WriteAllText(Path.Combine(floaderPath + "/ABCompareInfo.txt"),abCompareInfo);
            AssetDatabase.Refresh();
            Debug.Log(floaderPath + "中AB包生成成功！");
        }
    
        private static string GetMD5(string filePath)
        {
            using (FileStream file = new FileStream(filePath,FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] md5Info = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                foreach (var t in md5Info)
                    sb.Append(t.ToString("x2"));
                return sb.ToString();
            } 
        }
    
    }
}
