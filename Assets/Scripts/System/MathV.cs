using UnityEngine;

/// <summary>
///ベクトル関係の汎用計算クラス
///一部オーバーロード用の数値クラスが混じっているので注意
/// </summary>
public static class MathV
{
    /// <summary>
    ///大きい方のベクトルを取得
    /// </summary>
    public static Vector2 max(this Vector2 main, Vector2 sub)
        => main.magnitude >= sub.magnitude ? main : sub;
    /// <summary>
    ///ベクトル長の最小値を設定
    /// </summary>
    public static Vector2 max(this Vector2 main, float limit)
        => max(main, main.normalized * limit);
    /// <summary>
    ///小さい方のベクトルを取得
    /// </summary>
    public static Vector2 min(this Vector2 main, Vector2 sub)
        => main.magnitude <= sub.magnitude ? main : sub;
    /// <summary>
    ///ベクトル長の最大値を設定
    /// </summary>
    public static Vector2 min(this Vector2 main, float limit)
        => min(main, main.normalized * limit);
    /// <summary>
    ///各要素の絶対値を取ったベクトルを取得
    /// </summary>
    public static Vector2 abs(this Vector2 main)
        => Vector2.right * Mathf.Abs(main.x) + Vector2.up * Mathf.Abs(main.y);
    /// <summary>
    ///mainのベクトルをsubに合わせて補正する
    /// </summary>
    public static Vector2 correct(this Vector2 main, Vector2 sub, float degree = 0.5f)
        => main * degree + sub * (1 - degree);
    /// <summary>
    ///mainの数値をsubに合わせて補正する
    /// </summary>
    public static float correct(this float main, float sub, float degree = 0.5f)
        => main * degree + sub * (1 - degree);
    /// <summary>
    ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
    /// </summary>
    public static Vector2 scaling(this Vector2 main, Vector2 scale)
        => Vector2.right * main.x * scale.x + Vector2.up * main.y * scale.y;
    /// <summary>
    ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
    /// </summary>
    public static Vector2 scaling(this Vector2 main, float scaleX, float scaleY)
        => scaling(main, new Vector2(scaleX, scaleY));
    /// <summary>
    ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
    /// </summary>
    public static Vector2 rescaling(this Vector2 main, Vector2 scale)
        => Vector2.right * main.x / scale.x + Vector2.up * main.y / scale.y;
    /// <summary>
    ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
    /// </summary>
    public static Vector2 rescaling(this Vector2 main, float scaleX, float scaleY)
        => rescaling(main, new Vector2(scaleX, scaleY));
    /// <summary>
    ///向きと長さからベクトル生成
    /// </summary>
    public static Vector2 recalculation(this Vector2 direction, float length)
        => direction.magnitude != 0 ? direction * length / direction.magnitude : Vector2.zero;
    /// <summary>
    ///向きと長さからベクトル生成
    /// </summary>
    public static Vector2 recalculation(this Vector2 direction, Vector2 length)
        => recalculation(direction, length.magnitude);
    /// <summary>
    ///向きと長さからベクトル生成
    /// </summary>
    public static Vector2 recalculation(this Quaternion direction, float length)
        => recalculation(direction * Vector2.right, length);
    /// <summary>
    ///向きと長さからベクトル生成
    /// </summary>
    public static Vector2 recalculation(this float direction, float length)
        => recalculation(direction.toRotation(), length);
    /// <summary>
    ///向きと長さからベクトル生成
    /// </summary>
    public static Vector2 recalculation(this float direction, Vector2 length)
        => recalculation(direction.toRotation(), length.magnitude);
    /// <summary>
    ///ベクトルを指定枠内に収まる値に補正
    /// </summary>
    public static Vector2 within(this Vector2 main, Vector2 lowerLeft, Vector2 upperRight)
    {
        Vector2 returnVector = main;
        returnVector.x = Mathf.Clamp(returnVector.x, lowerLeft.x, upperRight.x);
        returnVector.y = Mathf.Clamp(returnVector.y, lowerLeft.y, upperRight.y);
        return returnVector;
    }

    /// <summary>
    ///ベクトルイージング関数群
    /// </summary>
    public static class EasingV
    {
        /// <summary>
        ///始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 elliptical(Vector2 end, float time, float limit, bool clockwise)
        {
            bool verticalIn = clockwise ^ (end.x * end.y > 0);
            float right = verticalIn
                ? Easing.sinusoidal.Out(end.x, time, limit)
                : Easing.sinusoidal.In(end.x, time, limit);
            float up = verticalIn
                ? Easing.sinusoidal.In(end.y, time, limit)
                : Easing.sinusoidal.Out(end.y, time, limit);
            return new Vector2(right, up);
        }
        /// <summary>
        ///始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 elliptical(Vector2 start, Vector2 end, float time, float limit, bool clockwise) => start + elliptical(end - start, time, limit, clockwise);
    }
}
