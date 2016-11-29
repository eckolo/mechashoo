using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public partial class Methods : MonoBehaviour {
    /// <summary>
    ///フィールドサイズ
    /// </summary>
    protected static Vector2 fieldSize {
        get {
            return viewSize * 2;
        }
    }
    /// <summary>
    ///フィールド左下端
    /// </summary>
    protected static Vector2 fieldLowerLeft {
        get {
            return -fieldSize / 2;
        }
    }
    /// <summary>
    ///フィールド右上端
    /// </summary>
    protected static Vector2 fieldUpperRight {
        get {
            return fieldSize / 2;
        }
    }
    /// <summary>
    ///フィールド視野サイズ
    /// </summary>
    protected static Vector2 viewSize {
        get {
            return Camera.main.ViewportToWorldPoint(Vector2.one) - Camera.main.ViewportToWorldPoint(Vector2.zero);
        }
    }
    /// <summary>
    ///フィールド視点位置
    /// </summary>
    protected static Vector2 viewPosition {
        get {
            return Camera.main.transform.localPosition;
        }
        set {
            var edge = (fieldSize - viewSize) / 2;
            Vector3 setPosition = MathV.within(value, -edge, edge);
            setPosition.z = 0;
            Camera.main.transform.localPosition = setPosition;
            setPosition.z = 1;
            sysView.transform.localPosition = setPosition;
        }
    }
    /// <summary>
    ///ピクセル単位のキャンバスサイズ
    /// </summary>
    protected static Vector2 screenSize {
        get {
            return sysCanvas.GetComponent<CanvasScaler>().referenceResolution;
        }
    }
    /// <summary>
    ///1マス当たりのピクセルサイズ
    /// </summary>
    protected static Vector2 baseMas {
        get {
            return MathV.rescaling(screenSize, viewSize);
        }
    }
}
