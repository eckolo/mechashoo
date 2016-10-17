using UnityEngine;
using System.Collections;

public class Window : Materials
{
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
            return position
                   - Vector2.right * size.x * baseMas.x
                   - Vector2.up * size.y * baseMas.y;
        }
    }
    public Vector2 upperRight
    {
        get
        {
            return position
                   + Vector2.right * size.x * baseMas.x
                   + Vector2.up * size.y * baseMas.y;
        }
    }
}
