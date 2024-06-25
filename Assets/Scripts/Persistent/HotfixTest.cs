using System.IO;
using UnityEngine;
using XLua;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class HotfixTest : MonoBehaviour
{
    private const string hotfixT = "http://1.14.18.29/UnityHotfix/";
    private LuaEnv luaenv;
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI sizeT;
    void Start()
    {
        // StartCoroutine(DownloadAndLoadResourceHotfix("ttt"));
        // StartCoroutine(DownloadResourceHotfix("tttDispose"));
    }
    
    private byte[] MyLoader(ref string filename)
    {
        if (File.Exists(Application.persistentDataPath + "/" + filename + ".lua"))
            return File.ReadAllBytes(Application.persistentDataPath + "/" + filename + ".lua");
        if (File.Exists(Application.streamingAssetsPath + "/" + filename + ".lua")) 
            return File.ReadAllBytes(Application.streamingAssetsPath + "/" + filename + ".lua");
        Debug.Log("找不到"+filename+".lua文件");
        return null;
    }
    
    private IEnumerator DownloadResource(string filename,string suffix)
    {
        UnityWebRequest request = UnityWebRequest.Get(hotfixT + filename + suffix);
        request.SendWebRequest();
        while (!request.isDone)
        {
            var size = request.uploadProgress;
            sizeT.text = size.ToString(CultureInfo.InvariantCulture);
            yield return 0;
        }
        string str = request.downloadHandler.text;
        string path = Application.streamingAssetsPath + "/" + filename + suffix;
        
    }
    
    private IEnumerator DownloadResourceHotfix(string filename)
    {
        UnityWebRequest request = UnityWebRequest.Get(hotfixT + filename + ".lua.txt");
        yield return request.SendWebRequest();
        prompt.text = "download context" + request.downloadHandler.text;
        string str = request.downloadHandler.text;
        string path = Application.persistentDataPath + "/" + filename + ".lua";
        File.WriteAllText(path,str);
    }
    
    private IEnumerator DownloadAndLoadResourceHotfix(string filename)
    {
        luaenv = new LuaEnv();
        luaenv.AddLoader(MyLoader);
        UnityWebRequest request = UnityWebRequest.Get(hotfixT + filename + ".lua.txt");
        yield return request.SendWebRequest();
        string str = request.downloadHandler.text;
        string path = Application.persistentDataPath + "/" + filename + ".lua";
        File.WriteAllText(path,str);
        luaenv.DoString("require('"+filename+"')");
    }

    public void CheckHotfix(string filename)
    {
        xLuaEnv.Instance.Run("require('ttt')");
        //StartCoroutine(DownloadAndLoadResourceHotfix("ttt"));
    }
    
    private void OnDisable()
    {
        //luaenv.DoString("require('tttDispose')");
    }
    private void OnDestroy()
    {
        //luaenv.Dispose();
    }
}
