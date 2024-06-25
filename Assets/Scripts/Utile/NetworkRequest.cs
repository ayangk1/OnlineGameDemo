using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class NetworkRequest : SingletonMonoBehaviour<NetworkRequest>
{
    /// <summary>
    /// http的post请求
    /// </summary>
    /// <param name="_data">请求数据</param>
    /// <param name="url">请求的url</param>
    public void NetworkHttpPost(string _data,string url)
    {
        HttpWebRequest req = WebRequest.CreateHttp(url);
        req.Method = "POST";
        req.KeepAlive = false;

        //注意不要留空格
        string post = _data;
        byte[] bytes = Encoding.UTF8.GetBytes(post);

        Stream uploadStream = req.GetRequestStream();
        uploadStream.Write(bytes, 0, bytes.Length);
        uploadStream.Close();
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        try
        {
            using (StreamReader reader = new StreamReader(res.GetResponseStream() ?? throw new InvalidOperationException()))
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
    
    public void UploadFtp(string url,string filename)
    {
        FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
        NetworkCredential n = new NetworkCredential("hFSRZjX9BfJG","cnt4Z9KytT9d");
        req.Credentials = n;
        req.Proxy = null;
        req.KeepAlive = false;
        req.Method = WebRequestMethods.Ftp.UploadFile;
        req.UseBinary = true;
            
        Stream uploadStream = req.GetRequestStream();
        using (FileStream file = File.OpenRead(filename))
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
    
    
    public void DownloadFtp(string url,string filename,UnityAction<bool> isOver)
    {
        FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
        NetworkCredential n = new NetworkCredential("hFSRZjX9BfJG","cnt4Z9KytT9d");
        req.Credentials = n;
        req.Proxy = null;
        req.KeepAlive = false;
        req.Method = WebRequestMethods.Ftp.DownloadFile;
        req.UseBinary = true;
            
        FtpWebResponse res = req.GetResponse() as FtpWebResponse;
        Stream downloadStream = res.GetResponseStream();
        using (FileStream file = File.Create(filename))
        {
            byte[] bytes = new byte[2048];
            int contentLength = downloadStream.Read(bytes, 0, bytes.Length);

            while (contentLength != 0)
            {
                file.Write(bytes,0,contentLength);
                contentLength = downloadStream.Read(bytes, 0, bytes.Length);
            }
                
            file.Close();
            downloadStream.Close();
        }

        isOver(true);
        Debug.Log("FTP下载成功");
    }
}