using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new();

    public static void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (_executionQueue.Count > 0)
        {
            var action = _executionQueue.Dequeue();
            action?.Invoke();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (FindObjectOfType<UnityMainThreadDispatcher>() == null)
        {
            var go = new GameObject("UnityMainThreadDispatcher");
            DontDestroyOnLoad(go);
            go.AddComponent<UnityMainThreadDispatcher>();
        }
    }
}
