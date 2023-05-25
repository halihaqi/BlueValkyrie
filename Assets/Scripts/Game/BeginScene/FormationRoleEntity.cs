using System;
using Game.Base;
using Game.Model;
using UnityEngine;

namespace Game.Entity
{
    public class FormationRoleEntity : RoleBase
    {
        private static readonly int Pick = Animator.StringToHash("pick");
        private StudentItem _student;
        private bool _isPick = false;
        private Vector3 _targetPos;

        public StudentItem Student => _student;

        protected override void Awake()
        {
            base.Awake();
            //设置为UI层级
            var layer = LayerMask.NameToLayer("UI");
            SetLayerRecursively(this.gameObject, layer);
        }

        private void Update()
        {
            if(!_isPick) return;
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 5);
        }

        public void SetStudent(StudentItem student) => _student = student;

        public void SetPick(bool isPick)
        {
            _isPick = isPick;
            anim.SetBool(Pick, isPick);
            roleCollider.enabled = !isPick;
        }

        public void SetDragTarget(Vector3 targetPos) => _targetPos = targetPos;
    }
}