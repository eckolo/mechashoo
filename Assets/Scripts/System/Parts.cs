using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 関節挙動とかするパーツ部位
/// </summary>
public class Parts : Materials
{
    /// <summary>
    ///接続先のParts
    /// </summary>
    public Parts childParts = null;
    /// <summary>
    ///接続関連の座標
    /// </summary>
    public Vector2 parentConnection = Vector2.zero;
    public Vector2 selfConnection = Vector2.zero;
    /// <summary>
    ///関節挙動のターゲット座標
    /// </summary>
    public Vector2 basePosition = Vector2.right;
    /// <summary>
    ///制御元のMaterial
    /// </summary>
    [System.NonSerialized]
    public Things parentMaterial = null;
    /// <summary>
    ///先端位置補正
    /// </summary>
    [System.NonSerialized]
    public Vector2 correctionVector = Vector2.zero;
    /// <summary>
    ///関節の最小折り畳み角度
    /// </summary>
    public float lowerLimitAngle = 0;

    // Update is called once per frame
    public override void Start()
    {
        base.Start();
        setPosition();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        setPosition();
        transform.localScale = new Vector2(transform.localScale.x, Mathf.Abs(transform.localScale.y) * (heightPositive ? 1 : -1));
    }

    private void setPosition()
    {
        var parent = transform.parent != null ? transform.parent : transform;
        var parentConnectionRotation = (Vector2)(parent.transform.rotation * nowParentConnection);
        var selfConnectionRotation = (Vector2)(transform.rotation * nowSelfConnection);
        transform.position = parent.transform.position + (Vector3)(parentConnectionRotation - selfConnectionRotation);
    }

    public Vector2 nowParentConnection
    {
        get
        {
            var parent = transform.parent != null ? transform.parent : transform;
            return MathV.scaling(parentConnection, getLossyScale(parent));
        }
    }
    public Vector2 nowSelfConnection
    {
        get
        {
            return MathV.scaling(selfConnection, getLossyScale());
        }
    }
    public virtual Vector2 nowCorrection
    {
        get
        {
            if (childParts == null) return correctionVector;
            //if (childParts.correctionVector.magnitude != 0) Debug.Log(childParts + " : " + childParts.correctionVector);
            return correctionVector + childParts.nowCorrection;
        }
    }
    public Vector2 nowBasePosition
    {
        get
        {
            return correctWidthVector(basePosition);
        }
    }
    public Vector2 nowLengthVector
    {
        get
        {
            Vector2 baseVector = -selfConnection;

            Weapon weapon = GetComponent<Weapon>();
            if (weapon != null)
            {
                if (weapon.injections.Count <= 0) return baseVector;
                return baseVector + weapon.injections[0].hole;
            }

            Hand hand = GetComponent<Hand>();
            if (hand != null)
            {
                if (childParts == null) return baseVector + hand.takePosition;
                return baseVector + hand.takePosition + childParts.nowLengthVector;
            }

            if (childParts != null) return baseVector + childParts.parentConnection;

            return baseVector * 2;
        }
    }
    public Parts grandsonParts
    {
        get
        {
            if (childParts == null) return null;
            return childParts.childParts;
        }
    }

    public Vector2 setManipulator(Vector2 targetVector, bool positive = true)
    {
        if (childParts == null)
        {
            basePosition = targetVector;
            transform.localEulerAngles = new Vector3(0, 0, MathA.compile(MathA.toAngle(targetVector + nowCorrection)));
            return targetVector;
        }
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;
        var parentScale = MathV.abs(parentMaterial.getLossyScale());

        var targetPosition = targetVector;
        targetPosition = MathV.max(targetPosition, Mathf.Abs(rootLange - partsLange));
        targetPosition = MathV.min(targetPosition, rootLange + partsLange);
        targetPosition = MathV.scaling(targetPosition, parentScale);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return MathV.rescaling(targetPosition, parentScale);
    }
    public Vector2 setAlignment(Vector2 targetPosition, bool positive = true)
    {
        var targetLange = targetPosition.magnitude;
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;

        if (targetLange < rootLange + partsLange) return setManipulator(targetPosition, positive);

        var baseAngle = MathA.compile(MathA.toAngle(targetPosition));
        var angleCorrection = (Mathf.Abs(baseAngle < 180 ? baseAngle : baseAngle - 360)
            * (1 - 1 / (Mathf.Abs(targetLange - (rootLange + partsLange)) + 1)) - 90) / 90;
        var alignmentLange = targetLange + (rootLange * angleCorrection);

        setLangeToAngle(rootLange, alignmentLange, targetPosition, positive);

        return targetPosition;
    }
    private void setLangeToAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true, bool corrected = false)
    {
        if (!corrected) basePosition = targetPosition;

        var baseAngle = MathA.toAngle(targetPosition);
        var targetLange = targetPosition.magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = MathA.compile(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = MathA.compile(jointAngle * (positive ? 1 : -1));

        if (nowCorrection.magnitude != 0 && !corrected)
        {
            var rootVector = MathV.recalculation(parentAngle, nowLengthVector);
            var partsVector = MathV.recalculation(parentAngle + childAngle, childParts.nowLengthVector);

            Vector2 tipsPosition = rootVector + partsVector;

            Vector2 correction = MathA.toRotation(tipsPosition) * nowCorrection;

            setLangeToAngle(nowLengthVector.magnitude, childParts.nowLengthVector.magnitude, tipsPosition + correction, positive, true);
        }
        else
        {
            setAngle(parentAngle);
            childParts.setChildAngle(childAngle);
        }
    }
    public void setChildAngle(float targetAngle)
    {
        setAngle(MathA.min(MathA.compile(targetAngle), 180 - lowerLimitAngle));
    }

    private static float getDegree(float A, float B, float C)
    {
        return Mathf.Acos(Mathf.Clamp((Mathf.Pow(C, 2) + Mathf.Pow(A, 2) - Mathf.Pow(B, 2)) / (2 * A * C), -1, 1)) * Mathf.Rad2Deg;
    }
    public void setParent(Things setedParent)
    {
        parentMaterial = setedParent;
        if (childParts != null) childParts.setParent(setedParent);
    }
    public Things nowParent
    {
        get
        {
            if (parentMaterial == null) return null;
            if (parentMaterial.GetComponent<Parts>() == null) return parentMaterial;
            return parentMaterial.GetComponent<Parts>().nowParent;
        }
    }

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public override void selfDestroy(bool system = false)
    {
        var material = GetComponent<Things>();
        if (material != null) material.deleteParts();

        if (childParts != null) childParts.selfDestroy();

        base.selfDestroy();
    }
}
