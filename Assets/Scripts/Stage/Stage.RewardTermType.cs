using System.Linq;

public abstract partial class Stage : Methods
{
    /// <summary>
    /// 報酬取得条件一覧
    /// </summary>
    protected enum RewardTermType
    {
        UNCONDITIONAL,
        MOST_SHOOTING_DOWN,
        HIDDEN_ASSASSINATION,
        ENEMY_PLANE_WIPE_OUT,
        SNEAKING,
        ALMOST_NON_KILLING,
        NO_DAMAGE,
        NO_SHOT,
        HALF_HIT,
        FULL_MISSILE,
        SINGLE_EQUIPMENT,
        PARTIAL_DESTRUCTION,
        VERGE_OF_DEATH
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
            case RewardTermType.UNCONDITIONAL:
                return new RewardTerm
                {
                    term = () => true,
                    explanation = "依頼の達成"
                };
            case RewardTermType.MOST_SHOOTING_DOWN:
                return new RewardTerm
                {
                    term = () => sys.shotDownRate >= 0.8f,
                    explanation = "敵機の大半を撃墜"
                };
            case RewardTermType.HIDDEN_ASSASSINATION:
                return new RewardTerm
                {
                    term = () => sys.nowStage.shotsToKill >= sys.nowStage.opposeEnemy,
                    explanation = "感知された敵機を全て撃墜"
                };
            case RewardTermType.ENEMY_PLANE_WIPE_OUT:
                return new RewardTerm
                {
                    term = () => sys.shotDownRate >= 1,
                    explanation = "全敵機の撃墜"
                };
            case RewardTermType.SNEAKING:
                return new RewardTerm
                {
                    term = () => sys.opposeEnemyRate < 0.5f,
                    explanation = "敵機の半数に感知されず"
                };
            case RewardTermType.ALMOST_NON_KILLING:
                return new RewardTerm
                {
                    term = () => sys.nowStage.shotsToKill <= sys.nowStage.minimumShotDown,
                    explanation = "最低限の敵機のみ撃墜"
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
            case RewardTermType.HALF_HIT:
                return new RewardTerm
                {
                    term = () => sys.accuracy >= 0.5f,
                    explanation = "命中率が0.5以上"
                };
            case RewardTermType.FULL_MISSILE:
                return new RewardTerm
                {
                    term = () => sys.accuracy >= 1,
                    explanation = "命中率が1以上"
                };
            case RewardTermType.SINGLE_EQUIPMENT:
                return new RewardTerm
                {
                    term = () => sysPlayer.coreData.weapons.Where(weapon => weapon != null).Count() <= 1,
                    explanation = "装備数1での依頼達成"
                };
            case RewardTermType.PARTIAL_DESTRUCTION:
                return new RewardTerm
                {
                    term = () => sysPlayer.palamates.nowArmor <= sysPlayer.maxArmor / 2,
                    explanation = "装甲残量半分以下"
                };
            case RewardTermType.VERGE_OF_DEATH:
                return new RewardTerm
                {
                    term = () => sysPlayer.palamates.nowArmor <= sysPlayer.maxArmor * 0.2f,
                    explanation = "装甲残量2割以下"
                };
            default: return new RewardTerm();
        }
    }
}
