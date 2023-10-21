using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    
    public TMP_InputField InputFieldMessage;
    
    public TMP_Text textForMessages;

    public static UiManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    public void UpdateText(string text_for_update)
    {
        textForMessages.text += text_for_update + "\n";

    }


}
