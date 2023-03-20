using System.Collections.Generic;
using Game.BattleScene;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class FireEntity : MonoBehaviour
    {
        public List<Transform> firePos;
        private BattleRoleEntity _role;
        private bool _isEnemy;
        private Transform _target;

        public void SetOwner(BattleRoleEntity owner)
        {
            _isEnemy = owner.IsEnemy;
            _role = owner;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }


        public void OnFire()
        {
            //生成发射特效和子弹
            if (firePos == null || firePos.Count <= 0) return;
            
            //生成子弹
            foreach (var pos in firePos)
            {
                ObjectPoolMgr.Instance.PopObj(BattleConst.BULLET_PATH, obj =>
                {
                    var bullet = obj.GetComponent<BulletEntity>();
                    bullet.SetTarget(_role, pos, _target);
                });
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_BULLET_SHOOT);
            }
        }
    }
}