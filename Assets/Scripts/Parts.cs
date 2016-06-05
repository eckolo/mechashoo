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
    public Object parentRoot = null;
    //先端位置補正
    public Vector2 correctionVector = new Vector2(0, 0);
    //関節の最小折り畳み角度を定義するパラメータ
    public float lowerLimitRange = 0;

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
    public virtual Vector2 getCorrection()
    {
        Vector2 baseVector = transform.rotation * correctionVector;
        if (childParts == null) return baseVector;
        return baseVector + childParts.getCorrection();
    }

    public Vector2 setManipulatePosition(Vector2 targetVector, bool positive = true)
    {
        var baseAngle = toAngle(targetVector + getCorrection());
        if (childParts == null)
        {
            transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(baseAngle));
            return targetVector;
        }
        var rootLange = (childParts.getParentConnection() - getSelfConnection()).magnitude;
        var partsLange = Mathf.Abs(childParts.getSelfConnection().x)
            + (childParts.GetComponent<Weapon>() != null
            ? childParts.GetComponent<Weapon>().injectionHole[0].x
            : Mathf.Abs(childParts.getSelfConnection().x));
        var rootLimit = rootLange + partsLange;
        var parentScale = parentRoot.transform.lossyScale.magnitude;

        var targetPosition = targetVector.normalized * Mathf.Clamp(targetVector.magnitude * parentScale, lowerLimitRange * parentScale + Mathf.Abs(partsLange - rootLange), rootLimit);

        setAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition / parentScale;
    }
    public Vector2 setManipulateEim(Vector2 targetPosition, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition + getCorrection());
        var rootLange = (childParts.getParentConnection() - getSelfConnection()).magnitude;
        var partsLange = (targetPosition + getCorrection()).magnitude + (rootLange * (Mathf.Abs(baseAngle) - 90) / 90);

        setAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition;
    }
    private void setAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition + getCorrection());
        var targetLange = (targetPosition + getCorrection()).magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = compileMinusAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileMinusAngle(jointAngle * (positive ? 1 : -1));

        transform.localEulerAngles = new Vector3(0, 0, parentAngle);
        setChildAngle(childAngle, childParts);
    }
    private void setChildAngle(float targetAngle, Parts targetChild)
    {
        if (targetChild.traceRoot) targetChild.transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(targetAngle));
        if (targetChild.childParts != null) setChildAngle(targetAngle * (-1), targetChild.childParts);
    }

    private static float compileMinusAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }
    private static float getDegree(float A, float B, float C)
    {
        return Mathf.Acos(Mathf.Clamp((Mathf.Pow(C, 2) + Mathf.Pow(A, 2) - Mathf.Pow(B, 2)) / (2 * A * C), -1, 1)) * Mathf.Rad2Deg;
    }
    private static float toAngle(Vector2 targetVector)
    {
        return Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
    }
    public void setParent(Object setedParent)
    {
        parentRoot = setedParent;
        if (childParts != null) childParts.setParent(setedParent);
    }
}
