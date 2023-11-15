using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerStruct
{
    //player movement
    public Vector3 position;
    public Quaternion rotation;

    //actions
    public bool shooting;
    public bool isMoving;
}
