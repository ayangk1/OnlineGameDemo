using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Editor;

public class CustomEditor : MonoBehaviour
{
    static void GetMD5()
    {
        StringBuilder sb = new StringBuilder();
        using (FileStream file = new FileStream(Path.Combine(Application.streamingAssetsPath,"version"),FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Info = md5.ComputeHash(file);
            file.Close();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
        } 
        Debug.Log(sb.ToString());
    }

    static void GetWindow()
    {
        CustomEditorWindow win = EditorWindow.GetWindow<CustomEditorWindow>();
        win.Show();
    }
}
