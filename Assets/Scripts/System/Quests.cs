using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Quests : Methods
{
    public List<Quest> allQuests = new List<Quest>();
    [Serializable]
    public class Quest
    {
        public string name = "依頼名";
        [Multiline]
        public string explanation = "依頼内容";
        public Stage stage;
        public int level = 1;
    }
}
