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
    untagged = -1,
    boom = 0, flag = 1,
    two, three, four, five, six, seven,         //remove "8" because it seems like infinite
    nine = 9,
    infinite,
}

/// <summary>
/// The base class of chess
/// </summary>
[DefaultExecutionOrder(-100)]
public class Chess : MonoBehaviour, IEffect_Selected
{
    public CType type = CType.two;
    public Group group = Group.orange;
    public TileNode myNode;

    public Vector2Int NodePos { get { return myNode.pos; } }

    private SpriteRenderer baseRenderer;

    private void Awake()
    {
        baseRenderer = GetComponent<SpriteRenderer>();
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
        return myNode.ConnectedNodes();
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
