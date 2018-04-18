using System.IO;
using UnityEngine;
using EditorTemp;

namespace EditorTemp
{
    public class Temp
    {
        public Vector2Int size;
        public NType type = NType.Basic;
        public Vector2Int pos;
        public float sideLength;
    }
}

/// <summary>
/// Replace this with a playmode map class when you release
/// </summary>
public class EditorQuadMap : QuadMap
{

    public TextAsset mapAsset;
    public bool importOnAwake = true;
    public GameObject fileExport;

    public new void Awake()
    {
        base.Awake();

#if !UNITY_EDITOR
        string data;
        try
        {
            data = File.ReadAllText(Path.Combine(Application.dataPath, "map.txt"));
        }
        catch
        {
            return;
        }

        if (data != null && data != "") Import(data);
#else
        if (importOnAwake && mapAsset)
        {
            if (size == Vector2Int.one) Import();
            else Q.WarningPrint(transform, GetType().ToString(), "Dynamic import must act on empty map.");
        }
#endif
    }

    [SerializeField] private GameObject basic;
    [SerializeField] private GameObject quick;
    [SerializeField] private GameObject camp;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject obstacle;

    public void AddRow()
    {
        if (size.x >= MAX_RC) return;
        GameObject newRow = new GameObject("row_" + size.x.ToString());
        newRow.transform.parent = transform;
        newRow.transform.position = transform.position;

        for (int i = 0; i < size.y; i++)
        {
            GameObject obj = Instantiate(basic);
            obj.name = "col_" + i.ToString();
            obj.transform.parent = newRow.transform;
            Vector2Int newPos = new Vector2Int(size.x, i);
            obj.transform.position = GridsPosToWorld(newPos);
            TileNode node = obj.GetComponent<TileNode>();
            node.pos = newPos;
            m[size.x, i] = node;
        }
        size.x++;
    }

    public void DelRow()
    {
        if (size.x <= 1) return;
        size.x--;
        DestroyImmediate(FindRow(size.x).gameObject);
    }

    public void AddColumn()
    {
        if (size.y >= MAX_RC) return;
        for (int i = 0; i < size.x; i++)
        {
            GameObject obj = Instantiate(basic);
            obj.name = "col_" + size.y.ToString();
            obj.transform.parent = FindRow(i).transform;
            Vector2Int newPos = new Vector2Int(i, size.y);
            obj.transform.position = GridsPosToWorld(newPos);
            TileNode node = obj.GetComponent<TileNode>();
            node.pos = newPos;
            m[i, size.y] = node;
        }
        size.y++;
    }

    public void DelColumn()
    {
        if (size.y <= 1) return;
        size.y--;

        for (int i = 0; i < size.x; i++)
        {
            DestroyImmediate(FindByRC(i, size.y).gameObject);
        }
    }


    private Transform FindRow(int i)
    {
        return transform.Find("row_" + i.ToString());
    }

    private Transform FindByRC(int row, int column)
    {
        Transform rowT = FindRow(row);
        return rowT.Find("col_" + column.ToString());
    }

    public void AddNodeFromJson(string json)
    {
        Temp node = JsonUtility.FromJson<Temp>(json);
        if (node == null) return;
        Transform target = FindByRC(node.pos.x, node.pos.y);

        target.GetComponent<TileNode>().type = node.type;

        switch (node.type)
        {
            case NType.Basic: target.GetComponent<SpriteRenderer>().sprite = basic.GetComponent<SpriteRenderer>().sprite; break;
            case NType.Other: target.GetComponent<SpriteRenderer>().sprite = camp.GetComponent<SpriteRenderer>().sprite; break;
            case NType.Obstacle: target.GetComponent<SpriteRenderer>().sprite = home.GetComponent<SpriteRenderer>().sprite; break;
            case NType.NULL: target.GetComponent<SpriteRenderer>().sprite = obstacle.GetComponent<SpriteRenderer>().sprite; break;
            case NType.Quick: target.GetComponent<SpriteRenderer>().sprite = quick.GetComponent<SpriteRenderer>().sprite; break;
        }
    }


    public void Export()
    {
        ExportToPath(Path.Combine(Application.dataPath,"TileSystem"));

#if !UNITY_EDITOR
        fileExport.SetActive(true);
#endif
    }

    public void ExportToPath(string path)
    {
        string[] jsons = new string[size.x * size.y + 1];

        jsons[0] = JsonUtility.ToJson(this);
        for (int i = 0, cnt = 1; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++, cnt++)
            {
                jsons[cnt] = JsonUtility.ToJson(m[i, j]);
            }
        }

        File.WriteAllLines(Path.Combine(path, "map.txt"), jsons);
    }



    public void Import()
    {
        Import(mapAsset.text);
    }

    public void Import(string data)
    {
        string[] jsons = data.Split('\n');

        Temp head = JsonUtility.FromJson<Temp>(jsons[0]);

        for (int i = 1; i < head.size.x; i++) AddRow();
        for (int i = 1; i < head.size.y; i++) AddColumn();

        for (int i = 1; i < jsons.Length; i++) AddNodeFromJson(jsons[i]);
        //Debug.Log("Import successful!");
    }
#if !UNITY_EDITOR
    private void OnApplicationQuit()
    {
        Export();
    }
#endif
}
