using System;
using System.Collections.Generic;
using System.Linq;
using Game.Model;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public class CampRoundInfo
    {
        public RoleType type;
        public int curAp;
    }
    
    public class RoundEngine
    {
        //key 阵营种类   value 阵营ap
        private HashSet<RoleType> _addedCamp;
        private LinkedList<CampRoundInfo> _camps;
        private int _maxRound;
        private int _logicAp;

        private int _curRound;
        private LinkedListNode<CampRoundInfo> _curCamp;
        private LinkedListNode<CampRoundInfo> _cacheNode;
        private bool _isRun;


        public int CurRound => _curRound;

        public int MaxRound => _maxRound;

        public CampRoundInfo CurCamp => _curCamp.Value;

        public bool IsOver => !_isRun;

        public void Init(int maxRound, int logicAp)
        {
            _camps = new LinkedList<CampRoundInfo>();
            _addedCamp = new HashSet<RoleType>();
            _curCamp = null;
            _curRound = _maxRound = maxRound;
            _logicAp = logicAp;
        }

        public void Start(RoleType type)
        {
            if (!_addedCamp.Contains(type))
            {
                Debug.Log($"Has no camp {type} in RoundEngine.");
                return;
            }

            _cacheNode = _camps.First;
            while (_cacheNode != null)
            {
                if(_cacheNode.Value.type == type)
                    break;
                _cacheNode = _cacheNode.Next;
            }

            _curCamp = _cacheNode;
            _cacheNode = null;
            _isRun = true;
        }

        public void AddCamp(RoleType type)
        {
            if (!_addedCamp.Add(type))
            {
                Debug.Log($"Has the same camp {type} in RoundEngine.");
                return;
            }

            CampRoundInfo info = new CampRoundInfo();
            info.type = type;
            info.curAp = _logicAp;
            _camps.AddLast(info);
        }

        public void Run()
        {
            _curCamp.Value.curAp--;
            //行动点用完，切换到下个队伍
            if (_curCamp.Value.curAp <= 0)
                CampOver();
        }
        
        /// <summary>
        /// 一个阵营行动结束
        /// </summary>
        public void CampOver()
        {
            _curCamp = _curCamp.Next;
            //所有队伍行动完成，结束一回合
            if (_curCamp == null)
                RoundOver();

            _curCamp.Value.curAp += _logicAp;//每回合恢复行动点
            _curCamp.Value.curAp = Math.Min(BattleConst.ROUND_MAX_AP, _curCamp.Value.curAp);
        }

        /// <summary>
        /// 一个回合结束
        /// </summary>
        public void RoundOver()
        {
            _curRound--;
            _curCamp = _camps.First;
            if(_curRound <= 0)
                BattleOver();
        }

        public void BattleOver()
        {
            _isRun = false;
            _curRound = 0;
            _curCamp = null;
        }

        public void ShutDown()
        {
            BattleOver();
            _addedCamp = null;
            _camps = null;
            _cacheNode = null;
            _curCamp = null;
        }

        public void AutoRun()
        {
            
        }
    }
}