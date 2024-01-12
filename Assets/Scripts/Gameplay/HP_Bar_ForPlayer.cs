using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HP_Bar_ForPlayer : MonoBehaviour
{
    [field:SerializeField]
    public int MaxValue { get; private set; }

    [field:SerializeField]
    public int Value {  get; private set; }

    [field: SerializeField]
    private RectTransform _topBar;

    [field: SerializeField]
    private RectTransform _bottomBar;

    private float _fullWidth;
    private float TargetWidth => Value * _fullWidth / MaxValue;

    [SerializeField]
    private float _animationSpeed = 10f;

    private Coroutine _adjustBarWidthCoroutine;

    [SerializeField]
    private Transform canvasTransform;

    private CinemachineVirtualCamera camForCanvas;

    int hpDifference;

    BulletHitManager bulletHitManager_;

    public void Change(int amount)
    {
        Value = Mathf.Clamp(Value + amount, 0, MaxValue);

        if(_adjustBarWidthCoroutine != null)
        {
            StopCoroutine(_adjustBarWidthCoroutine);
        }

        _adjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(amount));
    }
    public void Set(int amount)
    {
        Value = amount;

        if (_adjustBarWidthCoroutine != null)
        {
            StopCoroutine(_adjustBarWidthCoroutine);
        }

        _adjustBarWidthCoroutine = StartCoroutine(AdjustBarWidth(amount));
    }
    private void Awake()
    {
        camForCanvas = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        _fullWidth = _topBar.rect.width;
        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        hpDifference = bulletHitManager_.entityLife;

        Change(100);
    }

    void Update()
    {
        //if (hpDifference != bulletHitManager_.entityLife)
        //{
        //    int calculateDamage = hpDifference - bulletHitManager_.entityLife;
        //    if (calculateDamage < 0)
        //    {
        //        calculateDamage = calculateDamage * -1;
        //    }

        //    //Change(-calculateDamage);
        //    Change(-10); //Harcoded because we only have a 10 point damage option y porque estoy hasta la polla de debuguear resultados in�tiles

        //    hpDifference = bulletHitManager_.entityLife;
        //}

        //if (bulletHitManager_.entityLife <= 0)
        //{
        //    bulletHitManager_.entityLife = 100;
        //    hpDifference = bulletHitManager_.entityLife;
        //    Change(100);
        //    Change(-1);
        //}
    }

    private void LateUpdate()
    {
        canvasTransform.LookAt(transform.position + camForCanvas.transform.forward);
    }

    private IEnumerator AdjustBarWidth(int amount)
    {
        var suddenChangeBar = amount >= 0 ? _bottomBar : _topBar;
        var slowChangeBar = amount >= 0 ? _bottomBar : _topBar;

        suddenChangeBar.SetWidth(TargetWidth);

        while (Mathf.Abs(suddenChangeBar.rect.width - slowChangeBar.rect.width) > 1f)
        {
            slowChangeBar.SetWidth(Mathf.Lerp(slowChangeBar.rect.width, TargetWidth, Time.deltaTime * _animationSpeed));

            yield return null;
        }

        slowChangeBar.SetWidth(TargetWidth);
    }

    
}
