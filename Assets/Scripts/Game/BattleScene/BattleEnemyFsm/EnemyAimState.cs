using System.Collections;
using DG.Tweening;
using Game.Entity;
using Game.UI.Battle;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene.BattleEnemyFsm
{
    public class EnemyAimState : FsmState<BattleEnemyEntity>
    {
        private BattleEnemyEntity _enemy;
        private IFsm<BattleEnemyEntity> _fsm;
        private BattleMaster _bm;

        protected internal override void OnEnter(IFsm<BattleEnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _enemy = fsm.Owner;
            _fsm = fsm;
            _enemy.anim.SetBool(BattleEnemyEntity.BattleMove, false);
            _enemy.anim.SetBool(BattleEnemyEntity.BattleAim, true);
            _enemy.Agent.isStopped = true;
            _enemy.thirdPersonCam.sensitive = BattleRoleEntity.AIM_CAM_SENSITIVE;
            _enemy.thirdPersonCam.camDistance = BattleRoleEntity.AIM_CAM_DIS;
            DOTween.To(() => _enemy.thirdPersonCam.shoulderOffset.x,
                val => _enemy.thirdPersonCam.shoulderOffset.x = val,
                BattleRoleEntity.AIM_CAM_OFFSET_X, 0.5f);
            _bm.BattlePanel.ShowTargetHp(_enemy.Target);

            MonoMgr.Instance.StartCoroutine(ShootCoroutine());
        }

        protected internal override void OnLeave(IFsm<BattleEnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _bm.BattlePanel.HideTargetHp();
            _enemy.anim.SetBool(BattleEnemyEntity.BattleAim, false);
            _enemy.thirdPersonCam.sensitive = BattleRoleEntity.ORI_CAM_SENSITIVE;
            _enemy.thirdPersonCam.camDistance = BattleRoleEntity.ORI_CAM_DIS;
            DOTween.To(() => _enemy.thirdPersonCam.shoulderOffset.x,
                val => _enemy.thirdPersonCam.shoulderOffset.x = val,
                BattleRoleEntity.ORI_CAM_OFFSET_X, 0.5f);
        }

        private IEnumerator ShootCoroutine()
        {
            //先转到朝向目标
            _enemy.Agent.updateRotation = false;
            _enemy.transform.DOLookAt(_enemy.Target.transform.position, 0.5f);
            yield return new WaitForSeconds(1);
            _enemy.Agent.updateRotation = true;
            _enemy.anim.SetTrigger(BattleEnemyEntity.BattleShoot);
            
            EventMgr.Instance.AddListener(ClientEvent.BATTLE_BULLET_SHOOT, Atk);
            yield return new WaitForSeconds(2);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_BULLET_SHOOT, Atk);

            _enemy.anim.SetTrigger(BattleEnemyEntity.BattleReload);

            yield return new WaitForSeconds(2);
            ChangeState<EnemyRestState>(_fsm);
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
        }

        //逻辑层攻击
        private void Atk()
        {
            _enemy.SubAmmo();
            bool isCrit = Random.Range(0, 100) > 70;
            _bm.Atk(isCrit, true, _enemy.RoleIndex, _enemy.Target.RoleIndex);
        }
    }
}