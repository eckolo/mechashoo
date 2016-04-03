using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    public List<Parts> childPartsList = new List<Parts>();

    public Vector2 xy = new Vector2(0, 0);
    public float rootLimit;
    public Vector2 targetPosition;
    public float monoAngle;
    public float baseAngle;
    public float parentAngle;
    public float childAngle;

    void Start()
    {
    }

    void Update()
    {
        xy.x += Input.GetAxisRaw("Horizontal") / 10;
        xy.y += Input.GetAxisRaw("Vertical") / 10;

        foreach (var childParts in childPartsList)
        {
            setManipulatePosition(xy, childParts);
        }
    }

    public void setManipulatePosition(Vector2 targetVector, Parts targetParts, bool positive = true)
    {
        rootLimit = (targetParts.childParts.parentConnectionLocal - targetParts.selfConnectionLocal).magnitude * 2;
        targetPosition = targetVector.normalized * getMax(targetVector.magnitude, rootLimit);
        monoAngle = Mathf.Acos(getMaxMin(targetPosition.magnitude / rootLimit, 1, -1)) * Mathf.Rad2Deg;
        baseAngle = Vector2.Angle(Vector2.right, targetPosition) * (Vector2.Angle(Vector2.up, targetPosition) <= 90 ? 1 : -1);
        parentAngle = covertMinusAngle(monoAngle + baseAngle);
        childAngle = covertMinusAngle(monoAngle * -2);

        targetParts.transform.localEulerAngles = new Vector3(0, 0, parentAngle);
        targetParts.childParts.transform.localEulerAngles = new Vector3(0, 0, childAngle);

        return;
    }
    private float covertMinusAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }
    private float getMax(float value, float max) { return value > max ? max : value; }
    private float getMin(float value, float min) { return value < min ? min : value; }
    private float getMaxMin(float value, float max, float min)
    {
        return getMin(getMax(value, max), min);
    }
}
