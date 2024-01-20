using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO MILE & RAMI: CREATE A WAY TO CHECK THE CD OF THE PLAYER'S SHIELD, CHECK PLAYER VARIABLES FOR TIMINGS AND GET THE PLAYER?S SCRIPT
public class CD_PlayersUI : MonoBehaviour
{

    float setBarWidth = 0;


    [field: SerializeField]
    public int MaxValue { get; private set; }

    [field: SerializeField]
    public int Value { get; private set; }

    [field: SerializeField]
    private RectTransform shieldBar_CD;

    [field: SerializeField]
    private RectTransform shieldBarComplete_CD;

    private float _fullWidth_15;
    

    // Start is called before the first frame update
    void Start()
    {
        _fullWidth_15 = shieldBar_CD.rect.width; //it has to be harcoded aparently so all CD bars will have to be 15 or you make another function
    }

    // Update is called once per frame
    void Update()
    {
        setBarWidth = Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime;

        if(setBarWidth > 15)
        {
            setBarWidth = 15;
        }

        SetWidth_fw15(setBarWidth, shieldBar_CD);

        Debug.Log("Shield bar width:" + setBarWidth);

        if (Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime > gameObject.GetComponent<PlayerController>().shieldDelay)
        {
            SetWidth_fw15(15, shieldBarComplete_CD);
        }
        else
        {
            SetWidth_fw15(0, shieldBarComplete_CD);
        }
    }

    public void SetWidth_fw15(float width, RectTransform targetBar)
    {
        //float fullWidth = targetBar.rect.width;

        float newTargetWidth = width * _fullWidth_15 / MaxValue;

        targetBar.sizeDelta = new Vector2(newTargetWidth, targetBar.rect.height);
    }
}
