﻿using System;
using Game.Model;
using Hali_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Game.BattleScene.BattleRole
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class BattleEnemy : MonoBehaviour, IBattleRole
    {
        public static readonly int BattleMove = Animator.StringToHash("move");
        public static readonly int BattleAim = Animator.StringToHash("aim");
        public static readonly int BattleShoot = Animator.StringToHash("shoot");
        public static readonly int BattleReload = Animator.StringToHash("reload");
        public static readonly int BattleDead = Animator.StringToHash("dead");
        
        private int _roleId;
        private int _curHp;
        private float _curAp;
        private int _curAmmo;
        private int _maxHp;
        private float _maxAp;
        private int _maxAmmo;
        private bool _isControl;
        private bool _isDead;
        private RoleType _roleType;
        private AtkType _atkType;
        private int _atkRange;
        
        //组件
        private NavMeshAgent _agent;
        private CapsuleCollider _collider;
        private Animator _anim; 
        
        //相机
        public Transform followTarget;
        
        //角色信息
        public int RoleId => _roleId;
        public int MaxHp => _maxHp;
        public float MaxAp => _maxAp;
        public int MaxAmmo => _maxAmmo;
        public AtkType AtkType => _atkType;
        public int AtkRange => _atkRange;
        public RoleType RoleType => _roleType;
        public GameObject Go => gameObject;
        
        //角色状态
        public bool IsControl
        {
            get => _isControl;
            set => _isControl = value;
        }
        public int CurHp => _curHp;
        public float CurAp => _curAp;
        public int CurAmmo => _curAmmo;
        public bool IsDead => _isDead;
        public IBattleRole AtkTarget { get; set; }
        
        //代理
        public NavMeshAgent Agent => _agent;
        
        
        public void SubAp(float ap)
        {
            if(_isDead) return;
            _curAp -= ap;
        }

        public void SubHp(int hp)
        {
            if(_isDead) return;
            _curHp -= hp;
            if (_curHp > 0) return;
            KillMe();
        }

        public void SubAmmo(int num = 1)
        {
            if(_isDead) return;
            _curAmmo -= num;
        }

        public void ResetAmmo()
        {
            if(_isDead) return;
            _curAmmo = _maxAmmo;
        }

        public void InitMe(object info)
        {
            if (!(info is EnemyInfo enemy))
                throw new Exception("Init enemy info valid.");
            _roleId = enemy.roleId;
            _maxHp = _curHp = enemy.hp;
            _maxAp = _curAp = enemy.ap;
            _maxAmmo = _curAmmo = enemy.ammo;
            _atkType = (AtkType)enemy.atkType;
            _atkRange = enemy.atkRange;
            _roleType = RoleType.Enemy;
            
            _isDead = false;
            _isControl = false;
            AtkTarget = null;
            _anim.SetBool(BattleDead, false);
        }
        
        private void KillMe()
        {
            _isDead = true;
            _isControl = false;
            _curHp = 0;
            _curAp = 0;
            _curAmmo = 0;
            _anim.SetBool(BattleDead, true);
        }

        #region 生命周期

        private void Awake()
        {
            _anim = GetComponentInChildren<Animator>();
            _collider = GetComponent<CapsuleCollider>();
            _agent = GetComponent<NavMeshAgent>();
            gameObject.layer = LayerMask.NameToLayer(GameConst.ROLE_LAYER);
            _collider.center = Vector3.up * 1.03f;
            gameObject.tag = GameConst.ENEMY_TAG;
            followTarget = transform.Find("followTarget");
        }

        #endregion
    }
}