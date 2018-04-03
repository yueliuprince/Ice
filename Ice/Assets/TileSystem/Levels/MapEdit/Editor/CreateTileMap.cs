using UnityEngine;
using UnityEditor;

public class CreateTileMap : ScriptableWizard
{
    public GameObject defaultMap;
    [MenuItem("xiao_D/TileSystem/Create TileMap")]
    static void CreateWizerd()
    {
        DisplayWizard("Create TileMap", typeof(CreateTileMap), "OK");
    }

    private void OnWizardCreate()
    {
        if (defaultMap == null) return;
        if (GameObject.Find("tMap") == null)
        {
            GameObject go = Instantiate(defaultMap);
            go.transform.position = Vector3.zero;
            go.name = "tMap";
        }
    }


}
