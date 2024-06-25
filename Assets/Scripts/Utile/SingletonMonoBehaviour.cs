using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        instance = this as T;
    }

}