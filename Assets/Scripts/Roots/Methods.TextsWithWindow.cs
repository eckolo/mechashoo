using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public partial class Methods : MonoBehaviour
{
    protected class TextsWithWindow : IDisposable
    {
        public TextsWithWindow()
        {
            texts = new List<Text>();
        }
        public void Dispose()
        {
            selfDestroy(system: true);
        }

        //MEMO:デストラクタで呼ばせる
        public void selfDestroy(bool setMotion = true, bool system = false)
        {
            foreach(var text in texts) deleteSysText(text);
            if(backWindow != null) deleteWindow(backWindow, setMotion ? Choice.WINDOW_MOTION_TIME : 0, system);
        }

        public List<string> textNames
        {
            get {
                return texts.Select(textObj => textObj != null ? textObj.name : "").ToList();
            }
        }
        public Text text
        {
            set {
                texts = new List<Text> { value };
            }
        }
        public List<Text> texts { get; set; }
        public Window backWindow { get; set; }
        public Vector2 underLeft
        {
            get {
                if(backWindow == null) return position - textArea / 2;
                return backWindow.underLeft;
            }
        }
        public Vector2 upperRight
        {
            get {
                if(backWindow == null) return position + textArea / 2;
                return backWindow.upperRight;
            }
        }
        Vector2 position
        {
            get {
                if(backWindow == null)
                {
                    return texts
                         .Select(textObj => textObj.GetComponent<RectTransform>().localPosition)
                         .Aggregate((vec1, vec2) => vec1 + vec2) / texts.Count;
                }
                return MathV.scaling(backWindow.position, baseMas);
            }
        }
        Vector2 textArea
        {
            get {
                var rects = texts
                    .Select(text => text.GetComponent<RectTransform>());
                var upper = rects.Max(rect => rect.localPosition.y + rect.sizeDelta.y / 2);
                var righter = rects.Max(rect => rect.localPosition.x + rect.sizeDelta.x / 2);
                var downer = rects.Min(rect => rect.localPosition.y - rect.sizeDelta.y / 2);
                var lefter = rects.Min(rect => rect.localPosition.x - rect.sizeDelta.x / 2);
                return new Vector2(righter - lefter, upper - downer);
            }
        }
    }
}
