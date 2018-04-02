using UnityEngine;
using System.Collections;

public class Reflection : MonoBehaviour
{
    private Camera refCamera;
    private Material refMat;
    // Use this for initialization
    void Start()
    {
        if (refCamera == null)
        {
            GameObject go = new GameObject("refCamera");
            refCamera = go.AddComponent<Camera>();
            refCamera.CopyFrom(Camera.main);
            refCamera.enabled = false;
            refCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Water"));
        }

        refMat = GetComponent<Renderer>().sharedMaterial;
        RenderTexture refTexture = new RenderTexture(Mathf.FloorToInt(Camera.main.pixelWidth), Mathf.FloorToInt(Camera.main.pixelHeight), 24);
        refTexture.hideFlags = HideFlags.DontSave;    
        refCamera.targetTexture = refTexture;
        refCamera.targetTexture.wrapMode = TextureWrapMode.Repeat;
    }

    public void OnWillRenderObject()
    {
        RenderRefection();
    }

    void RenderRefection()
    {
        Vector3 normal = transform.forward;
        float d = -Vector3.Dot(normal, transform.position);
        Matrix4x4 refMatrix = new Matrix4x4
        {
            m00 = 1 - 2 * normal.x * normal.x,
            m01 = -2 * normal.x * normal.y,
            m02 = -2 * normal.x * normal.z,
            m03 = -2 * d * normal.x,

            m10 = -2 * normal.x * normal.y,
            m11 = 1 - 2 * normal.y * normal.y,
            m12 = -2 * normal.y * normal.z,
            m13 = -2 * d * normal.y,

            m20 = -2 * normal.x * normal.z,
            m21 = -2 * normal.y * normal.z,
            m22 = 1 - 2 * normal.z * normal.z,
            m23 = -2 * d * normal.z,

            m30 = 0,
            m31 = 0,
            m32 = 0,
            m33 = 1
        };

        refCamera.worldToCameraMatrix = Camera.main.worldToCameraMatrix * refMatrix;
        refCamera.transform.position = refMatrix.MultiplyPoint(Camera.main.transform.position);

        Vector3 forward = Camera.main.transform.forward;
        Vector3 up = Camera.main.transform.up;
        forward = refMatrix.MultiplyPoint(forward);
        refCamera.transform.forward = forward;

        GL.invertCulling = true;
        refCamera.Render();
        GL.invertCulling = false;
     
        refMat.SetTexture("_RefTexture", refCamera.targetTexture);
    }
}
