using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapHelper : ScriptableWizard
{
    public enum Cmd
    {
        replace,
        del,
    }

    public Cmd cmd;
    public EditorQuadMap map;
    public GameObject go;
    public NType type;

    [MenuItem("xiao_D/TileSystem/MapHelper")]
    static void CreateWizerd()
    {
        DisplayWizard("Map Helper", typeof(MapHelper), "Close", "RunCommand");
    }

    private void OnEnable()
    {
        map = FindObjectOfType<EditorQuadMap>();
    }


    private void OnWizardOtherButton()
    {
        switch (cmd)
        {
            case Cmd.replace:
                {
                    if (go == null) return;
                    List<TileNode> nodes = map.FindNodesByType(type);
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        GameObject obj = Instantiate(go);
                        obj.transform.position = new Vector3(0, go.transform.position.y, 0);

                        if (obj.GetComponent<Chess>() == null) obj.transform.SetParent(nodes[i].transform, true);

                        nodes[i].AlignToThis(obj.transform);
                    }
                    break;
                }
            case Cmd.del:
                {
                    List<TileNode> nodes = map.FindNodesByType(type);
                    for (int i = 0; i < nodes.Count; i++) Q.DestroyAllChildren(nodes[i].gameObject);
                    break;
                }
        }
    }

    private void OnWizardCreate()
    {

    }
}
