using System.Collections;
using System.Collections.Generic;
using Game.BattleScene.BattleRole;
using Game.Entity;
using Game.Managers;
using Game.Model;
using Game.Utils;
using Hali_Framework;
using UnityEngine;

namespace Game.BattleScene
{
    public static class BattleLoadHelper
    {
        public static IEnumerator LoadSolider(CampStation camp, List<IBattleRole> container)
        {
            //防止有残留
            container.Clear();
            switch (camp.type)
            {
                case RoleType.Student:
                    yield return MonoMgr.Instance.StartCoroutine
                        (LoadStudent(camp, container));
                    break;
                case RoleType.Enemy:
                    yield return MonoMgr.Instance.StartCoroutine
                        (LoadEnemy(camp, container));
                    break;
            }
        }

        private static IEnumerator LoadStudent(CampStation camp, List<IBattleRole> container)
        {
            var station = camp.roles;
            var formation = PlayerMgr.Instance.CurFormation;
            int roleCount = formation.students.Length;
            //防止站位和士兵数量不对导致的溢出
            int maxCount = Mathf.Min(station.Count, roleCount);
            int loadedCount = 0;
            for (int i = 0; i < maxCount; i++)
            {
                int index = i;
                var student = formation.students[i];
                var info = RoleMgr.Instance.GetBattleRole(student.roleId);
                BattleStudent entity = null;
                bool loaded = false;
                ResMgr.Instance.LoadAsync<GameObject>(GameConst.BATTLE_SCENE, ResPath.GetStudentObj(info), obj =>
                {
                    //设置站位
                    obj.transform.position = station[index].position;
                    obj.transform.rotation = station[index].rotation;
                    
                    //添加战斗脚本
                    entity = obj.AddComponent<BattleStudent>();
                    entity.InitMe(info);
                    loaded = true;
                });
                while (!loaded)
                    yield return null;
                container.Add(entity);
            }
        }

        private static IEnumerator LoadEnemy(CampStation camp, List<IBattleRole> container)
        {
            var station = camp.roles;
            var formation = 
        }
    }
}