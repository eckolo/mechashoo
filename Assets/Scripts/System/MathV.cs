using UnityEngine;

/// <summary>
/// ベクトル関係の汎用計算クラス
/// 一部オーバーロード用の数値クラスが混じっているので注意
/// </summary>
public static class MathV
{
    /// <summary>
    /// 大きい方のベクトルを取得
    /// </summary>
    /// <param name="main">比較対象ベクトル1</param>
    /// <param name="sub">比較対象ベクトル2</param>
    /// <returns>大きい方のベクトル</returns>
    public static Vector2 Max(this Vector2 main, Vector2 sub) => main.magnitude >= sub.magnitude ? main : sub;
    /// <summary>
    /// ベクトル長の最小値を設定
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="limit">最小長</param>
    /// <returns>最小長以上の元ベクトル</returns>
    public static Vector2 Max(this Vector2 origin, float limit) => Max(origin, origin.normalized * limit);
    /// <summary>
    /// 小さい方のベクトルを取得
    /// </summary>
    /// <param name="main">比較対象ベクトル1</param>
    /// <param name="sub">比較対象ベクトル2</param>
    /// <returns>小さい方のベクトル</returns>
    public static Vector2 Min(this Vector2 main, Vector2 sub) => main.magnitude <= sub.magnitude ? main : sub;
    /// <summary>
    /// ベクトル長の最大値を設定
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="limit">最大長</param>
    /// <returns>最大長以下の元ベクトル</returns>
    public static Vector2 Min(this Vector2 origin, float limit) => Min(origin, origin.normalized * limit);
    /// <summary>
    /// 各要素の絶対値を取ったベクトルを取得
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <returns>各要素が絶対値となったベクトル</returns>
    public static Vector2 Abs(this Vector2 origin) => new Vector2(Mathf.Abs(origin.x), Mathf.Abs(origin.y));
    /// <summary>
    /// mainのベクトルをsubに合わせて補正する
    /// </summary>
    /// <param name="main">元ベクトル</param>
    /// <param name="sub">補正ベクトル</param>
    /// <param name="degree">元ベクトルへの寄せ度合い</param>
    /// <returns>補正済みのベクトル</returns>
    public static Vector2 Correct(this Vector2 main, Vector2 sub, float degree = 0.5f)
        => main * degree + sub * (1 - degree);
    /// <summary>
    /// mainの数値をsubに合わせて補正する
    /// </summary>
    /// <param name="main">元値</param>
    /// <param name="sub">補正値</param>
    /// <param name="degree">元値への寄せ度合い</param>
    /// <returns>補正済みの値</returns>
    public static float Correct(this float main, float sub, float degree = 0.5f)
        => main * degree + sub * (1 - degree);
    /// <summary>
    /// ベクトルの各要素にサイズ補正をXYの軸毎に掛ける
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="scale">倍率ベクトル</param>
    /// <returns>補正されたベクトル</returns>
    public static Vector2 Scaling(this Vector2 origin, Vector2 scale)
        => Vector2.right * origin.x * scale.x + Vector2.up * origin.y * scale.y;
    /// <summary>
    /// ベクトルの各要素にサイズ補正をXYの軸毎に掛ける
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="scaleX">X軸倍率</param>
    /// <param name="scaleY">Y軸倍率</param>
    /// <returns>補正されたベクトル</returns>
    public static Vector2 Scaling(this Vector2 origin, float scaleX, float scaleY)
        => Scaling(origin, new Vector2(scaleX, scaleY));
    /// <summary>
    /// ベクトルの各要素にサイズ補正をXYの軸毎に割る
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="scale">倍率ベクトル</param>
    /// <returns>補正されたベクトル</returns>
    public static Vector2 Rescaling(this Vector2 origin, Vector2 scale)
        => Vector2.right * origin.x / scale.x + Vector2.up * origin.y / scale.y;
    /// <summary>
    /// ベクトルの各要素にサイズ補正をXYの軸毎に割る
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="scaleX">X軸倍率</param>
    /// <param name="scaleY">Y軸倍率</param>
    /// <returns>補正されたベクトル</returns>
    public static Vector2 Rescaling(this Vector2 origin, float scaleX, float scaleY)
        => Rescaling(origin, new Vector2(scaleX, scaleY));
    /// <summary>
    /// ベクトルを所定の座標を軸に回転させる
    /// </summary>
    /// <param name="origin">回転対象ベクトル</param>
    /// <param name="pivot">軸座標</param>
    /// <param name="angle">回転角度</param>
    /// <returns>回転後のベクトル</returns>
    public static Vector2 Rotate(this Vector2 origin, Vector2 pivot, float angle)
        => (Vector2)(angle.ToRotation() * (origin - pivot)) + pivot;
    /// <summary>
    /// 向きと長さからベクトル生成
    /// </summary>
    /// <param name="direction">向き</param>
    /// <param name="length">長さ</param>
    /// <returns>生成されたベクトル</returns>
    public static Vector2 ToVector(this Vector2 direction, float length = 1)
        => direction.magnitude != 0 ? direction * length / direction.magnitude : Vector2.zero;
    /// <summary>
    /// 向きと長さからベクトル生成
    /// </summary>
    /// <param name="direction">向き</param>
    /// <param name="length">長さ</param>
    /// <returns>生成されたベクトル</returns>
    public static Vector2 ToVector(this Vector2 direction, Vector2 length)
        => ToVector(direction, length.magnitude);
    /// <summary>
    /// 向きと長さからベクトル生成
    /// </summary>
    /// <param name="direction">向き</param>
    /// <param name="length">長さ</param>
    /// <returns>生成されたベクトル</returns>
    public static Vector2 ToVector(this Quaternion direction, float length = 1)
        => ToVector(direction * Vector2.right, length);
    /// <summary>
    /// 向きと長さからベクトル生成
    /// </summary>
    /// <param name="direction">向き</param>
    /// <param name="length">長さ</param>
    /// <returns>生成されたベクトル</returns>
    public static Vector2 ToVector(this float direction, float length = 1)
        => ToVector(direction.ToRotation(), length);
    /// <summary>
    /// 向きと長さからベクトル生成
    /// </summary>
    /// <param name="direction">向き</param>
    /// <param name="length">長さ</param>
    /// <returns>生成されたベクトル</returns>
    public static Vector2 ToVector(this float direction, Vector2 length)
        => ToVector(direction.ToRotation(), length.magnitude);
    /// <summary>
    /// ベクトルの左右反転
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <returns>左右反転されたベクトル</returns>
    public static Vector2 Invert(this Vector2 origin) => new Vector2(origin.x * -1, origin.y);
    /// <summary>
    /// ベクトルを指定枠内に収まる値に補正
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="lowerLeft">枠の左下座標</param>
    /// <param name="upperRight">枠の右上座標</param>
    /// <returns>枠内に収まった座標ベクトル</returns>
    public static Vector2 Within(this Vector2 origin, Vector2 lowerLeft, Vector2 upperRight)
    {
        Vector2 returnVector = origin;
        returnVector.x = Mathf.Clamp(returnVector.x, lowerLeft.x, upperRight.x);
        returnVector.y = Mathf.Clamp(returnVector.y, lowerLeft.y, upperRight.y);
        return returnVector;
    }
    /// <summary>
    /// アンカーパラメータからアンカー一座標を取得する関数
    /// </summary>
    /// <param name="anchor">アンカーパラメータ</param>
    /// <param name="pibot">軸</param>
    /// <returns>アンカー座標</returns>
    public static Vector2 GetAxis(this TextAnchor anchor, TextAnchor? pibot = null)
    {
        var pibotPosition = pibot?.GetAxis() ?? Vector2.zero;
        switch(anchor)
        {
            case TextAnchor.UpperLeft:
                return Vector2.up - pibotPosition;
            case TextAnchor.UpperCenter:
                return Vector2.right / 2 + Vector2.up - pibotPosition;
            case TextAnchor.UpperRight:
                return Vector2.right + Vector2.up - pibotPosition;
            case TextAnchor.MiddleLeft:
                return Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleCenter:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleRight:
                return Vector2.right + Vector2.up / 2 - pibotPosition;
            case TextAnchor.LowerLeft:
                return Vector2.zero - pibotPosition;
            case TextAnchor.LowerCenter:
                return Vector2.right / 2 - pibotPosition;
            case TextAnchor.LowerRight:
                return Vector2.right - pibotPosition;
            default:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
        }
    }
    /// <summary>
    /// ベクトル長の上昇度合いを抑える補正関数
    /// </summary>
    /// <param name="origin">元ベクトル</param>
    /// <param name="baseNumber">補正の底数</param>
    /// <returns>補正のかかったベクトル</returns>
    public static Vector2 Log(this Vector2 origin, float? baseNumber = null)
        => origin.ToVector(origin.magnitude.Log(baseNumber));

    /// <summary>
    /// ベクトルイージング関数群
    /// </summary>
    public static class EasingV
    {
        /// <summary>
        /// 始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 Elliptical(Vector2 end, float time, float limit, bool clockwise)
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
        /// 始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 Elliptical(Vector2 start, Vector2 end, float time, float limit, bool clockwise) => start + Elliptical(end - start, time, limit, clockwise);
    }
}
