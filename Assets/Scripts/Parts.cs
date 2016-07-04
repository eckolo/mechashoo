using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 関節挙動とかするパーツ部位
/// </summary>
public class Parts : Roots
{
    /// <summary>
    ///接続先のParts
    /// </summary>
    public Parts childParts = null;
    /// <summary>
    ///接続関連の座標
    /// </summary>
    public Vector2 parentConnection = new Vector2(0, 0);
    public Vector2 selfConnection = new Vector2(0, 0);
    /// <summary>
    ///関節挙動のターゲット座標
    /// </summary>
    public Vector2 basePosition = Vector2.right;
    /// <summary>
    ///親Partsの角度をトレースするか否かフラグ
    /// </summary>
    public bool traceRoot = false;
    /// <summary>
    ///制御元のMaterial
    /// </summary>
    public Material parentMaterial = null;
    /// <summary>
    ///先端位置補正
    /// </summary>
    public Vector2 correctionVector = new Vector2(0, 0);
    /// <summary>
    ///関節の最小折り畳み角度を定義するパラメータ
    /// </summary>
    public float lowerLimitRange = 0;
    /// <summary>
    ///縦方向の非反転フラグ
    /// </summary>
    [SerializeField]
    public bool heightPositive = true;

    // Update is called once per frame
    protected override void baseStart()
    {
        setPosition();
    }

    // Update is called once per frame
    protected override void baseUpdate()
    {
        setPosition();
        transform.localScale = new Vector2(transform.localScale.x, Mathf.Abs(transform.localScale.y) * (heightPositive ? 1 : -1));
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
    public virtual Vector2 getCorrection()
    {
        if (childParts != null)
        {
            Quaternion baseRotation = childParts.traceRoot
                ? Quaternion.FromToRotation(Vector2.right, getBasePosition())
                : new Quaternion(0, 0, 0, 0);
            return correctWidthVector(
                baseRotation * correctionVector * getPositive()
                + baseRotation * childParts.getCorrection()
                );
        }
        else
        {
            return correctWidthVector(correctionVector * getPositive());
        }
    }
    public Vector2 getBasePosition()
    {
        return correctWidthVector(basePosition);
    }
    protected Vector2 correctWidthVector(Vector2 inputVector)
    {
        return new Vector2(inputVector.x * getPositive(), inputVector.y);
    }

    public Vector2 setManipulatePosition(Vector2 targetVector, bool positive = true)
    {
        var baseAngle = toAngle(targetVector + getCorrection());
        if (childParts == null)
        {
            basePosition = targetVector;
            transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(baseAngle));
            return targetVector;
        }
        var rootLange = (childParts.getParentConnection() - getSelfConnection()).magnitude;
        var partsLange = Mathf.Abs(childParts.getSelfConnection().x)
            + (childParts.GetComponent<Weapon>() != null
            ? childParts.GetComponent<Weapon>().injectionHoles[0].x
            : childParts.GetComponent<Hand>() != null
            ? childParts.GetComponent<Hand>().takePosition.x
            : Mathf.Abs(childParts.getSelfConnection().x));
        var rootLimit = rootLange + partsLange;
        var parentScale = parentMaterial.transform.lossyScale.magnitude;

        var targetPosition = targetVector.normalized * Mathf.Clamp(targetVector.magnitude * parentScale, lowerLimitRange * parentScale + Mathf.Abs(partsLange - rootLange), rootLimit);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition / parentScale;
    }
    public Vector2 setManipulateEim(Vector2 targetPosition, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition + getCorrection());
        var rootLange = (childParts.getParentConnection() - getSelfConnection()).magnitude;
        var partsLange = (targetPosition + getCorrection()).magnitude + (rootLange * (Mathf.Abs(baseAngle) - 90) / 90);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition;
    }
    private void setLangeToAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true)
    {
        basePosition = targetPosition;

        var baseAngle = toAngle(targetPosition + getCorrection());
        var targetLange = (targetPosition + getCorrection()).magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = compileMinusAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileMinusAngle(jointAngle * (positive ? 1 : -1));

        setAngle(parentAngle);
        setChildAngle(childAngle, childParts);
    }
    private void setChildAngle(float targetAngle, Parts targetChild)
    {
        if (targetChild.traceRoot) targetChild.setAngle(compileMinusAngle(targetAngle));
        if (targetChild.childParts != null) setChildAngle(targetAngle * (-1), targetChild.childParts);
    }

    private static float getDegree(float A, float B, float C)
    {
        return Mathf.Acos(Mathf.Clamp((Mathf.Pow(C, 2) + Mathf.Pow(A, 2) - Mathf.Pow(B, 2)) / (2 * A * C), -1, 1)) * Mathf.Rad2Deg;
    }
    public void setParent(Material setedParent)
    {
        parentMaterial = setedParent;
        if (childParts != null) childParts.setParent(setedParent);
    }
    public Material followParent()
    {
        if (parentMaterial == null) return null;
        if (parentMaterial.GetComponent<Parts>() == null) return parentMaterial;
        return parentMaterial.GetComponent<Parts>().followParent();
    }
}
