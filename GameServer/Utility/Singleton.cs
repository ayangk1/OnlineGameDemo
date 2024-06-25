using System;
using System.Collections;
using System.Collections.Generic;


public class Singleton<T> where T : class,new()
{
    private static T? instance;
    private static readonly object syncRoot = new object();

    public static T Instance
    {
        get 
        { 
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new T();
                }
            }
            return instance; 
        }
    }
}