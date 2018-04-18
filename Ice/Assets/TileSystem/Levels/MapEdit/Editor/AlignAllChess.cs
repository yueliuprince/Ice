using UnityEditor;
using UnityEngine;

public class AlignAllChess : ScriptableWizard
{
    public LayerMask nodeLayer;

    [MenuItem("xiao_D/TileSystem/Aligh All Chess")]
    static void CreateWizerd()
    {
        DisplayWizard("Aligh All Chess", typeof(AlignAllChess), "Align");
    }

    private void OnEnable()
    {
        nodeLayer = LayerMask.GetMask("tNode");
    }

    private void OnWizardCreate()
    {
        Chess[] cs = FindObjectsOfType<Chess>();
        TileNode[] ns = FindObjectsOfType<TileNode>();
        for (int i = 0; i < ns.Length; i++) ns[i].Clear();

        RaycastHit hit;
        for (int i = 0; i < cs.Length; i++)
        {

            bool result = Physics.Raycast(cs[i].transform.position + Vector3.up * 5, Vector3.down, out hit, Mathf.Infinity, nodeLayer);
            if (result)
            {
                cs[i].mainNode = hit.transform.GetComponent<TileNode>();
                cs[i].mainNode.chess = cs[i];

                cs[i].mainNode.ResetCurrentChess(cs[i]);
            }
            else
            {
                Debug.LogWarning("A chess has aligning failed.", hit.transform);
            }
        }

        Debug.Log("You should click [Save Scene As] and override the previous file,or the change maybe lost.");
        AssetDatabase.SaveAssets();
    }
}
