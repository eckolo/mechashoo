using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    public List<Parts> childPartsList = new List<Parts>();
    public float lowerLimitRange = 0;

    public Vector2 xy = new Vector2(0, 0);
    public bool positive = true;

    void Start()
    {
    }

    void Update()
    {
        xy.x += Input.GetAxisRaw("Horizontal") / 10;
        xy.y += Input.GetAxisRaw("Vertical") / 10;

        foreach (var childParts in childPartsList)
        {
            xy = setManipulatePosition(xy, childParts);
        }
    }

    public Vector2 setManipulatePosition(Vector2 targetVector, Parts targetParts)
    {
        var baseAngle = Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
        if (targetParts.childParts == null)
        {
            targetParts.transform.localEulerAngles = new Vector3(0, 0, baseAngle);
            return targetVector;
        }
        var rootLimit = (targetParts.childParts.parentConnectionLocal - targetParts.selfConnectionLocal).magnitude * 2;
        var targetPosition = targetVector.normalized * getMaxMin(targetVector.magnitude, rootLimit, lowerLimitRange * transform.lossyScale.magnitude);
        var monoAngle = Mathf.Acos(getMaxMin(targetPosition.magnitude / rootLimit, 1, -1)) * Mathf.Rad2Deg;
        var parentAngle = compileMinusAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileMinusAngle(monoAngle * (positive ? 2 : -2));

        targetParts.transform.localEulerAngles = new Vector3(0, 0, parentAngle);
        targetParts.childParts.transform.localEulerAngles = new Vector3(0, 0, childAngle);

        return targetPosition;
    }

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
    private static Vector2 compileAngleVector(float lange, float angle) { return new Vector2(lange * Mathf.Cos(angle), lange * Mathf.Sin(angle)); }
}
