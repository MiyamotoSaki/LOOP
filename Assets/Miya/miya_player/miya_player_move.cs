using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miya_player_move : MonoBehaviour
{
	// �Q��
	public miya_player_state sc_state;

	// �ϐ�
	Rigidbody Rigid;
	[SerializeField] private GameObject Camera;                                                                       // �����I�ɕ����̃J�����̒�����A�N�e�B�u�Ȃ��̈��I�Ԃ��ƂɂȂ�
	[SerializeField] private float Speed_Move = 8.0f;
	[SerializeField] private float RotateSpeed = 20.0f;
	[SerializeField] private float Speed_Fall = 4.0f;
	//[SerializeField] private float Speed_Climb = 4.0f;
	[SerializeField] private float Height_Climb_Block = 2.3f;
	//[SerializeField] private float Height_Climb_Stage = 0.75f;//1.8f;
	[SerializeField] private float GoLength_AfterClimbing = 0.5f;
	[SerializeField] private float Rotate_Tolerance = 0.1f;
	[SerializeField] private float Camera_DistanceTolerance = 100;
	private Vector3 Position_Latest_m;
	private Vector3 StartPosition = new Vector3(0, 0, 0);

	private bool is_block = false;
	private bool is_stage = false;

	//private int Frame_Climb_m = 0;
	//[SerializeField] private float SECOND_FOR_CLEAR_BUG = 1.0f;

	bool IsUnder_m = false;
	
	[SerializeField] private float m_Second_Climb = 3.0f;
	private float m_Count_Second = 0;


	// ������
	void Start()
	{
		// Rigidbody�擾
		Rigid = this.GetComponent<Rigidbody>();
		// �ߋ��̈ʒu
		Position_Latest_m = this.transform.position;

		// �J�������ݒ莞
		if (!Camera) Debug.Log("�ymiya_player_move�zthere is no camera");

		// ������
		IsUnder_m = false;

		m_Count_Second = 0;
	}

	// ����X�V
	void FixedUpdate()
	{
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
					sc_state.Set_AnimationState(miya_player_state.e_PlayerAnimationState.HOVERING);
					Rigid.velocity = new Vector3(direction_move.x, -Speed_Fall, direction_move.z);
				}
				else if (sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.HOVERING)
				{
					// ���n
					sc_state.Set_AnimationState(miya_player_state.e_PlayerAnimationState.WAITING);
				}

				// ��]
				if (sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.WALKING)
				{
					// ��������
					if (IsUnder_m) Rigid.AddForce(new Vector3(0, 0.2f, 0));
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
			if (sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.PUSH_PUSHING)
			{
				// ��������
				if (IsUnder_m) Rigid.AddForce(new Vector3(0, 0.2f, 0));
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
				Rigid.velocity = direction_move * Speed_Move * 0.5f;

				// ��]
				// ����
				difference.y = 0;
				if (difference.magnitude > Rotate_Tolerance * 0.1f)
				{
					// ��]�v�Z//�I�t�Z�b�g��]

					//Rigid.centerOfMass = new Vector3(0, 0, 1);
					//Rigid.angularVelocity = new Vector3(0, 1, 0);

					Quaternion rot = Quaternion.LookRotation(direction_move);
					rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * RotateSpeed * 0.5f);
					this.transform.rotation = rot;
				}//difference.magnitude > Rotate_Tolerance
			}//�u���b�N����

			// �悶�o��
			if (sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.CLIMBING)
			{
				// ���[�v
				if (m_Count_Second > m_Second_Climb)
				{
					// �ʒu
					Vector3 new_vec = new Vector3(0, 0, 0);
					new_vec = StartPosition + this.transform.forward * GoLength_AfterClimbing;
					new_vec.y += Height_Climb_Block;
					this.transform.position = new_vec;

					// ������
					sc_state.Set_CanAction(true);
					Rigid.useGravity = true;
					sc_state.Set_IsBlock(false);
					sc_state.Set_IsStage(false);

					m_Count_Second = 0;
				}

				// ����
				m_Count_Second += Time.deltaTime;

				//// �u���b�N
				if (is_block)
				{
				//	if (this.transform.position.y < StartPosition.y + Height_Climb_Block)
				//	{
				//		Rigid.velocity = new Vector3(0, Speed_Climb, 0);
				//	}
				//	else
				//	{
				//		Vector3 length = this.transform.position - StartPosition; length.y = 0;
				//		if (length.magnitude < GoLength_AfterClimbing && Frame_Climb_m < SECOND_FOR_CLEAR_BUG * 50)
				//		{
				//			Rigid.velocity = this.transform.forward;
				//		}
				//		// �I��
				//		else
				//		{
				//			sc_state.Set_CanAction(true);
				//			Rigid.useGravity = true;

				//			sc_state.Set_IsBlock(false);

				//			Frame_Climb_m = 0;
				//		}

				//		Frame_Climb_m++;
				//	}
				}
				//// �X�e�[�W
				if (is_stage)
				{
				//	if (this.transform.position.y < StartPosition.y + Height_Climb_Stage)
				//	{
				//		Rigid.velocity = new Vector3(0, Speed_Climb, 0);
				//	}
				//	else
				//	{
				//		Vector3 length = this.transform.position - StartPosition; length.y = 0;
				//		if (length.magnitude < GoLength_AfterClimbing && Frame_Climb_m < SECOND_FOR_CLEAR_BUG * 50)
				//		{
				//			Rigid.velocity = this.transform.forward;
				//		}
				//		// �I��
				//		else
				//		{
				//			sc_state.Set_CanAction(true);
				//			Rigid.useGravity = true;

				//			sc_state.Set_IsStage(false);

				//			Frame_Climb_m = 0;
				//		}

				//		Frame_Climb_m++;
				//	}
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
}
