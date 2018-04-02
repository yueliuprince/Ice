using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum Language
{
    English = 0,
    Chinese,  
    Other,
    OriginText,
}

public static class Lang
{
    private class Sentence
    {
        public string originText = null;
        public string[] trs = null;
    }

    public static void LoadLanguageAsset(TextAsset trAsset)
    {
        if (dics.Count > 0) return;
        string trText = trAsset.text;
        string[] ss = trText.Split('\n');

        Sentence s;
        for (int i = 0; i < ss.Length; i++)
        {
            s = JsonConvert.DeserializeObject<Sentence>(ss[i]);
            if (s != null) dics.Add(s.originText, s.trs);
        }
    } 

    private static Dictionary<string, string[]> dics = new Dictionary<string, string[]>();
    private static Language lang = Language.English;

    public static Language GlobalLanguage {
        get { return lang; }
        set {
            lang = value;
            TrForUI[] tfus=UnityEngine.Object.FindObjectsOfType<TrForUI>();
            for (int i = 0; i < tfus.Length; i++) tfus[i].UpdateLanguage();
        }
    }

    public static string Tr(string originText)
    {
#if UNITY_EDITOR
        if (dics.Count == 0)
        {
            Debug.Log("The language file isn't loaded.");
            return originText;
        }
#endif
        if (dics.ContainsKey(originText))
        {
            string result = dics[originText][(int)lang];
            if (result != "") return result;
        }

        return originText;
    }
}



///文件结构：
///每行一个json，表示一个结构体，包括源英语，和翻译的字符串数组

/// <summary>
/// 标记一个常量字符串为可翻译的
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class Internationable : Attribute
{
    public Internationable()
    {

    }
}