using UnityEngine;

namespace FogOfWar
{
    public abstract class FOWAbstractRevealer : MonoBehaviour
    {
        public bool isEnabled = true;
        public float radius = 2f;
        public Vector3 worldPos;

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            FOWSystem.AddRevealer(this);
        }

        protected abstract void OnAwake();

        void OnDrawGizmosSelected()
        {
            Matrix4x4 m44 = Matrix4x4.identity;
            m44.SetColumn(3, transform.position);

            Gizmos.matrix = m44;
            Gizmos.color = new Color(0, 0.6f, 1, 0.7f);
            // 绘制圆环
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f)
            {
                float x = radius * Mathf.Cos(theta);
                float z = radius * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z);
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }
                beginPoint = endPoint;
            }

            // 绘制最后一条线段
            Gizmos.DrawLine(firstPoint, beginPoint);
        }


        private void OnDestroy()
        {
            FOWSystem.DelRevealer(this);
        }
    }
}
