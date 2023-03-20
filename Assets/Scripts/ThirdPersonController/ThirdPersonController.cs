using System;
using Hali_Framework;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Base")]
    public float moveSpeed = 3;
    public float sprintSpeed = 5;
    public float rotateSmoothTime = 0.12f;
    public float jumpForce = 1;

    [Header("Gravity")]
    public float gravity = -15;

    [Header("FollowCamera")]
    public Camera followCamera;
    public Transform followTarget;
    public float topClamp = 70;//相机最大仰角
    public float bottomClamp = -30;//相机最大俯角
    public bool isFlipPitch = true;

    [Header("GroundCheck")]
    public float groundOffset = -0.29f;
    public float groundRadius = 0.28f;
    public LayerMask groundLayers = 1;
    
    [Header("HeadCheck")]
    public float headOffset = 0.29f;
    public float headRadius = 0.28f;
    public LayerMask headLayers = 1;

    [Header("Switch")] 
    public bool useSprint = true;
    public bool useJump = true;
    public bool useAnim = true;
    public bool useMove = true;
    public bool useRotate = true;
    public bool useGravity = true;

    //输入参数
    private Vector2 _inputLook;
    private Vector2 _inputMove;
    private float _threshold = 0.01f;//输入最低门槛
    private bool _isInputShift = false;

    //移动参数
    private float _targetRot = 0;
    private float _rotVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53;
    
    //跳跃参数
    private bool _isGrounded = true;

    //相机参数
    [HideInInspector] public ThirdPersonCam thirdPersonCam;
    private float _camTargetYaw;
    private float _camTargetPitch;

    //Component
    [HideInInspector] public Animator anim;
    [HideInInspector] public CharacterController cc;
    private bool _hasAnim;
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Ground = Animator.StringToHash("ground");
    private static readonly int Jump = Animator.StringToHash("jump");

    public bool IsMove => useMove && InputMgr.Instance.Enabled && _inputMove.magnitude > 0;

    protected virtual void Awake()
    {
        //获取组件
        anim = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();
    }
    
    protected virtual void Start()
    {
        //初始化跟随相机
        if (followCamera == null)
            throw new Exception("Player has no follow camera.");
        if (!followCamera.TryGetComponent(out thirdPersonCam))
            thirdPersonCam = followCamera.gameObject.AddComponent<ThirdPersonCam>();
        if (thirdPersonCam.followTarget == null)
        {
            thirdPersonCam.followTarget = followTarget;
            if (thirdPersonCam.followTarget == null)
                throw new Exception("Player camera has no follow target.");
        }

        //打开输入监听
        InputMgr.Instance.Enabled = true;
        EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnShift);
        EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_UP, OnShiftEnd);
        EventMgr.Instance.AddListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnSpace);
    }

    protected virtual void OnDestroy()
    {
        //关闭输入监听
        InputMgr.Instance.Enabled = false;
        EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnShift);
        EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_UP, OnShiftEnd);
        EventMgr.Instance.RemoveListener<KeyCode>(ClientEvent.GET_KEY_DOWN, OnSpace);
    }

    protected virtual void Update()
    {
        _hasAnim = anim.runtimeAnimatorController != null;
        
        Gravity();
        GroundHeadCheck();
        UpdateInput();
        Move();
        CameraTargetRotation();       
    }
    
    protected virtual void OnShift(KeyCode key)
    {
        if(!useSprint) return;
        if (key == KeyCode.LeftShift)
            _isInputShift = true;
    }

    protected virtual void OnShiftEnd(KeyCode key)
    {
        if(key == KeyCode.LeftShift)
            _isInputShift = false;
    }

    protected virtual void OnSpace(KeyCode key)
    {
        if(!useJump) return;
        if(key == KeyCode.Space)
            OnJump();
    }

    /// <summary>
    /// 更新输入参数
    /// </summary>
    private void UpdateInput()
    {
        _inputLook = InputMgr.Instance.InputLook;
        _inputMove = InputMgr.Instance.InputMove;
    }

    /// <summary>
    /// 移动和转身
    /// </summary>
    private void Move()
    {
        if(!useMove)
        {
            if(_hasAnim && useAnim)
                anim.SetFloat(Speed, 0);
            return;
        }
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
        cc.Move(targetDir.normalized * (targetSpeed * Time.deltaTime) + Vector3.up * (_verticalVelocity * Time.deltaTime));

        //移动动画
        if(_hasAnim && useAnim)
            anim.SetFloat(Speed, _isInputShift ? inputDir.magnitude : inputDir.magnitude / 2);
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    private void GroundHeadCheck()
    {
        if(!useGravity) return;
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
    private void Gravity()
    {
        if (!useGravity)
        {
            _isGrounded = true;
            return;
        }
        if (_hasAnim && useAnim)
            anim.SetBool(Ground, _isGrounded);
        
        if (_isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        //Gravity
        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += gravity * Time.deltaTime;
    }

    private void OnJump()
    {
        //Jump
        if (_isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            
            if(_hasAnim && useAnim)
                anim.SetTrigger(Jump);
        }
    }

    /// <summary>
    /// 第三人称相机旋转
    /// </summary>
    private void CameraTargetRotation()
    {
        if(!useRotate) return;
        //如果输入大于阈值
        if (_inputLook.sqrMagnitude >= _threshold)
        {
            _camTargetYaw += _inputLook.x;
            _camTargetPitch += _inputLook.y * (isFlipPitch ? -1 : 1);
        }
        //限制相机角度
        _camTargetYaw = TransformUtils.ClampAngle(_camTargetYaw, float.MinValue, float.MaxValue);
        _camTargetPitch = TransformUtils.ClampAngle(_camTargetPitch, bottomClamp, topClamp);
        //移动相机目标点
        followTarget.rotation = Quaternion.Euler(_camTargetPitch, _camTargetYaw, 0);
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
