using UnityEngine;

public partial class Ship : Things
{
    /// <summary>
    /// パーツパラメータベースクラス
    /// </summary>
    public class PartsState : ICopyAble<PartsState>
    {
        public Vector2 rootPosition = Vector2.zero;
        public float positionZ = 1;

        public int partsNum { get; set; }
        public PartsState myself
        {
            get {
                return new PartsState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum
                };
            }
        }
    }
}
