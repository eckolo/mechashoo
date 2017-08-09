using UnityEngine;
using System.Collections;

public class Window : Materials
{
    public int timeRequired { set { _timeRequired = value; } }
    public int _timeRequired = 0;
    public bool system = false;

    public string defaultLayer { get; set; } = Configs.SortLayers.PUBLIC_STATE;
    public override void Start()
    {
        base.Start();
        nowSort = defaultLayer;
        StartCoroutine(SettingMotion());
    }
    public override void Update()
    {
        base.Update();
        if(finishMotion) transform.localScale = nowSize;
    }

    public IEnumerator SettingMotion()
    {
        finishMotion = false;
        transform.localScale = Vector2.zero;
        yield return Wait(1, isSystem: true);

        int halfTimeRequired = _timeRequired / 2;

        int firstTimeLimit = halfTimeRequired + _timeRequired % 2;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale = Vector2.right * _size.x * Easing.circular.In(time, firstTimeLimit - 1)
                + Vector2.up * _size.y * Easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return Wait(1, isSystem: system);
        }

        int latterTimeLimit = halfTimeRequired;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale = Vector2.right * _size.x
                + Vector2.up * _size.y * Easing.circular.In(time, latterTimeLimit - 1);
            yield return Wait(1, isSystem: system);
        }

        finishMotion = true;
        yield break;
    }

    public override void DestroyMyself(bool system = false)
    {
        StartCoroutine(DeleteMotion(system));
    }
    public IEnumerator DeleteMotion(bool system)
    {
        yield return Wait(1, isSystem: true);
        finishMotion = false;
        int halfTimeRequired = _timeRequired / 2;

        int firstTimeLimit = halfTimeRequired;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * Easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return Wait(1, isSystem: system);
        }

        int latterTimeLimit = halfTimeRequired + _timeRequired % 2;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * Easing.circular.SubIn(time, latterTimeLimit - 1)
                + Vector2.up * _size.y * Easing.circular.In(time, latterTimeLimit - 1);
            yield return Wait(1, isSystem: system);
        }

        base.DestroyMyself(system);
        yield break;
    }

    public Vector2 nowScaleTweak { get; set; } = Vector2.one;

    public bool finishMotion { get; private set; } = true;
    Vector2 _size = Vector2.zero;
    public Vector2 nowSize
    {
        get {
            return _size.Scaling(nowScaleTweak);
        }
        set {
            _size = value;
        }
    }
    public Vector2 underLeft
    {
        get {
            return position.Scaling(baseMas) - nowSize.Scaling(baseMas) / 2;
        }
    }
    public Vector2 upperRight
    {
        get {
            return position.Scaling(baseMas) + nowSize.Scaling(baseMas) / 2;
        }
    }
}
