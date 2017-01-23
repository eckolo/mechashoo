using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 関節挙動とかするパーツ部位
/// </summary>
public class Parts : Materials
{
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
        checkConnection();
        setPosition();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        setPosition();
        transform.localScale = new Vector2(transform.localScale.x, Mathf.Abs(transform.localScale.y) * toSign(heightPositive));
    }

    public void checkConnection()
    {
        if(nowConnectParent == null && nowParent != null) nowConnectParent = nowParent.GetComponent<Materials>();
        if(childParts == null)
        {
            foreach(Transform child in transform)
            {
                var parts = child.GetComponent<Parts>();
                if(parts != null) parts.nowConnectParent = this;
            }
        }
        if(childParts != null) childParts.checkConnection();
    }

    private void setPosition()
    {
        if(nowConnectParent == null) return;
        position = parentConnection - (Vector2)(transform.localRotation * selfConnection);
    }
    /// <summary>
    ///接続先のParts
    /// </summary>
    public Parts childParts
    {
        get {
            return _childParts;
        }
        set {
            _childParts = value;
            _childParts.nowRoot = nowRoot;
            _childParts.checkConnection();
        }
    }
    private Parts _childParts = null;

    public Parts grandsonParts
    {
        get {
            if(childParts == null) return null;
            return childParts.childParts;
        }
    }

    public virtual Vector2 nowCorrection
    {
        get {
            if(childParts == null) return correctionVector;
            return correctionVector + childParts.nowCorrection;
        }
    }
    public Vector2 nowBasePosition
    {
        get {
            return correctWidthVector(basePosition);
        }
    }
    public Vector2 nowLengthVector
    {
        get {
            Vector2 baseVector = -selfConnection;

            Weapon weapon = GetComponent<Weapon>();
            if(weapon != null)
            {
                if(weapon.injections.Count <= 0) return baseVector;
                return baseVector + weapon.injections[0].hole;
            }

            Hand hand = GetComponent<Hand>();
            if(hand != null)
            {
                if(childParts == null) return baseVector + hand.takePosition;
                return baseVector + hand.takePosition + childParts.nowLengthVector;
            }

            if(childParts != null) return baseVector + childParts.parentConnection;

            return baseVector * 2;
        }
    }

    public Vector2 setManipulator(Vector2 targetVector, bool positive = true)
    {
        if(childParts == null)
        {
            basePosition = targetVector;
            transform.localEulerAngles = new Vector3(0, 0, MathA.compile(MathA.toAngle(targetVector + nowCorrection)));
            return targetVector;
        }
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;
        var rootScale = MathV.abs(nowRoot.lossyScale);

        var targetPosition = targetVector;
        targetPosition = MathV.max(targetPosition, Mathf.Abs(rootLange - partsLange));
        targetPosition = MathV.min(targetPosition, rootLange + partsLange);
        targetPosition = MathV.scaling(targetPosition, rootScale);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return MathV.rescaling(targetPosition, rootScale);
    }
    public Vector2 setAlignment(Vector2 targetPosition, bool positive = true)
    {
        var setPosition = correctWidthVector(targetPosition);
        var targetLange = setPosition.magnitude;
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;

        if(targetLange < rootLange + partsLange) return setManipulator(setPosition, positive);

        var baseAngle = MathA.compile(MathA.toAngle(setPosition));
        var angleCorrection = (Mathf.Abs(baseAngle < 180 ? baseAngle : baseAngle - 360)
            * (1 - 1 / (Mathf.Abs(targetLange - (rootLange + partsLange)) + 1)) - 90) / 90;
        var alignmentLange = targetLange + (rootLange * angleCorrection);

        setLangeToAngle(rootLange, alignmentLange, setPosition, positive);

        return targetPosition;
    }
    private void setLangeToAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true, bool corrected = false)
    {
        if(!corrected) basePosition = targetPosition;

        var baseAngle = MathA.toAngle(targetPosition);
        var targetLange = targetPosition.magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = MathA.compile(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = MathA.compile(jointAngle * (positive ? 1 : -1));

        if(nowCorrection.magnitude != 0 && !corrected)
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
    /// <summary>
    ///制御元のオブジェクト
    /// </summary>
    public Things nowRoot
    {
        get {
            return _nowRoot;
        }
        set {
            _nowRoot = value;
            if(childParts != null) childParts.nowRoot = value;
        }
    }
    private Things _nowRoot = null;

    /// <summary>
    ///接続されてる親のPartsもしくはルートオブジェクト
    /// </summary>
    public Materials nowConnectParent
    {
        get {
            if(nowParent == null) return null;
            var things = nowParent.GetComponent<Things>();
            var parts = nowParent.GetComponent<Parts>();
            if(things != null && things.getPartsList.Contains(this)) return things;
            if(parts != null && parts.childParts == this) return parts;
            return null;
        }
        set {
            var parts = value.GetComponent<Parts>();
            var things = value.GetComponent<Things>();
            if(parts != null && parts.childParts == null) parts.childParts = this;
            if(things != null) things.setParts(this);
        }
    }

    /// <summary>
    ///奥行き位置の設定
    /// </summary>
    public override float nowZ
    {
        get {
            return base.nowZ;
        }

        set {
            base.nowZ = value;
            if(childParts != null) childParts.nowZ = value;
        }
    }

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public override void selfDestroy(bool system = false)
    {
        if(this == null) return;

        var material = GetComponent<Things>();
        if(material != null) material.deleteParts();

        if(childParts != null) childParts.selfDestroy();

        base.selfDestroy();
    }
}
