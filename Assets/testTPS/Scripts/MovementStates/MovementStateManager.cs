using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    //www.youtube.com/watch?v=KCYr5pFC6Sw&list=PLX_yguE0Oa8QmfmFiMM9_heLBeSA6sNKx

    #region Movement References
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed =2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;
    public float airSpeed = 1.5f;
       
    [HideInInspector] public Vector3 dir;
    public float hzInput, vInput;
    CharacterController controller;
    #endregion

    #region GroundCheck
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;
    #endregion

    #region Gravity
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
    [HideInInspector] public bool jumped;
    Vector3 velocity;
    #endregion

    public MovementBaseState previousState;
    public MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState(); 
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    [HideInInspector] public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        controller = GetComponent<CharacterController>();
        SwitchState(Idle);
    }

    
    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();

        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    void GetDirectionAndMove()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero;

        // 바닥에 있으면
        if (!IsGrounded()) airDir = transform.forward * vInput + transform.right * hzInput;
        // 공중에 있으면
        else dir = transform.forward * vInput + transform.right * hzInput;       

        controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime);
        // 싯팔 ㅠ
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
        return false;        
    }
    void Gravity()
    {
        // 떠있을 때
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        // 바닥에 닿고 잠시 떠있을 오류 방지 위함
        else if (velocity.y < 0)
        {
            velocity.y = -2;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    void Falling() => anim.SetBool("Falling", !IsGrounded());
    public void JumpForce() => velocity.y += jumpForce;

    public void Jumped() => jumped = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    }

}
