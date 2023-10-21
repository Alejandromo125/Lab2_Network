using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageEventHandler : MonoBehaviour
{
    public event Action OnButtonClicked;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void TriggerButtonClickedEvent()
    {
        OnButtonClicked?.Invoke();
    }
}
