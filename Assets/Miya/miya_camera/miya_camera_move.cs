using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miya_camera_move : MonoBehaviour
{
	// �Q��
	public miya_player_state sc_state;

	// �萔
	const float HEIGHT_MAX = 17.5f;
	const float HEIGHT_MIN = 4.0f;

	// �ϐ�------------------------------------------------------------------------------------------
	// ���_���[�h
	bool	Looking_FromUp_m	= false;
	// ��{
	float	Length_FromCenter	= 0;
	float Length_FromCenter_Current = 0;
	[SerializeField] private float Speed_Rotate = 60.0f;
	//[SerializeField] private float Speed_Height = 2.0f;
	float	Height_Default	= 0;
	float	Height			= 0;
	// �p�x
	float Degree = -180;
	// �^���[���쎞
	float Length_FromCenter_Zoom = 7;
	// �I�u�W�F�N�g�Q��
	public GameObject GazePoint = null;
	public GameObject Tower_m = null;

	// ������--------------------------------------------------------------------------------------------
	void Start()
    {
		// �����l�擾
		Length_FromCenter = Mathf.Abs(this.transform.position.z);
		Length_FromCenter_Current = Length_FromCenter;
		Height_Default = this.transform.position.y;
		Height = Height_Default;
		Length_FromCenter_Zoom = 7;
	}

	// �X�V
	void Update()
	{
		// ����
		if (Input.GetKey(KeyCode.LeftArrow)) Degree += Speed_Rotate * Time.deltaTime;
		if (Input.GetKey(KeyCode.RightArrow)) Degree -= Speed_Rotate * Time.deltaTime;

		if (Tower_m && sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.WAITING_TOWER)
		{
			// �����_
			Vector3 new_pos = new Vector3(0, 5.692f, 0);
			new_pos.x = Tower_m.transform.position.x;
			new_pos.z = Tower_m.transform.position.z;
			GazePoint.transform.position = new_pos;
			// Length����
			//if (Length_FromCenter_Current < Length_MostNear) Length_FromCenter_Current = Length_MostNear;
			//if (Length_FromCenter_Current > Length_MostFar) Length_FromCenter_Current = Length_MostFar;
			// �J�����ʒu
			Vector3 result = new Vector3(0, 0, 0);
			result.x = Mathf.Sin(Degree * Mathf.Deg2Rad) * Length_FromCenter_Zoom;
			result.z = Mathf.Cos(Degree * Mathf.Deg2Rad) * Length_FromCenter_Zoom;
			result.y = Height_Default;
			this.transform.position = result;
		}
		else
		{
			// �����_
			Vector3 new_pos = new Vector3(0, 5.692f, 0);
			GazePoint.transform.position = new_pos;

			// �ړ�
			if (!Looking_FromUp_m)
			{
				// �����낵���_�֐ؑ�
				if (Input.GetKey(KeyCode.UpArrow)) Looking_FromUp_m = true;
				// �ߋ���
				//if (Input.GetKey(KeyCode.UpArrow	)) Height += Speed_Height * Time.deltaTime;
				//if (Height > HEIGHT_MAX - 0.1f) Height = HEIGHT_MAX - 0.1f;

				// �ړ�
				Vector3 result = new Vector3(0, 0, 0);
				result.x = Mathf.Sin(Degree * Mathf.Deg2Rad) * Length_FromCenter;
				result.z = Mathf.Cos(Degree * Mathf.Deg2Rad) * Length_FromCenter;
				result.y = Height_Default;
				this.transform.position = result;
			}
			else
			{
				// ����
				Height = HEIGHT_MAX - 0.1f;

				// �����̕ύX�ɔ�����������̋����ύX
				float degree = Height * 90.0f / HEIGHT_MAX;// 0.0f~10.0f = 0��~90��
				Length_FromCenter_Current = Mathf.Cos(degree * Mathf.Deg2Rad) * Length_FromCenter;// 90��= 0.0f

				// �ړ�
				Vector3 result = new Vector3(0, 0, 0);
				result.x = Mathf.Sin(Degree * Mathf.Deg2Rad) * Length_FromCenter_Current;
				result.z = Mathf.Cos(Degree * Mathf.Deg2Rad) * Length_FromCenter_Current;
				result.y = Height;
				this.transform.position = result;

				// �ʏ펋�_�֐ؑ�
				if (Input.GetKey(KeyCode.DownArrow)) Looking_FromUp_m = false;
				// �ߋ���
				//if (Input.GetKey(KeyCode.DownArrow)) Height -= Speed_Height * Time.deltaTime;
			}
		}
	}

    // ����X�V
    void FixedUpdate()
    {
		
	}

	public void Set_Tower(GameObject _Tower)
	{
		Tower_m = _Tower;
	}
	public void Release_Tower()
	{
		Tower_m = null;
	}
}
