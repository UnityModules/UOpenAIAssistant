using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;
    public static UnityMainThreadDispatcher Instance =>
    _instance ?? (_instance = 
    new GameObject(nameof(UnityMainThreadDispatcher)).AddComponent<UnityMainThreadDispatcher>());
    private Queue<Action> _actionQueue = new();

    private void Awake() =>
        DontDestroyOnLoad(_instance.gameObject);

    private void Update()
    {
        lock (_actionQueue)
            while (_actionQueue.Count > 0)
                _actionQueue.Dequeue().Invoke();
    }

    public void Enqueue(Action action)
    {
        lock (_actionQueue)
            _actionQueue.Enqueue(action);
    }
}