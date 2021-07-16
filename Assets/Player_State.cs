using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{
    // �Q��
    public Player sc_move;
    public Player_Forword sc_forword;
    public Player_Check sc_check;
    public Animator animator;
    public UI_Menu UI_menu;
    public Camera_Move cm;
    public THE_WORLD the_world;


	public follow_camera_miya sc_follow_camera;


	// ��
	public enum e_PlayerAnimationState
    {
        WAITING,        // �ҋ@
        WALKING,        // ����
        ABANDONED,      // ���u
        RUNNING,        // ����
        CLIMBING,       // �悶�o��
        PUSH_WAITING,   // �����ҋ@
        PUSH_PUSHING,   // ����
        PULL_WAITING,   // �����ҋ@
        PULL_PULLING,   // ����
        LEVER_WAITING,  // ���o�[�ҋ@
        LEVER_RIGHT,    // ���o�[�E
        LEVER_LEFT,     // ���o�[��
        HOVERING,       // ��
        LANDING,        // ���n
        BRIDGE_SET,     // ���ɂ�郏�[�v�����ύX
        BRIDGE_IN,      // ���ɂ�郏�[�v�z������
        BRIDGE_MOVE,    // ���ɂ�郏�[�v�ړ�
        BRIDGE_POP,     // ���ɂ�郏�[�v�ďo��
        DOOR_SET,       // ���ɂ�郏�[�v�����ύX
        DOOR_IN,        // ���ɂ�郏�[�v�z������
        DOOR_POP,       // ���ɂ�郏�[�v�ďo��
        BLOCK_MOVE,     // �u���b�N�̋K��ʒu�ւ̈ړ�
        BLOCK_LOOK,     // �u���b�N��������
        PUSH_PUSHING_INV,   // �����t�Đ�
    }

    // �ϐ�
    Rigidbody Rigid;
    public int m_AnimationState = (int)e_PlayerAnimationState.WAITING;  //��ԃX�e�[�g
    public int m_AnimationState_Motion = (int)e_PlayerAnimationState.WAITING;   //���A�j���[�V�����X�e�[�g
    public bool m_CanAction = true;
    //bool	m_IsClockwise		= true;
    public bool m_CanClimb_forword = false;
    public bool m_CanClimb_check = false;
    public bool IsBlock = false;
    public bool IsStage = false;
    bool IsRunning = false;

    public GameObject m_parent;

    //�����ǉ�
    public bool IsLever = false;
    public bool IsTower = false;
    public bool IsBridge = false;
    public bool IsDoor = false;
    bool Clear;
    bool Menu_ON;
    bool FLOOR_SPIN;    //�t���A��]��

    int wait_Act;
    int wait_key;

    // �f�o�b�O�p
    int state_past = (int)e_PlayerAnimationState.WAITING;


    // �T�E���hmiya
    public sound_select sc_select;


	// �T�E���h����
	AudioSource audio;


	// ������
	void Start()
    {
        // Rigidbody�擾
        Rigid = this.GetComponent<Rigidbody>();
        wait_Act = 0;
        wait_key = 0;
        Clear = false;
        Menu_ON = false;


		// �T�E���h����
		audio = this.GetComponent<AudioSource>();
	}

    // �X�V
    void Update()
    {
        if(Clear)
        {
            animator.SetInteger("state", m_AnimationState_Motion);
            return;
        }

        // �f�o�b�O
        if (state_past != m_AnimationState)
        {

			// �T�E���h����
			if (m_AnimationState == (int)e_PlayerAnimationState.WALKING)
			{
				audio.pitch = 1.0f;
				audio.Play();
			}
			else if (m_AnimationState == (int)e_PlayerAnimationState.RUNNING)
			{
				audio.pitch = 1.5f;
				audio.Play();
			}
			else
			{
				audio.pitch = 1.0f;
				audio.Stop();
			}



			state_past = m_AnimationState;
            //Debug.Log("Animation State�F" + m_AnimationState);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu"))
        {
            UI_menu.SetShow();
            sc_move.Set_Menu_On();
            Menu_ON = !Menu_ON;

            if(Menu_ON)
            {
                the_world.world_stop();
            }
            else
            {
                the_world.world_start();
            }



            // �T�E���hmiya
            if (sc_select) sc_select.Play();


            if (Menu_ON)
            {
                cm.Set_Menu(true);

				if (sc_follow_camera)sc_follow_camera.Set_isMenu(true);
			}
            else
            {
                cm.Set_Menu(false);

				if (sc_follow_camera) sc_follow_camera.Set_isMenu(false);
			}                       
        }

        //�s���K�����͓��͋֎~
        if (wait_Act > 0 || wait_key > 0 || Menu_ON)
        {
            Debug.Log(m_AnimationState);
            animator.SetInteger("state", m_AnimationState_Motion);
            return;
        }

        // �A�N�V�����\
        if (m_CanAction)
        {
            // �������Ă��Ȃ�
            m_AnimationState = (int)e_PlayerAnimationState.WAITING;
            m_AnimationState_Motion = (int)e_PlayerAnimationState.WAITING;

            // ���s
            if
            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // ���c�N�p3�ύX
                if (!IsRunning)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.WALKING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.WALKING;
                }
                else
                {
                    m_AnimationState = (int)e_PlayerAnimationState.RUNNING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.RUNNING;
                }
            }
            else if (Mathf.Abs(Input.GetAxis("Vertical_p")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal_p")) > 0)
            {
                // ���c�N�p3�ύX
                if (!IsRunning)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.WALKING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.WALKING;
                }
                else
                {
                    m_AnimationState = (int)e_PlayerAnimationState.RUNNING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.RUNNING;
                }
            }

            // �f�o�b�O
            //Debug.Log("F : " + m_CanClimb_forword);
            //Debug.Log("C : " + m_CanClimb_check);

            // �悶�o�� Climb
            if ((Input.GetKey(KeyCode.Space) || Input.GetButton("Run")) && !sc_move.GetSPIN_NOW())
            {
                // �o�����̂������
                if (m_CanClimb_forword && !m_CanClimb_check)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.CLIMBING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.CLIMBING;
                    m_CanAction = false;

                    sc_move.SetNO_SPIN(true);

                    //���[�v�Ȃ̂ŕs�v
                    //Rigid.useGravity = false;

                    sc_move.Set_StartPosition(this.transform.position);
                }
            }

            // �쓮
            if (Input.GetKey(KeyCode.J) || Input.GetButton("OK"))// A�{�^��
            {
                // �Ώۂɂ���ăX�e�[�g�ύX
                // �u���b�N
                if (IsBlock && !sc_move.GetSPIN_NOW())
                {
                    //m_AnimationState = (int)e_PlayerAnimationState.PUSH_WAITING;
                    m_AnimationState = (int)e_PlayerAnimationState.BLOCK_MOVE;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.WALKING;
                    sc_move.Set_ActMove_Block();
                    the_world.world_stop();
                    m_CanAction = false;
                    //sc_move.Block_Catch();
                    //sc_move.Set_Catch();

                    // �u���b�N���v���C���[�̎q��
                    //if (sc_forword.Get_Block())
                    //{
                    /*
                    sc_forword.Get_Block().transform.parent = this.transform;
                    sc_forword.Get_Block().GetComponent<BoxCollider>().size = new Vector3(2.2f, 1.8f, 2.2f);
                    //sc_forword.Get_Block().GetComponent<Rigidbody>().useGravity = false;
                    sc_forword.Get_Block().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    sc_forword.Get_Block().GetComponent<Rigidbody>().mass =1;
                    sc_move.Block_Catch();
                    */

                    sc_move.MOVE_STOP();    //���x���S�L�����Z��
                    /*
                    m_parent.GetComponent<Player_Axis>().LookConect(sc_move.GetForward());  //�������v���C���[�Ɠ���
                    m_parent.GetComponent<Player_Axis>().SetPosition(this.transform.position + sc_move.GetForward());   //�ʒu���v���C���[�̑O����
                    m_parent.GetComponent<Player_Axis>().Set_View(sc_move.GetLastDirection());
                    m_parent.GetComponent<Player_Axis>().SetUse(true);

                    sc_forword.Get_Block().transform.parent = this.transform;
                    // �v���C���[�𒆐S���̎q��
                    this.transform.parent = m_parent.transform;
                    */
                    //}

                }
                else if (IsLever)
                {
                    sc_move.UseLever();
                }
                else if (IsBridge && !sc_move.GetSPIN_NOW())
                {
                    if (sc_move.Check_Bridge())
                    {
                        m_AnimationState = (int)e_PlayerAnimationState.BRIDGE_SET;
                        m_AnimationState_Motion = (int)e_PlayerAnimationState.WALKING;
                        m_CanAction = false;
                        sc_move.Set_Act_spin();
                        the_world.world_stop(); //�t���A�̉�]�𖳌�
                    }
                }
                else if (IsDoor && !sc_move.GetSPIN_NOW())
                {
                    //�ړ��悪���܂��Ă��Ȃ����
                    if (sc_move.GET_WARP_OK())
                    {
                        m_AnimationState = (int)e_PlayerAnimationState.DOOR_SET;
                        m_AnimationState_Motion = (int)e_PlayerAnimationState.WALKING;
                        m_CanAction = false;
                        sc_move.Set_Act_spin();
                        the_world.world_stop(); //�t���A�̉�]�𖳌�
                    }
                }
            }

            if (Input.GetKey(KeyCode.I) || Input.GetButton("NO"))// B�{�^��
            {
                // �Ώۂɂ���ăX�e�[�g�ύX

                if (IsLever)
                {
                    sc_move.UseLever_inv();
                }
            }


        }//m_CanAction
        else
        {
            // ����
            if (m_AnimationState == (int)e_PlayerAnimationState.PUSH_WAITING)
            {
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                    Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_PUSHING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.PUSH_PUSHING;
                }
                else if (Mathf.Abs(Input.GetAxis("Vertical_p")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal_p")) > 0)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_PUSHING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.PUSH_PUSHING;
                }
            }
            else if (m_AnimationState == (int)e_PlayerAnimationState.PUSH_PUSHING)
            {
                if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
                    !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) &&
                    Mathf.Abs(Input.GetAxis("Vertical_p")) == 0 && 
                    Mathf.Abs(Input.GetAxis("Horizontal_p")) == 0)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_WAITING;
                    m_AnimationState_Motion = (int)e_PlayerAnimationState.PUSH_WAITING;
                }
            }
        }

        // �L�����Z��
        if (Input.GetKey(KeyCode.K) || Input.GetButton("NO"))// B�{�^��
        {
            // �Y������`�F�b�N
            if
            (
                m_AnimationState == (int)e_PlayerAnimationState.PUSH_WAITING ||
                m_AnimationState == (int)e_PlayerAnimationState.PUSH_PUSHING ||
                m_AnimationState == (int)e_PlayerAnimationState.PULL_WAITING ||
                m_AnimationState == (int)e_PlayerAnimationState.PULL_PULLING ||
                m_AnimationState == (int)e_PlayerAnimationState.LEVER_WAITING ||
                m_AnimationState == (int)e_PlayerAnimationState.LEVER_RIGHT ||
                m_AnimationState == (int)e_PlayerAnimationState.LEVER_LEFT
            )
            {
                m_AnimationState = (int)e_PlayerAnimationState.WAITING;
                m_AnimationState_Motion = (int)e_PlayerAnimationState.WAITING;
                m_CanAction = true;

                if (sc_move.Get_Catch())
                {
                    /*
                    sc_forword.Get_Block().transform.parent = null;
                    sc_forword.Get_Block().GetComponent<BoxCollider>().size = new Vector3(2.2f, 2.2f, 2.2f);
                    //sc_forword.Get_Block().GetComponent<Rigidbody>().useGravity = true;
                    sc_forword.Get_Block().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    sc_forword.Get_Block().GetComponent<Rigidbody>().mass = 2000;
                    sc_move.Block_relase();
                    sc_move.Clare_Catch();
                    m_parent.GetComponent<Player_Axis>().SetUse(false);
                    */
                    release_block();
                }
            }
        }
        //Debug.Log(m_AnimationState);
        animator.SetInteger("state", m_AnimationState_Motion);
    }

    // ����X�V
    void FixedUpdate()
    {
        if(wait_Act > 0)
        {
            wait_Act--;
            if(wait_Act == 0)
            {
                m_CanAction = true;
            }
        }

        if(wait_key > 0)
        {
            wait_key--;
        }
    }

    public void Set_CanAction(bool _can)
    {
        m_CanAction = _can;
    }

    public void Set_End_Act(int C)
    {
        wait_Act = C;
    }

    public void Set_Wait_key(int C)
    {
        wait_key = C;
    }

    public void Set_AnimationState(e_PlayerAnimationState _state)
    {
        m_AnimationState = (int)_state;
    }

    public void Set_Motion(e_PlayerAnimationState _state)
    {
        m_AnimationState_Motion = (int)_state;
    }
    public void Set_CanClimb_Forword(bool _can)
    {
        m_CanClimb_forword = _can;
    }
    public void Set_CanClimb_Check(bool _can)
    {
        m_CanClimb_check = _can;
    }
    public void Set_IsBlock(bool _is)
    {
        IsBlock = _is;
    }
    public void Set_IsStage(bool _is)
    {
        IsStage = _is;
    }

    public void Set_IsLever(bool _is)
    {
        IsLever = _is;
    }

    public void Set_IsTower(bool _is)
    {
        IsTower = _is;
    }

    public void Set_IsBridge(bool _is)
    {
        IsBridge = _is;
    }

    public void Set_IsDoor(bool _is)
    {
        IsDoor = _is;
    }

    public int Get_AnimationState()
    {
        return m_AnimationState;
    }
    public bool Get_CanAction()
    {
        return m_CanAction;
    }
    public bool Get_IsBlock()
    {
        return IsBlock;
    }
    public bool Get_IsStage()
    {
        return IsStage;
    }

    public bool Getcan_Clime()
    {
        return m_CanClimb_forword  && !m_CanClimb_check;
    }

    public void Set_IsRunning(bool _is)
    {
        IsRunning = _is;
    }

    public void Set_Clear()
    {
        Clear = true;
    }

    public void BlockUse()
    {
        m_parent.GetComponent<Player_Axis>().LookConect(sc_move.GetForward());  //�������v���C���[�Ɠ���
        m_parent.GetComponent<Player_Axis>().SetPosition(this.transform.position + sc_move.GetForward());   //�ʒu���v���C���[�̑O����
        m_parent.GetComponent<Player_Axis>().Set_View(sc_move.GetLastDirection());
        m_parent.GetComponent<Player_Axis>().SetUse(true);

        sc_forword.Get_Block().transform.parent = this.transform;
        // �v���C���[�𒆐S���̎q��
        this.transform.parent = m_parent.transform;
    }

    private void release_block()
    {
        m_CanAction = true;
        sc_forword.Get_Block().transform.parent = null;
        sc_forword.Get_Block().GetComponent<BoxCollider>().size = new Vector3(2.2f, 2.2f, 2.2f);
        //sc_forword.Get_Block().GetComponent<Rigidbody>().useGravity = true;
        sc_forword.Get_Block().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        sc_forword.Get_Block().GetComponent<Rigidbody>().mass = 2000;
        sc_move.Block_relase();
        sc_move.Clare_Catch();
        m_parent.GetComponent<Player_Axis>().SetUse(false);
        m_AnimationState = (int)e_PlayerAnimationState.WAITING;
        m_AnimationState_Motion = (int)e_PlayerAnimationState.WAITING;
    }

    public void release_block2()
    {
        release_block();
    }

    public void Menu_OFF()
    {
        Menu_ON = false;
        sc_move.Set_Menu_On();
        cm.Set_Menu(false);
    }

    public void SET_FLOOR_SPIN(bool _is)
    {
        FLOOR_SPIN = _is;
    }

    public void WORLD_START()
    {
        the_world.world_start();
    }
}
