using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �Q��
    public Player_State sc_state;
    public Player_Axis sc_axis;
    public Dummy_Field Dummy_Field;

    TOWER TOWER;
    Leba leba;
    //leba_2 leba_2;
    Bridge bridge;
    Door door;
    Block block;
    CATCH_POINT catch_point;
    GameObject Pipe1;
    GameObject Pipe2;
    GameObject Pipe3;

    GameObject UI_bridge;
    GameObject UI_block;
    GameObject UI_block2;
    GameObject UI_frame;
    GameObject UI_leba;
    GameObject UI_stage;
    GameObject UI_Cristal1;
    GameObject UI_Cristal2;
    GameObject UI_Cristal3;

    public Player_Forword Player_Forword;
    public Player_Check Player_Check;
    public Player_Under Player_Under;

    //int NoComand;

    public int Actcount;    //�e�A�N�V�����̏�������
    public float Act_spin;  //�M�~�b�N���쎞�̌����␳�l
    public Vector3 Act_move;  //�A�N�V�����ɂ��ړ���
    public Vector3 Gimmikpoint; //����Ώۂ̈ʒu
    public Vector3 Gatepoint;   //�z�����܂��ꏊ
    public float pop_y;         //���o��
    public float len;  //����
    private float searchAngle = 80f;    //����p
    private float Size = 1.0f;
    private float rot_z;   //��]���x
    private Vector3 Last_Direction; //�Ō�̓��͌���
    private int Last_State; //���O�̃X�e�[�g
    public float mag;

    Vector3 Base_Size;

    bool HIT_TOWER = false;
    bool HIT_LEVER = false;
    //public bool HIT_LEVER2;
    bool HIT_BRIDGE = false;
    bool HIT_LEVER_BACK = false;
    bool HIT_DOOR = false;

    public bool IsUnder_m = false;

    bool Forced;    //�����������s��
    bool CATCH; //�u���b�N�������Ă�

    bool Clear;
    bool Menu_ON;

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

    [SerializeField] private float m_Second_Climb = 3.0f;
    private float m_Count_Second = 0;


    private Vector3 Position_Latest_m;
    private Vector3 StartPosition = new Vector3(0, 0, 0);

    public bool is_block = false;
    public bool is_stage = false;
    bool _isMove;

    // ����
    float Speed_Walk = 25;
    float Speed_Run = 40;

    public int Under_count;
    public int No_Under;

    public void Set_Camera(GameObject _camera)
	{
        Camera = _camera;
	}


	Camera_Move sc_camera_move;


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

        UI_bridge = GameObject.Find("UI_Bridge");
        UI_block = GameObject.Find("UI_Block");
        UI_block2 = GameObject.Find("UI_Block2");
        UI_frame = GameObject.Find("UI_Frame");
        UI_leba = GameObject.Find("UI_leba");
        UI_stage = GameObject.Find("UI_Stage");
        UI_Cristal1 = GameObject.Find("UI_crystal");
        UI_Cristal2 = GameObject.Find("crystal_back");
        UI_Cristal3 = GameObject.Find("UI_crystal_num_Image");

        m_Count_Second = 0;
        Last_Direction = new Vector3(0, 0, -1);
        Forced = false;

        Base_Size = transform.localScale;

        Under_count = 0;
        No_Under = 0;
        Clear = false;
        Menu_ON = false;




		sc_camera_move = Camera.GetComponent<Camera_Move>();

	}

    void Update()
    {
		if (!CATCH)
        {
            len = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2));
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

        if(Clear)
        {
            UIset_Non();
            sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);
            sc_state.Set_Motion(Player_State.e_PlayerAnimationState.WAITING);
        }
        else if(is_block)
        {
            if(!CATCH)
            {
                UIset_Block();
            }
            else
            {
                UIset_Block2();
            }
        }
        else if(HIT_LEVER)
        {
            UIset_Leba();
        }
        else if(HIT_BRIDGE && bridge != null)
        {
            if (Check_Bridge())
            {
                UIset_Bridge();
            }
            else
            {
                UIset_Non();
            }
        }
        else if(HIT_DOOR)
        {
            UIset_Frame();
        }
        else if(sc_state.Getcan_Clime())
        {
            UIset_Stage();
        }
        else
        {
            UIset_Non();
        }
    }

    // ����X�V
    void FixedUpdate()
    {
        /*
        if (IsUnder_m)
        {
            Rigid.AddForce(new Vector3(0, 0.15f, 0));
        }
        */

        if(Clear || Menu_ON)
        {
            return;
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
        else
        {
            is_block = false;
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


		Vector3 camera_up;
		camera_up = Camera.transform.up;


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
                // ���� �i�s�����̊m��
                Vector3 direction_move = new Vector3(0, 0, 0);
                _isMove = false;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                {
					if (sc_camera_move.Get_Looking_FromUp())
					{
						if (Input.GetKey(KeyCode.W)) direction_move += camera_up;
						if (Input.GetKey(KeyCode.S)) direction_move -= camera_up;
					}
					else
					{
						if (Input.GetKey(KeyCode.W)) direction_move += camera_front;
						if (Input.GetKey(KeyCode.S)) direction_move -= camera_front;
					}
                    if (Input.GetKey(KeyCode.D)) direction_move += camera_right;
                    if (Input.GetKey(KeyCode.A)) direction_move -= camera_right;

                    // ����
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Speed_Move = Speed_Run;
                        sc_state.Set_IsRunning(true);
                    }
                    else if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        Speed_Move = Speed_Walk;
                        sc_state.Set_IsRunning(false);
                    }

                    _isMove = true;
                }
                else
                {
                    // ���c�N�p3
                    if (Mathf.Abs(Input.GetAxis("Vertical_p")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal_p")) > 0)
                    {
						// ���X�������R���g���[���[����
						if (sc_camera_move.Get_Looking_FromUp())
						{
							direction_move += camera_up * Input.GetAxis("Vertical_p");
                            direction_move += camera_right * Input.GetAxis("Horizontal_p");
                        }
						else
						{
							direction_move += camera_front * Input.GetAxis("Vertical_p");
                            direction_move += camera_right * Input.GetAxis("Horizontal_p");
                        }

						// ����
						if (Input.GetButton("Run"))
                        {
                            Speed_Move = Speed_Run;
                            sc_state.Set_IsRunning(true);
                        }
                        else if (!Input.GetButton("Run"))
                        {
                            Speed_Move = Speed_Walk;
                            sc_state.Set_IsRunning(false);
                        }

                        _isMove = true;
                    }
                }

                //�����܂łŐi�s���������߂�

                // ���K��
                if (direction_move != new Vector3(0, 0, 0))
                {
                    // Y�������폜
                    direction_move.y = 0;
                    direction_move = direction_move.normalized;// * Time.deltaTime;

                    Last_Direction = direction_move;
                }

                Debug.Log(direction_move);

                // �ړ�//�i�s�����ɃI�u�W�F�N�g����������@�������։�]
                //Rigid.velocity = direction_move * Speed_Move;

                if (_isMove)
                {
                    Vector3 Vel = Rigid.velocity;
                    Vel.y = 0;
                    Rigid.velocity = Vel;

                    if (Speed_Move == Speed_Walk)
                    {
                        if (Rigid.velocity.magnitude < 4)
                        {
                            Vector3 vec_m = transform.forward;
                            //vec_m.y += 0.35f;
                            Rigid.AddForce(vec_m * Speed_Move);
                            //Debug.Log(Rigid.velocity.magnitude);
                        }
                    }
                    else
                    {
                        if (Rigid.velocity.magnitude < 7)
                        {
                            Rigid.AddForce(transform.forward * Speed_Move);
                            //Debug.Log(Rigid.velocity.magnitude);
                        }
                    }
                }

                // ����
                if (difference.y < -0.003f)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.HOVERING);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.HOVERING);
                    Rigid.velocity = new Vector3(direction_move.x, -Speed_Fall, direction_move.z);
                }
                else if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.HOVERING)
                {
                    // ���n
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.WAITING);
                }

                // ��]
                if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.WALKING)
                {
                    // ����
                    /*
                    difference.y = 0;

                    if (difference.magnitude > Rotate_Tolerance)
                    {
                        // ��]�v�Z
                        Quaternion rot = Quaternion.LookRotation(direction_move);
                        rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * RotateSpeed);
                        this.transform.rotation = rot;
                    }//difference.magnitude > Rotate_Tolerance
                    */
                }//sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.WALKING
            }//�ړ�

        }//sc_state.Get_CanAction()
        else
        {
            // �u���b�N����
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.PUSH_PUSHING)
            {
                if (IsUnder_m) Rigid.AddForce(new Vector3(0, 0.2f, 0));

                // ����
                Vector3 direction_move = new Vector3(0, 0, 0);
                _isMove = false;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
				{
					if (sc_camera_move.Get_Looking_FromUp())
					{
						if (Input.GetKey(KeyCode.W)) direction_move += camera_up;
						if (Input.GetKey(KeyCode.S)) direction_move -= camera_up;
					}
					else
					{
						if (Input.GetKey(KeyCode.W)) direction_move += camera_front;
						if (Input.GetKey(KeyCode.S)) direction_move -= camera_front;
					}
					if (Input.GetKey(KeyCode.D)) direction_move += camera_right;
                    if (Input.GetKey(KeyCode.A)) direction_move -= camera_right;

                    _isMove = true;
                }
                else
				{
					if (sc_camera_move.Get_Looking_FromUp())
					{
						direction_move += camera_up * Input.GetAxis("Vertical_p");
					}
					else
					{
						direction_move += camera_front * Input.GetAxis("Vertical_p");
					}
                    direction_move += camera_right * Input.GetAxis("Horizontal_p");

                    _isMove = true;
                }

                // ���K��
                if (direction_move != new Vector3(0, 0, 0))
                {
                    // Y�������폜
                    direction_move.y = 0;
                    direction_move = direction_move.normalized;// * Time.deltaTime;

                    //Last_Direction = direction_move;

                    sc_axis.Set_View(direction_move);
                }

                // �ړ�//�i�s�����ɃI�u�W�F�N�g����������@�������։�]
                //Rigid.velocity = direction_move * Speed_Move * 1.0f;

                

                if (_isMove)
                {
                    sc_axis.Addspeed();
                    /*
                    Vector3 Vel = Rigid.velocity;
                    Vel.y = 0;
                    Rigid.velocity = Vel;

                    if (Rigid.velocity.magnitude < 4)
                    {
                        Vector3 vec_m = transform.forward;
                        //vec_m.y += 0.35f;
                        Rigid.AddForce(vec_m * Speed_Move);
                        //Debug.Log(Rigid.velocity.magnitude);
                    }
                    */
                }

                /*
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
                */
            }//�u���b�N����

            // �悶�o��
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.CLIMBING)
            {
                // ���[�v
                if (m_Count_Second > m_Second_Climb)
                {
                    // �ʒu
                    Vector3 new_vec = new Vector3(0, 0, 0);
                    //new_vec = StartPosition + this.transform.forward * GoLength_AfterClimbing;
                    new_vec = transform.position + this.transform.forward * GoLength_AfterClimbing;
                    //new_vec.y += Height_Climb_Block;
                    new_vec.y += 1.75f; //���傤�ǂ�������
                    this.transform.position = new_vec;
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.WAITING);

                    // ������
                    //sc_state.Set_CanAction(true);
                    sc_state.Set_End_Act(5);    //�኱�̓��͋K�����Z�b�g
                    Rigid.useGravity = true;
                    sc_state.Set_IsBlock(false);
                    sc_state.Set_IsStage(false);

                    m_Count_Second = 0;
                    SetNO_SPIN(false);
                }

                // �J�E���^����
                m_Count_Second += Time.deltaTime;

                //�������ꗥ�ɂȂ����̂ŕ��򂪕s�v��
                /*
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
                */
            }

            //���ɂ�郏�[�v�ړ��i�����ύX�j
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BRIDGE_SET)
            {
                Actcount--;

                transform.Rotate(0, Act_spin, 0);

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.BRIDGE_IN);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.WAITING);
                    Actcount = 100;
                    //���������ɂ��ړ��̖�����
                    this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    Size = 1.0f;
                    rot_z = 1.0f;

                    Act_move = (Gatepoint - transform.position) / 50;
                }
            }

            //���ɂ�郏�[�v�ړ��i�z�����݁j
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BRIDGE_IN)
            {
                Actcount--;

                transform.Rotate(0, 0, rot_z);

                rot_z += 1.5f;

                if (Actcount <= 50)
                {
                    if (Actcount > 25)
                    {
                        Size -= 0.04f;
                    }

                    transform.localScale = new Vector3(Base_Size.x * Size, Base_Size.y * Size, Base_Size.z * Size);
                    transform.position += Act_move;
                }

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.BRIDGE_MOVE);
                    transform.position = Gatepoint;
                    Actcount = 70;

                    Act_move = (bridge.Getpair_pos() - transform.position) / 50;

                    Quaternion angle = Quaternion.identity;

                    angle.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 0);
                    transform.rotation = angle;

                    //�o�����̌��������킹��
                    Vector3 View = bridge.Getpair_pos();
                    View.y = transform.position.y;
                    this.transform.LookAt(View);
                    Player_Forword.PosReset();
                    Player_Check.PosReset();
                    Player_Under.PosReset();
                }
            }

            //���ɂ�郏�[�v�ړ��i�ړ��j
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BRIDGE_MOVE)
            {
                Actcount--;


                if (Actcount <= 50)
                {
                    transform.position += Act_move;
                }


                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.BRIDGE_POP);
                    Act_move = Act_move.normalized;

                    transform.position += Act_move * 0.5f;

                    Actcount = 100;
                    pop_y = 0.2f;
                }
            }

            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BRIDGE_POP)
            {
                Actcount--;

                if(Actcount == 50)
                {
                    Size = 0;
                }

                if (Actcount <= 50)
                {
                    transform.position += (Act_move * 0.03f);
                    Size += 0.02f;
                    transform.localScale = new Vector3(Base_Size.x * Size, Base_Size.y * Size, Base_Size.z * Size);

                    Vector3 pos = transform.position;
                    pos.y += pop_y;

                    transform.position = pos;

                    pop_y -= 0.008f;

                }


                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);

                    this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    this.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    transform.localScale = new Vector3(Base_Size.x, Base_Size.y, Base_Size.z);
                    sc_state.Set_CanAction(true);

                    Player_Forword.PosReset();
                    Player_Check.PosReset();
                    Player_Under.PosReset();
                    sc_state.WORLD_START();

                    Forced = false;
                }
            }

            /////////////////////

            //�z���ɂ�郏�[�v�ړ��i�����ύX�j
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.DOOR_SET)
            {
                Actcount--;

                transform.Rotate(0, Act_spin, 0);

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.DOOR_IN);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.WAITING);
                    Actcount = 100;
                    //���������ɂ��ړ��̖�����
                    this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                    Size = 1.0f;
                    rot_z = 1.0f;

                    Act_move = (Gatepoint - transform.position) / 50;
                }
            }

            //�z���ɂ�郏�[�v�ړ��i�z�����݁j
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.DOOR_IN)
            {
                Actcount--;

                transform.Rotate(0, 0, rot_z);

                rot_z += 1.5f;

                if (Actcount <= 50)
                {
                    if (Actcount > 25)
                    {
                        Size -= 0.04f;
                    }

                    transform.localScale = new Vector3(Base_Size.x * Size, Base_Size.y * Size, Base_Size.z * Size);
                    transform.position += Act_move;
                }

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.DOOR_POP);
                    transform.position = door.Getpair_pos();
                    Actcount = 70;

                    Quaternion angle = Quaternion.identity;

                    angle.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, 0);
                    transform.rotation = angle;

                    //�o�����̌��������킹��
                    Vector3 View = new Vector3(0, 0, 0);
                    View.y = transform.position.y;
                    this.transform.LookAt(View);

                    Player_Forword.PosReset();
                    Player_Check.PosReset();
                    Player_Under.PosReset();
                }
            }

            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.DOOR_POP)
            {
                Actcount--;

                if (Actcount == 50)
                {
                    Size = 0;
                }

                if (Actcount <= 50)
                {
                    transform.position += (transform.forward * 0.05f);
                    Size += 0.02f;
                    transform.localScale = new Vector3(Base_Size.x * Size, Base_Size.y * Size, Base_Size.z * Size);

                    Vector3 pos = transform.position;
                    //pos.y += pop_y;

                    //transform.position = pos;

                    //pop_y -= 0.004f;

                }

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.WAITING);

                    this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    this.gameObject.GetComponent<CapsuleCollider>().enabled = true;

                    transform.localScale = new Vector3(Base_Size.x, Base_Size.y, Base_Size.z);
                    sc_state.Set_CanAction(true);

                    Player_Forword.PosReset();
                    Player_Check.PosReset();
                    Player_Under.PosReset();

                    sc_state.WORLD_START();

                    IsUnder_m = false;
                    Under_count = 0;

                    ClearHIT_DOOR();

                    Forced = false;
                }
            }

            /////////////////////

            //�u���b�N�����ވׂ̈ړ�
            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BLOCK_MOVE)
            {
                Actcount--;

                transform.position += Act_move;

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.BLOCK_LOOK);
                    Actcount = 20;

                    //�@ �Ώۂ̕���
                    Vector3 Direction = catch_point.GetBlockPoint() - transform.position;
                    float sub_y = Direction.y;

                    Direction.y = 0;

                    Vector3 forward = transform.forward;

                    forward.y = 0;

                    var axis = Vector3.Cross(forward, Direction);   //�ǂ��������H
                    var angle = Vector3.Angle(forward, Direction);  //�p�x�i�傫�������j

                    if (axis.y > 0)
                    {
                        Act_spin = angle / 20;
                    }
                    else
                    {
                        Act_spin = -angle / 20;
                    }
                }
            }

            if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.BLOCK_LOOK)
            {
                Actcount--;

                transform.Rotate(0, Act_spin, 0);

                if (Actcount == 0)
                {
                    sc_state.Set_AnimationState(Player_State.e_PlayerAnimationState.PUSH_WAITING);
                    sc_state.Set_Motion(Player_State.e_PlayerAnimationState.PUSH_WAITING);
                    sc_state.Set_Wait_key(2);    //�኱�̓��͋K�����Z�b�g
                    Block_Catch();
                    Set_Catch();
                    sc_state.BlockUse();

                    sc_state.WORLD_START();

                    Forced = false;

                }
            }
        }



        /*
        if (sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.WAITING ||
                sc_state.Get_AnimationState() == (int)Player_State.e_PlayerAnimationState.PUSH_WAITING)
        {
            Rigid.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            Rigid.constraints = RigidbodyConstraints.None;
            Rigid.constraints = RigidbodyConstraints.FreezeRotation;
        }
        */

        /*
        //����������������
        if(Last_State == (int)Player_State.e_PlayerAnimationState.HOVERING)
        {
            if(sc_state.Get_AnimationState() != (int)Player_State.e_PlayerAnimationState.HOVERING)
            {
                Vector3 Vel = Rigid.velocity;
                Vel.y = 0;
                Rigid.velocity = Vel;   //�c�����̑��x���L�����Z��
            }
        }
        */

        //Debug.Log(Rigid.velocity);

        Rigid.velocity *= 0.95f;
        //Rigid.velocity *= 0.00f;
        //Debug.Log(Rigid.velocity);

        Last_State = sc_state.Get_AnimationState();


        //�����̐؂�ւ�
        if (!CATCH && !Forced)
        {
            Vector3 axis = Vector3.Cross(transform.forward, Last_Direction);    //�ǂ��������H

            Quaternion rot = Quaternion.LookRotation(Last_Direction);

            rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * RotateSpeed);
            this.transform.rotation = rot;
        }

        if(IsUnder_m && Actcount == 0)
        {
            Under_count++;

            if(Under_count == 15)
            {
                if(transform.position.y > 12.4f)
                {
                    Dummy_Field.Setlevel(4);
                }
                else if (transform.position.y > 8.9f)
                {
                    Dummy_Field.Setlevel(3);
                }
                else if(transform.position.y > 5.45f)
                {
                    Dummy_Field.Setlevel(2);
                }
                else
                {
                    Dummy_Field.Setlevel(1);
                }
            }
        }
        else
        {
            No_Under++;

            if(No_Under == 5)
            {
                Dummy_Field.Setlevel(1);
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

        if(!IsUnder_m)
        {
            Under_count = 0;
        }
        else
        {
            No_Under = 0;
        }
    }

    public void Set_Act_spin()
    {
        //�@ �Ώۂ̕���
        Vector3 Direction = Gimmikpoint - transform.position;
        float sub_y = Direction.y;

        Direction.y = 0;

        Vector3 forward = transform.forward;

        forward.y = 0;

        var axis = Vector3.Cross(forward, Direction);   //�ǂ��������H
        var angle = Vector3.Angle(forward, Direction);  //�p�x�i�傫�������j

        if(axis.y > 0)
        {
            Act_spin = angle / 50;
        }
        else
        {
            Act_spin = -angle / 50; 
        }

        Forced = true;
        Actcount = 50;

    }

    public void Set_ActMove_Block()
    {
        //���������ɂ��ړ��̖�����
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

        Vector3 target = catch_point.GetPoint();
        target.y = transform.position.y;

        transform.LookAt(target);
        Act_move = (target- transform.position) / 50;

        Forced = true;
        Actcount = 50;
    }


    //�I�u�W�F�N�g����̓����蔻�葀��

    public void SetHIT_TOWER(Vector3 pos)
    {
        if (CheckView(pos))
        {
            HIT_TOWER = true;
            sc_state.Set_IsTower(HIT_TOWER);
            Gimmikpoint = pos;
        }
        else
        {
            ClearHIT_TOWER();
        }
    }

    public void ClearHIT_TOWER()
    {
        HIT_TOWER = false;
        sc_state.Set_IsTower(HIT_TOWER);
    }

    public void SetHIT_LEVER(Vector3 pos)
    {
        if(CheckView(pos))
        {
            HIT_LEVER = true;
            sc_state.Set_IsLever(HIT_LEVER);
            Gimmikpoint = pos;
        }
        else
        {
            ClearHIT_LEVER();
        }
    }

    public bool SetHIT_Bridge(Vector3 pos)
    {
        if (CheckView(pos))
        {
            HIT_BRIDGE = true;
            sc_state.Set_IsBridge(HIT_BRIDGE);
            Gimmikpoint = pos;
            return true;
        }
        else
        {
            ClearHIT_BRIDGE();
        }
        return false;
    }

    public bool SetHIT_Door(Vector3 pos)
    {
        if (CheckView(pos))
        {
            HIT_DOOR = true;
            sc_state.Set_IsDoor(HIT_DOOR);
            Gimmikpoint = pos;
            return true;
        }
        else
        {
            ClearHIT_DOOR();
        }
        return false;
    }

    public void ClearHIT_LEVER()
    {
        HIT_LEVER = false;
        sc_state.Set_IsLever(HIT_LEVER);
    }

    public void ClearHIT_BRIDGE()
    {
        HIT_BRIDGE = false;
        sc_state.Set_IsBridge(HIT_BRIDGE);
    }

    public void ClearHIT_DOOR()
    {
        HIT_DOOR = false;
        sc_state.Set_IsDoor(HIT_DOOR);
    }

    public void SetHIT_LEVER_BACK()
    {
        HIT_LEVER_BACK = true;
    }

    public void ClearHIT_LEVER_BACK()
    {
        HIT_LEVER_BACK = false;
    }

    public bool Check_Bridge()
    {
        return bridge.GetUse();
    }

    bool CheckView(Vector3 pos)
    {
        //�@ �Ώۂ̕���
        Vector3 Direction = pos - transform.position;
        float sub_y = Direction.y;

        Direction.y = 0;

        Vector3 forward = transform.forward;

        forward.y = 0;

        var angle = Vector3.Angle(forward, Direction);

        //�@�T�[�`����p�x���������甭�����Ă���
        if (angle <= searchAngle)
        {
            return true;
        }

        return false;
    }

    public void SetGate(Vector3 pos)
    {
        Gatepoint = pos;
    }

    public void UseLever()
    {
        if (HIT_LEVER)
        {
            leba.SpinL();
        }
    }

    public void UseLever_inv()
    {
        if (HIT_LEVER)
        {
            leba.SpinR();
        }
    }

    public void Set_Block(Block scr)
    {
        block = scr;
        block.SetPlayer(this);
    }

    public void Block_Catch()
    {
        block.Set_ON();
    }

    public void Block_relase()
    {
        block.Clare_ON();
    }


    public void Set_Catch()
    {
        CATCH = true;
    }

    public void Clare_Catch()
    {
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        CATCH = false;
    }

    public bool Get_Catch()
    {
        return CATCH;
    }

    public void MOVE_STOP()
    {
        Rigid.velocity = new Vector3(0, 0, 0);
    }

    public Vector3 GetForward()
    {
        return this.transform.forward;
    }

    public Vector3 GetLastDirection()
    {
        return Last_Direction;
    }

    public void Set_Clear()
    {
        Clear = true;
        sc_state.Set_Clear();
    }

    public void block_out()
    {
        sc_state.release_block2();
    }

    public bool GET_WARP_OK()
    {
        return door.GET_WARP_OK_pair();
    }

    public void Set_Menu_On()
    {
        Menu_ON = !Menu_ON;
    }

    public void SetNO_SPIN(bool _is)
    {
        Pipe1.GetComponent<SPIN_FloorOne>().Set_No_SPIN(_is);
        Pipe2.GetComponent<SPIN_FloorOne>().Set_No_SPIN(_is);
        Pipe3.GetComponent<SPIN_FloorOne>().Set_No_SPIN(_is);
    }

    public bool GetSPIN_NOW()
    {
        bool flag;

        flag = Pipe1.GetComponent<SPIN_FloorOne>().SPIN_NOW()  || Pipe2.GetComponent<SPIN_FloorOne>().SPIN_NOW() || Pipe3.GetComponent<SPIN_FloorOne>().SPIN_NOW();

        return flag;
    }

    private void UIset_Bridge()
    {
        UI_bridge.SetActive(true);
        UI_block.SetActive(false);
        UI_block2.SetActive(false);
        UI_frame.SetActive(false);
        UI_leba.SetActive(false);
        UI_stage.SetActive(false);
    }

    private void UIset_Block()
    { 
        UI_bridge.SetActive(false);
        UI_block.SetActive(true);
        UI_block2.SetActive(false);
        UI_frame.SetActive(false);
        UI_leba.SetActive(false);
        UI_stage.SetActive(false);
    }

    private void UIset_Block2()
    {
        UI_bridge.SetActive(false);
        UI_block.SetActive(false);
        UI_block2.SetActive(true);
        UI_frame.SetActive(false);
        UI_leba.SetActive(false);
        UI_stage.SetActive(false);
    }

    private void UIset_Frame()
    {
        UI_bridge.SetActive(false);
        UI_block.SetActive(false);
        UI_block2.SetActive(false);
        UI_frame.SetActive(true);
        UI_leba.SetActive(false);
        UI_stage.SetActive(false);
    }

    private void UIset_Leba()
    {
        UI_bridge.SetActive(false);
        UI_block.SetActive(false);
        UI_block2.SetActive(false);
        UI_frame.SetActive(false);
        UI_leba.SetActive(true);
        UI_stage.SetActive(false);
    }

    private void UIset_Stage()
    {
        UI_bridge.SetActive(false);
        UI_block.SetActive(false);
        UI_block2.SetActive(false);
        UI_frame.SetActive(false);
        UI_leba.SetActive(false);
        UI_stage.SetActive(true);
    }

    private void UIset_Non()
    {
        UI_bridge.SetActive(false);
        UI_block.SetActive(false);
        UI_block2.SetActive(false);
        UI_frame.SetActive(false);
        UI_leba.SetActive(false);
        UI_stage.SetActive(false);
        if (Clear)
        {
            UI_Cristal1.SetActive(false);
            UI_Cristal2.SetActive(false);
            UI_Cristal3.SetActive(false);
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
            //leba_2 = other.GetComponent<leba_2>();
        }

        if (other.gameObject.CompareTag("Bridge_HIT"))
        {
            bridge = other.GetComponent<Bridge_HIT>().GetBridge();
        }

        if (other.gameObject.CompareTag("Door_HIT"))
        {
            door = other.GetComponent<Door_HIT>().GetDoor();
        }

        if (other.gameObject.CompareTag("CATCH_POINT"))
        {
            catch_point = other.GetComponent<CATCH_POINT>();
        }
    }
    
    public void Spin_Stage(int Spin)
    {
        //transform.Rotate(0, 1 * Spin, 0);
        Last_Direction = Quaternion.Euler(0, 1 * Spin, 0) * Last_Direction;
    }
}
