using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parts : MonoBehaviour
{
    public Parts childParts = null;

    public Vector2 parentConnection = new Vector2(1, 0);
    public Vector2 selfConnection = new Vector2(-1, 0);

    // Update is called once per frame
    void Update()
    {
        setPosition();
    }

    private void setPosition()
    {
        var parentConnectionRotation = (Vector2)(transform.parent.transform.rotation * getParentConnection());
        parentConnectionRotation = new Vector2(parentConnectionRotation.x, parentConnectionRotation.y * getPositive());
        var selfConnectionRotation = (Vector2)(transform.rotation * getSelfConnection());
        selfConnectionRotation = new Vector2(selfConnectionRotation.x, selfConnectionRotation.y * getPositive());
        transform.position = transform.parent.transform.position + (Vector3)(parentConnectionRotation - selfConnectionRotation);
    }

    private float getPositive()
    {
        return getLossyScale(transform).x == 0
            ? 0
            : getLossyScale(transform).x / Mathf.Abs(getLossyScale(transform).x);
    }
    public Vector2 getParentConnection()
    {
        return new Vector2(
            parentConnection.x * getLossyScale(transform.parent).x,
            parentConnection.y * getLossyScale(transform.parent).y * getPositive()
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
