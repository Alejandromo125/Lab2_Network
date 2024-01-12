using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextHP_Set : MonoBehaviour
{
    // Start is called before the first frame update

    public int playerHP = 100;

    public TMP_Text text;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = playerHP.ToString();
    }
}
