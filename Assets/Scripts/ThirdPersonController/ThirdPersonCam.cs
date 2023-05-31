using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Base")]
    public Transform followTarget;//相机跟随目标
    public float sensitive = 12;//相机跟随敏感度
    public Vector3 shoulderOffset = Vector3.right;//跟随目标肩部偏移
    public float camDistance = 3;//相机距目标距离
    [Range(0f, 1f)]
    public float camSide = 0.6f;//画面相对于肩部偏移的左右偏移

    [Header("Obstacles")] 
    public bool isUseObstacles = true;
    public LayerMask camCollisionFilter = 1;//相机检测墙壁层级
    [Tag]
    public string ignoreTag = "Player";//检测忽略目标Tag，一般为目标物体Tag
    [Range(0.001f, 1f)]
    public float camRadius = 0.15f;//相机碰撞大小

    public float minDistance = 0.3f;//相机距target的最小距离

    //射线检测
    private Ray _ray;
    private RaycastHit _hit;

    //目标位置
    private Vector3 _offset;
    private Vector3 _targetFollowPos;
    private Vector3 _targetLookPos;
    private Vector3 _lastRotation;
    private Camera _camera;
    private bool _isTransmit = false;

    public Camera Camera => _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (followTarget == null)
            return;

        //计算目标位置和旋转
        CalTargetPos();

        //如果被阻挡
        if (isUseObstacles && IsObstructed())
        {
            //应该修正到的距离
            //增加一点距离保证修正后仍处于遮挡状态
            float reviseDis = Vector3.Distance(_hit.point, followTarget.position) + 0.15f;
            reviseDis = Mathf.Max(reviseDis, minDistance);
            _targetFollowPos = followTarget.position + _offset - followTarget.forward * reviseDis;
            if (!_isTransmit)
            {
                transform.position = _targetFollowPos;
                _isTransmit = true;
            }
        }
        else
            _isTransmit = false;

        //移动和旋转相机
        transform.position = Vector3.Lerp(transform.position, _targetFollowPos, sensitive * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(_targetLookPos - transform.position, Vector3.up);
    }

    /// <summary>
    /// 检测是否被阻挡
    /// </summary>
    /// <returns></returns>
    private bool IsObstructed()
    {
        _ray.origin = _targetLookPos;
        _ray.direction = transform.position - _ray.origin;

        //Physics.SphereCast(ray, camRadius, out hit, 1000, camCollisionFilter);
        Physics.Raycast(_ray, out _hit, 1000, camCollisionFilter);
        if (_hit.collider != null && !_hit.collider.CompareTag(ignoreTag))
        {
            //如果玩家和障碍的距离短于玩家和相机的距离
            //说明视线被遮挡
            float dis = Vector3.Distance(followTarget.position, _hit.point);
            if(dis < Vector3.Distance(followTarget.position, transform.position))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 计算目标移动旋转
    /// </summary>
    private void CalTargetPos()
    {
        //计算相机位置和朝向偏移值
        _offset = followTarget.right * (shoulderOffset.x * (camSide - 0.5f) * 2)
                    + followTarget.up * shoulderOffset.y
                        + followTarget.forward * shoulderOffset.z;
        _targetFollowPos = followTarget.position + _offset - followTarget.forward * camDistance;
        _targetLookPos = followTarget.position + _offset;
    }

    private void OnDrawGizmosSelected()
    {
        if(followTarget == null)
            return;
        _offset = followTarget.right * shoulderOffset.x * (camSide - 0.5f) * 2
                    + followTarget.up * shoulderOffset.y
                        + followTarget.forward * shoulderOffset.z;
        Vector3 targetFollowPos = followTarget.position + _offset - followTarget.forward * camDistance;
        Vector3 targetLookPos = followTarget.position + _offset;

        //相机位置
        Gizmos.color = Color.red;
        Gizmos.DrawLine(targetLookPos, targetFollowPos);
        Gizmos.DrawSphere(targetFollowPos, 0.05f);

        //观察位置
        Gizmos.color = Color.green;
        Gizmos.DrawLine(targetLookPos, targetLookPos);
        Gizmos.DrawSphere(targetLookPos, 0.05f);

        //遮挡检测范围
        Gizmos.color = Color.blue;
        //圆形检测
        _ray.origin = targetLookPos;
        _ray.direction = transform.position - targetLookPos;
        if (Physics.Raycast(_ray, out _hit, 1000, camCollisionFilter))
        {
            Gizmos.DrawLine(_ray.origin, _hit.point);
            Gizmos.DrawSphere(_hit.point, camRadius);
        }
    }

}
