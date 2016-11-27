using UnityEngine;
using System.Collections;

public partial class Methods : MonoBehaviour {
    /// <summary>
    ///ベクトル関係の汎用計算クラス
    ///一部オーバーロード用の数値クラスが混じっているので注意
    /// </summary>
    protected static class MathV {
        /// <summary>
        ///大きい方のベクトルを取得
        /// </summary>
        public static Vector2 max(Vector2 main, Vector2 sub) {
            return main.magnitude >= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最小値を設定
        /// </summary>
        public static Vector2 max(Vector2 main, float limit) {
            return max(main, main.normalized * limit);
        }
        /// <summary>
        ///小さい方のベクトルを取得
        /// </summary>
        public static Vector2 min(Vector2 main, Vector2 sub) {
            return main.magnitude <= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最大値を設定
        /// </summary>
        public static Vector2 min(Vector2 main, float limit) {
            return min(main, main.normalized * limit);
        }
        /// <summary>
        ///各要素の絶対値を取ったベクトルを取得
        /// </summary>
        public static Vector2 abs(Vector2 main) {
            return Vector2.right * Mathf.Abs(main.x) + Vector2.up * Mathf.Abs(main.y);
        }
        /// <summary>
        ///mainのベクトルをsubに合わせて補正する
        /// </summary>
        public static Vector2 correct(Vector2 main, Vector2 sub, float degree = 0.5f) {
            return main * degree + sub * (1 - degree);
        }
        /// <summary>
        ///mainの数値をsubに合わせて補正する
        /// </summary>
        public static float correct(float main, float sub, float degree = 0.5f) {
            return main * degree + sub * (1 - degree);
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
        /// </summary>
        public static Vector2 scaling(Vector2 main, Vector2 scale) {
            return Vector2.right * main.x * scale.x + Vector2.up * main.y * scale.y;
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
        /// </summary>
        public static Vector2 scaling(Vector2 main, float scaleX, float scaleY) {
            return scaling(main, new Vector2(scaleX, scaleY));
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
        /// </summary>
        public static Vector2 rescaling(Vector2 main, Vector2 scale) {
            return Vector2.right * main.x / scale.x + Vector2.up * main.y / scale.y;
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
        /// </summary>
        public static Vector2 rescaling(Vector2 main, float scaleX, float scaleY) {
            return rescaling(main, new Vector2(scaleX, scaleY));
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Vector2 direction, float length) {
            return direction.normalized * length;
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Vector2 direction, Vector2 length) {
            return recalculation(direction, length.magnitude);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Quaternion direction, float length) {
            return recalculation(direction * Vector2.right, length);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(float direction, float length) {
            return recalculation(MathA.toRotation(direction), length);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(float direction, Vector2 length) {
            return recalculation(MathA.toRotation(direction), length.magnitude);
        }
        /// <summary>
        ///ベクトルを指定枠内に収まる値に補正
        /// </summary>
        public static Vector2 within(Vector2 main, Vector2 lowerLeft, Vector2 upperRight) {
            Vector2 returnVector = main;
            returnVector.x = Mathf.Clamp(returnVector.x, lowerLeft.x, upperRight.x);
            returnVector.y = Mathf.Clamp(returnVector.y, lowerLeft.y, upperRight.y);
            return returnVector;
        }
    }

    /// <summary>
    ///角度系の汎用計算クラス
    /// </summary>
    protected static class MathA {
        /// <summary>
        ///鋭角の大きい方の角度を取得
        /// </summary>
        public static float max(float main, float sub) {
            return acute(main) >= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の小さい方の角度を取得
        /// </summary>
        public static float min(float main, float sub) {
            return acute(main) <= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の取得
        /// </summary>
        public static float acute(float angle) {
            return Mathf.Min(compile(angle), 360 - compile(angle));
        }
        /// <summary>
        ///角度補正関数
        ///主にイージングとか
        /// </summary>
        public static float correct(float main, float sub, float degree = 0.5f) {
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
        public static float compile(float angle) {
            while(angle < 0)
                angle += 360;
            while(angle >= 360)
                angle -= 360;
            return angle;
        }
        /// <summary>
        ///ベクトルを角度化
        /// </summary>
        public static float toAngle(Vector2 targetVector) {
            return Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
        }
        /// <summary>
        ///クォータニオンを角度化
        /// </summary>
        public static float toAngle(Quaternion targetRotation) {
            return toAngle(targetRotation * Vector2.right);
        }
        /// <summary>
        ///角度をクォータニオン化
        /// </summary>
        public static Quaternion toRotation(float targetAngle) {
            var returnRotation = new Quaternion() {
                eulerAngles = new Vector3(0, 0, targetAngle)
            };
            return returnRotation;
        }
        /// <summary>
        ///ベクトルをクォータニオン化
        /// </summary>
        public static Quaternion toRotation(Vector2 targetVector) {
            return Quaternion.AngleAxis(toAngle(targetVector), Vector3.forward);
        }
    }
}
