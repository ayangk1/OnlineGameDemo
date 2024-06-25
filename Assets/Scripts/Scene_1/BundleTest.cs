using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;
using XLua;

[Hotfix]
public class BundleTest : SingletonMonoBehaviour<BundleTest>
{
    private List<AssetBundle> loadedAB = new();
    public string abName = "";
    public string bundleName = "";
    public string assetName = "";

    private void Start()
    {
        Test();
    }

    [LuaCallCSharp]
    public void Test()
    {
        bundleName = "model";
        assetName = "bundle";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
            LoadDependencies();
            InstantiateBundle();
        }

        
    }

    public AssetBundle LoadAB(string filename)
    {
        AssetBundle asset = null;
        if (File.Exists(Path.Combine(Application.persistentDataPath, filename)))
            loadedAB.Add(asset = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, filename)));
        else if (File.Exists(Path.Combine(Application.streamingAssetsPath, filename)))
            loadedAB.Add(asset = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, filename)));
        else
            Debug.Log("找不到源文件");
        return asset;
    }

    private void LoadDependencies()
    {
        AssetBundle assetBundle;
        AssetBundleManifest _manifest;
        if (File.Exists(Path.Combine(Application.persistentDataPath,abName)))
        {
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath,abName));
            _manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] dependencies = _manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
                LoadAB(dependency);
            assetBundle.Unload(false);
        }
        else if (File.Exists(Path.Combine(Application.streamingAssetsPath,abName)))
        {
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath,abName));
            _manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] dependencies = _manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
                LoadAB(dependency);
            assetBundle.Unload(false);
        }
        else
            Debug.Log("找不到源文件");
    }
    
    private void InstantiateBundle()
    {
        AssetBundle asset = LoadAB(bundleName);
         // GetComponent<MeshRenderer>().materials = UnityEngine.Color.blue;
        var obj = asset.LoadAsset<GameObject>(assetName);
        Instantiate(obj,Vector3.zero,Quaternion.identity);
        asset.Unload(false);
        var s = AssetBundle.GetAllLoadedAssetBundles();
        foreach (var fe in s)
            if(fe.name == bundleName + "depend")
                fe.Unload(false);
    }

    private void ABundle()
    {
        
    }
}
