using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STL.Geometry;
using DG.Tweening;

public class PlayerChess : Chess
{
    private Rigidbody m_Rigidbody;
    public static PlayerChess PUBLIC;
    public TileNode nextNode;
    public float acceleration = 1f;
    public float maxSpeed = 3f;
    public Vector2Int currentDir = Vector2Int.zero;
    private float sqrMaxSpeed;

    protected override void OnAwake()
    {
        PUBLIC = this;
        m_Rigidbody = GetComponent<Rigidbody>();
        sqrMaxSpeed = maxSpeed * maxSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (nextNode == null)
        {
            Vector2Int inputAxis = _AndroidInput.PUBLIC.AbsAxisFor_4Dir;
            currentDir = Vector2Int.zero;

            currentDir.x = -inputAxis.y;
            currentDir.y = inputAxis.x;
            if (currentDir != Vector2Int.zero)
                nextNode = QuadMap.PUBLIC.Find(mainNode.pos + currentDir);
        }
    }

    private void FixedUpdate()
    {
        UpdateCurrentNode();
        DealWithNext();
    }

    private Tweener tweener;
    private void DealWithNext()
    {
        if (tweener != null && tweener.IsActive()) return;

        if (nextNode == null || (nextNode.chess != null && nextNode.chess != this))
        {
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        Vector3 dir3D = QuadMap.PUBLIC.GetWorldDir(currentDir);
        Vector3 nextVelocity = m_Rigidbody.velocity + dir3D * acceleration * Time.fixedDeltaTime;
        if (nextVelocity.sqrMagnitude > sqrMaxSpeed)
        {
            nextVelocity = nextVelocity.normalized * maxSpeed;
        }
        m_Rigidbody.velocity = nextVelocity;

        switch (mainNode.type)
        {
            case NType.Basic:
                {
                    if (CheckEpsion())
                    {
                        m_Rigidbody.velocity = Vector3.zero;
                        m_Rigidbody.MovePosition(PlanePos(nextNode));
                        nextNode = null;
                    }
                    break;
                }
            case NType.Quick:
                {
                    nextNode = QuadMap.PUBLIC.Find(mainNode.pos + currentDir);
                    break;
                }
        }

        if (nextNode == null || (nextNode.chess != null && nextNode.chess != this))
        {
            float dur = Math.PlaneDistance(WorldPos, mainNode.WorldPos) / Mathf.Clamp(m_Rigidbody.velocity.magnitude, maxSpeed * 0.32f, maxSpeed) * 1.2f;
            tweener = m_Rigidbody.DOMove(PlanePos(mainNode), dur).OnComplete(() => { nextNode = null; });
            Debug.Log("tween!");
            return;
        }
    }

    private bool CheckEpsion(float sqrDistance = -1)
    {
        if (nextNode == null) return true;
        if (sqrDistance == -1) sqrDistance = m_Rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime * Time.fixedDeltaTime;
        if (Math.SqrPlaneDistance(WorldPos, nextNode.WorldPos) <= sqrDistance)
        {
            return true;
        }
        else return false;
    }

    private void UpdateCurrentNode()
    {
        RaycastHit hit;
        if (Physics.Raycast(WorldPos, Vector3.down, out hit, Mathf.Infinity, QuadMap.nodeLayer))
        {
            TileNode tNode = hit.transform.GetComponent<TileNode>();
            bool nodeChange = SetMainNode(tNode);
#if UNITY_EDITOR
            if (nodeChange) Debug.Log(mainNode.pos);
#endif
        }
    }
}
