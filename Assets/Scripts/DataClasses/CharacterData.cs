using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int HealthPoints;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public TypesOfActions actions;
}
