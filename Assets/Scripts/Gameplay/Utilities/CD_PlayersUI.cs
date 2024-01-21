using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO MILE & RAMI: CREATE A WAY TO CHECK THE CD OF THE PLAYER'S SHIELD, CHECK PLAYER VARIABLES FOR TIMINGS AND GET THE PLAYER?S SCRIPT
public class CD_PlayersUI : MonoBehaviour
{

    float setShieldBarWidth = 0;
    float setShotgunBarWidth = 0;

    [field: SerializeField]
    private RectTransform shieldBar_CD;

    [field: SerializeField]
    private RectTransform shieldBarComplete_CD;

    [field: SerializeField]
    private RectTransform shotgunBar_CD;

    [field: SerializeField]
    private RectTransform shotgunBarComplete_CD;

    private float _fullWidth_15;
    

    // Start is called before the first frame update
    void Start()
    {
        _fullWidth_15 = shieldBar_CD.rect.width; //it has to be harcoded aparently so all CD bars will have to be 15 or you make another function
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShieldBarCD();
        UpdateShotgutnBarCD();

    }

    public void SetWidth_fw15(float width, RectTransform targetBar)
    {
        //float fullWidth = targetBar.rect.width;

        float newTargetWidth = width * _fullWidth_15 / 15;

        targetBar.sizeDelta = new Vector2(newTargetWidth, targetBar.rect.height);
    }

    public void SetWidth_fw5(float width, RectTransform targetBar)
    {
        //float fullWidth = targetBar.rect.width;

        float newTargetWidth = width * _fullWidth_15 / 5;

        targetBar.sizeDelta = new Vector2(newTargetWidth, targetBar.rect.height);
    }

    void UpdateShieldBarCD()
    {
        setShieldBarWidth = Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime;

        if (setShieldBarWidth > 15)
        {
            setShieldBarWidth = 15;
        }

        SetWidth_fw15(setShieldBarWidth, shieldBar_CD);

        if (Time.time - gameObject.GetComponent<PlayerController>().lastShieldTime > gameObject.GetComponent<PlayerController>().shieldDelay)
        {
            SetWidth_fw15(15, shieldBarComplete_CD);
        }
        else
        {
            SetWidth_fw15(0, shieldBarComplete_CD);
        }
    }

    void UpdateShotgutnBarCD()
    {
        setShotgunBarWidth = Time.time - gameObject.GetComponent<PlayerController>().lastShootTimeShotgun;

        if (setShotgunBarWidth > 5)
        {
            setShotgunBarWidth = 5;
        }

        SetWidth_fw5(setShotgunBarWidth, shotgunBar_CD);

        if (Time.time - gameObject.GetComponent<PlayerController>().lastShootTimeShotgun > gameObject.GetComponent<PlayerController>().shootDelayshotgun)
        {
            SetWidth_fw5(5, shotgunBarComplete_CD);
        }
        else
        {
            SetWidth_fw5(0, shotgunBarComplete_CD);
        }
    }
}
