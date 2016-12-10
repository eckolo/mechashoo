using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public partial class Methods : MonoBehaviour
{
    protected class TextsWithWindow
    {
        public TextsWithWindow()
        {
            textNames = new List<string>();
        }

        //MEMO:デストラクタで呼ばせる
        public void selfDestroy(bool setMotion = true, bool system = false)
        {
            for(int index = 0; index < textNames.Count; index++) deleteSysText(textNames[index]);
            deleteWindow(backWindow, setMotion ? Choice.WINDOW_MOTION_TIME : 0, system);
        }

        public List<string> textNames
        {
            get
            {
                return texts.Select(textObj => textObj.name).ToList();
            }
            set
            {
                texts = value.Select(name => GameObject.Find(name).GetComponent<Text>()).ToList();
            }
        }
        public List<Text> texts { get; set; }
        public Window backWindow { get; set; }
        public Vector2 underLeft
        {
            get
            {
                if(backWindow == null) return position - textArea / 2;
                return backWindow.underLeft;
            }
        }
        public Vector2 upperRight
        {
            get
            {
                if(backWindow == null) return position + textArea / 2;
                return backWindow.upperRight;
            }
        }
        Vector2 position
        {
            get
            {
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
            get
            {
                var allText = texts
                    .Select(textObj => textObj.text)
                    .Aggregate((text1, text2) => text1 + text2);
                var width = allText
                    .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => getTextWidth(line))
                    .Max();
                var height = texts.First().fontSize * 1.5f * getLines(allText);
                return new Vector2(width, height);
            }
        }
    }
}
