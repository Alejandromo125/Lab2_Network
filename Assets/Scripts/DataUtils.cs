using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataUtils 
{
   public string informationToSend;
   public TypeOfMessage typeOfMessage;
}


public enum TypeOfMessage
{
    None,
    GameObjectInformation,
    SceneChange,
    MessageSend
}
