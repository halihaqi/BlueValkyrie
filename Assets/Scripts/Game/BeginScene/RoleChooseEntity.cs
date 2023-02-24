using System;
using System.Collections;
using Game.Base;
using Hali_Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Game.BeginScene
{
    public class RoleChooseEntity : RoleBase
    {
        [SerializeField]
        private float pickedSpeed = 3;

        private Transform _showPos;
        private Transform _hidePos;
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int Reaction = Animator.StringToHash("reaction");
        private static readonly int Pick = Animator.StringToHash("pick");

        private bool _canPick = false;
        private bool _isDruging = false;

        private Action _showCompleteEvent;

        protected override void Awake()
        {
            base.Awake();
            _showPos = BeginSceneMonoMgr.Instance.ShowPos;
            _hidePos = BeginSceneMonoMgr.Instance.HidePos;
        }

        private void Start()
        {
            SetTrigger(true);
        }

        private void OnEnable()
        {
            EventMgr.Instance.AddListener(ClientEvent.ROLE_CHANGE_COMPLETE, OnRoleChangeComplete);
        }

        private void OnDisable()
        {
            EventMgr.Instance.RemoveListener(ClientEvent.ROLE_CHANGE_COMPLETE, OnRoleChangeComplete);
        }

        private void Update()
        {
            if(!_canPick) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,1000,1<<LayerMask.NameToLayer(GameConst.ROLE_LAYER)) && Input.GetMouseButtonDown(0))
            {
                _isDruging = true;
            }
            
            if (_isDruging)
            {
                if (Input.GetMouseButton(0))
                {
                    _anim.SetBool(Pick, true);
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = Mathf.Abs(Camera.main.transform.position.x - transform.position.x);
                    Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);
                    targetPos.x = transform.position.x;
                    if (targetPos.y < _showPos.transform.position.y)
                        targetPos.y = _showPos.transform.position.y;
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pickedSpeed);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    transform.position = _showPos.position;
                    _anim.SetBool(Pick, false);
                    _isDruging = false;
                }
            }
        }

        //动画关键帧触发
        private void OnRoleChangeComplete()
        {
            _canPick = true;
            _showCompleteEvent?.Invoke();
        }

        public void ShowMe(Action callback)
        {
            _showCompleteEvent = callback;
            _anim.SetFloat(Speed, 1);
            StopAllCoroutines();
            StartCoroutine(Move(_showPos.position, () =>
            {
                _anim.SetFloat(Speed, 0);
                StartCoroutine(TurnRound(-transform.right, () =>
                {
                    _anim.SetTrigger(Reaction);
                }));
            }));
        }

        public void HideMe(Action callback)
        {
            transform.position = _showPos.position;
            _anim.SetBool(Pick, false);
            _isDruging = false;
            _canPick = false;
            StopAllCoroutines();
            StartCoroutine(TurnRound(transform.right, () =>
            {
                _anim.SetFloat(Speed, 1);
                StartCoroutine(Move(_hidePos.position, () =>
                {
                    transform.localPosition = Vector3.zero;
                    _anim.SetFloat(Speed, 0);
                    callback?.Invoke();
                }));
            }));
        }

        public void ResetMe()
        {
            StopAllCoroutines();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _isDruging = false;
            _canPick = false;
        }

        private IEnumerator Move(Vector3 targetPos, UnityAction callback)
        {
            while (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards
                    (transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            callback?.Invoke();
        }

        private IEnumerator TurnRound(Vector3 targetDir, UnityAction callback)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            while (transform.rotation != targetRot)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
                yield return null;
            }
            callback?.Invoke();
        }
    }
}