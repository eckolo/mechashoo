using UnityEngine;
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
        var positive = getLossyScale(transform).x == 0
            ? 0
            : getLossyScale(transform).x / Mathf.Abs(getLossyScale(transform).x);
        parentConnectionLocal = new Vector2(
            parentConnection.x * getLossyScale(transform.parent).x,
            parentConnection.y * getLossyScale(transform.parent).y * positive
            );
        selfConnectionLocal = new Vector2(
            selfConnection.x * getLossyScale(transform).x,
            selfConnection.y * getLossyScale(transform).y * positive
            );
        var parentConnectionRotation = (Vector2)(transform.parent.transform.rotation * parentConnectionLocal);
        parentConnectionRotation = new Vector2(parentConnectionRotation.x , parentConnectionRotation.y * positive);
        var selfConnectionRotation = (Vector2)(transform.rotation * selfConnectionLocal);
        selfConnectionRotation = new Vector2(selfConnectionRotation.x, selfConnectionRotation.y * positive);
        transform.position = transform.parent.transform.position + (Vector3)(parentConnectionRotation - selfConnectionRotation);
    }

    public Vector2 getLossyScale(Transform origin)
    {
        var next = origin.parent != null ? getLossyScale(origin.parent) : new Vector2(1, 1);
        return new Vector2(origin.localScale.x * next.x, origin.localScale.y * next.y);
    }
}
