using UnityEngine;
using System.Collections;

public class Window : Materials
{
    public override void Start()
    {
        base.Start();
        nowOrder = Order.systemState;
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
    public Vector2 size
    {
        get
        {
            return transform.localScale;
        }
        set
        {
            transform.localScale = value;
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
