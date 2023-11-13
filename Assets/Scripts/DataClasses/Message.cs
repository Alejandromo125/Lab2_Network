using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message 
{
    public string message;
    public CharacterData characterData = null;
    public TypesOfMessage type;

    public Message(string message,CharacterData charactherData,TypesOfMessage type) 
    {
        this.message = message;
        this.characterData = charactherData;
        this.type = type;
    }
}
