using UnityEngine;

[DefaultExecutionOrder(-2000)]
public class GlobalSettings : MonoBehaviour
{
    public TextAsset trAsset;
    public Language globalLanguage = Language.English;

    public static bool isAwake = false;
    private void Awake()
    {
        if (isAwake) return;
        isAwake = true;

        if (trAsset != null) Lang.LoadLanguageAsset(trAsset);
        else Debug.Log("The language file is missing!");
        
        Lang.GlobalLanguage = globalLanguage;
    }
}
