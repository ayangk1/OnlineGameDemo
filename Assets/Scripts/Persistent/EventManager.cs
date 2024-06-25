using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonMonoBehaviour<EventManager>
{
    public Queue<Action> eventQueue = new Queue<Action>();
}
