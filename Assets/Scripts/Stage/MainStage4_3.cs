using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 対共和国
/// </summary>
public class MainStage4_3 : MainStage4Origin
{
    [SerializeField]
    List<MobileBattery> batteryList = new List<MobileBattery>();

    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            if(sys.dominance.oldKingdom < 0) return false;
            return base.challengeable;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return WaitMessages("人工頭脳", new[] {
            @"…先ほど司令部より電信がありました。",
            @"共和国軍が国境付近の転移中継地点を通過したとのこと。
国境防衛部隊は別方面に出ていたため、ほぼ1大隊が素通りしたようです。
恐らく共和国側の囮にかかりましたね。",
            @"それにしても思ったより大規模な戦闘となりそうです。"
        });
        yield return Wait(INTERVAL_A_LITTLE);
        yield return ProduceCaution(400, 2);
        yield return WaitMessages("人工頭脳", new[] {
            @"…来ました。
落ちない程度に稼ぐとしましょう。"
        }, callSound: false);
        MainSystems.SetBGM(BGMList[1]);
        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.2f, -0.4f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.4f), activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(1.3f, 0), levelTweak: 5, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.2f, -0.8f), 160, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.2f, 0.8f), -160, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.4f), 170, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.3f, 0.4f), -170, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.3f), 170, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(1, new Vector2(1.2f, 0.3f), -170, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.6f), 160, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.2f, 0.6f), -160, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.2f, 0.7f), -165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(1.3f, 0.3f), -165, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.7f), 165, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(1.3f, -0.3f), 165, levelTweak: 5, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"転移反応、来ます。
状況から見て共和国軍の援軍でしょう。",
            @"…反応が妙です。
機体ではありません。",
            @"これは…共和国の移動砲台ですね。
長距離射撃が来ます、注意してください。"
        });

        var battery = batteryList.FirstOrDefault();

        SetEnemy(0, new Vector2(1.3f, -0.4f), 170, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, -0.75f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.8f, 1.2f), -120);
        SetEnemy(0, new Vector2(1.2f, 0.75f), 190, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, 0.4f), 190, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.8f, -1.2f), 120);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0), levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.5f, -1.2f), 120);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.5f, 1.2f), 120);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.7f, 1.2f), 135);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.7f, -1.2f), 135);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.2f, 0.7f), -165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, -0.1f), -165, activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, -0.3f), -165, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.8f), 170);
        SetMobileBattery(battery, new Vector2(1.2f, 0.6f), -175);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, 0.2f), -180);
        SetMobileBattery(battery, new Vector2(1.2f, -0.2f), 180);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.6f), 175);
        SetMobileBattery(battery, new Vector2(1.2f, 0.8f), -170);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, 0.8f), -170);
        SetMobileBattery(battery, new Vector2(1.2f, -0.6f), 175);
        SetEnemy(0, new Vector2(1.2f, -0.7f), 165, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, 0.1f), 165, activityLimit: INTERVAL);
        SetEnemy(3, new Vector2(1.3f, 0.3f), 165, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.2f), 180);
        SetMobileBattery(battery, new Vector2(1.2f, 0.2f), -180);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, 0.6f), -175);
        SetMobileBattery(battery, new Vector2(1.2f, -0.8f), 170);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.8f), 170);
        SetMobileBattery(battery, new Vector2(1.2f, 0.6f), -175);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.2f), 180);
        SetMobileBattery(battery, new Vector2(1.2f, 0.2f), -170);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1.2f, -0.2f), 175);
        SetMobileBattery(battery, new Vector2(1.2f, 0.2f), -180);

        yield return WaitWave(INTERVAL);

        SetEnemy(2, new Vector2(1.2f, 0.5f), -175, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0f, -1.3f), 95);
        SetMobileBattery(battery, new Vector2(0f, 1.3f), -85);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.4f), -176, levelTweak: 2, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(-0.2f, -1.3f), 94);
        SetMobileBattery(battery, new Vector2(0.2f, 1.3f), -86);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0.9f), -171, levelTweak: 3, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.8f, -1.3f), 99);
        SetMobileBattery(battery, new Vector2(-0.8f, 1.3f), -81);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.7f), -187, levelTweak: 4, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.4f, 1.3f), -83);
        SetMobileBattery(battery, new Vector2(-0.4f, -1.3f), 97);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.2f), -178, levelTweak: 5, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(-0.6f, -1.3f), 92);
        SetMobileBattery(battery, new Vector2(0.6f, 1.3f), -88);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, -0.5f), -185, levelTweak: 6, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0f, 1.3f), -85);
        SetMobileBattery(battery, new Vector2(0f, -1.3f), 95);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.3f), -183, levelTweak: 7, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(-0.4f, 1.3f), -87);
        SetMobileBattery(battery, new Vector2(0.4f, -1.3f), 93);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.8f), -188, levelTweak: 8, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.6f, 1.3f), -82);
        SetMobileBattery(battery, new Vector2(-0.6f, -1.3f), 98);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0.9f), -171, levelTweak: 9, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.8f, -1.3f), 99);
        SetMobileBattery(battery, new Vector2(-0.8f, 1.3f), -81);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.1f), -179, levelTweak: 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(-0.8f, -1.3f), 91);
        SetMobileBattery(battery, new Vector2(0.8f, 1.3f), -89);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 1f), -170, levelTweak: 11, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(1f, -1.3f), 100);
        SetMobileBattery(battery, new Vector2(-1f, 1.3f), -80);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, -0.9f), -189, levelTweak: 12, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.8f, 1.3f), -81);
        SetMobileBattery(battery, new Vector2(-0.8f, -1.3f), 99);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.1f), -179, levelTweak: 13, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(-0.8f, -1.3f), 91);
        SetMobileBattery(battery, new Vector2(0.8f, 1.3f), -89);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -0.8f), -188, levelTweak: 14, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetMobileBattery(battery, new Vector2(0.6f, 1.3f), -82);
        SetMobileBattery(battery, new Vector2(-0.6f, -1.3f), 98);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(3, new Vector2(1.2f, 0.3f), levelTweak: 15, onTheWay: false);
        SetEnemy(3, new Vector2(1.2f, -0.3f), levelTweak: 15, onTheWay: false);
        SetEnemy(3, new Vector2(1.2f, 0f), levelTweak: 15, onTheWay: false);
        while(allEnemiesInField.Any())
        {
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(0.8f, 1.2f), -135);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(-0.8f, -1.2f), 45);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(1.2f, 0.5f), 180);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(-1.2f, -0.5f), 0);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(0.8f, -1.2f), 135);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(-0.8f, 1.2f), -45);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(1.2f, -0.5f), 180);
            yield return Wait(INTERVAL_A_LITTLE);
            if(!allEnemiesInField.Any()) continue;
            SetMobileBattery(battery, new Vector2(-1.2f, 0.5f), 0);
        }

        yield return Wait(() => !allEnemyObjects.Any());
        sysPlayer.canRecieveKey = false;
        yield return WaitMessages("人工頭脳", new[] {
            @"司令部より電信。",
            @"移動砲台の全機撃墜に成功したとのこと。
これで少しは落ち着き…"
        });
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"…大型機の反応、1。",
            @"動きからして作戦行動ではありませんね。
砲台を落とされて焦った、とでもいうところでしょうか。",
            @"敵軍の感傷に付き合う必要はありません。
手早く落として友軍に合流しましょう。"
        }, callSound: false);
        sysPlayer.canRecieveKey = true;

        SetEnemy(enemyList.Count - 1, new Vector2(1.2f, 0.8f), -170, levelTweak: 12, onTheWay: false);
        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"敵大型機の撃墜を確認。",
            @"…ついでに敵全軍の撤退も確認。
虎の子の移動砲台が落ちたためでしょう。",
            @"自軍が撤退したのなら大人しく下がって欲しかったところですが。",
            @"ひとまずこちらも帰投しましょう。
お疲れ様でした。"
        });

        var returningX = viewPosition.x - viewSize.x;
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = new Vector2(returningX, sysPlayer.position.y);
        yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
        yield return sysPlayer.StoppingAction();

        yield return WaitMessages("人工頭脳", new[] {
            @"…そういえばあの移動砲台、どこから転移してきたのでしょうか。
古王国が転移可能範囲内であんな大きい構造物を見逃すとも思えないのですが…"
        });

        if(!isCleared && sys.storyPhase < Configs.StoryPhase.END_FOUR_PRACTICE)
        {
            sys.dominance.oldKingdom++;
            sys.dominance.republic--;
        }
        yield return base.SuccessAction();
        yield break;
    }
}
