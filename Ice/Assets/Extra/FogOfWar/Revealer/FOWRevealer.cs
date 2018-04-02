using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 说明：动态视野对象
/// 
/// @by xiao_D 2017-12-14
/// </summary>

namespace FogOfWar
{
    [DefaultExecutionOrder(100)]
    public sealed class FOWRevealer : FOWAbstractRevealer
    {
        protected override void OnAwake()
        {
        }

        private void Update()
        {
            worldPos = transform.position;
        }
    }
}
