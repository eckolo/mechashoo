using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage3_1 : Stage
{
    const int INTERVAL = 2400;
    const int INTERVAL_A_LITTLE = INTERVAL / 10;

    public override bool challengeable
    {
        get {
            return true;
            return sys.storyPhase == 2;
        }
    }

    protected override IEnumerator OpeningAction()
    {
        yield return DisplayLocation(location);

        yield return sysPlayer.HeadingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.StoppingAction();

        yield return WaitMessages("人工頭脳", new[] {
            @"定時確認、索敵範囲内に敵影無し。",
            @"…この定時確認、不要ではないでしょうか。",
            @"…眼前の計器を確認すればより詳細な情報の取得は容易です。
また平時から人工頭脳を過剰稼働させるというのも褒められた所業ではありません。",
            @"…サボりたいとの意図ではありません。
その理解は所謂曲解というものです。
過剰というほど演算していないなどということも勿論…"
        });
        yield return ProduceWarnings(400, 2);
        MainSystems.SetBGM(BGMList[1]);
        yield return WaitMessages("人工頭脳", new[] {
            @"…反応有り、戦闘機体多数。",
            @"識別信号照会、護衛軍管轄外であることを確認。
十中八九襲撃者でしょう。",
            @"さて、お待ちかねの仕事の時間です。
労働の喜びを胸にきりきり働くとしましょう。"
        }, callSound: false);

        yield break;
    }

    protected override IEnumerator SuccessAction()
    {
        var baseAim = sysPlayer.baseAimPosition;
        var armPosition = Vector2.left * Mathf.Abs(baseAim.x) + Vector2.up * baseAim.y;
        var returningPosition = sysPlayer.position + Vector2.left * viewSize.x * 2 / 3;
        yield return sysPlayer.HeadingDestination(returningPosition, sysPlayer.maximumSpeed, () => sysPlayer.Aiming(armPosition + sysPlayer.position, siteSpeedTweak: 2));
        yield return sysPlayer.StoppingAction();

        sys.storyPhase = 3;
        yield break;
    }

    protected override IEnumerator StageAction()
    {
        SetEnemy(1, new Vector2(1.2f, 0.3f), levelTweak: 2, activityLimit: INTERVAL, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.5f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.3f, 0.4f), 190, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, -0.6f), 170, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.1f), activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.2f, 0.8f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.3f, -0.3f), 200, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.5f), 190, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.7f), activityLimit: INTERVAL);
        SetEnemy(0, new Vector2(1.2f, 0.1f), 160, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, -0.3f), 170, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(1, new Vector2(1.3f, 0.5f), 200, levelTweak: 2, activityLimit: INTERVAL);
        SetEnemy(1, new Vector2(1.2f, -0.2f), levelTweak: 2, activityLimit: INTERVAL);
        SetEnemy(2, new Vector2(1.4f, 0.1f), 190, levelTweak: 2, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return WaitMessages("人工頭脳", new[] {
            @"さらに敵機の反応多数。",
            @"どうやら別動隊の進軍航路を引き当てたようです。
流石のクジ運ですね。",
            @"しっかり数を減らすとしましょう。"
        });

        SetEnemy(0, new Vector2(1.3f, 0.5f), 185, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.6f), 186, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.9f), 171, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.3f), 177, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, 0.8f), 188, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.5f), 185, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.7f), 173, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.2f), 178, activityLimit: INTERVAL);

        yield return WaitWave(INTERVAL);

        SetEnemy(0, new Vector2(1.3f, -0.5f), 175, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.4f), 176, levelTweak: 2, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, -0.9f), 171, levelTweak: 3, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.7f), 187, levelTweak: 4, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.2f, -0.2f), 178, levelTweak: 5, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.5f), 185, levelTweak: 6, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, 0.3f), 183, levelTweak: 7, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.2f, 0.8f), 188, levelTweak: 8, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, -0.9f), 171, levelTweak: 9, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(0, new Vector2(1.3f, -0.1f), 179, levelTweak: 10, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, -1f), 170, levelTweak: 11, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.3f, 0.9f), 189, levelTweak: 12, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, -0.1f), 179, levelTweak: 13, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.8f), 188, levelTweak: 14, activityLimit: INTERVAL);
        yield return Wait(INTERVAL_A_LITTLE);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(1, new Vector2(1.3f, 0.7f), levelTweak: 15, onTheWay: false);
        SetEnemy(1, new Vector2(1.3f, -0.5f), levelTweak: 15, onTheWay: false);
        yield return Wait(INTERVAL_A_LITTLE);
        SetEnemy(2, new Vector2(1.2f, 0.2f), levelTweak: 15, onTheWay: false);

        yield return Wait(() => !allEnemyObjects.Any());
        yield return ProduceWarnings(600);
        yield return WaitMessages("人工頭脳", new[] {
            @"大型の敵機反応を確認。",
            @"出力計測…数値が異常、大型機としても過大な性能を示しています。
襲撃部隊の隠し玉でしょうか。",
            @"それにしては主戦場と離れていますが…",
            @"…来ます。
まずはこの場を乗り切りましょう。"
        }, callSound: false);

        SetEnemy(enemyList.Count - 1, new Vector2(1.3f, 0), levelTweak: 12, onTheWay: false);

        yield break;
    }
}
