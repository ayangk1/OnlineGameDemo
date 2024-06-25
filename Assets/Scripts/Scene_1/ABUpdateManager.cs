using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Net;
using UnityEditor;
using System.Threading.Tasks;
using UnityEngine.Events;
using XLua;

public class ABUpdateManager : MonoBehaviour
{
    private static ABUpdateManager instance;
    public static ABUpdateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ABUpdateManager");
                instance = obj.AddComponent<ABUpdateManager>();
            }
            return instance;
        }
    }
    private void OnDestroy()
    {
        instance = null;
    }

    private readonly string abCompareInfo = "/ABCompareInfo.txt";
    private readonly string hotfixT = "http://1.14.18.29/UnityHotfix/AB/";
    
    private Dictionary<string, ABInfo> remoteAbInfo = new();
    private Dictionary<string, ABInfo> localAbInfo = new();
    public List<string> downLoadList = new();
    public long totalSize = 0;
    public bool checkOver;
    public bool beginUpdate;
    public bool updateOver;
    public IEnumerator DownloadABCompareInfo()
    {
        UnityWebRequest req = UnityWebRequest.Get(hotfixT + "ABCompareInfo.txt");
        yield return req.SendWebRequest();
        //远端数据
        string info = req.downloadHandler.text;
        string[] strs = info.Split('|');
        for (int i = 0; i < strs.Length; i++)
        {
            var infos = strs[i].Split(' ');
            ABInfo abInfo = new ABInfo(infos[0], infos[1], infos[2]);
            remoteAbInfo.Add(infos[0],abInfo);
        }
        
        using (FileStream file = File.Create(Application.persistentDataPath + "/ABCompareInfo_TMP.txt"))
            file.Write(req.downloadHandler.data,0,req.downloadHandler.data.Length);
        
        //获取本地
        GetLocalABCompareFile(isOver =>
        {
            if (isOver)
            {
                Debug.Log("获取本地文件成功");
                //对比AB文件包
                CompareAB();
            }
        });
        
    }

    public void CompareAB()
    {
        //遍历远端对比文件
        foreach (string abName in remoteAbInfo.Keys)
        {
            //如果本地对比文件没有则记录
            if (!localAbInfo.ContainsKey(abName))
            {
                //没有则下载
                downLoadList.Add(abName);
                Debug.Log("本地需要下载文件：" + abName);
            }
            else
            {
                //有则判断MD5码 不一样则更新
                if (localAbInfo[abName].md5 != remoteAbInfo[abName].md5)
                {
                    //将需要下载的文件添加到列表中
                    downLoadList.Add(abName);
                    Debug.Log("本地需要更新文件：" + abName);
                }
                localAbInfo.Remove(abName);
            }
        }

        foreach (string abName in localAbInfo.Keys)
        {
            if (File.Exists(Application.persistentDataPath + "/" + abName))
                File.Delete(Application.persistentDataPath + "/" + abName);
        }

        
        
        foreach (string abName in downLoadList)
        {
            totalSize += long.Parse(remoteAbInfo[abName].size);
        }
        
        //下载需要下载的文件
        // StartCoroutine(DownloadABFile());
        
        checkOver = true;
    }
    
    /// <summary>
    /// 获取本地AB对比文件
    /// </summary>
    public void GetLocalABCompareFile(UnityAction<bool> isOver)
    {
        string persistInfo = Application.persistentDataPath + abCompareInfo;
        string streamingInfo = Application.streamingAssetsPath + abCompareInfo;
        if (File.Exists(persistInfo))
            StartCoroutine(GetLocalFile(persistInfo, over =>
            {
                if (over)
                {
                    Debug.Log("获取本地文件成功（persistentDataPath）");
                    isOver(true);
                }
                else
                {
                    isOver(false);
                }
            }));
        else if (File.Exists(streamingInfo))
            StartCoroutine(GetLocalFile(streamingInfo, over =>
            {
                if (over)
                {
                    Debug.Log("获取本地文件成功（streamingAssetsPath）");
                    isOver(true);
                }
                else
                {
                    isOver(false);
                }
            }));
        else
        {
            isOver(false);
            Debug.Log("获取本地文件失败");
        }
    }
    
    public IEnumerator GetLocalFile(string filePath,UnityAction<bool> isOver)
    {
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();
        string info = File.ReadAllText(filePath);
        string[] strs = info.Split('|');
        for (int i = 0; i < strs.Length; i++)
        {
            var infos = strs[i].Split(' ');
            
            ABInfo abInfo = new ABInfo(infos[0], infos[1], infos[2]);
            localAbInfo.Add(infos[0],abInfo);
        }

        
        isOver(true);
    }
    
    /// <summary>
    /// 下载对比文件
    /// </summary>
    public IEnumerator DownloadABFile()
    {
        if (updateOver) yield break;
        
        for (int i = 0; i < downLoadList.Count; i++)
        {
            UnityWebRequest req = UnityWebRequest.Get(hotfixT + downLoadList[i]);
            req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("下载发生错误:" + req.error);
                yield break;
            }
            while (!req.isDone)
            {
                //Debug.Log("下载进度:" + req.downloadProgress);
                yield return 0;
            }
            if (req.isDone)
            {
                using (FileStream file = File.Create(Application.persistentDataPath + "/" + downLoadList[i]))
                        file.Write(req.downloadHandler.data,0,req.downloadHandler.data.Length);
                    
                Debug.Log(downLoadList[i] + "下载成功");
            }
            Debug.Log("下载进度:" + (i+1) + @"\" + downLoadList.Count);
        }

        var data = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
        File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt",data);
           
        updateOver = true;
        Debug.Log("全部下载完成!!!");
    }

    

    private class ABInfo
    {
        public string name;
        public string size;
        public string md5;

        public ABInfo(string name,string size,string md5)
        {
            this.name = name;
            this.size = size;
            this.md5 = md5;
        }
    }
}
