using UnityEngine;

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

        /// <summary>
        ///ベクトルイージング関数群
        /// </summary>
        public static class Easing {
            /// <summary>
            ///始点から終点まで円軌道を描く
            /// </summary>
            public static Vector2 within(Vector2 main, Vector2 lowerLeft, Vector2 upperRight) {
                Vector2 returnVector = main;
                returnVector.x = Mathf.Clamp(returnVector.x, lowerLeft.x, upperRight.x);
                returnVector.y = Mathf.Clamp(returnVector.y, lowerLeft.y, upperRight.y);
                return returnVector;
            }
        }
    }
}
