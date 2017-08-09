using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage1 : Stage
{
    public override bool challengeable
    {
        get {
            if(sys.storyPhase >= Configs.StoryPhase.GAME_CLEAR) return true;
            return sys.storyPhase >= 1;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();
        yield return Wait(Configs.Window.DEFAULT_MOTION_TIME);

        yield return WaitMessages("人工頭脳", new[] {
            @"…お久しぶりです。",
            @"傭兵業に復帰した、との情報は認識しています。
また共に頑張りましょう。",
            @"さて、今回の目標は民間の客船です。",
            @"ただし我々は直接略奪に参加しません。
あくまで客船を護衛する民間警備機関の迎撃が主任務です。",
            @"…警備機関の小型機、接近してきました。
民間とはいえしっかり武装した相手です。
お気をつけて。"
        });

        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, -0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.5f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.5f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, -0.6f), 175, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), -175, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.3f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.3f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.6f), -170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), -170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), -170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), -170, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.6f), 170, levelTweak: 5, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"こちらの受け持ち分の掃討、完了しました。",
            @"他の部隊の援護にまわりましょう。"
        });
        yield return ProduceCaution(400, 2);
        yield return WaitMessages("人工頭脳", new[] {
            @"…新たな機体反応有り。",
            @"これは…央星帝国軍の巡回警邏部隊です。
どうやら臨時哨戒にぶつかったようですね。"
        }, callSound: false);
        yield return WaitMessages("人工頭脳", new[] {
            @"…本部より電信。",
            @"強奪班の緊急離脱完了まで軍を足止めせよ、とのことです。",
            @"また、別途互助会長より伝言を預かりました。"
        });
        yield return WaitMessages("傭兵互助会会長", new[] {
            @"『今回出てるのはひよっこばかりであてにならん。お前さんが頼りだ、頼んだぞ』"
        });
        yield return WaitMessages("人工頭脳", new[] {
            @"以上です。",
            @"…敵機、来ました。
応戦してください。"
        });

        SetEnemy(1, new Vector2(1.2f, 0), activityLimit: INTERVAL, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.3f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, -0.3f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.3f), -170, activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, -0.3f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.9f), -150, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.3f, -0.9f), 150, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.2f, 0.6f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.6f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.6f), activityLimit: INTERVAL, onTheWay: false);
        SetEnemy(0, new Vector2(1.2f, -0.6f), activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0), levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.8f), -170, levelTweak: 5, onTheWay: false);
        SetEnemy(1, new Vector2(1.2f, -0.8f), 170, levelTweak: 5, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0), levelTweak: 15, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"警邏部隊の一部が進路を変更、こちらへ接近しています。
動きから推測するに指揮官機と随伴部隊のようです。",
            @"指揮官を撃退すれば警邏部隊も撤退を選ぶでしょう。
多少戦力差はありますが、よろしくお願いします。"
        });

        SetEnemy(2, new Vector2(1.2f, 0.6f), levelTweak: 15, onTheWay: false);
        SetEnemy(2, new Vector2(1.2f, -0.6f), levelTweak: 15, onTheWay: false);
        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, 0), levelTweak: 36, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.3f), levelTweak: 15, onTheWay: false);
        SetEnemy(2, new Vector2(1.3f, -0.3f), levelTweak: 15, onTheWay: false);
        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        yield return WaitMessages("人工頭脳", new[] {
            @"…敵指揮官機および随伴部隊の全機撃墜を確認。
警邏部隊も全隊引き上げていきます。",
            @"つい先ほど客船の方へ警邏隊の一部が取り付き救助活動を開始したそうです。
指揮官も落としましたし、これ以上追撃が来ることも無いでしょう。",
            @"お疲れ様でした。"
        });

        var returningX = viewPosition.x - viewSize.x;
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = new Vector2(returningX, sysPlayer.position.y + 1);
        yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
        yield return sysPlayer.StoppingAction();

        sys.storyPhase = 2;
        yield break;
    }
}
