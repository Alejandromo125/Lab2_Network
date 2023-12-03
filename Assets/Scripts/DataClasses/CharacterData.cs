using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CharacterData
{
    public int HealthPoints = 100;
    public int GameScore = 0;
    public Team team = Team.NONE;
    public Vector3 position = Vector3.zero;
    public Quaternion rotation = Quaternion.identity;
    public TypesOfActions actions;
}


[Serializable]
public enum Team
{
    NONE,
    RED_TEAM,
    BLUE_TEAM
}
