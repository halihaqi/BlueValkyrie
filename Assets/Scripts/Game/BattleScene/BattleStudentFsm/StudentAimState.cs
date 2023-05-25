using System;
using System.Collections;
using DG.Tweening;
using Game.Entity;
using Game.UI.Battle;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene.BattleStudentFsm
{
    public class StudentAimState : FsmState<BattleStudentEntity>
    {
        private const float AIM_SENSITIVE = 0.7f;
        
        private BattleStudentEntity _role;
        private IFsm<BattleStudentEntity> _fsm;
        private BattleMaster _bm;
        
        private float _camTargetYaw;
        private float _camTargetPitch;
        private bool _isShooting = false;
        
        protected internal override void OnEnter(IFsm<BattleStudentEntity> fsm)
        {
            base.OnEnter(fsm);
            _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _role = fsm.Owner;
            _fsm = fsm;
            _role.IsControl = false;
            _role.anim.SetBool(BattleStudentEntity.BattleAim, true);
            _camTargetYaw = _role.followTarget.eulerAngles.y;
            _role.thirdPersonCam.sensitive = BattleRoleEntity.AIM_CAM_SENSITIVE;
            _role.thirdPersonCam.camDistance = BattleRoleEntity.AIM_CAM_DIS;
            DOTween.To(() => _role.thirdPersonCam.shoulderOffset.x,
                val => _role.thirdPersonCam.shoulderOffset.x = val,
                BattleRoleEntity.AIM_CAM_OFFSET_X, 0.5f);
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
        }

        protected internal override void OnUpdate(IFsm<BattleStudentEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if(_isShooting) return;
            RotateRole();
            Raycast();
        }

        protected internal override void OnLeave(IFsm<BattleStudentEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _role.IsControl = true;
            _bm.BattlePanel.HideTargetHp();
            _bm = null;
            _role.anim.SetBool(BattleStudentEntity.BattleAim, false);
            _role.followTarget.transform.rotation = Quaternion.identity;
            _role.transform.rotation = Quaternion.Euler(0, _camTargetYaw, 0);
            _role.thirdPersonCam.sensitive = BattleRoleEntity.ORI_CAM_SENSITIVE;
            _role.thirdPersonCam.camDistance = BattleRoleEntity.ORI_CAM_DIS;
            DOTween.To(() => _role.thirdPersonCam.shoulderOffset.x,
                val => _role.thirdPersonCam.shoulderOffset.x = val,
                BattleRoleEntity.ORI_CAM_OFFSET_X, 0.5f);
            EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
        }

        private void RotateRole()
        {
            var input = InputMgr.Instance.InputMove;
            _camTargetYaw += input.x * AIM_SENSITIVE;
            _camTargetPitch += -input.y * AIM_SENSITIVE;
            _camTargetYaw = TransformUtils.ClampAngle(_camTargetYaw, float.MinValue, float.MaxValue);
            _camTargetPitch = TransformUtils.ClampAngle(_camTargetPitch, -15, 15);
            _role.transform.rotation = Quaternion.Euler(_camTargetPitch, _camTargetYaw, 0);
            _role.followTarget.transform.rotation = _role.transform.rotation;
        }

        private void Raycast()
        {
            var ray = _role.followCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, _role.AtkRange, ~0, QueryTriggerInteraction.Ignore))
            {
                _role.Target = _bm.GetEnemy(hit.collider.gameObject);
                
                _bm.BattlePanel.SwitchArrow(_role.Target != null);
                if(_role.Target != null)
                    _bm.BattlePanel.ShowTargetHp(_role.Target);
                else
                    _bm.BattlePanel.HideTargetHp();
            }
            else
            {
                _bm.BattlePanel.SwitchArrow(false);
                _bm.BattlePanel.HideTargetHp();
            }
            
            //Debug
            Debug.DrawLine(ray.origin, ray.direction * _role.AtkRange + ray.origin);
        }

        private void OnKeyDown(KeyCode key)
        {
            if (key == KeyCode.F)
            {
                MonoMgr.Instance.StartCoroutine(ShootCoroutine());
                EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnKeyDown);
            }

            if (key == KeyCode.Q)
            {
                ChangeState<StudentRestState>(_fsm);
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            }
        }

        private IEnumerator ShootCoroutine()
        {
            _isShooting = true;
            if(_role.Target != null)
                _bm.BattlePanel.ShowTargetHp(_role.Target);
            else
                _bm.BattlePanel.HideTargetHp();
            
            yield return new WaitForSeconds(1);
            _role.anim.SetTrigger(BattleStudentEntity.BattleShoot);

            EventMgr.Instance.AddListener(ClientEvent.BATTLE_BULLET_SHOOT, Atk);
            yield return new WaitForSeconds(2);
            EventMgr.Instance.RemoveListener(ClientEvent.BATTLE_BULLET_SHOOT, Atk);

            _role.anim.SetTrigger(BattleStudentEntity.BattleReload);

            yield return new WaitForSeconds(2.5f);
            ChangeState<StudentRestState>(_fsm);
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
            _isShooting = false;
        }

        //数据层atk
        private void Atk()
        {
            _role.SubAmmo();
            if (_role.Target == null) return;
            
            _role.FireEntity.SetTarget(_role.Target.followTarget);
            _bm.Atk(true, false, _role.RoleIndex, _role.Target.RoleIndex);
        }
    }
}