using UnityEngine;

/// <summary>
///角度系の汎用計算クラス
/// </summary>
public static class MathA
{
    /// <summary>
    ///180°以下の角が大きい方の角度を取得
    /// </summary>
    public static float MaxAngle(this float main, float sub) => Acute(main) >= Acute(sub) ? main : sub;
    /// <summary>
    ///180°以下の角が小さい方の角度を取得
    /// </summary>
    public static float MinAngle(this float main, float sub) => Acute(main) <= Acute(sub) ? main : sub;
    /// <summary>
    ///180°以下の角の取得
    /// </summary>
    public static float Acute(this float angle) => Mathf.Min(Compile(angle), 360 - Compile(angle));
    /// <summary>
    ///角度補正関数
    ///主にイージングとか
    /// </summary>
    public static float CorrectAngle(this float main, float sub, float degree = 0.5f)
    {
        main = Compile(main);
        sub = Compile(sub);

        bool normalOrder = Mathf.Abs(main - sub) > 180;
        float startPoint = normalOrder ? main : sub;
        float endPoint = normalOrder ? sub : main;
        float actualDegree = normalOrder ? degree : 1 - degree;

        return Compile(startPoint.Correct(endPoint, actualDegree));
    }
    /// <summary>
    ///角度を0から360までに収める
    /// </summary>
    public static float Compile(this float angle)
    {
        while(angle < 0) angle += 360;
        while(angle >= 360) angle -= 360;
        return angle;
    }
    /// <summary>
    ///ベクトルを角度化
    /// </summary>
    public static float ToAngle(this Vector2 targetVector) => Compile(Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1));
    /// <summary>
    ///クォータニオンを角度化
    /// </summary>
    public static float ToAngle(this Quaternion targetRotation) => targetRotation.eulerAngles.z;
    /// <summary>
    ///角度をクォータニオン化
    /// </summary>
    public static Quaternion ToRotation(this float targetAngle) => new Quaternion()
    {
        eulerAngles = new Vector3(0, 0, targetAngle)
    };
    /// <summary>
    ///ベクトルをクォータニオン化
    /// </summary>
    public static Quaternion ToRotation(this Vector2 targetVector) => ToRotation(ToAngle(targetVector));
    /// <summary>
    ///角度を左右反転
    /// </summary>
    public static float Invert(this float targetAngle) => Compile(180 - targetAngle);
    /// <summary>
    ///角度を左右反転
    /// </summary>
    public static Quaternion Invert(this Quaternion targetRotation) => ToRotation(Invert(ToAngle(targetRotation)));
}
