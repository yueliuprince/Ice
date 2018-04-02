using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Basic3D
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    [RequireComponent(typeof(PlayerActor))]
    public sealed class PlayerControl : BaseCtrl
    {
        private PlayerActor m_Actor;
        public Signal outSignal;

        private Vector3 gravityOffset, standardGravityAxis;

        private _AndroidInput m_AndroidInput;
        private Transform cameraTrans;

        private Vector3 blueAxis, redAxis;
        private void Start() {
            m_Actor = GetComponent<PlayerActor>();

            m_AndroidInput = _AndroidInput.PUBLIC;

            cameraTrans = Camera.main.transform;

            isLocked = true;
            Invoke("UnLockSelf", P.PUBLIC.startEffectDuration);

            ResetGravityController();
        }

        void UnLockSelf() { isLocked = false; }

        private Vector3 dir;
        private Vector2 inputAxis;

        private void Update() {
            blueAxis = cameraTrans.forward;
            redAxis = cameraTrans.right;
            blueAxis.y = 0;
            redAxis.y = 0;

            if (isLocked) return;

            inputAxis = m_AndroidInput.GradientAxis;
            gravityOffset = Input.acceleration - standardGravityAxis;
            //P.PUBLIC.debugText2.text = gravityOffset.ToString();

            //重力在本地坐标系的向量为standard，当坐标系旋转后，变成Input.acceleration
            //坐标系所有点乘以旋转矩阵mat，Input.acceleration乘以mat的逆矩阵等于standard

            //Matrix4x4 rotateMatrix = STL.Geometry.Math.CalcRotateMatrix(Input.acceleration, standardGravityAxis);

            Vector3 rotateAxis = Vector3.Cross(Input.acceleration, standardGravityAxis);
            float angle = Vector3.Angle(Input.acceleration, standardGravityAxis);

            //m_rigidbody.MoveRotation
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, rotateAxis), 0.05f);
            m_Actor.GoForward();

            dir = inputAxis.x * redAxis + inputAxis.y * blueAxis;
            /*
            if (_AndroidInput.wantFly) {
                dir.y += 1;
            }
            else if (dir.y > Mathf.Epsilon) {
                dir.y -= 0.5f;
            }

            m_Actor.Fly(dir);*/
            //m_Actor.Move(dir);


            if (_AndroidInput.wantReset) ResetGravityController();
        }

        public void ResetGravityController() {
            gravityOffset = standardGravityAxis = Input.acceleration;
        }

    }
}
