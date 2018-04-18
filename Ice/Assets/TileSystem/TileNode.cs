using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NType
{
    Basic,
    Quick,
    Other,
    Obstacle,
    NULL,
}

[DefaultExecutionOrder(-100)]
public class TileNode : MonoBehaviour, IEffect_Selected
{

    private SpriteRenderer myRenderer;
    static Vector2Int[] dir = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    public NType type = NType.Basic;
    public Vector2Int pos;
    public Vector3 WorldPos { get { return transform.position; } }
    public Chess chess;


    private List<TileNode> cNodes = new List<TileNode>();     //temp
    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Check a pos whether the player can move to
    /// </summary>
    /// <param name="fliter">Define the vaild types,if you don't provide,that means all.</param>
    private TileNode CheckPos(Vector2Int pos, params NType[] fliter)
    {
        TileNode node = QuadMap.PUBLIC.Find(pos);
        if (node != null)
        {
            if (fliter != null && fliter.Length > 0)
            {
                for (int i = 0; i < fliter.Length; i++) if (fliter[i] == node.type) goto VAILD;
                return null;
            }

            VAILD:
            cNodes.Add(node);
        }
        return node;
    }


    /// <summary>
    /// return a node list which the chess can move to(commend using by player rather than AI)
    /// </summary>
    public List<TileNode> ConnectedNodes()
    {
        cNodes.Clear();
        Vector2Int leftDown = pos - Vector2Int.one;
        Vector2Int rightUp = pos + Vector2Int.one;

        switch (type)
        {
            case NType.NULL: return cNodes;
            case NType.Basic:
            case NType.Other:
            case NType.Obstacle:
                {
                    for (int i = 0; i < 4; i++) CheckPos(pos + dir[i]);
                    CheckPos(leftDown, NType.Other);
                    CheckPos(leftDown + Vector2Int.up * 2, NType.Other);
                    CheckPos(rightUp, NType.Other);
                    CheckPos(leftDown + Vector2Int.right * 2, NType.Other);
                    break;
                }

            case NType.Quick:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int nextPos = pos + dir[i];
                        TileNode node = CheckPos(nextPos);
                        if (node != null && node.chess == null && node.type == NType.Quick)
                        {
                            do
                            {
                                nextPos += dir[i];
                                node = CheckPos(nextPos, NType.Quick);
                            }
                            while (node != null && node.chess == null);
                        }
                    }
                    break;
                }
        }
        return cNodes;
    }



    /// <summary>
    /// Reset the node to empty
    /// </summary>
    public void Clear()
    {
        chess = null;
    }


    public void ResetCurrentChess(Chess c)
    {
        if (c == null) return;
        this.chess = c;
        chess.mainNode = this;

        Vector3 alignPos = transform.position;
        alignPos.y = c.transform.position.y;
        c.transform.position = alignPos;
    }


    public void AlignToThis(Transform t)
    {
        t.position = new Vector3(transform.position.x, t.position.y, transform.position.z);
    }

    public void Effect_Selected()
    {
        myRenderer.color = new Color(1, 1, 1, 0.85f);
    }

    public void Effect_UnSelected()
    {
        myRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }
}
