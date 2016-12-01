using UnityEngine;

public partial class Methods : MonoBehaviour
{

    /// <summary>
    ///角度系の汎用計算クラス
    /// </summary>
    protected static class MathA
    {
        /// <summary>
        ///鋭角の大きい方の角度を取得
        /// </summary>
        public static float max(float main, float sub)
        {
            return acute(main) >= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の小さい方の角度を取得
        /// </summary>
        public static float min(float main, float sub)
        {
            return acute(main) <= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の取得
        /// </summary>
        public static float acute(float angle)
        {
            return Mathf.Min(compile(angle), 360 - compile(angle));
        }
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

            return compile(MathV.correct(startPoint, endPoint, degree));
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
        public static float toAngle(Vector2 targetVector)
        {
            return Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
        }
        /// <summary>
        ///クォータニオンを角度化
        /// </summary>
        public static float toAngle(Quaternion targetRotation)
        {
            return toAngle(targetRotation * Vector2.right);
        }
        /// <summary>
        ///角度をクォータニオン化
        /// </summary>
        public static Quaternion toRotation(float targetAngle)
        {
            var returnRotation = new Quaternion()
            {
                eulerAngles = new Vector3(0, 0, targetAngle)
            };
            return returnRotation;
        }
        /// <summary>
        ///ベクトルをクォータニオン化
        /// </summary>
        public static Quaternion toRotation(Vector2 targetVector)
        {
            return Quaternion.AngleAxis(toAngle(targetVector), Vector3.forward);
        }
    }
}
