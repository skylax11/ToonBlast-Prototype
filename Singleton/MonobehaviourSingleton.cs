using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonobehaviourSingleton<T> : MonoBehaviour where T : class
{
    public static T Instance;

    private void Awake()
    {
        Instance = this as T;
    }
}
