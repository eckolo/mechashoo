using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Methods : MonoBehaviour
{
    /// <summary>
    ///イージング関数群
    /// </summary>
    protected static Easing easing = new Easing();

    protected delegate bool Terms(Material target);
    protected delegate float Rank(Material target);

    /// <summary>
    ///ボタン1
    /// </summary>
    protected static KeyCode ButtomZ = KeyCode.Z;
    /// <summary>
    ///ボタン2
    /// </summary>
    protected static KeyCode ButtomX = KeyCode.X;
    /// <summary>
    ///ボタン3
    /// </summary>
    protected static KeyCode ButtomC = KeyCode.C;
    /// <summary>
    ///ボタン4
    /// </summary>
    protected static KeyCode ButtomA = KeyCode.A;
    /// <summary>
    ///ボタン5
    /// </summary>
    protected static KeyCode ButtomS = KeyCode.S;
    /// <summary>
    ///ボタン6
    /// </summary>
    protected static KeyCode ButtomD = KeyCode.D;
    /// <summary>
    ///サブボタン
    /// </summary>
    protected static KeyCode ButtomSub = KeyCode.LeftShift;
    /// <summary>
    ///↑ボタン
    /// </summary>
    protected static KeyCode ButtomUp = KeyCode.UpArrow;
    /// <summary>
    ///↓ボタン
    /// </summary>
    protected static KeyCode ButtomDown = KeyCode.DownArrow;
    /// <summary>
    ///←ボタン
    /// </summary>
    protected static KeyCode ButtomLeft = KeyCode.LeftArrow;
    /// <summary>
    ///→ボタン
    /// </summary>
    protected static KeyCode ButtomRight = KeyCode.RightArrow;
    /// <summary>
    ///水平方向のキー入力
    /// </summary>
    protected static string ButtomNameWidth = "Horizontal";
    /// <summary>
    ///垂直方向のキー入力
    /// </summary>
    protected static string ButtomNameHeight = "Vertical";

    /// <summary>
    ///BGM音量
    /// </summary>
    protected static float volumeBGM = 50;
    protected const float baseVolumeBGM = 0.003f;
    /// <summary>
    ///SE音量
    /// </summary>
    protected static float volumeSE = 50;
    protected const float baseVolumeSE = 0.001f;

    protected const float maxVolume = 100;
    protected const float minVolume = 0;

    /// <summary>
    ///フィールドサイズ
    /// </summary>
    protected static Vector2 fieldSize
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(new Vector2(2, 2)) - Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        }
    }
    /// <summary>
    ///フィールド左下端
    /// </summary>
    protected static Vector2 fieldLowerLeft
    {
        get
        {
            return -fieldSize / 2;
        }
    }
    /// <summary>
    ///フィールド右上端
    /// </summary>
    protected static Vector2 fieldUpperRight
    {
        get
        {
            return fieldSize / 2;
        }
    }
    /// <summary>
    ///フィールド視野サイズ
    /// </summary>
    protected static Vector2 viewSize
    {
        get
        {
            return Camera.main.ViewportToWorldPoint(Vector2.one) - Camera.main.ViewportToWorldPoint(Vector2.zero);
        }
    }
    /// <summary>
    ///フィールド視点位置
    /// </summary>
    protected static Vector2 viewPosition
    {
        get
        {
            return Camera.main.transform.localPosition;
        }
        set
        {
            var edge = (fieldSize - viewSize) / 2;
            Vector3 setPosition = MathV.within(value, -edge, edge);
            Camera.main.transform.localPosition = setPosition;
            sysView.transform.localPosition = setPosition;
        }
    }
    /// <summary>
    ///ピクセル単位のキャンバスサイズ
    /// </summary>
    protected static Vector2 screenSize
    {
        get
        {
            return sysCanvas.GetComponent<CanvasScaler>().referenceResolution;
        }
    }
    /// <summary>
    ///1マス当たりのピクセルサイズ
    /// </summary>
    protected static Vector2 baseMas
    {
        get
        {
            return MathV.rescaling(screenSize, viewSize);
        }
    }

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    static protected int defaultTextSize = 18;

    /// <summary>
    ///メインシステム記憶キャッシュ
    /// </summary>
    private static MainSystems systemRoot = null;
    /// <summary>
    ///メインシステムオブジェクト取得関数
    /// </summary>
    static protected MainSystems Sys
    {
        get
        {
            return systemRoot = systemRoot ?? GameObject.Find("SystemRoot").GetComponent<MainSystems>();
        }
    }

    /// <summary>
    ///プレイヤー記憶キャッシュ
    /// </summary>
    private static Player player = null;
    /// <summary>
    ///プレイヤーオブジェクト取得関数
    /// </summary>
    static protected Player sysPlayer
    {
        get
        {
            if (player == null)
            {
                visualizePlayer();
                transparentPlayer();
            }
            return player;
        }
    }
    /// <summary>
    ///プレイヤーオブジェクト設置関数
    /// </summary>
    static protected Player visualizePlayer()
    {
        if (player == null)
        {
            player = Instantiate(Sys.initialPlayer);
            player.transform.SetParent(sysPanel.transform);
            player.setCoreStatus(new Ship.CoreData());
        }
        player.gameObject.SetActive(true);
        return player;
    }
    /// <summary>
    ///プレイヤーオブジェクト消去（透明化）関数
    /// </summary>
    static protected Player transparentPlayer()
    {
        player.gameObject.SetActive(false);
        return player;
    }

    /// <summary>
    ///パネルオブジェクト名
    /// </summary>
    protected static string panelName = "Panel";
    /// <summary>
    ///パネル記憶キャッシュ
    /// </summary>
    private static Panel nowPanel = null;
    /// <summary>
    ///パネルオブジェクト取得関数
    /// </summary>
    static protected Panel sysPanel
    {
        get
        {
            if (nowPanel != null) return nowPanel;

            nowPanel = GameObject.Find(panelName) != null
                ? GameObject.Find(panelName).GetComponent<Panel>()
                : null;
            if (nowPanel != null) return nowPanel;

            nowPanel = Instantiate(Sys.basicPanel);
            nowPanel.name = panelName;
            return nowPanel;
        }
    }

    /// <summary>
    ///ビューオブジェクト名
    /// </summary>
    protected static string ViewName = "View";
    /// <summary>
    ///ビュー記憶キャッシュ
    /// </summary>
    private static Panel nowView = null;
    /// <summary>
    ///ビューオブジェクト取得関数
    /// </summary>
    static protected Panel sysView
    {
        get
        {
            if (nowView != null) return nowView;

            nowView = GameObject.Find(ViewName) != null
                ? GameObject.Find(ViewName).GetComponent<Panel>()
                : null;
            if (nowView != null) return nowView;

            nowView = Instantiate(Sys.basicPanel);
            nowView.name = ViewName;
            return nowView;
        }
    }

    /// <summary>
    ///キャンバスオブジェクト名
    /// </summary>
    protected static string canvasName = "Canvas";
    /// <summary>
    ///キャンバス記憶キャッシュ
    /// </summary>
    private static Canvas nowCanvas = null;
    /// <summary>
    ///キャンバスオブジェクト取得関数
    /// </summary>
    static protected Canvas sysCanvas
    {
        get
        {
            if (nowCanvas != null) return nowCanvas;

            nowCanvas = GameObject.Find(canvasName) != null
                ? GameObject.Find(canvasName).GetComponent<Canvas>()
                : null;
            if (nowCanvas != null) return nowCanvas;

            nowCanvas = Instantiate(Sys.basicCanvas);
            nowCanvas.name = canvasName;
            return nowCanvas;
        }
    }

    /// <summary>
    ///Bar取得関数
    /// </summary>
    protected Bar getBar(barType barName, Color? setColor = null)
    {
        Bar barObject = GameObject.Find(barName.ToString()) != null
            ? GameObject.Find(barName.ToString()).GetComponent<Bar>()
            : null;
        if (barObject != null) return barObject;

        barObject = Instantiate(Sys.basicBar);
        barObject.transform.parent = sysView.transform;
        barObject.name = barName.ToString();
        barObject.GetComponent<SpriteRenderer>().color = setColor ?? Color.red;
        return barObject;
    }
    protected enum barType
    {
        HPbar, BRbar, ENbar
    }

    /// <summary>
    ///オブジェクト検索関数
    /// </summary>
    protected static List<Material> getAllObject(Terms map = null)
    {
        var returnList = new List<Material>();
        foreach (Material value in FindObjectsOfType(typeof(Material)))
        {
            if (map == null || map(value)) returnList.Add(value);
        }
        return returnList;
    }
    /// <summary>
    ///最大値条件型オブジェクト検索関数
    /// </summary>
    protected static List<Material> searchMaxObject(Rank refine, Terms map = null)
    {
        List<Material> returnList = new List<Material>();
        foreach (var value in getAllObject(map))
        {
            if (returnList.Count <= 0)
            {
                returnList.Add(value);
            }
            else if (refine(value) > refine(returnList[0]))
            {
                returnList = new List<Material> { value };
            }
            else if (refine(value) == refine(returnList[0]))
            {
                returnList.Add(value);
            }
        }

        return returnList;
    }
    /// <summary>
    ///最寄りオブジェクト検索関数
    /// </summary>
    protected List<Material> getNearObject(Terms map = null)
    {
        return searchMaxObject(target => -(target.transform.position - transform.position).magnitude, map);
    }

    /// <summary>
    ///回転計算関数
    /// </summary>
    protected static Quaternion getRotation(Quaternion baseRotation, float calculation)
    {
        Vector3 axis = new Vector3(baseRotation.x, baseRotation.y, baseRotation.z).normalized;
        return new Quaternion(axis.x, axis.y, axis.z, baseRotation.w * calculation);
    }
    /// <summary>
    ///逆回転生成関数
    /// </summary>
    protected static Quaternion getReverse(Quaternion baseRotation)
    {
        return getRotation(baseRotation, -1);
    }

    /// <summary>
    ///ベクトル関係の汎用計算クラス
    ///一部オーバーロード用の数値クラスが混じっているので注意
    /// </summary>
    protected static class MathV
    {
        /// <summary>
        ///大きい方のベクトルを取得
        /// </summary>
        public static Vector2 Max(Vector2 main, Vector2 sub)
        {
            return main.magnitude >= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最小値を設定
        /// </summary>
        public static Vector2 Max(Vector2 main, float limit)
        {
            return Max(main, main.normalized * limit);
        }
        /// <summary>
        ///小さい方のベクトルを取得
        /// </summary>
        public static Vector2 Min(Vector2 main, Vector2 sub)
        {
            return main.magnitude <= sub.magnitude ? main : sub;
        }
        /// <summary>
        ///ベクトル長の最大値を設定
        /// </summary>
        public static Vector2 Min(Vector2 main, float limit)
        {
            return Min(main, main.normalized * limit);
        }
        /// <summary>
        ///各要素の絶対値を取ったベクトルを取得
        /// </summary>
        public static Vector2 Abs(Vector2 main)
        {
            return Vector2.right * Mathf.Abs(main.x) + Vector2.up * Mathf.Abs(main.y);
        }
        /// <summary>
        ///mainのベクトルをsubに合わせて補正する
        /// </summary>
        public static Vector2 correct(Vector2 main, Vector2 sub, float degree = 0.5f)
        {
            return main * degree + sub * (1 - degree);
        }
        /// <summary>
        ///mainの数値をsubに合わせて補正する
        /// </summary>
        public static float correct(float main, float sub, float degree = 0.5f)
        {
            return main * degree + sub * (1 - degree);
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
        /// </summary>
        public static Vector2 scaling(Vector2 main, Vector2 scale)
        {
            return Vector2.right * main.x * scale.x + Vector2.up * main.y * scale.y;
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に掛ける
        /// </summary>
        public static Vector2 scaling(Vector2 main, float scaleX, float scaleY)
        {
            return scaling(main, new Vector2(scaleX, scaleY));
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
        /// </summary>
        public static Vector2 rescaling(Vector2 main, Vector2 scale)
        {
            return Vector2.right * main.x / scale.x + Vector2.up * main.y / scale.y;
        }
        /// <summary>
        ///mainの数値にscaleのサイズ補正をXYの軸毎に割る
        /// </summary>
        public static Vector2 rescaling(Vector2 main, float scaleX, float scaleY)
        {
            return rescaling(main, new Vector2(scaleX, scaleY));
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Vector2 direction, float length)
        {
            return direction.normalized * length;
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Vector2 direction, Vector2 length)
        {
            return recalculation(direction, length.magnitude);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(Quaternion direction, float length)
        {
            return recalculation(direction * Vector2.right, length);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(float direction, float length)
        {
            return recalculation(MathA.toRotation(direction), length);
        }
        /// <summary>
        ///向きと長さからベクトル生成
        /// </summary>
        public static Vector2 recalculation(float direction, Vector2 length)
        {
            return recalculation(MathA.toRotation(direction), length.magnitude);
        }
        /// <summary>
        ///ベクトルを指定枠内に収まる値に補正
        /// </summary>
        public static Vector2 within(Vector2 main, Vector2 lowerLeft, Vector2 upperRight)
        {
            Vector2 returnVector = main;
            returnVector.x = Mathf.Clamp(returnVector.x, lowerLeft.x, upperRight.x);
            returnVector.y = Mathf.Clamp(returnVector.y, lowerLeft.y, upperRight.y);
            return returnVector;
        }
    }

    /// <summary>
    ///角度系の汎用計算クラス
    /// </summary>
    public static class MathA
    {
        /// <summary>
        ///鋭角の大きい方の角度を取得
        /// </summary>
        public static float Max(float main, float sub)
        {
            return acute(main) >= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の小さい方の角度を取得
        /// </summary>
        public static float Min(float main, float sub)
        {
            return acute(main) <= acute(sub) ? main : sub;
        }
        /// <summary>
        ///鋭角の取得
        /// </summary>
        public static float acute(float angle)
        {
            return Mathf.Min(compile(angle), 360 - compile(angle));
        }
        /// <summary>
        ///角度補正関数
        ///主にイージングとか
        /// </summary>
        public static float correct(float main, float sub, float degree = 0.5f)
        {
            main = compile(main);
            sub = compile(sub);

            bool normalOrder = Mathf.Abs(main - sub) > 180;
            float startPoint = normalOrder ? main : sub;
            float endPoint = normalOrder ? sub : main;

            return compile(MathV.correct(startPoint, endPoint, degree));
        }
        /// <summary>
        ///角度を0から360までに収める
        /// </summary>
        public static float compile(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }
        /// <summary>
        ///ベクトルを角度化
        /// </summary>
        public static float toAngle(Vector2 targetVector)
        {
            return Vector2.Angle(Vector2.right, targetVector) * (Vector2.Angle(Vector2.up, targetVector) <= 90 ? 1 : -1);
        }
        /// <summary>
        ///クォータニオンを角度化
        /// </summary>
        public static float toAngle(Quaternion targetRotation)
        {
            return toAngle(targetRotation * Vector2.right);
        }
        /// <summary>
        ///角度をクォータニオン化
        /// </summary>
        public static Quaternion toRotation(float targetAngle)
        {
            var returnRotation = new Quaternion();
            returnRotation.eulerAngles = new Vector3(0, 0, targetAngle);
            return returnRotation;
        }
        /// <summary>
        ///ベクトルをクォータニオン化
        /// </summary>
        public static Quaternion toRotation(Vector2 targetVector)
        {
            return Quaternion.AngleAxis(toAngle(targetVector), Vector3.forward);
        }
    }

    /// <summary>
    ///SE鳴らす関数
    /// </summary>
    protected AudioSource soundSE(AudioClip soundEffect, float baseVolume = 1, float pitch = 1)
    {
        if (soundEffect == null) return null;

        AudioSource soundObject = Instantiate(Sys.SErootObject).GetComponent<AudioSource>();

        soundObject.clip = soundEffect;
        soundObject.volume = volumeSE * baseVolumeSE * baseVolume;
        soundObject.pitch = pitch;

        soundObject.Play();

        return soundObject;
    }

    /// <summary>
    ///アンカーパラメータからアンカー一座標を取得する関数
    /// </summary>
    protected static Vector2 getAxis(TextAnchor anchor, TextAnchor? pibot = null)
    {
        var pibotPosition = pibot != null ? getAxis((TextAnchor)pibot) : Vector2.zero;
        switch (anchor)
        {
            case TextAnchor.UpperLeft:
                return Vector2.up - pibotPosition;
            case TextAnchor.UpperCenter:
                return Vector2.right / 2 + Vector2.up - pibotPosition;
            case TextAnchor.UpperRight:
                return Vector2.right + Vector2.up - pibotPosition;
            case TextAnchor.MiddleLeft:
                return Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleCenter:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
            case TextAnchor.MiddleRight:
                return Vector2.right + Vector2.up / 2 - pibotPosition;
            case TextAnchor.LowerLeft:
                return Vector2.zero - pibotPosition;
            case TextAnchor.LowerCenter:
                return Vector2.right / 2 - pibotPosition;
            case TextAnchor.LowerRight:
                return Vector2.right - pibotPosition;
            default:
                return Vector2.right / 2 + Vector2.up / 2 - pibotPosition;
        }
    }

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected static Text setSysText(string setText, string textName, Vector2? position = null, int? size = null, TextAnchor textPosition = TextAnchor.UpperLeft)
    {
        Vector2 setPosition = position ?? Vector2.zero;
        GameObject textObject = GameObject.Find(textName);
        if (textObject == null)
        {
            textObject = Instantiate(Sys.basicText).gameObject;
            textObject.transform.SetParent(sysCanvas.transform);
            textObject.name = textName;
        }

        var body = textObject.GetComponent<Text>();
        body.text = setText;
        body.fontSize = size ?? defaultTextSize;
        body.alignment = textPosition;

        Vector2 axis = getAxis(textPosition);

        var setting = textObject.GetComponent<RectTransform>();
        setting.localPosition = setPosition;
        setting.localScale = new Vector3(1, 1, 1);
        setting.anchorMin = axis;
        setting.anchorMax = axis;
        setting.anchoredPosition = setPosition;
        setting.sizeDelta = new Vector2(body.preferredWidth, body.preferredHeight);
        setting.pivot = axis;

        return body;
    }
    /// <summary>
    ///システムテキストへの文字設定
    ///位置指定バラバラ版
    /// </summary>
    protected void setSysText(string setText, string textName, float posX, float posY)
    {
        setSysText(setText, textName, new Vector2(posX, posY));
        return;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected string getSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if (textObject == null) return "";
        return textObject.GetComponent<Text>().text;
    }
    /// <summary>
    ///システムテキストの削除
    /// </summary>
    protected static void deleteSysText(string textName)
    {
        var textObject = GameObject.Find(textName);
        if (textObject == null) return;
        Destroy(textObject);
        return;
    }
    protected static float getTextWidth(string setText, int? size = null)
    {
        const string temporary = "temporary";

        var returnValue = setSysText(setText, temporary, Vector2.zero, size).GetComponent<RectTransform>().sizeDelta.x;
        deleteSysText(temporary);

        return returnValue;
    }


    /// <summary>
    ///複数キーのOR押下判定
    /// </summary>
    protected bool onKeysDecision(List<KeyCode> keys, keyTiming timing = keyTiming.on)
    {
        if (keys == null || keys.Count <= 0) return false;

        keyDecision decision = T => false;
        switch (timing)
        {
            case keyTiming.down:
                decision = key => Input.GetKeyDown(key);
                break;
            case keyTiming.on:
                decision = key => Input.GetKey(key);
                break;
            case keyTiming.up:
                decision = key => Input.GetKeyUp(key);
                break;
            default:
                break;
        }

        foreach (var key in keys) if (decision(key)) return true;
        return false;
    }
    protected delegate bool keyDecision(KeyCode timing);
    protected enum keyTiming { down, on, up }

    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay, List<KeyCode> interruptions)
    {
        for (var i = 0; i < delay; i++)
        {
            if (onKeysDecision(interruptions)) yield break;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay, KeyCode? interruption = null)
    {
        List<KeyCode> interruptions = new List<KeyCode>();
        if (interruption != null) interruptions.Add((KeyCode)interruption);
        yield return wait(delay, interruptions);
    }

    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public virtual void selfDestroy(bool system = false)
    {
        if (gameObject == null) return;
        Destroy(gameObject);
    }
    /// <summary>
    /// 全体削除関数
    /// </summary>
    protected void destroyAll()
    {
        foreach (Transform target in sysPanel.transform)
        {
            if (target.GetComponent<Player>() != null) continue;

            var targetMethod = target.GetComponent<Methods>();
            if (targetMethod == null) continue;

            targetMethod.selfDestroy(true);
        }

        selfDestroy(true);
        return;
    }

    /// <summary>
    /// 選択肢結果値保存変数
    /// </summary>
    protected static int? lastSelected = null;
    /// <summary>
    /// 選択肢結果値保存変数
    /// </summary>
    protected static string choiceTextName(int index)
    {
        return "choices" + index;
    }
    /// <summary>
    /// 選択肢関数
    /// 結果の値はlastSelectに保存
    /// </summary>
    protected static IEnumerator getChoices(List<string> choices,
        UnityAction<int> selectedAction = null,
        UnityAction<int, bool, bool> horizontalAction = null,
        bool horizontalBarrage = false,
        int horizontalInterval = 0,
        Vector2? setPosition = null,
        TextAnchor pibot = TextAnchor.UpperCenter,
        bool ableCancel = false,
        int? maxChoices = null,
        int? textSize = null,
        int newSelect = 0)
    {
        var choiceNums = new List<int>();
        for (int i = 0; i < choices.Count; i++) if (choices[i].Length > 0) choiceNums.Add(i);

        int selectNum = Mathf.Clamp(choiceNums.IndexOf(newSelect), 0, choiceNums.Count - 1);
        int firstDisplaied = selectNum;
        int choiceableCount = Mathf.Min(maxChoices ?? choiceNums.Count, choiceNums.Count);

        lastSelected = null;

        int baseSize = textSize ?? defaultTextSize;
        var monoHeight = baseSize * 1.5f;

        float maxWidth = 0;
        for (int i = 0; i < choiceNums.Count; i++)
        {
            var choice = (i == selectNum ? ">\t" : "\t") + choices[choiceNums[i]] + "\t\t";
            maxWidth = Mathf.Max(getTextWidth(choice, baseSize), maxWidth);
        }
        var windowSize = new Vector2(maxWidth + baseMas.x, monoHeight * (choiceableCount + 1));
        var textHeight = choiceableCount - 1;

        Vector2 windowPosition = (setPosition ?? Vector2.zero)
            - MathV.scaling(getAxis(pibot, TextAnchor.MiddleCenter), windowSize);
        Vector2 textBasePosition = windowPosition
            + Vector2.right * (screenSize.x - maxWidth) / 2
            + Vector2.up * monoHeight * textHeight / 2;

        Window backWindow = Instantiate(Sys.basicWindow);
        backWindow.transform.SetParent(sysView.transform);
        backWindow.transform.localPosition = viewPosition + windowPosition / baseMas.x;

        yield return null;

        bool toDecision = false;
        bool toCancel = false;
        long horizontalCount = 0;
        while (!toDecision && !toCancel)
        {
            selectNum %= choiceNums.Count;
            if (selectedAction != null) selectedAction(choiceNums[selectNum]);

            firstDisplaied = Mathf.Clamp(firstDisplaied,
                Mathf.Max(selectNum + 1 - choiceableCount, 0),
                Mathf.Min(selectNum, choiceNums.Count - choiceableCount));
            var endDisplaied = firstDisplaied + choiceableCount;

            for (int i = firstDisplaied; i < endDisplaied; i++)
            {
                var index = i - firstDisplaied;
                var choice = (i == selectNum ? ">\t" : "\t") + choices[choiceNums[i]];
                var nowPosition = textBasePosition + Vector2.down * monoHeight * index;
                setSysText(choice, choiceTextName(index), nowPosition, baseSize, TextAnchor.MiddleLeft);
            }
            backWindow.transform.localScale = Vector2.right * windowSize.x / baseMas.x
                + Vector2.up * windowSize.y / baseMas.y;

            bool inputUpKey = false;
            bool inputDownKey = false;
            bool? inputHorizontalKey = null;
            bool inputHorizontalFirst = false;

            while (!toDecision && !toCancel && !inputUpKey && !inputDownKey && inputHorizontalKey == null)
            {
                toDecision = Input.GetKeyDown(ButtomZ);
                toCancel = Input.GetKeyDown(ButtomX) && ableCancel;
                inputUpKey = Input.GetKeyDown(ButtomUp);
                inputDownKey = Input.GetKeyDown(ButtomDown);
                if (horizontalAction != null)
                {
                    horizontalCount %= (horizontalInterval + 1);
                    if (Input.GetKey(ButtomRight) && horizontalBarrage) inputHorizontalKey = true;
                    if (Input.GetKey(ButtomLeft) && horizontalBarrage) inputHorizontalKey = false;
                    if (Input.GetKeyDown(ButtomRight))
                    {
                        horizontalCount = 0;
                        inputHorizontalFirst = true;
                        inputHorizontalKey = true;
                    }
                    if (Input.GetKeyDown(ButtomLeft))
                    {
                        horizontalCount = 0;
                        inputHorizontalFirst = true;
                        inputHorizontalKey = false;
                    }
                }

                yield return null;
            }

            if (horizontalAction != null
                && inputHorizontalKey != null
                && horizontalCount++ % (horizontalInterval + 1) == 0)
                horizontalAction(choiceNums[selectNum], (bool)inputHorizontalKey, inputHorizontalFirst);

            if (inputDownKey) selectNum += 1;
            if (inputUpKey) selectNum += choiceNums.Count - 1;
            if (toCancel) selectNum = -1;
        }

        lastSelected = selectNum >= 0 ? choiceNums[selectNum] : -1;
        for (int i = 0; i < choiceNums.Count; i++) deleteSysText(choiceTextName(i));
        backWindow.selfDestroy();
        yield break;
    }

    protected class Easing
    {
        /// <summary>
        ///線形変動
        /// </summary>
        public Linear liner = new Linear();
        /// <summary>
        ///二乗変動
        /// </summary>
        public Quadratic quadratic = new Quadratic();
        /// <summary>
        ///三乗変動
        /// </summary>
        public Cubic cubic = new Cubic();
        /// <summary>
        ///四乗変動
        /// </summary>
        public Quartic quartic = new Quartic();
        /// <summary>
        ///五乗変動
        /// </summary>
        public Quintic quintic = new Quintic();
        /// <summary>
        ///円形変動
        /// </summary>
        public Sinusoidal sinusoidal = new Sinusoidal();
        /// <summary>
        ///累乗変動
        /// </summary>
        public Exponential exponential = new Exponential();
        /// <summary>
        ///乗根変動
        /// </summary>
        public Circular circular = new Circular();

        public class BaseEaaing
        {
            public virtual float In(float max, float time, float limit)
            {
                Debug.Log(max);
                return max;
            }
            public float In(float time, float limit)
            {
                return In(1, time, limit);
            }
            public float SubIn(float max, float time, float limit)
            {
                return max - In(max, time, limit);
            }
            public float SubIn(float time, float limit)
            {
                return 1 - In(1, time, limit);
            }

            public float Out(float max, float time, float limit)
            {
                return max - In(max, limit - time, limit);
            }
            public float Out(float time, float limit)
            {
                return Out(1, time, limit);
            }
            public float SubOut(float max, float time, float limit)
            {
                return max - Out(max, time, limit);
            }
            public float SubOut(float time, float limit)
            {
                return 1 - Out(1, time, limit);
            }

            public float InOut(float max, float time, float limit)
            {
                return time < limit / 2
                    ? In(max / 2, time, limit / 2)
                    : Out(max / 2, time - limit / 2, limit / 2) + max / 2;
            }
            public float InOut(float time, float limit)
            {
                return InOut(1, time, limit);
            }
            public float SubInOut(float max, float time, float limit)
            {
                return max - InOut(max, time, limit);
            }
            public float SubInOut(float time, float limit)
            {
                return 1 - InOut(1, time, limit);
            }
        }
        public class Linear : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time / limit;
            }
        }
        public class Quadratic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time / limit / limit;
            }
        }
        public class Cubic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time / limit / limit / limit;
            }
        }
        public class Quartic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time / limit / limit / limit / limit;
            }
        }
        public class Quintic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time * time / limit / limit / limit / limit / limit;
            }
        }
        public class Sinusoidal : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * Mathf.Cos(time * Mathf.PI / limit / 2) + max;
            }
        }
        public class Exponential : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * Mathf.Pow(2, 10 * (time - limit) / limit);
            }
        }
        public class Circular : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * (Mathf.Sqrt(1 - time * time / limit / limit) - 1);
            }
        }
    }
}
