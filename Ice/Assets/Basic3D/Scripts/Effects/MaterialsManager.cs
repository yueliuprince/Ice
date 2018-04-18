using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour {

    public static MaterialsManager PUBLIC;
    private void Awake()
    {
        PUBLIC = this; 
    }
    public Material block_Normal;
    public Material block_Transparent;
    public Material block_Highlight;
    public Material block_CannotPlace;
}
