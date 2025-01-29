using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BspNode : MonoBehaviour
{
    public BspNode leftNode;
    public BspNode rightNode;
    public BspNode parNode;
    public RectInt nodeRect; //분리된 공간의 rect정보
    public BspNode(RectInt rect)
    {
        this.nodeRect = rect;
    }
}
