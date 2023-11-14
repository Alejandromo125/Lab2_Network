using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerStruct
{
    //player movement
    public Transform playerTransform;
    public Transform gunTransform; //<-- maybe we don't need this one

    //actions
    public bool shooting;
    public bool isMoving;
}
