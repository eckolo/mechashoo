using UnityEngine;

/// <summary>
///角度系の汎用計算クラス
/// </summary>
public static class MathA
{
    /// <summary>
    ///180°以下の角が大きい方の角度を取得
    /// </summary>
    public static float max(float main, float sub) => acute(main) >= acute(sub) ? main : sub;
    /// <summary>
    ///180°以下の角が小さい方の角度を取得
    /// </summary>
    public static float min(float main, float sub) => acute(main) <= acute(sub) ? main : sub;
    /// <summary>
    ///180°以下の角の取得
    /// </summary>
    public static float acute(float angle) => Mathf.Min(compile(angle), 360 - compile(angle));
    /// <summary>
    ///角度補正関数
    ///主にイージングとか
    /// </summary>
    public static float correct(float main, float sub, float degree = 0.5f)
    {
        main = compile(main);
        sub = compile(sub);

        bool normalOrder = Mathf.Abs(main - sub) > 180;
        float startPoint = normalOrder ? main : sub;
        float endPoint = normalOrder ? sub : main;
        float actualDegree = normalOrder ? degree : 1 - degree;

        return compile(MathV.correct(startPoint, endPoint, actualDegree));
    }
    /// <summary>
    ///角度を0から360までに収める
    /// </summary>
    public static float compile(float angle)
    {
        while(angle < 0) angle += 360;
        while(angle >= 360) angle -= 360;
        return angle;
    }
    /// <summary>
    ///ベクトルを角度化
    /// </summary>
    public static float toAngle(Vector2 targetVector) => compile(Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1));
    /// <summary>
    ///クォータニオンを角度化
    /// </summary>
    public static float toAngle(Quaternion targetRotation) => targetRotation.eulerAngles.z;
    /// <summary>
    ///角度をクォータニオン化
    /// </summary>
    public static Quaternion toRotation(float targetAngle) => new Quaternion()
    {
        eulerAngles = new Vector3(0, 0, targetAngle)
    };
    /// <summary>
    ///ベクトルをクォータニオン化
    /// </summary>
    public static Quaternion toRotation(Vector2 targetVector) => toRotation(toAngle(targetVector));
    /// <summary>
    ///角度を左右反転
    /// </summary>
    public static float invert(float targetAngle) => compile(180 - targetAngle);
    /// <summary>
    ///角度を左右反転
    /// </summary>
    public static Quaternion invert(Quaternion targetRotation) => toRotation(invert(toAngle(targetRotation)));
}
