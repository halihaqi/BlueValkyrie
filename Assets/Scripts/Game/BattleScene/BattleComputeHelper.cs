using Game.BattleScene.BattleRole;
using UnityEngine;

namespace Game.BattleScene
{
    public static class BattleComputeHelper
    {
        public static int Atk(BattleRoleState atker, BattleRoleState defer)
        {
            //闪避成功
            int rad = Random.Range(1, 101);
            if (rad <= defer.agi) return 0;
            
            int hit = atker.atk - defer.def;
            hit = Mathf.Clamp(hit, 0, 99999);
            return hit;
        }
    }
}