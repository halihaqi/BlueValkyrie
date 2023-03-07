using System;
using System.Collections;
using System.Collections.Generic;
using Game.Entity;
using Game.UI.Battle;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public class BattleSceneMonoMgr : SingletonMono<BattleSceneMonoMgr>
    {
        [SerializeField]
        private List<Transform> formationPos;

        private List<BattleRoleInfo> _formationInfo;
        private List<BattleRoleEntity> _formation;
        private ThirdPersonCam _followCam;
        private int _curRoleIndex = 0;
        
        private bool _initComplete = false;

        public void InitFormationEntity(List<BattleRoleInfo> infos, Camera followCam)
        {
            _formationInfo = infos;
            StartCoroutine(InitBattle(followCam));
            UIMgr.Instance.ShowPanel<BattlePanel>(userData: _formationInfo[0]);
            
            //todo Test
            EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, TestChangeRole);
        }

        private void TestChangeRole(KeyCode key)
        {
            if(key != KeyCode.Tab) return;
            _curRoleIndex = _curRoleIndex + 1 >= _formation.Count ? 0 : _curRoleIndex + 1;
            SetControlRole(_curRoleIndex, Camera.main);
        }

        public void SetControlRole(int index, Camera cam)
        {
            for (int i = 0; i < _formation.Count; i++)
            {
                _formation[i].IsControl = i == index;
                _formation[i].SetFollowCamera(i == index ? cam : null);
            }
            EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_ROLE_CHANGE, _formationInfo[index]);
        }

        private IEnumerator InitBattle(Camera followCam)
        {
            if(_formationInfo == null || formationPos.Count <= 0)
                throw new Exception("Need battle role info or formation pos.");

            _initComplete = false;
            _formation = new List<BattleRoleEntity>(formationPos.Count);
            int index = 0;
            int completeCount = 0;
            foreach (var pos in formationPos)
            {
                int i = index;
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetStudentObj(_formationInfo[i]), obj =>
                {
                    var entity = obj.AddComponent<BattleRoleEntity>();
                    entity.SetBattleRoleInfo(_formationInfo[i]);
                    obj.transform.position = pos.position;
                    obj.transform.rotation = pos.rotation;
                    _formation.Add(entity);
                    ++completeCount;
                });
                //如果传入的队伍info少于队伍点位，只加载有的
                if(++index > _formationInfo.Count - 1) break;
            }
            
            //等待预制体全部加载完
            while (completeCount < _formationInfo.Count)
                yield return null;
            
            //初始化相机
            if (!followCam.TryGetComponent(out _followCam))
                _followCam = followCam.gameObject.AddComponent<ThirdPersonCam>();
            _followCam.camDistance = 2;
            _followCam.camSide = 0.8f;
            
            //操作权移交给第一个角色
            _curRoleIndex = 0;
            SetControlRole(_curRoleIndex, followCam);
            
            _initComplete = true;
            InputMgr.Instance.Enabled = true;
        }
    }
}