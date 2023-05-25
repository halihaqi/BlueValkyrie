using Game.Entity;
using Hali_Framework;
using UnityEngine;
using UnityEngine.AI;

namespace Game.BattleScene.BattleEnemyFsm
{
    public class EnemyMoveState : FsmState<BattleEnemyEntity>
    {
        private BattleEnemyEntity _enemy;
        private BattleMaster _bm;
        private bool _pathSeted = false;
        
        protected internal override void OnEnter(IFsm<BattleEnemyEntity> fsm)
        {
            base.OnEnter(fsm);
            _pathSeted = false;
            _enemy = fsm.Owner;
            InputMgr.Instance.Enabled = false;
            _enemy.Agent.isStopped = false;
            _bm = FsmMgr.Instance.GetFsm<BattleMaster>(BattleConst.BATTLE_FSM).Owner;
            _enemy.SetFollowCamera(_bm.cam, _bm.followCam);
            
            _enemy.Target = FindAtkTarget(_enemy, _bm);
            if (_enemy.Target != null)
            {
                ChangeState<EnemyAimState>(fsm);
                return;
            }
            
            _enemy.SetMovePos(CalcMoveTarget(_enemy, _bm));
            DelayUtils.Instance.Delay(1,1, DelayMove);
        }

        protected internal override void OnUpdate(IFsm<BattleEnemyEntity> fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            if(!_pathSeted) return;
            var vel = _enemy.Agent.velocity.magnitude;
            _enemy.anim.SetBool(BattleEnemyEntity.BattleMove, vel > 0);
            _enemy.SubAp(vel * 0.1f);

            //每帧检测是否周围有敌人
            _enemy.Target = FindAtkTarget(_enemy, _bm);
            if (_enemy.Target != null)
            {
                ChangeState<EnemyAimState>(fsm);
                return;
            }

            if (_enemy.CurAp <= 0 || _enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            {
                EventMgr.Instance.TriggerEvent(ClientEvent.BATTLE_STEP_OVER);
                ChangeState<EnemyRestState>(fsm);
            }
        }

        protected internal override void OnLeave(IFsm<BattleEnemyEntity> fsm, bool isShutdown)
        {
            base.OnLeave(fsm, isShutdown);
            _enemy.Agent.isStopped = true;
            _enemy.Agent.ResetPath();
            DelayUtils.Instance.Remove(DelayMove);
        }

        private Vector3 CalcMoveTarget(BattleEnemyEntity enemy, BattleMaster bm)
        {
            var enemys = bm.enemies;
            var students = bm.students;
            var shelters = bm.shelterPos;
            var flags = bm.flags;
            
            //首先寻找最近旗帜
            FlagEntity nearestFlag = flags[0];
            float nearestDis = float.MaxValue;
            
            //旗帜是每场战斗都存在的，所以必然有一个最近的旗帜
            foreach (var flag in flags)
            {
                var dis = Vector3.Distance(enemy.transform.position, flag.transform.position);
                if (dis <= nearestDis)
                {
                    nearestDis = dis;
                    nearestFlag = flag;
                }
            }

            bool isAtk = false;
            //判断旗帜周围是否有队友
            foreach (var e in enemys)
            {
                if(e == enemy) continue;
                var dis = Vector3.Distance(e.transform.position, nearestFlag.transform.position);
                //如果队友在旗帜范围内，进攻
                //否则防守这个旗帜
                if (dis < nearestFlag.Radius)
                {
                    isAtk = true;
                    break;
                }
            }

            if (isAtk)
            {
                BattleStudentEntity nearestStudent = null;
                nearestDis = float.MaxValue;
                //找到最近学生
                foreach (var s in students)
                {
                    var dis = Vector3.Distance(enemy.transform.position, s.transform.position);
                    if (dis < nearestDis)
                    {
                        nearestDis = dis;
                        nearestStudent = s;
                    }
                }

                return nearestStudent.transform.position +
                       (enemy.transform.position - nearestStudent.transform.position).normalized *
                       (enemy.AtkRange * 0.7f);
            }
            
            //如果已经在旗帜范围内，不用移动
            var dis1 = Vector3.Distance(enemy.transform.position, nearestFlag.transform.position);
            if (dis1 < nearestFlag.Radius)
                return enemy.transform.position;
            
            //否则返回旗帜位置
            return nearestFlag.transform.position;
        }
        
        private BattleStudentEntity FindAtkTarget(BattleEnemyEntity enemy, BattleMaster bm)
        {
            var enemyPos = enemy.transform.position;
            var enemyAtkRange = enemy.EnemyInfo.atkRange;

            BattleStudentEntity target = null;
            float minDis = enemyAtkRange;
            foreach (var student in bm.students)
            {
                var dis = Vector3.Distance(enemyPos, student.transform.position);
                if (dis <= minDis)
                {
                    target = student;
                    minDis = dis;
                }
            }

            return target;
        }

        private void DelayMove(object obj)
        {
            _enemy.Agent.SetDestination(_enemy.TargetPos);
            _pathSeted = true;
        }
    }
}