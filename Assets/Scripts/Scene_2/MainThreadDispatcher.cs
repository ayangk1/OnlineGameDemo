using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
 
 
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;
 
 
    private Queue<Action> actionQueue = new ();
 
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
 
 
    private void Update()
    {
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                Action action = actionQueue.Dequeue();
                action.Invoke();
            }
        }
    }
 
 
    public static void RunOnMainThread(Action action)
    {
        lock (instance.actionQueue)
        {
            instance.actionQueue.Enqueue(action);
        }
    }
}