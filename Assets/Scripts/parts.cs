﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parts : MonoBehaviour
{
    public Vector2 parentConnection = new Vector2(1, 0);
    public Vector2 selfConnection = new Vector2(-1, 0);
    public Vector2 parentConnectionLocal = new Vector2(1, 0);
    public Vector2 selfConnectionLocal = new Vector2(-1, 0);

    public Parts childParts = null;

    // Update is called once per frame
    void Update()
    {
        parentConnectionLocal = new Vector2(
            parentConnection.x * getLssyScale(transform.parent).x,
            parentConnection.y * getLssyScale(transform.parent).y
            );
        selfConnectionLocal = new Vector2(
            selfConnection.x * getLssyScale(transform).x,
            selfConnection.y * getLssyScale(transform).y
            );
        transform.position = transform.parent.transform.position + transform.parent.transform.rotation * parentConnectionLocal - transform.rotation * selfConnectionLocal;
    }

    public Vector2 getLssyScale(Transform origin)
    {
        var next = origin.parent != null ? getLssyScale(origin.parent) : new Vector2(1, 1);
        return new Vector2(origin.localScale.x * next.x, origin.localScale.y * next.y);
    }
}
