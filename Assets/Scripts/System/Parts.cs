using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public virtual Vector2 correctionVector { get; set; } = Vector2.zero;
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
        transform.localScale = new Vector2(transform.localScale.x, Mathf.Abs(transform.localScale.y) * heightPositive.toSign());
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
    public virtual Vector2 nowLengthVector
    {
        get {
            Vector2 baseVector = -selfConnection;
            if(childParts != null) return baseVector + childParts.parentConnection;
            return baseVector * 2;
        }
    }

    /// <summary>
    /// 複数接続時の先端位置指定角度制御
    /// </summary>
    /// <param name="targetVector">先端位置</param>
    /// <param name="positive">接続部の突き出しが時計回り方向</param>
    /// <returns></returns>
    public Vector2 setManipulator(Vector2 targetVector, bool positive = true)
    {
        if(childParts == null)
        {
            basePosition = targetVector;
            transform.localEulerAngles = new Vector3(0, 0, (targetVector + nowCorrection).toAngle().compile());
            return targetVector;
        }
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;
        var rootScale = nowRoot.lossyScale.abs();

        var targetPosition = targetVector;
        targetPosition = targetPosition.max(Mathf.Abs(rootLange - partsLange));
        targetPosition = targetPosition.min(rootLange + partsLange);
        targetPosition = targetPosition.scaling(rootScale);

        setLangeToAngle(rootLange, partsLange, targetPosition, positive);

        return targetPosition.rescaling(rootScale);
    }
    /// <summary>
    /// 複数接続時の照準位置指定角度制御
    /// </summary>
    /// <param name="targetPosition">照準位置</param>
    /// <param name="bendingCondition">接続部の角度補正</param>
    /// <param name="positive">接続部の突き出しが時計回り方向</param>
    /// <returns></returns>
    public Vector2 setAlignment(Vector2 targetPosition, float bendingCondition = 0, bool positive = true)
    {
        var setPosition = correctWidthVector(targetPosition);
        var targetLange = setPosition.magnitude;
        var rootLange = nowLengthVector.magnitude;
        var partsLange = childParts.nowLengthVector.magnitude;

        if(targetLange < rootLange + partsLange) return setManipulator(setPosition, positive);

        var baseAngle = setPosition.toAngle().compile();
        var baseAngleAbs = Mathf.Abs(baseAngle < 180 ? baseAngle : baseAngle - 360) + 90 * bendingCondition;
        var langeDifference = Mathf.Abs(targetLange - (rootLange + partsLange));
        var angleCorrection = (baseAngleAbs * (1 - 1 / (langeDifference + 1)) - 90) / 90;
        var alignmentLange = targetLange + (rootLange * angleCorrection);

        setLangeToAngle(rootLange, alignmentLange, setPosition, positive);

        return targetPosition;
    }
    private void setLangeToAngle(float rootLange, float partsLange, Vector2 targetPosition, bool positive = true, bool corrected = false)
    {
        if(!corrected) basePosition = targetPosition;

        var baseAngle = targetPosition.toAngle();
        var targetLange = targetPosition.magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = (baseAngle + monoAngle * (positive ? -1 : 1)).compile();
        var childAngle = (jointAngle * (positive ? 1 : -1)).compile();

        if(nowCorrection.magnitude != 0 && !corrected)
        {
            var rootVector = parentAngle.recalculation(nowLengthVector);
            var partsVector = (parentAngle + childAngle).recalculation(childParts.nowLengthVector);

            Vector2 tipsPosition = rootVector + partsVector;

            Vector2 correction = tipsPosition.toRotation() * nowCorrection;

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
        setAngle(targetAngle.compile().minAngle(180 - lowerLimitAngle));
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
            var rootParts = _nowRoot?.GetComponent<Parts>();
            if(rootParts != null) return rootParts.nowRoot;
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
            checkConnection();
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

        base.selfDestroy(system);
    }
}
