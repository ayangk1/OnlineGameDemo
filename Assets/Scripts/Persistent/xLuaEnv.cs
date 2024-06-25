using System.Collections;
using System.IO;
using UnityEngine;
using XLua;
public class xLuaEnv
{
    private LuaEnv _Env;
    
    private xLuaEnv()
    {
        _Env = new LuaEnv();
        _Env.AddLoader(MyLoader);
    }

    private static xLuaEnv _Instance = null;

    public static xLuaEnv Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new xLuaEnv();
            }

            return _Instance;
        }
    }
    
    private byte[] MyLoader(ref string filename)
    {
        string persistentPath = Application.persistentDataPath + "/" + filename + ".lua";
        string streamingPath = Application.streamingAssetsPath + "/" + filename + ".lua";
        if (File.Exists(persistentPath))
            return File.ReadAllBytes(persistentPath);
        if (File.Exists(streamingPath))
            return File.ReadAllBytes(streamingPath);
        
        Debug.Log("找不到Lua源文件");
        return null;
    }

    public void Free()
    {
        _Env.Dispose();
        _Instance = null;
    }

    public object[] Run(string code)
    {
        return _Env.DoString($"require'{code}'");
    }

    public LuaTable Global => _Env.Global;
}
