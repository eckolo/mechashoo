using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parts : MonoBehaviour
{
    //接続先のParts
    public Parts childParts = null;
    //接続関連の座標
    public Vector2 parentConnection = new Vector2(0, 0);
    public Vector2 selfConnection = new Vector2(0, 0);
    //親Partsの角度をトレースするか否かフラグ
    public bool traceRoot = false;
    //制御元のRoot
    public Root parentRoot = null;

    // Update is called once per frame
    public virtual void Start()
    {
        setPosition();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        setPosition();
    }

    private void setPosition()
    {
        var parent = transform.parent != null ? transform.parent : transform;
        var parentConnectionRotation = (Vector2)(parent.transform.rotation * getParentConnection());
        parentConnectionRotation = new Vector2(parentConnectionRotation.x, parentConnectionRotation.y * getPositive());
        var selfConnectionRotation = (Vector2)(transform.rotation * getSelfConnection());
        selfConnectionRotation = new Vector2(selfConnectionRotation.x, selfConnectionRotation.y * getPositive());
        transform.position = parent.transform.position + (Vector3)(parentConnectionRotation - selfConnectionRotation);
    }

    private float getPositive()
    {
        return getLossyScale(transform).x == 0
            ? 0
            : getLossyScale(transform).x / Mathf.Abs(getLossyScale(transform).x);
    }
    public Vector2 getParentConnection()
    {
        var parent = transform.parent != null ? transform.parent : transform;
        return new Vector2(
            parentConnection.x * getLossyScale(parent).x,
            parentConnection.y * getLossyScale(parent).y * getPositive()
            );
    }
    public Vector2 getSelfConnection()
    {
        return new Vector2(
            selfConnection.x * getLossyScale(transform).x,
            selfConnection.y * getLossyScale(transform).y * getPositive()
            );
    }
    public Vector2 getLossyScale(Transform origin)
    {
        if (origin == null) return getLossyScale(transform);
        var next = origin.parent != null ? getLossyScale(origin.parent) : new Vector2(1, 1);
        return new Vector2(origin.localScale.x * next.x, origin.localScale.y * next.y);
    }
}
