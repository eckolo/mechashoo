public abstract partial class Stage : Methods
{
    /// <summary>
    /// 報酬取得条件一覧
    /// </summary>
    protected enum RewardTermType
    {
        ENEMY_PLANE_WIPE_OUT,
        NO_DAMAGE,
        NO_SHOT,
        FULL_MISSILE
    }

    /// <summary>
    /// 報酬条件タイプから報酬条件への変換
    /// </summary>
    /// <param name="termType">変換元報酬条件タイプ</param>
    /// <returns>報酬条件</returns>
    static RewardTerm GetTerm(RewardTermType termType)
    {
        switch(termType)
        {
            case RewardTermType.ENEMY_PLANE_WIPE_OUT:
                return new RewardTerm
                {
                    term = () => sys.shotDownRate >= 1,
                    explanation = "全敵機の撃墜"
                };
            case RewardTermType.NO_DAMAGE:
                return new RewardTerm
                {
                    term = () => sys.protectionRate == null || sys.protectionRate >= 1,
                    explanation = "装甲への損害無し"
                };
            case RewardTermType.NO_SHOT:
                return new RewardTerm
                {
                    term = () => sys.evasionRate >= 1,
                    explanation = "全敵弾を回避"
                };
            case RewardTermType.FULL_MISSILE:
                return new RewardTerm
                {
                    term = () => sys.accuracy >= 1,
                    explanation = "攻撃命中回数が弾丸射出回数以上"
                };
            default: return new RewardTerm();
        }
    }
}
