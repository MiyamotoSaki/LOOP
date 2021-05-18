using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �Q��
    public Player_State sc_state;

    TOWER TOWER;
    Leba leba;
    leba_2 leba_2;
    GameObject Pipe1;
    GameObject Pipe2;
    GameObject Pipe3;
    bool LOCK;  //�蓮����֎~���
    int NoComand;
    public float len;  //����

    public bool HIT_TOWER;
    public bool HIT_LEVER;
    public bool HIT_LEVER2;
    bool HIT_LEVER_BACK;

    bool IsUnder_m = false;

    // �ϐ�
    Rigidbody Rigid;
    [SerializeField] private GameObject Camera;                                                                       // �����I�ɕ����̃J�����̒�����A�N�e�B�u�Ȃ��̈��I�Ԃ��ƂɂȂ�
    [SerializeField] private float Speed_Move = 8.0f;
    [SerializeField] private float RotateSpeed = 20.0f;
    [SerializeField] private float Speed_Fall = 4.0f;
    [SerializeField] private float Speed_Climb = 4.0f;
    [SerializeField] private float Height_Climb_Block = 2.3f;
    [SerializeField] private float Height_Climb_Stage = 0.75f;//1.8f;
    [SerializeField] private float GoLength_AfterClimbing = 0.5f;
    [SerializeField] private float Rotate_Tolerance = 0.1f;
    [SerializeField] private float Camera_DistanceTolerance = 100;
    private Vector3 Position_Latest_m;
    private Vector3 StartPosition = new Vector3(0, 0, 0);

    public bool is_block = false;
    public bool is_stage = false;

    // ������
    void Start()
    {
        // Rigidbody�擾
        Rigid = this.GetComponent<Rigidbody>();
        // �ߋ��̈ʒu
        Position_Latest_m = this.transform.position;

        // �J�������ݒ莞
        if (!Camera) Debug.Log("�ymiya_player_move�zthere is no camera");

        Pipe1 = GameObject.Find("FloorOne");
        Pipe2 = GameObject.Find("FloorTwo");
        Pipe3 = GameObject.Find("FloorThree");
    }

    void Update()
    {
        len = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2));
        //Debug.Log(len);
        if (len >= 12.0f)
        {
            transform.SetParent(Pipe3.transform);
        }
        else if (len >= 8.5f)
        {
            transform.SetParent(Pipe2.transform);
        }
        else
        {
            transform.SetParent(Pipe1.transform);
        }
    }

    // ����X�V
    void FixedUpdate()
    {
        if (IsUnder_m)
        {
            Rigid.AddForce(new Vector3(0, 0.15f, 0));
        }

        // ���
        Vector3 difference = this.transform.position - Position_Latest_m;
        Position_Latest_m = this.transform.position;

        // �ǂ���
        if (sc_state.Get_IsBlock())
        {
            is_block = true;
            is_stage = false;
        }
        if (sc_state.Get_IsStage())
        {
            is_block = false;
            is_stage = true;
        }

        // �J�����x�N�g���擾
        Vector3 distance = this.transform.position - Camera.transform.position; distance.y = 0;
        Vector3 camera_front;
        Vector3 camera_right;
        if (distance.magnitude < Camera_DistanceTolerance)
        {
            camera_front = Camera.transform.forward;
            camera_right = Camera.transform.right;
        }
        else
        {
            camera_front = distance;
            camera_right = Quaternion.Euler(0, 90, 0) * camera_front;
        }

        // �A�N�V�����\
        if (sc_state.Get_CanAction())
        {
            // �ړ�
            {
                // ����
                Vector3 direction_move = new Vector3(0, 0, 0);
                if (Input.GetKey(KeyCode.W)) direction_move += camera_front;
                if (Input.GetKey(KeyCode.S)) direction_move -= camera_front;
                if (Input.GetKey(KeyCode.D)) direction_move += camera_right;
                if (Input.GetKey(KeyCode.A)) direction_move -= camera_right;

                // ���K��
                if (direction_move != new Vector3(0, 0, 0))
                {
                    // Y�������폜
                    direction_move.y = 0;
                    direction_move = direction_move.normalized;// * Time.deltaTime;
                }

                // �ړ�//�i�s�����ɃI�u�W�F�N�g����������@�������։�]
                Rigid.velocity = direction_move * Speed_Move;

                // ����
                if (difference.y < -0.003f)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.HOVERING);
                    Rigid.velocity = new Vector3(direction_move.x, -Speed_Fall, direction_move.z);
                }
                else if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.HOVERING)
                {
                    // ���n
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);
                }

                // ��]
                if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.WALKING)
                {
                    // ����
                    difference.y = 0;

                    if (difference.magnitude > Rotate_Tolerance)
                    {
                        // ��]�v�Z
                        Quaternion rot = Quaternion.LookRotation(direction_move);
                        rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * RotateSpeed);
                        this.transform.rotation = rot;
                    }//difference.magnitude > Rotate_Tolerance
                }//sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.WALKING
            }//�ړ�
        }//sc_state.Get_CanAction()
        else
        {
            // �u���b�N����
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.PUSH_PUSHING)
            {
                // ����
                Vector3 direction_move = new Vector3(0, 0, 0);
                if (Input.GetKey(KeyCode.W)) direction_move += camera_front;
                if (Input.GetKey(KeyCode.S)) direction_move -= camera_front;
                if (Input.GetKey(KeyCode.D)) direction_move += camera_right;
                if (Input.GetKey(KeyCode.A)) direction_move -= camera_right;

                // ���K��
                if (direction_move != new Vector3(0, 0, 0))
                {
                    // Y�������폜
                    direction_move.y = 0;
                    direction_move = direction_move.normalized;// * Time.deltaTime;
                }

                // �ړ�//�i�s�����ɃI�u�W�F�N�g����������@�������։�]
                Rigid.velocity = direction_move * Speed_Move * 1.0f;

                // ��]
                // ����
                difference.y = 0;

                if (difference.magnitude > Rotate_Tolerance * 0.0f) //*0.5f
                {
                    // ��]�v�Z
                    Quaternion rot = Quaternion.LookRotation(direction_move);
                    rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * RotateSpeed * 0.5f);
                    this.transform.rotation = rot;
                }//difference.magnitude > Rotate_Tolerance
            }//�u���b�N����

            // �悶�o��
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.CLIMBING)
            {
                // �u���b�N
                if (is_block)
                {
                    if (this.transform.position.y < StartPosition.y + Height_Climb_Block)
                    {
                        Rigid.velocity = new Vector3(0, Speed_Climb, 0);
                    }
                    else
                    {
                        Vector3 length = this.transform.position - StartPosition; length.y = 0;
                        if (length.magnitude < GoLength_AfterClimbing && true)// �b�����w�肵�ăo�O�����
                        {
                            Rigid.velocity = this.transform.forward;
                        }
                        // �I��
                        else
                        {
                            sc_state.Set_CanAction(true);
                            Rigid.useGravity = true;

                            sc_state.Set_IsBlock(false);
                        }
                    }
                }
                // �X�e�[�W
                if (is_stage)
                {

                    if (this.transform.position.y < StartPosition.y + Height_Climb_Stage)
                    {
                        Rigid.velocity = new Vector3(0, Speed_Climb, 0);
                    }
                    else
                    {
                        Vector3 length = this.transform.position - StartPosition; length.y = 0;
                        if (length.magnitude < GoLength_AfterClimbing && true)// �b�����w�肵�ăo�O�����
                        {
                            Rigid.velocity = this.transform.forward;
                        }
                        // �I��
                        else
                        {
                            sc_state.Set_CanAction(true);
                            Rigid.useGravity = true;

                            sc_state.Set_IsStage(false);
                        }
                    }
                }
            }
        }
    }//FixedUpdate

    public void Set_StartPosition(Vector3 _start)
    {
        StartPosition = _start;
    }

    public void Set_IsUnder(bool _is)
    {
        IsUnder_m = _is;
    }

    ////�I�u�W�F�N�g���G��Ă����
    //void OnCollisionStay(Collision collision)
    //{
    //	Debug.Log("Hiting");
    //}

    //�I�u�W�F�N�g����̓����蔻�葀��

    public void SetHIT_TOWER()
    {
        HIT_TOWER = true;
        sc_state.Set_IsTower(HIT_TOWER);
    }

    public void ClearHIT_TOWER()
    {
        HIT_TOWER = false;
        sc_state.Set_IsTower(HIT_TOWER);
    }

    public void SetHIT_LEVER()
    {
        HIT_LEVER = true;
        sc_state.Set_IsLever(HIT_LEVER);
    }

    public void SetHIT_LEVER2()
    {
        HIT_LEVER2 = true;
        sc_state.Set_IsLever(HIT_LEVER2);
    }

    public void ClearHIT_LEVER()
    {
        HIT_LEVER = false;
        sc_state.Set_IsLever(HIT_LEVER);
    }

    public void ClearHIT_LEVER2()
    {
        HIT_LEVER2 = false;
        sc_state.Set_IsLever(HIT_LEVER2);
    }

    public void SetHIT_LEVER_BACK()
    {
        HIT_LEVER_BACK = true;
    }

    public void ClearHIT_LEVER_BACK()
    {
        HIT_LEVER_BACK = false;
    }

    public void UseLever()
    {
        if (HIT_LEVER)
        {
            leba.SpinL();
        }

        if (HIT_LEVER2)
        {
            leba_2.SpinL();
        }
    }

    public void UseLever_inv()
    {
        if (HIT_LEVER)
        {
            leba.SpinR();
        }

        if (HIT_LEVER2)
        {
            leba_2.SpinR();
        }
    }
    

    //�I�u�W�F�N�g�𔭌������ۂɃX�N���v�g���l������
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TOWER"))
        {
            TOWER = other.GetComponent<TOWER>();
        }

        if (other.gameObject.CompareTag("LEVER"))
        {
            leba = other.GetComponent<Leba>();
        }

        if (other.gameObject.CompareTag("LEVER_BACK"))
        {
            //Debug.Log("�l��");
            leba_2 = other.GetComponent<leba_2>();
        }
    }
    

}
