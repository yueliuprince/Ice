//#define SMOOTH_DAMP_ENABLED
using UnityEngine;

namespace Basic3D
{
    [RequireComponent(typeof(Camera))]
    public class SmoothTracking : MonoBehaviour
    {
        public Transform target;
#if SMOOTH_DAMP_ENABLED
        public float smoothDampTime = 0.2f;
        private Vector3 _smoothDampVelocity;
#endif
        private Vector3 targetPos;
        private Vector3 targetPrePos;


        void Start()
        {
            if (target == null)
            {
                Q.WarningPrint(transform, this.GetType().ToString());
                return;
            }

            targetPos = target.transform.position;
            targetPrePos = target.transform.position;
        }

        void LateUpdate()
        {
            targetPos = target.transform.position;
            if (targetPos != targetPrePos)
            {
                Vector3 change = targetPos - targetPrePos;
                
#if SMOOTH_DAMP_ENABLED
                transform.position = Vector3.SmoothDamp(transform.position, transform.position + change, ref _smoothDampVelocity, smoothDampTime);
#else
                transform.position += change;
#endif
            }

            targetPrePos = targetPos;
        }

    }
}