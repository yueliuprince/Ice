using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_UI : MonoBehaviour
{
    protected RaycastHit2D Touch2D(Vector2 screenPoint, LayerMask touchLayer)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        return Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, touchLayer);
    }

    protected Vector3 InputWorldPositon()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
#else
        return Camera.main.ScreenToWorldPoint(Input.touches[0].position);
#endif
    }

    protected Vector3 InputScreenPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#else
        return Input.touches[0].position;
#endif
    }

    protected bool Touch(Vector3 screenPoint, out RaycastHit hit, LayerMask touchLayer)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, touchLayer);
    }
}
