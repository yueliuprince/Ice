using System.Collections;
using UnityEngine;

namespace Basic3D
{
    [RequireComponent(typeof(PlayerActor))]
    [RequireComponent(typeof(PlayerControl))]
    public sealed class PlayerState : BaseState
    {
        public float hitDuration = 0.4f;
        private int starNumber = 0;
        //private PlayerControl m_Controller;

        public int StarNumber {
            get { return starNumber; }
            set {
                starNumber = value;
            }
        }

        // Use this for initialization
        void Start() {
            base.Init();
            //m_Controller = GetComponent<PlayerControl>();
            //InvokeRepeating("FogChecking", 2f, 0.5f);
        }


        //void FogChecking() {
        //    if (FOWSystem.PUBLIC.IsVisible(transform.position) == false) Change_HP(-10f, false);
        //}

        public override void OnDeath() {
            this.gameObject.SetActive(false);
            CancelInvoke();
            P.PUBLIC.ReLoadCurrentScene();
        }

    }
}