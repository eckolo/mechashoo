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
        var baseAngle = toAngle(targetVector);
        if (targetParts.childParts == null)
        {
            targetParts.transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(baseAngle));
            return targetVector;
        }
        var rootLange = (targetParts.childParts.getParentConnection() - targetParts.getSelfConnection()).magnitude;
        var partsLange = Mathf.Abs(targetParts.childParts.getSelfConnection().x)
            + (targetParts.childParts.GetComponent<Weapon>() != null
            ? targetParts.childParts.GetComponent<Weapon>().injectionHole.x
            : Mathf.Abs(targetParts.childParts.getSelfConnection().x));
        var rootLimit = rootLange + partsLange;

        var targetPosition = targetVector.normalized * Mathf.Clamp(targetVector.magnitude * transform.lossyScale.magnitude, lowerLimitRange * transform.lossyScale.magnitude + Mathf.Abs(partsLange - rootLange), rootLimit);

        setAngle(rootLange, partsLange, targetPosition, targetParts, positive);

        return targetPosition / transform.lossyScale.magnitude;
    }
    public Vector2 setManipulateEim(Vector2 targetPosition, Parts targetParts, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition);
        var rootLange = (targetParts.childParts.getParentConnection() - targetParts.getSelfConnection()).magnitude;
        var partsLange = targetPosition.magnitude + (rootLange * (Mathf.Abs(baseAngle) - 90) / 90);

        setAngle(rootLange, partsLange, targetPosition, targetParts, positive);

        return targetPosition;
    }
    private void setAngle(float rootLange, float partsLange, Vector2 targetPosition, Parts targetParts, bool positive = true)
    {
        var baseAngle = toAngle(targetPosition);
        var targetLange = targetPosition.magnitude;
        var monoAngle = getDegree(rootLange, partsLange, targetLange);
        var jointAngle = monoAngle + getDegree(partsLange, rootLange, targetLange);

        var parentAngle = compileMinusAngle(baseAngle + monoAngle * (positive ? -1 : 1));
        var childAngle = compileMinusAngle(jointAngle * (positive ? 1 : -1));

        targetParts.transform.localEulerAngles = new Vector3(0, 0, parentAngle);
        setChildAngle(childAngle, targetParts.childParts);
    }
    private void setChildAngle(float targetAngle, Parts targetChild)
    {
        targetChild.transform.localEulerAngles = new Vector3(0, 0, compileMinusAngle(targetAngle));
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
}
