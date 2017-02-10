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
                return backWindow.position.scaling(baseMas);
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
