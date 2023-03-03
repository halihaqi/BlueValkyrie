using System;
using Hali_Framework;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Base")]
    public float moveSpeed = 3;
    public float sprintSpeed = 5;
    public float rotateSpeed = 50;
    public float rotateSmoothTime = 0.12f;
    public float jumpForce = 1;
    public float jumpTimeout = 0.5f;

    [Header("Gravity")]
    public float gravity = -15;

    [Header("FollowCamera")]
    public Camera followCamera;
    public Transform followTarget;
    public float topClamp = 70;//相机最大仰角
    public float bottomClamp = -30;//相机最大俯角
    public bool isFilpPitch = true;

    [Header("GroundCheck")]
    public float groundOffset = -0.29f;
    public float groundRadius = 0.28f;
    public LayerMask groundLayers = 1;
    
    [Header("HeadCheck")]
    public float headOffset = 0.29f;
    public float headRadius = 0.28f;
    public LayerMask headLayers = 1;

    //输入参数
    private Vector2 _inputLook;
    private Vector2 _inputMove;
    private float _threshold = 0.01f;//输入最低门槛
    private bool _isInputShift = false;
    private bool _isInputJump = false;

    //移动参数
    private float _targetRot = 0;
    private float _rotVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53;
    
    //跳跃参数
    private bool _isGrounded = true;
    private float _jumpTimeoutDelta;

    //相机参数
    private ThirdPersonCam _thirdPersonCam;
    private float _camTargetYaw;
    private float _camTargetPitch;

    //Component
    private bool _hasAnim;
    private Animator _anim;
    private CharacterController _cc;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Ground = Animator.StringToHash("ground");
    private static readonly int Jump = Animator.StringToHash("jump");
    
    protected virtual void Awake()
    {
        //获取组件
        _anim = GetComponentInChildren<Animator>();
        _cc = GetComponent<CharacterController>();
    }
    
    protected virtual void Start()
    {
        //初始化跟随相机
        if (followCamera == null)
            throw new Exception("Player has no follow camera.");
        if (!followCamera.TryGetComponent(out _thirdPersonCam))
            _thirdPersonCam = followCamera.gameObject.AddComponent<ThirdPersonCam>();
        if (_thirdPersonCam.followTarget == null)
        {
            _thirdPersonCam.followTarget = followTarget;
            if (_thirdPersonCam.followTarget == null)
                throw new Exception("Player camera has no follow target.");
        }

        followCamera.transform.position = this.transform.position - this.transform.forward;

        //打开输入监听
        InputMgr.Instance.Enabled = true;
    }

    protected virtual void OnDestroy()
    {
        //关闭输入监听
        InputMgr.Instance.Enabled = false;
    }

    protected virtual void Update()
    {
        _hasAnim = _anim != null;
        
        JumpAndGravity();
        GroundHeadCheck();
        UpdateInput();
        Move();
        CameraTargetRotation();       
    }

    private void OnInput(KeyCode key)
    {
        _isInputShift = key == KeyCode.LeftShift;
        _isInputJump = key == KeyCode.Space;
    }

    /// <summary>
    /// 更新输入参数
    /// </summary>
    private void UpdateInput()
    {
        _inputLook = InputMgr.Instance.GetInputLook;
        _inputMove = InputMgr.Instance.GetInputMove;
    }

    /// <summary>
    /// 移动和转身
    /// </summary>
    private void Move()
    {
        //判断是否为冲刺速度
        float targetSpeed = _isInputShift ? sprintSpeed : moveSpeed;

        //判断移动输入是否为0
        if (_inputMove.sqrMagnitude < 0.001f)
            targetSpeed = 0;

        //输入的方向
        Vector3 inputDir = new Vector3(_inputMove.x, 0, _inputMove.y);

        //人物移动时的八向旋转
        if(_inputMove != Vector2.zero)
        {
            _targetRot = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + followCamera.transform.eulerAngles.y;
            float rot = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRot, ref _rotVelocity, rotateSmoothTime);

            transform.rotation = Quaternion.Euler(0, rot, 0);
        }

        //旋转后的移动方向
        Vector3 targetDir = Quaternion.Euler(0, _targetRot, 0) * Vector3.forward;

        //移动人物,加上垂直速度
        _cc.Move(targetDir.normalized * (targetSpeed * Time.deltaTime) + Vector3.up * (_verticalVelocity * Time.deltaTime));

        //移动动画
        if(_hasAnim)
            _anim.SetFloat(Speed, _isInputShift ? inputDir.magnitude : inputDir.magnitude / 2);
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    private void GroundHeadCheck()
    {
        var pos = transform.position;
        Vector3 feetPosition =
            new Vector3(pos.x, pos.y - groundOffset, pos.z);
        _isGrounded = Physics.CheckSphere(feetPosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
        
        //头部检测，如果撞到头顶并且不在地面，跳跃速度归零
        Vector3 headPosition =
            new Vector3(pos.x, pos.y + headOffset, pos.z);
        if (!_isGrounded && Physics.CheckSphere(headPosition, headRadius, headLayers, QueryTriggerInteraction.Ignore))
            _verticalVelocity = 0f;
    }

    /// <summary>
    /// 重力
    /// </summary>
    private void JumpAndGravity()
    {
        //Jump
        if (_isGrounded)
        {
            if (_hasAnim)
                _anim.SetBool(Ground, true);

            if (_verticalVelocity < 0)
                _verticalVelocity = -2f;
            
            //jump
            if (Input.GetKeyDown(KeyCode.Space) && _jumpTimeoutDelta <= 0)
            {
                _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                
                if(_hasAnim)
                    _anim.SetTrigger(Jump);
            }

            if (_jumpTimeoutDelta >= 0)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;
            if(_hasAnim)
                _anim.SetBool(Ground, false);
        }
        
        //Gravity
        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += gravity * Time.deltaTime;
    }

    /// <summary>
    /// 第三人称相机旋转
    /// </summary>
    private void CameraTargetRotation()
    {
        //如果输入大于阈值
        if (_inputLook.sqrMagnitude >= _threshold)
        {
            _camTargetYaw += _inputLook.x;
            _camTargetPitch += _inputLook.y * (isFilpPitch ? -1 : 1);
        }
        //限制相机角度
        _camTargetYaw = TransformUtils.ClampAngle(_camTargetYaw, float.MinValue, float.MaxValue);
        _camTargetPitch = TransformUtils.ClampAngle(_camTargetPitch, bottomClamp, topClamp);
        //移动相机目标点
        _thirdPersonCam.followTarget.rotation = Quaternion.Euler(_camTargetPitch, _camTargetYaw, 0);
    }

    private void OnDrawGizmosSelected()
    {
        //如果在地面为绿色，如果不在为红色
        Color groundedColorGreen = new Color(0, 1, 0, 0.35f);
        Color airColorRed = new Color(1, 0, 0, 0.35f);
        Gizmos.color = _isGrounded ? groundedColorGreen : airColorRed;

        //画球
        var pos = transform.position;
        Vector3 feetPosition =
            new Vector3(pos.x, pos.y - groundOffset, pos.z);
        Gizmos.DrawSphere(feetPosition, groundRadius);
        
        //头部检测，如果撞到头顶，跳跃速度归零
        Vector3 headPosition =
            new Vector3(pos.x, pos.y + headOffset, pos.z);
        bool isHeadHit = Physics.CheckSphere(headPosition, headRadius, headLayers, QueryTriggerInteraction.Ignore);
        Gizmos.color = !isHeadHit ? groundedColorGreen : airColorRed;
        Gizmos.DrawSphere(headPosition, headRadius);
    }

}
