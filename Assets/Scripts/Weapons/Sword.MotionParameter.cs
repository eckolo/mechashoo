using System;

public partial class Sword : Weapon
{
    [Serializable]
    protected class MotionParameter
    {
        public AttackType type = AttackType.SINGLE;
        public bool forward = true;
        public float power = 1;
        public float size = 1;
        public float density = 1;
        public float timeTweak = 1;
        public float fuelCostTweak = 1;
        public int turnoverRate = 0;
    }
}
