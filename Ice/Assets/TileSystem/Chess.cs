using System.Collections.Generic;
using UnityEngine;

public enum Group
{
    orange = 1,        //player 0
    green = 2,         //player 1
    blue = -1,         // enemy 0
    purple = -2,       // enemy 1
    neutral = 0,       // neutral
}

public enum CType
{
    Player,
    Obstacle,
}

/// <summary>
/// The base class of chess
/// </summary>
[DefaultExecutionOrder(-100)]
public class Chess : MonoBehaviour, IEffect_Selected
{
    public CType type;
    public Group group = Group.orange;
    public TileNode mainNode;

    public Vector2Int NodePos { get { return mainNode.pos; } }
    public Vector3 WorldPos { get { return transform.position; } }

    private SpriteRenderer baseRenderer;
    protected Vector3 PlanePos(TileNode node)
    {
        Vector3 tPos = node.WorldPos;
        tPos.y = WorldPos.y;
        return tPos;
    }

    private void Awake()
    {
        baseRenderer = GetComponent<SpriteRenderer>();
        OnAwake();
    }
    protected virtual void OnAwake() { }


    /// <summary>
    /// Return true after the node changed
    /// </summary>
    public bool SetMainNode(TileNode node)
    {
        if (node != mainNode) LeaveCurrentNode();
        else return false;
        mainNode = node;
        if (mainNode && mainNode.chess == null) mainNode.chess = this;
        return true;
    }
    public void LeaveCurrentNode()
    {
        if (mainNode && mainNode.chess == this) mainNode.Clear();
    }

    public bool IsOpposite(Chess c)
    {
        return ((int)group * (int)c.group < 0);
    }

    /// <summary>
    /// Get all nodes the chess can move to,return null if the chess can't move
    /// </summary>
    public List<TileNode> ConnectedNodes()
    {
        return mainNode.ConnectedNodes();
    }

    public void Effect_Selected()
    {
        baseRenderer.color = new Color(baseRenderer.color.r, baseRenderer.color.g, baseRenderer.color.b, 0.5f);
    }

    public void Effect_UnSelected()
    {
        baseRenderer.color = new Color(baseRenderer.color.r, baseRenderer.color.g, baseRenderer.color.b, 1f);
    }
}
