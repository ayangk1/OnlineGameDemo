using System;
using UnityEngine;
using XLua;

[CSharpCallLua]
public delegate void LifeCycle();

[GCOptimize]
public struct LuaCallLifeCycle
{
    public LifeCycle Start;
    public LifeCycle Update;
    public LifeCycle OnDestroy;

    public void Dispose()
    {
        Start = null;
        Update = null;
        OnDestroy = null;
    }
} 
public class LuaUpdate : MonoBehaviour
{
    public LuaCallLifeCycle lifeCycle;
    void Start()
    {
        xLuaEnv.Instance.Run("main");
        
        lifeCycle = xLuaEnv.Instance.Global.Get<LuaCallLifeCycle>("LifeCycleTable");
        lifeCycle.Start();
    }

    void Update()
    {
        lifeCycle.Update();
        
    }

    private void OnDisable()
    {
        lifeCycle.OnDestroy();
        lifeCycle.Dispose();
    }

    void OnDestroy()
    { 
        
        xLuaEnv.Instance.Free();
    }

}
