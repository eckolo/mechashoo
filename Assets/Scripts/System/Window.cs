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
        StartCoroutine(setMotion());
    }
    public override void Update()
    {
        base.Update();
        if(traceSize) transform.localScale = nowSize;
    }

    public IEnumerator setMotion()
    {
        traceSize = false;
        transform.localScale = Vector2.zero;
        yield return wait(1, isSystem: true);

        int halfTimeRequired = _timeRequired / 2;

        int firstTimeLimit = halfTimeRequired + _timeRequired % 2;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale = Vector2.right * _size.x * Easing.circular.In(time, firstTimeLimit - 1)
                + Vector2.up * _size.y * Easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1, isSystem: system);
        }

        int latterTimeLimit = halfTimeRequired;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale = Vector2.right * _size.x
                + Vector2.up * _size.y * Easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1, isSystem: system);
        }

        traceSize = true;
        yield break;
    }

    public override void selfDestroy(bool system = false)
    {
        StartCoroutine(deleteMotion(system));
    }
    public IEnumerator deleteMotion(bool system)
    {
        yield return wait(1, isSystem: true);
        traceSize = false;
        int halfTimeRequired = _timeRequired / 2;

        int firstTimeLimit = halfTimeRequired;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * Easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1, isSystem: system);
        }

        int latterTimeLimit = halfTimeRequired + _timeRequired % 2;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * Easing.circular.SubIn(time, latterTimeLimit - 1)
                + Vector2.up * _size.y * Easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1, isSystem: system);
        }

        base.selfDestroy(system);
        yield break;
    }

    public Vector2 nowScale { get; set; } = Vector2.one;

    bool traceSize = true;
    Vector2 _size = Vector2.zero;
    public Vector2 nowSize
    {
        get {
            return _size.scaling(nowScale);
        }
        set {
            Debug.Log($"{displayName}\t: {traceSize} => {value}");
            _size = value;
        }
    }
    public Vector2 underLeft
    {
        get {
            return position.scaling(baseMas) - nowSize.scaling(baseMas) / 2;
        }
    }
    public Vector2 upperRight
    {
        get {
            return position.scaling(baseMas) + nowSize.scaling(baseMas) / 2;
        }
    }
}
