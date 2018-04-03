using EditorTemp;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapImporter : ScriptableWizard
{
    public EditorQuadMap main;

    [MenuItem("xiao_D/TileSystem/Map Importer")]
    static void CreateWizerd()
    {
        DisplayWizard("Map Importer", typeof(MapImporter), "Import");
    }

    private void OnEnable()
    {
        main = FindObjectOfType<EditorQuadMap>();
    }

    private void Update()
    {
        if (main.mapAsset == null) errorString = "Drag the map asset to the main EditorQuadMap.";
        else helpString = "Ready.";
    }

    private void OnWizardCreate()
    {
        if (main && main.mapAsset)
        {
            ClearMapTo_11();

            main.importOnAwake = true;
            main.Awake();
            main.importOnAwake = false;
        }

        Debug.Log("You should click [Save Scene As] and override the previous file,or the change maybe lost.");
        AssetDatabase.SaveAssets();
    }

    public void ClearMapTo_11()
    {
        int n = main.transform.childCount;
        List<GameObject> ds = new List<GameObject>();
        for (int i = 0; i < n; i++)
        {
            Transform t = main.transform.GetChild(i);
            if (t.name != "row_0") ds.Add(t.gameObject);
        }
        for (int i = 0; i < ds.Count; i++) DestroyImmediate(ds[i]);
        ds.Clear();
        Transform row_0 = main.transform.GetChild(0);
        n = row_0.transform.childCount;
        for (int i = 0; i < n; i++)
        {
            Transform t = row_0.transform.GetChild(i);
            if (t.name != "col_0") ds.Add(t.gameObject);
        }
        for (int i = 0; i < ds.Count; i++) DestroyImmediate(ds[i]);
        ds.Clear();

        main.size = Vector2Int.one;
    }
}
