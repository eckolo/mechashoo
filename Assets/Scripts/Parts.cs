using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 関節挙動とかするパーツ部位
/// </summary>
public class Parts : Material
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
    ///親Partsの角度をトレースするか否かフラグ
    /// </summary>
    public bool traceRoot = false;
    /// <summary>
    ///制御元のMaterial
    /// </summary>
    public Things parentMaterial = null;
    /// <summary>
    ///先端位置補正
    /// </summary>
    public Vector2 correctionVector = Vector2.zero;
    /// <summary>
    ///関節の最小折り畳み角度を定義するパラメータ
    /// </summary>
    public float lowerLimitRange = 0;

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
        parentConnectionRotation = new Vector2(parentConnectionRotation.x, parentConnectionRotation.y * nWidthPositive);
        var selfConnectionRotation = (Vector2)(transform.rotation * nowSelfConnection);
        selfConnectionRotation = new Vector2(selfConnectionRotation.x, selfConnectionRotation.y * nWidthPositive);
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
            if (childParts != null)
            {
                Quaternion baseRotation = childParts.traceRoot
                    ? Quaternion.FromToRotation(Vector2.right, nowBasePosition)
                    : new Quaternion(0, 0, 0, 0);
                return correctWidthVector(
                    baseRotation * correctionVector * nWidthPositive
                    + baseRotation * childParts.nowCorrection
                    );
            }
            else
            {
                return correctWidthVector(correctionVector * nWidthPositive);
            }
        }
    }
    public Vector2 nowBasePosition
    {
        get
        {
            return correctWidthVector(basePosition);
        }
    }
    public Vector2 nowTipsPosition
    {
        get
        {
            Vector2 baseVector = selfConnection;

            Weapon weapon = GetComponent<Weapon>();
            if (weapon != null)
            {
                if (weapon.injectionHoles.Count <= 0) return baseVector;
                return baseVector + weapon.injectionHoles[0];
            }

            Hand hand = GetComponent<Hand>();
            if (hand != null) return baseVector + hand.takePosition + childParts.nowTipsPosition;

            if (childParts != null) return baseVector + childParts.nowParentConnection;

            return baseVector * 2;
        }
    }

    public Vector2 setManipulator(Vector2 targetVector, bool positive = true)
    {
        var baseAngle = toAngle(targetVector + nowCorrection);
        if (childParts == null)
        {
            basePosition = targetVector;
            transform.localEulerAngles = new Vector3(0, 0, compileAngle(baseAngle));
            return targetVector;
        }
        var rootLange = (childParts.nowParentConnection - nowSelfConnection).magnitude;
        var partsLange = childParts.nowTipsPosition.magnitude;
        var rootLimit = rootLange + partsLange;
        var parentScale = MathV.Abs(parentMaterial.getLossyScale());

        var targetPosition = targetVector;
        targetPosition = MathV.Max(targetPosition, Mathf.Abs(rootLange - partsLange));
        targetPosition = MathV.Min(targetPosition, rootLimit);
        targetPosition = MathV.scaling(targetPosition, parentScale);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return MathV.rescaling(targetPosition, parentScale);
    }
    public Vector2 setAlignment(Vector2 targetPosition, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition + nowCorrection);
        var rootLange = (childParts.nowParentConnection - nowSelfConnection).magnitude;
        var partsLange = (targetPosition + nowCorrection).magnitude + (rootLange * (Mathf.Abs(baseAngle) - 90) / 90);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition;
    }
    private void setLangeToAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true)
    {
        basePosition = targetPosition;

        var baseAngle = toAngle(targetPosition + nowCorrection);
        var targetLange = (targetPosition + nowCorrection).magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = compileAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileAngle(jointAngle * (positive ? 1 : -1));

        setAngle(parentAngle);
        setChildAngle(childAngle, childParts);
    }
    private void setChildAngle(float targetAngle, Parts targetChild)
    {
        if (targetChild.traceRoot) targetChild.setAngle(compileAngle(targetAngle));
        if (targetChild.childParts != null) setChildAngle(targetAngle * (-1), targetChild.childParts);
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
