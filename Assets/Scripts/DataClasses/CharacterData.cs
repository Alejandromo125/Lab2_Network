using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int HealthPoints;
    public Transform transform;
    public TypesOfActions actions;

    public CharacterData(int hp,Transform tr,TypesOfActions act) 
    {
        this.HealthPoints= hp;
        this.transform = tr;    
        this.actions = act;
    }
}
