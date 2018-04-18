using System.Collections.Generic;
using UnityEngine;

public class QuadMap : MonoBehaviour
{
    public Vector2Int size = Vector2Int.one;
    protected const int MAX_RC = 32;
    protected TileNode[,] m = new TileNode[MAX_RC, MAX_RC];
    public float sideLength = 1;

    public static QuadMap PUBLIC;
    public static LayerMask nodeLayer;
    public static LayerMask chessLayer;

    protected void Awake()
    {
        PUBLIC = this;

        nodeLayer = LayerMask.GetMask("tNode");
        chessLayer = LayerMask.GetMask("chess");
    }

    protected void Start()
    {
        TileNode[] nodes = GetComponentsInChildren<TileNode>();
        //MainLogic.UI.MapEdit_UI.PUBLIC.posText.text = nodes.Length.ToString();
        for (int i = 0; i < nodes.Length; i++) m[nodes[i].pos.x, nodes[i].pos.y] = nodes[i];
    }


    public Vector3 GetWorldDir(Vector2Int localDir)
    {
        return new Vector3(localDir.y, 0, -localDir.x);
    }

    public TileNode Find(Vector2Int pos, bool ignoreNull = true)
    {
        if (pos.x >= 0 && pos.x < size.x && pos.y >= 0 && pos.y < size.y)
        {
            if (ignoreNull)
            {
                if (m[pos.x, pos.y].type == NType.NULL) return null;
            }
            return m[pos.x, pos.y];
        }
        return null;
    }

    public Vector3 GridsPosToWorld(Vector2Int gPos)
    {
        Vector3 result = transform.position;
        result.z -= gPos.x * sideLength;
        result.x += gPos.y * sideLength;
        return result;
    }

    public List<TileNode> FindNodesByType(NType type)
    {
        List<TileNode> nodes = new List<TileNode>();
        if (Application.isPlaying)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (m[i, j].type == type) nodes.Add(m[i, j]);
                }
            }
        }
        else
        {
            TileNode[] allNodes = GetComponentsInChildren<TileNode>(true);
            for (int i = 0; i < allNodes.Length; i++)
            {
                if (allNodes[i].type == type) nodes.Add(allNodes[i]);
            }
        }
        return nodes;
    }

}
