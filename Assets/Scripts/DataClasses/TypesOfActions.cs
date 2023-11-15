using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypesOfActions
{
    public bool shoot;
    public bool walk;
    public bool run;
    public bool dash;
    public bool shield;

    public TypesOfActions(bool shoot, bool walk, bool run, bool dash, bool shield)
    {
        this.shoot = shoot;
        this.walk = walk;
        this.run = run;
        this.dash = dash;
        this.shield = shield;
    }
}
