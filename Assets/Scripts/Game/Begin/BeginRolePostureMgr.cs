using System;
using System.Collections.Generic;
using Hali_Framework;
using UnityEngine;

namespace Game.Begin
{
    public class BeginRolePostureMgr : SingletonMono<BeginRolePostureMgr>
    {
        [SerializeField]
        private Transform beginPos;
        [SerializeField]
        private Transform bornPos;
        [SerializeField]
        private Transform showPos;
        [SerializeField]
        private Transform hidePos;

        private RoleChooseEntity _hidingRole;
        private RoleChooseEntity _curRole;
        private Dictionary<int, RoleChooseEntity> _cachedRoles;
        private bool _isChanging = false;
        
        public bool IsChanging => _isChanging;
        
        public Transform BornPos => bornPos;

        public Transform ShowPos => showPos;

        public Transform HidePos => hidePos;

        protected override void Awake()
        {
            base.Awake();
            
            //设置开始人物动画
            var animators = beginPos.GetComponentsInChildren<Animator>();
            for (var i = 0; i < animators.Length; i++)
                animators[i].SetLayerWeight(1, 1);

            _cachedRoles = new Dictionary<int, RoleChooseEntity>();
        }

        private void BornRole(int roleId)
        {
            if (_cachedRoles.ContainsKey(roleId))
            {
                _cachedRoles[roleId].gameObject.SetActive(true);
                _cachedRoles[roleId].ShowMe(() => { _isChanging = false;});
                return;
            }
            
            var roles = BinaryDataMgr.Instance.GetTable<RoleInfoContainer>();
            if (roles.dataDic.TryGetValue(roleId, out var roleInfo))
            {
                string path = $"Prefabs/Students/{roleInfo.school}/{roleInfo.name}";
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.RES_GROUP_BEGIN, path, obj =>
                {
                    if (obj == null)
                        throw new Exception($"Resources load path:{path} is invalid.");
                    obj.transform.SetParent(bornPos, false);
                    _curRole = obj.AddComponent<RoleChooseEntity>();
                    _curRole.SetRoleInfo(roleInfo);
                    _curRole.ShowMe(() => { _isChanging = false;});
                });
            }
            else
            {
                _isChanging = false;
                Debug.LogError($"RoleInfo table has no member with id:{roleId}.");
                return;
            }
        }

        /// <summary>
        /// 根据角色id切换展示角色
        /// </summary>
        /// <param name="roleId"></param>
        public void ChangeRole(int roleId)
        {
            if(_isChanging) return;
            _isChanging = true;
            EventMgr.Instance.TriggerEvent(ClientEvent.ROLE_CHANGE);
            if (_curRole != null)
            {
                _hidingRole = _curRole;
                _hidingRole.HideMe(() =>
                {
                    if(!_cachedRoles.ContainsKey(_hidingRole.RoleInfo.id))
                        _cachedRoles.Add(_hidingRole.RoleInfo.id, _hidingRole);
                    _hidingRole.gameObject.SetActive(false);
                });
            }

            BornRole(roleId);
        }

        public void ResetRole()
        {
            if (_curRole != null)
            {
                if(!_cachedRoles.ContainsKey(_curRole.RoleInfo.id))
                    _cachedRoles.Add(_curRole.RoleInfo.id, _curRole);
                _curRole.ResetMe();
                _curRole.gameObject.SetActive(false);
                _curRole = null;
            }

            if (_hidingRole != null)
            {
                if(!_cachedRoles.ContainsKey(_hidingRole.RoleInfo.id))
                    _cachedRoles.Add(_hidingRole.RoleInfo.id, _hidingRole);
                _hidingRole.ResetMe();
                _hidingRole.gameObject.SetActive(false);
                _hidingRole = null;
            }

            _isChanging = false;
        }

        public void DisposeRes()
        {
            foreach (var role in _cachedRoles.Values)
            {
                Destroy(role.gameObject);
            }
            _cachedRoles.Clear();
        }
    }
}