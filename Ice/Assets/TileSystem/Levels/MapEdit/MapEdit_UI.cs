using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainLogic.UI
{
    public class MapEdit_UI : Basic_UI
    {
        public Text posText;

        public static MapEdit_UI PUBLIC;
        private void Awake()
        {
            PUBLIC = this;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit = Click(QuadMap.nodeLayer);
                if (hit.transform)
                {
                    CurrentNode = hit.transform.GetComponent<TileNode>();
                    CurrentNode.GetComponent<SpriteRenderer>().sprite = MapEdit_Panel.CurrPanel.mySprite;
                    CurrentNode.type = MapEdit_Panel.CurrPanel.nodeType;
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit = Click(QuadMap.nodeLayer);
                if (hit.transform)
                {
                    ResetHighLight();

                    CurrentNode = hit.transform.GetComponent<TileNode>();
                    highLightNodes.Add(CurrentNode);
                    highLightNodes.AddRange(CurrentNode.ConnectedNodes());

                    HighLight();
                }
            }
            if (CurrentNode)
            {
                posText.text = CurrentNode.pos.ToString();
                if (QuadMap.PUBLIC.Find(CurrentNode.pos, false) == null) posText.text = "null";
            }

        }
    }
}