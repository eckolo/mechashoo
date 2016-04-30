using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    public float lowerLimitRange = 0;

    public List<Parts> childPartsList = new List<Parts>();

    void Start() { }
    void Update() { }

    public Vector2 setManipulatePosition(Vector2 targetVector, Parts targetParts, bool positive = true)
    {
        var baseAngle = Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
        if (targetParts.childParts == null)
        {
            targetParts.transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(baseAngle));
            return targetVector;
        }
        var rootLange = (targetParts.childParts.parentConnectionLocal - targetParts.selfConnectionLocal).magnitude;
        var partsLange = Mathf.Abs(targetParts.childParts.selfConnectionLocal.x)
            + (targetParts.childParts.GetComponent<Weapon>() != null
            ? targetParts.childParts.GetComponent<Weapon>().injectionHole.x
            : Mathf.Abs(targetParts.childParts.selfConnectionLocal.x));
        var rootLimit = rootLange + partsLange;

        var targetPosition = targetVector.normalized * getMaxMin(targetVector.magnitude * transform.lossyScale.magnitude, rootLimit, lowerLimitRange * transform.lossyScale.magnitude + Mathf.Abs(partsLange - rootLange));
        var targetLange = targetPosition.magnitude;

        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = compileMinusAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileMinusAngle(jointAngle * (positive ? 1 : -1));

        targetParts.transform.localEulerAngles = new Vector3(0, 0, parentAngle);
        setChildAngle(childAngle, targetParts.childParts);

        return targetPosition / transform.lossyScale.magnitude;
    }
    private void setChildAngle(float targetAngle, Parts targetChild)
    {
        targetChild.transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(targetAngle));
        if (targetChild.childParts != null) setChildAngle(targetAngle * (-1), targetChild.childParts);
    }
    /*
    public Vector2 getManipulatePosition(Parts targetParts)
    {
        if (targetParts.childParts == null) return compileAngleVector(Mathf.Abs(targetParts.selfConnectionLocal.magnitude) * 2, targetParts.transform.eulerAngles.z);
        var monoLong = (targetParts.childParts.parentConnectionLocal - targetParts.selfConnectionLocal).magnitude;
    }
    */

    private static float compileMinusAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }
    private static float getMax(float value, float max) { return value > max ? max : value; }
    private static float getMin(float value, float min) { return value < min ? min : value; }
    private static float getMaxMin(float value, float max, float min)
    {
        return getMin(getMax(value, max), min);
    }
    private static Vector2 compileAngleVector(float lange, float angle)
    {
        return new Vector2(lange * Mathf.Cos(angle), lange * Mathf.Sin(angle));
    }
    private static float getDegree(float A, float B, float C)
    {
        return Mathf.Acos(getMaxMin((Mathf.Pow(C, 2) + Mathf.Pow(A, 2) - Mathf.Pow(B, 2)) / (2 * A * C), 1, -1)) * Mathf.Rad2Deg;
    }
}
