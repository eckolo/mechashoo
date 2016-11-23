using UnityEngine;
using System.Collections;

public class Window : Materials
{
    public int timeRequired = 0;

    public override void Start()
    {
        base.Start();
        nowOrder = Order.systemState;
        StartCoroutine(setMotion());
    }
    public IEnumerator setMotion()
    {
        int halfTimeRequired = timeRequired / 2;

        int firstTimeLimit = halfTimeRequired + timeRequired % 2;
        for (int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * easing.circular.In(time, firstTimeLimit - 1)
                + Vector2.up * _size.y * easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1);
        }

        int latterTimeLimit = halfTimeRequired;
        for (int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1);
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
        traceSize = false;
        int halfTimeRequired = timeRequired / 2;

        int firstTimeLimit = halfTimeRequired;
        for (int time = 0; time < firstTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x
                + Vector2.up * _size.y * easing.circular.SubIn(time, firstTimeLimit - 1);
            yield return wait(1);
        }

        int latterTimeLimit = halfTimeRequired + timeRequired % 2;
        for (int time = 0; time < latterTimeLimit; time++)
        {
            transform.localScale
                = Vector2.right * _size.x * easing.circular.SubIn(time, latterTimeLimit - 1)
                + Vector2.up * _size.y * easing.circular.In(time, latterTimeLimit - 1);
            yield return wait(1);
        }

        base.selfDestroy(system);
        yield break;
    }

    public Vector2 position
    {
        get
        {
            return transform.localPosition;
        }
        set
        {
            transform.localPosition = value;
        }
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
            if (traceSize) transform.localScale = value;
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
