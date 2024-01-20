using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO MILE & RAMI: CREATE A WAY TO CHECK THE CD OF THE PLAYER'S SHIELD, CHECK PLAYER VARIABLES FOR TIMINGS AND GET THE PLAYER?S SCRIPT
public class CD_PlayersUI : MonoBehaviour
{

    float shieldBarWidth_CD = 0;


    [field: SerializeField]
    public int MaxValue { get; private set; }

    [field: SerializeField]
    public int Value { get; private set; }

    [field: SerializeField]
    private RectTransform shieldBar_CD;

    [field: SerializeField]
    private RectTransform shieldBarComplete_CD;

    private float _fullWidth;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        shieldBarWidth_CD = Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime;

        if(shieldBarWidth_CD > 15)
        {
            shieldBarWidth_CD = 15;
        }

        SetWidth_v2(shieldBarWidth_CD, shieldBar_CD);

        if (Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime > gameObject.GetComponent<PlayerController>().shieldDelay)
        {
            SetWidth_v2(shieldBarWidth_CD, shieldBarComplete_CD);
        }
        else
        {
            SetWidth_v2(0, shieldBarComplete_CD);
        }
    }

    public void SetWidth_v2(float width, RectTransform targetBar)
    {
        _fullWidth = targetBar.rect.width;

        float newTargetWidth = width * _fullWidth / MaxValue;

        targetBar.sizeDelta = new Vector2(newTargetWidth, targetBar.rect.height);
    }
}
