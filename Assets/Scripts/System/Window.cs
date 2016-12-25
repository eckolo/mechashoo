using UnityEngine;
using System.Collections;

public class Window : Materials
{
    public int timeRequired = 0;
    public bool system = false;

    int defaultOrder = Order.SYSTEM_STATE;
    public override int nowOrder
    {
        get
        {
            return base.nowOrder;
        }

        set
        {
            defaultOrder = value;
            base.nowOrder = value;
        }
    }
    public override void Start()
    {
        base.Start();
        nowOrder = defaultOrder;
        StartCoroutine(setMotion());
    }
    public IEnumerator setMotion()
    {
        transform.localScale = Vector2.zero;
        yield return wait(1, system: true);

        int halfTimeRequired = timeRequired / 2;

        int firstTimeLimit = halfTimeRequired + timeRequired % 2;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * easing.circular.In(time, firstTimeLimit - 1)
                + Vector2.up * _size.y * easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1, system: system);
        }

        int latterTimeLimit = halfTimeRequired;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1, system: system);
        }

        transform.localScale = _size;
        traceSize = true;
        yield break;
    }

    public override void selfDestroy(bool system = false)
    {
        StartCoroutine(deleteMotion(system));
    }
    public IEnumerator deleteMotion(bool system)
    {
        yield return wait(1, system: true);
        traceSize = false;
        int halfTimeRequired = timeRequired / 2;

        int firstTimeLimit = halfTimeRequired;
        for(int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1, system: system);
        }

        int latterTimeLimit = halfTimeRequired + timeRequired % 2;
        for(int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * easing.circular.SubIn(time, latterTimeLimit - 1)
                + Vector2.up * _size.y * easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1, system: system);
        }

        base.selfDestroy(system);
        yield break;
    }

    bool traceSize = false;
    Vector2 _size = Vector2.zero;
    public Vector2 size
    {
        get
        {
            return traceSize ? (Vector2)transform.localScale : _size;
        }
        set
        {
            if(traceSize) transform.localScale = value;
            _size = value;
        }
    }
    public Vector2 underLeft
    {
        get
        {
            return MathV.scaling(position, baseMas)
                   - Vector2.right * size.x / 2 * baseMas.x
                   - Vector2.up * size.y / 2 * baseMas.y;
        }
    }
    public Vector2 upperRight
    {
        get
        {
            return MathV.scaling(position, baseMas)
                   + Vector2.right * size.x / 2 * baseMas.x
                   + Vector2.up * size.y / 2 * baseMas.y;
        }
    }
}
