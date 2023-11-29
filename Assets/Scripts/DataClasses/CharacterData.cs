using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CharacterData
{
    public int HealthPoints = 100;
    public int GamePoints = 0;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public TypesOfActions actions;
}
