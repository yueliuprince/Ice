using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FogOfWar;

public class CreateFOW : MonoBehaviour
{

    [MenuItem("xiao_D/FogOfWar/CreateSystem")]
    private static void NewFOWSystem()
    {
        if (FindObjectOfType<FOWSystem>()) return;
        Object prefabs = Resources.Load("FOWSystem");
        if (prefabs != null)
        {
            GameObject go = Instantiate(prefabs) as GameObject;
            go.name = "FOWSystem";
        }
    }

}
