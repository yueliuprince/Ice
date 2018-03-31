using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateFOW : MonoBehaviour
{

    [MenuItem("xiao_D/FogOfWar/CreateFOWSystem")]
    private static void NewFOWSystem() {
        GameObject root = new GameObject("FOWRenderRoot");
        root.AddComponent<FOWSystem>();
        root.transform.position = Vector3.zero;
    }


    [MenuItem("xiao_D/FogOfWar/CreateRender")]
    private static void CreateRender() {
        Transform parent = GameObject.Find("FOWRenderRoot").transform;
        if (parent == null) return;

        Object prefabs = Resources.Load("Prefabs/projector");
        if (prefabs != null) {
            GameObject projector = Instantiate(prefabs) as GameObject;
            if (projector != null) {
                projector.transform.parent = parent;
                projector.transform.position = parent.position + (Vector3.up * 64);
                FOWRender render = projector.gameObject.AddComponent<FOWRender>();
                render.GetComponent<Projector>().orthographicSize = FOWSystem.Instance.worldSize * 0.5f;
                render.gameObject.SetActive(false);
            }
        }
    }
}
