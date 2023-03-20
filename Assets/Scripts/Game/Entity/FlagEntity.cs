using System;
using System.Collections;
using System.Collections.Generic;
using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public enum FlagType
    {
        None,
        Enemy,
        Student,
    }
    
    public class FlagEntity : MonoBehaviour
    {
        [SerializeField] private Transform risePos;
        [SerializeField] private Transform fallPos;
        [SerializeField] private Transform studentFlag;
        [SerializeField] private Transform enemyFlag;
        [SerializeField] private float speed = 3;
        [SerializeField] private FlagType flagType = FlagType.None;
        private bool _isRising = false;
        private SphereCollider _collider;
        private List<GameObject> _aroundRole;

        public FlagType FlagType => flagType;
        
        public bool IsRising => _isRising;

        public float Radius => _collider.radius;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _aroundRole = new List<GameObject>();
        }

        private void Start()
        {
            StartCoroutine(Rise(flagType));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameConst.STUDENT_TAG) 
                || other.CompareTag(GameConst.ENEMY_TAG))
            {
                if(!_aroundRole.Contains(other.gameObject))
                    _aroundRole.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameConst.STUDENT_TAG) 
                || other.CompareTag(GameConst.ENEMY_TAG))
            {
                if(_aroundRole.Contains(other.gameObject))
                    _aroundRole.Remove(other.gameObject);
            }
        }

        public bool IsRoleAround(BattleRoleEntity role)
        {
            return _aroundRole.Contains(role.gameObject);
        }

        public void RiseFlag(FlagType type)
        {
            if(_isRising) return;
            StartCoroutine(Rise(type));
        }

        private IEnumerator Rise(FlagType type)
        {
            _isRising = true;
            flagType = type;
            switch (type)
            {
                case FlagType.None:
                    while (studentFlag.position != fallPos.position)
                    {
                        studentFlag.position =
                            Vector3.MoveTowards(studentFlag.position, fallPos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    while (enemyFlag.position != fallPos.position)
                    {
                        enemyFlag.position =
                            Vector3.MoveTowards(enemyFlag.position, fallPos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case FlagType.Enemy:
                    while (studentFlag.position != fallPos.position)
                    {
                        studentFlag.position =
                            Vector3.MoveTowards(studentFlag.position, fallPos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    while (enemyFlag.position != risePos.position)
                    {
                        enemyFlag.position =
                            Vector3.MoveTowards(enemyFlag.position, risePos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
                case FlagType.Student:
                    while (enemyFlag.position != fallPos.position)
                    {
                        enemyFlag.position =
                            Vector3.MoveTowards(enemyFlag.position, fallPos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    while (studentFlag.position != risePos.position)
                    {
                        studentFlag.position =
                            Vector3.MoveTowards(studentFlag.position, risePos.position, 
                                speed * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    break;
            }

            _isRising = false;
        }
    }
}