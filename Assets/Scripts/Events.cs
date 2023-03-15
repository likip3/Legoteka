using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Events : MonoBehaviour
{
    public static event Action IsNull;
    public static event Action IsNotNull;

    public static void InvokeIfNull()
    {
        IsNull?.Invoke();
    }

    public static void InvokeIfNotNull()
    {
        IsNotNull?.Invoke();
        
    }
}
