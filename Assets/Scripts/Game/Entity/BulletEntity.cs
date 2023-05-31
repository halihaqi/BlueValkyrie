using Hali_Framework;
using UnityEngine;

namespace Game.Entity
{
    public class BulletEntity : MonoBehaviour
    {
        // public float speed = 10;
        //
        // public const string BULLET_PATH = "Prefabs/Bullet";
        //
        // private BattleRoleEntity _owner;
        // private bool _isSetTarget;
        //
        // private void Update()
        // {
        //     if(_isSetTarget)
        //         transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        // }
        //
        // private void OnDisable()
        // {
        //     _isSetTarget = false;
        // }
        //
        // public void SetTarget(BattleRoleEntity owner, Transform firePos, Transform targetPos)
        // {
        //     _owner = owner;
        //     gameObject.tag = owner.tag;
        //     transform.position = firePos.position;
        //     if(targetPos != null)
        //         transform.LookAt(targetPos);
        //     _isSetTarget = true;
        //     Invoke(nameof(RecycleMe), 3);
        // }
        //
        // private void OnTriggerEnter(Collider other)
        // {
        //     if(other.CompareTag(_owner.tag)) return;
        //     RecycleMe();
        // }
        //
        // private void RecycleMe()
        // {
        //     ObjectPoolMgr.Instance.PushObj(BULLET_PATH, this.gameObject);
        //     CancelInvoke(nameof(RecycleMe));
        // }
    }
}