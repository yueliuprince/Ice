using UnityEngine;

/// <summary>
/// 说明：静态视野对象
/// 
/// @by xiao_D 2017-12-14
/// </summary>

namespace FogOfWar
{
    public sealed class FOWStaticRevealer : FOWAbstractRevealer
    {
        protected override void OnAwake()
        {
            worldPos = transform.position;
        }
    }
}