using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticEventHandler
{
    public static event Action<EXArgs> OnMapGenerating;

    public static void CallMapGeneratingEvent(int wayCount)
    {
        OnMapGenerating?.Invoke(new EXArgs() { wayCount = wayCount });
    }

}

public class EXArgs : EventArgs
{
    public int wayCount;
}
