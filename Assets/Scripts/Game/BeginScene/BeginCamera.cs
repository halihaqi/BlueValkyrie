using System;
using UnityEngine;

namespace Game.BeginScene
{
    public class BeginCamera : SingletonMono<BeginCamera>
    {
        public float rotateSpeed = 10;
        public float padding = 5;
        private Vector2 _mouseInput;
        private Vector3 _startRot;
        private Vector3 _frontRot;
        private Animator _anim;
        private Action _moveCompleteEvent;

        private void Start()
        {
            _startRot = transform.localEulerAngles;
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            //上一帧欧拉角，用于限制旋转角度
            _frontRot = transform.localEulerAngles;
            _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Quaternion targetRot = Quaternion.AngleAxis(_mouseInput.x, Vector3.up) *
                                   Quaternion.AngleAxis(-_mouseInput.y, Vector3.forward) * transform.rotation;
            //控制z轴偏移
            targetRot = Quaternion.Euler(targetRot.eulerAngles.x, targetRot.eulerAngles.y, _startRot.z);
            //旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);


            //限制旋转角度
            if (transform.localEulerAngles.y > _startRot.y + padding
                || transform.localEulerAngles.y < _startRot.y - padding)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                    _frontRot.y, transform.localEulerAngles.z);
            }

            if (transform.localEulerAngles.x > _startRot.x + padding
                || transform.localEulerAngles.x < _startRot.x - padding)
            {
                transform.localEulerAngles = new Vector3(_frontRot.x,
                    transform.localEulerAngles.y, transform.localEulerAngles.z);
            }

        }

        public void Move(Action callback = null)
        {
            _anim.SetTrigger("move");
            _moveCompleteEvent = callback;
        }

        private void OnMoveComplete()
        {
            _moveCompleteEvent?.Invoke();
        }
    }
}