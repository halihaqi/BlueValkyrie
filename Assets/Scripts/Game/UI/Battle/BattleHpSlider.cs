using Game.BattleScene;
using Game.BattleScene.BattleRole;
using Game.Entity;
using Hali_Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Battle
{
    public class BattleHpSlider : PanelBase
    {
        private Slider _sldHp;
        private IBattleRole _target;
        private float _lerpSpeed = 3;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _sldHp = GetControl<Slider>("sld_hp");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            if (userData is IBattleRole role)
            {
                _target = role;
            }

            _sldHp.maxValue = _target.RoleState.maxHp;
            _sldHp.value = _target.CurHp;
            var cam = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner.followCam.Camera;
            var targetPos = cam.WorldToScreenPoint(_target.FollowTarget.position + Vector3.up * 3);
            ((RectTransform)this.transform).position = targetPos;
        }

        protected internal override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            _sldHp.value = Mathf.Lerp(_sldHp.value, _target.CurHp, Time.deltaTime * _lerpSpeed);
        }
    }
}