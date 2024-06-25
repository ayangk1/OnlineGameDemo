using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using XLua;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

[Hotfix]
public class ADTest : MonoBehaviour
{
    public GameObject goPrefab;

    public AssetReference refPrefab;
    
    public AssetReference spriteAltas;

    public Image left;
    public Image right;
    public TextMeshProUGUI prompt;
    private LuaEnv luaenv;

    private void Start()
    {
        luaenv = new LuaEnv();
        luaenv.AddLoader(MyLoader);
    }
    
    private byte[] MyLoader(ref string filePath)
    {
        string absPath = Application.dataPath + "/" + filePath + ".lua";
        return File.ReadAllBytes(absPath);
    }

    [LuaCallCSharp]
    public void OnClickEvent()
    {
        prompt.text = "before Hotfix";
    }
    

    private void LoadSprite(AsyncOperationHandle<SpriteAtlas> handle)
    {
        
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(handle.Result);                
                //获取所有图
                // Sprite[] sps = new Sprite[handle.Result.spriteCount];
                // int length = handle.Result.GetSprites(sps);
                // for (int i = 0; i < length; i++)
                // {
                //     Debug.LogError($"获取所有Sprite = {sps[i]}");
                // }                
                //获取单张Sprite
                var sp1 = handle.Result.GetSprite("tp-1");
                left.sprite = sp1;
            }
    }

    IEnumerator LoadResourceHotfix(string filename)
    {
        UnityWebRequest request = UnityWebRequest.Get("http://www.ayangk.online/UnityHotfix/mmm.txt");
        yield return request.SendWebRequest();
        luaenv.DoString("require('ttt')");
        Debug.Log(request.downloadHandler.text);
    }
    private void OnDisable()
    {
        xLuaEnv.Instance.Run("require('tttDispose')");
        xLuaEnv.Instance.Free();
    }
    
}
