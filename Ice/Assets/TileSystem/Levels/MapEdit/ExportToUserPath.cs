using UnityEngine;
using UnityEngine.UI;

public class ExportToUserPath : MonoBehaviour
{

    public InputField path;
    public EditorQuadMap map;

    private void Awake()
    {
        path.text = System.Environment.CurrentDirectory;
    }

    public void OnClick()
    {
        if (path.text == "") return;
#if !UNITY_EDITOR
        map.ExportToPath(path.text);
#endif
        transform.parent.gameObject.SetActive(false);
    }
}
