using UnityEngine;

namespace FogOfWar
{
    public class FOWRender : MonoBehaviour
    {
        // 这里设置迷雾颜色
        public Color unexploredColor = new Color(0f, 0f, 0f, 250f / 255f);
        public Color exploredColor = new Color(0f, 0f, 0f, 200f / 255f);
        Material mMat;

        void Start()
        {
            mMat = GetComponentInChildren<Projector>().material;
            if (mMat == null)
            {
                enabled = false;
                return;
            }
        }

        public void Activate(bool active)
        {
            gameObject.SetActive(active);
        }

        public bool IsActive {
            get {
                return gameObject.activeSelf;
            }
        }

        void Update()
        {
            if (mMat != null && FOWSystem.PUBLIC.FogTexture != null)
            {
                mMat.SetTexture("_MainTex", FOWSystem.PUBLIC.FogTexture);
                mMat.SetFloat("_BlendFactor", FOWSystem.PUBLIC.blendFactor);
                if (FOWSystem.PUBLIC.enableFog)
                {
                    mMat.SetColor("_Unexplored", unexploredColor);
                }
                else
                {
                    mMat.SetColor("_Unexplored", exploredColor);
                }
                mMat.SetColor("_Explored", exploredColor);
            }
        }
    }
}