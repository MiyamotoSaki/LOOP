using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{
    // �Q��
    public Player sc_move;
    public Player_Forword sc_forword;
    public Player_Check sc_check;

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
    }

    // �ϐ�
    Rigidbody Rigid;
    public int m_AnimationState = (int)e_PlayerAnimationState.WAITING;
    public bool m_CanAction = true;
    //bool	m_IsClockwise		= true;
    bool m_CanClimb_forword = false;
    bool m_CanClimb_check = false;
    public bool IsBlock = false;
    public bool IsStage = false;

    //�����ǉ�
    public bool IsLever = false;
    public bool IsTower = false;

    // �f�o�b�O�p
    int state_past = (int)e_PlayerAnimationState.WAITING;

    // ������
    void Start()
    {
        // Rigidbody�擾
        Rigid = this.GetComponent<Rigidbody>();
    }

    // �X�V
    void Update()
    {
        // �f�o�b�O
        if (state_past != m_AnimationState)
        {
            state_past = m_AnimationState;
            Debug.Log("Animation State�F" + m_AnimationState);
        }

        // �A�N�V�����\
        if (m_CanAction)
        {
            // �������Ă��Ȃ�
            m_AnimationState = (int)e_PlayerAnimationState.WAITING;

            // ���s
            if
            (
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
            )
                m_AnimationState = (int)e_PlayerAnimationState.WALKING;

            // �f�o�b�O
            //Debug.Log("F : " + m_CanClimb_forword);
            //Debug.Log("C : " + m_CanClimb_check);

            // �悶�o��
            if (Input.GetKey(KeyCode.Space))
            {
                // �o�����̂������
                if (m_CanClimb_forword && !m_CanClimb_check)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.CLIMBING;
                    m_CanAction = false;

                    Rigid.useGravity = false;

                    sc_move.Set_StartPosition(this.transform.position);
                }
            }

            // �쓮
            if (Input.GetKey(KeyCode.J))// A�{�^��
            {
                // �Ώۂɂ���ăX�e�[�g�ύX
                // �u���b�N
                if (IsBlock)
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_WAITING;
                    m_CanAction = false;

                    if (sc_forword.Get_Block())
                    {
                        sc_forword.Get_Block().transform.parent = this.transform;
                        sc_forword.Get_Block().GetComponent<BoxCollider>().size = new Vector3(2.2f,1.8f,2.2f);
                        //sc_forword.Get_Block().GetComponent<Rigidbody>().useGravity = false;
                        sc_forword.Get_Block().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }

                }
                else if(IsLever)
                {
                    sc_move.UseLever();
                }
            }

            if (Input.GetKey(KeyCode.I))// X�{�^��
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
                if
                (
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
                )
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_PUSHING;
                }
            }
            else if (m_AnimationState == (int)e_PlayerAnimationState.PUSH_PUSHING)
            {
                if
                (
                !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) &&
                !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)
                )
                {
                    m_AnimationState = (int)e_PlayerAnimationState.PUSH_WAITING;
                }
            }
        }

        // �L�����Z��
        if (Input.GetKey(KeyCode.K))// B�{�^��
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
                m_CanAction = true;

                if (sc_forword.Get_Block())
                {
                    sc_forword.Get_Block().transform.parent = null;
                    sc_forword.Get_Block().GetComponent<BoxCollider>().size = new Vector3(2.2f, 2.2f, 2.2f);
                    //sc_forword.Get_Block().GetComponent<Rigidbody>().useGravity = true;
                    sc_forword.Get_Block().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }

    // ����X�V
    void FixedUpdate()
    {

    }

    public void Set_CanAction(bool _can)
    {
        m_CanAction = _can;
    }
    public void Set_AnimationState(e_PlayerAnimationState _state)
    {
        m_AnimationState = (int)_state;
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
}
