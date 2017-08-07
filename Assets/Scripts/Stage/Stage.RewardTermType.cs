using System.Linq;

public abstract partial class Stage : Methods
{
    /// <summary>
    /// 報酬取得条件一覧
    /// </summary>
    public enum RewardTermType
    {
        UNCONDITIONAL = 0000,
        MOST_SHOOTING_DOWN = 0100,
        HIDDEN_ASSASSINATION = 0101,
        ENEMY_PLANE_WIPE_OUT = 0102,
        SNEAKING = 0110,
        ALMOST_NON_KILLING = 0111,
        ALMOST_AVOIDED = 0200,
        NO_SHOT = 0202,
        NO_DAMAGE = 0210,
        HALF_HIT = 0300,
        FULL_MISSILE = 0301,
        SINGLE_EQUIPMENT = 0400,
        PARTIAL_DESTRUCTION = 0500,
        VERGE_OF_DEATH = 0501
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
                    explanation = "無事の帰還"
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
                    explanation = "ほぼ最低限の敵機のみ撃墜"
                };
            case RewardTermType.ALMOST_AVOIDED:
                return new RewardTerm
                {
                    term = () => sys.evasionRate > 0.5f,
                    explanation = "大半の敵弾を回避"
                };
            case RewardTermType.NO_SHOT:
                return new RewardTerm
                {
                    term = () => sys.evasionRate >= 1,
                    explanation = "全敵弾を回避"
                };
            case RewardTermType.NO_DAMAGE:
                return new RewardTerm
                {
                    term = () => sys.protectionRate == null || sys.protectionRate >= 1,
                    explanation = "装甲への損害無し"
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
