using Game.Entity;
using Hali_Framework;

namespace Game.BattleScene.BattleEnemyFsm
{
    public class EnemyRestState : FsmState<BattleEnemyEntity>
    {
        private BattleEnemyEntity _enemy;
        
        protected internal override void OnEnter(IFsm<BattleEnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            _enemy = fsm.Owner;
            _enemy.anim.SetBool(BattleEnemyEntity.BattleMove, false);
            _enemy.anim.SetBool(BattleEnemyEntity.BattleAim, false);
            _enemy.Agent.isStopped = true;
        }
    }
}