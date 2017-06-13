using UnityEngine;

public partial class Ship : Things
{
    /// <summary>
    /// 腕部パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class ArmState : PartsState, ICopyAble<ArmState>, System.IEquatable<ArmState>
    {
        public bool positive = true;
        public Arm entity = null;

        public Vector2 tipPosition { get; set; }

        public Vector2 siteTweak { get; set; }

        public new ArmState myself
        {
            get {
                return new ArmState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    positive = positive,
                    entity = entity,
                    tipPosition = tipPosition,
                    siteTweak = siteTweak
                };
            }
        }

        public bool Equals(ArmState other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;

            if(positive != other.positive) return false;
            if(entity != other.entity) return false;
            if(tipPosition != other.tipPosition) return false;
            if(siteTweak != other.siteTweak) return false;

            return true;
        }
    }
}
