using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public abstract partial class Methods : MonoBehaviour
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
            if(backWindow != null) deleteWindow(backWindow, setMotion ? Configs.Choice.WINDOW_MOTION_TIME : 0, system);
        }

        public List<string> textNames
        {
            get {
                return texts.Select(textObj => textObj != null ? textObj.name : "").ToList();
            }
        }
        public Text text
        {
            get {
                return texts.SingleOrDefault();
            }
            set {
                texts = new List<Text> { value };
            }
        }
        public List<Text> texts { get; set; }
        public Window backWindow { get; set; }
        public Vector2 underLeft
        {
            get {
                if(backWindow == null) return position - nowArea / 2;
                return backWindow.underLeft;
            }
            set {
                position = value + nowArea / 2;
            }
        }
        public Vector2 upperRight
        {
            get {
                if(backWindow == null) return position + nowArea / 2;
                return backWindow.upperRight;
            }
            set {
                position = value - nowArea / 2;
            }
        }
        Vector2 position
        {
            get {
                if(backWindow == null) return texts
                        .Select(textObj => textObj.rectTransform.localPosition)
                        .Aggregate((vec1, vec2) => vec1 + vec2) / texts.Count;
                return backWindow.position.scaling(baseMas);
            }
            set {
                if(backWindow != null) backWindow.position = value.rescaling(baseMas);
                var diff = value - position;
                foreach(var text in texts)
                {
                    Vector2 nowPosition = text.rectTransform.localPosition;
                    text.setPosition(nowPosition + diff);
                }
            }
        }
        public float nowAlpha
        {
            get {
                return Mathf.Max(backWindow.nowAlpha, texts.Max(text => text.color.a));
            }
            set {
                backWindow.nowAlpha = value;
                foreach(var text in texts) text.setAlpha(value);
            }
        }
        public Vector2 nowScale
        {
            get {
                return backWindow.nowScale;
            }
            set {
                var tempPosition = backWindow.position;
                backWindow.nowScale = value;
                backWindow.position = tempPosition;
                foreach(var text in texts)
                {
                    text.rectTransform.localScale = new Vector3(value.x, value.y, 1);
                    text.rectTransform.localPosition = new Vector3(tempPosition.x, tempPosition.y, text.rectTransform.localPosition.z);
                }
            }
        }
        public Vector2 nowArea
        {
            get {
                if(backWindow == null) return textArea;
                return backWindow.nowSize.scaling(baseMas);
            }
        }
        public Vector2 textArea
        {
            get {
                var locations = texts
                    .Select(text => new
                    {
                        position = text.GetComponent<RectTransform>().localPosition,
                        area = new Vector2(text.preferredWidth, text.preferredHeight)
                    });
                var upper = locations.Max(text => text.position.y + text.area.y / 2);
                var righter = locations.Max(text => text.position.x + text.area.x / 2);
                var downer = locations.Min(text => text.position.y - text.area.y / 2);
                var lefter = locations.Min(text => text.position.x - text.area.x / 2);
                return new Vector2(righter - lefter, upper - downer);
            }
        }
    }
}
