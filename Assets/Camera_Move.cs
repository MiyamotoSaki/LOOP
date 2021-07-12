using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class Camera_Move : MonoBehaviour
{
    // �Q��
    public Player sc_player;
    public Player_State sc_state;
    reflection reflection;

    // �萔
    [SerializeField]
    float HEIGHT_MAX = 17.5f;
    const float HEIGHT_MIN = 4.0f;

    // �ϐ�------------------------------------------------------------------------------------------
    // ���_���[�h
    bool Looking_FromUp_m = false;

	public bool Get_Looking_FromUp() { return Looking_FromUp_m; }

    // ��{
    float Length_FromCenter = 0;
    float Length_FromCenter_Current = 0;
    [SerializeField] private float Speed_Rotate = 60.0f;
    //[SerializeField] private float Speed_Height = 2.0f;
    float Height_Default = 0;
    float Height = 0;
    // �p�x
    public float Degree = -180;
    // �^���[���쎞
    float Length_FromCenter_Zoom = 7;
    // �I�u�W�F�N�g�Q��
    public GameObject GazePoint = null;
    public GameObject Tower_m = null;

    public GameObject Light_L;
    int diray = 0;

    int KeyWait = 0;

    // �N���A�J����
    CinemachineVirtualCamera normal_camera;
    public CinemachineVirtualCamera clear_camera;
    public CinemachineVirtualCamera follow_camera;
    public GameObject object_FollowCamera;

    public bool Menu_ON;
    bool CLEAR;


    // ������--------------------------------------------------------------------------------------------
    void Start()
    {
        // �����l�擾
        Length_FromCenter = Mathf.Abs(this.transform.position.z);
        Length_FromCenter_Current = Length_FromCenter;
        Height_Default = this.transform.position.y;
        Height = Height_Default;
        Length_FromCenter_Zoom = 7;

        reflection = GameObject.Find("Reflection Probe").GetComponent<reflection>();

        // �N���A�J����
        normal_camera = this.GetComponent<CinemachineVirtualCamera>();

        diray = 0;

        Menu_ON = false;
        CLEAR = false;

    }


    // �t�H���[�J����
    public void Set_FollowCamera()
    {
        clear_camera.Priority = 10;
        follow_camera.Priority = 100;
        normal_camera.Priority = 50;

        sc_player.Set_Camera(object_FollowCamera);
    }
    // �N���A�J����
    public void Set_ClearCamera()
    {
        clear_camera.Priority = 100;
        follow_camera.Priority = 50;
        normal_camera.Priority = 10;

        sc_player.Set_Camera(this.gameObject);
        CLEAR = true;
    }
    // �f�t�H���g�֖߂�
    public void Set_DefaultCamera()
    {
        clear_camera.Priority = 10;
        follow_camera.Priority = 50;
        normal_camera.Priority = 100;

        sc_player.Set_Camera(this.gameObject);
    }




    void FixedUpdate()
    {
		//// �f�o�b�O
		//if (Input.GetKey(KeyCode.O))
		//{
		//	Set_ClearCamera();
		//}
		//if (Input.GetKey(KeyCode.P))
		//{
		//	Set_DefaultCamera();
  //      }
  //      if (Input.GetKey(KeyCode.L))
  //      {
  //          Set_FollowCamera();
  //      }

        if (diray>0)
        {
            diray--;
            if (diray == 0)
            {
                Light_L.SetActive(true);
            }
        }

        if(KeyWait > 0)
        {
            KeyWait--;
        }
    }







    // �X�V
    void Update()
    {
        if (!Menu_ON)
        {
            // ����
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                if (Input.GetKey(KeyCode.LeftArrow)) Degree += Speed_Rotate * Time.deltaTime;
                if (Input.GetKey(KeyCode.RightArrow)) Degree -= Speed_Rotate * Time.deltaTime;
            }
            // �Q�[���p�b�h// ���c�N�p2
            else if (Mathf.Abs(Input.GetAxis("Horizontal_c")) > 0)
            {
                Degree += Input.GetAxis("Horizontal_c") * Speed_Rotate * Time.deltaTime;
            }
        }

        // ���c�N�p('��')�^���[�̂��߂̃Y�[��
        if (Tower_m && sc_state.Get_AnimationState() == (int)miya_player_state.e_PlayerAnimationState.WAITING_TOWER)
        {
            // �����_
            // ���c�N�p('��')new Vector3(0, 3, 0)�ɕύX
            Vector3 new_pos = new Vector3(0, 3, 0);
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
            // ���c�N�p('��')new Vector3(0, 2, 0)�ɕύX
            Vector3 new_pos = new Vector3(0, 2, 0);//����
            GazePoint.transform.position = new_pos;

            // �ړ�
            if (!Looking_FromUp_m)
            {



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



                
                // �ߋ���
                //if (Input.GetKey(KeyCode.DownArrow)) Height -= Speed_Height * Time.deltaTime;
            }


            if (!Menu_ON)
            {
                // �����낵���_�֐ؑ�
                // �Q�[���p�b�h// ���c�N�p2
                if ((Input.GetKey(KeyCode.UpArrow) || Input.GetAxisRaw("Change_c") == 1) && !CLEAR)
                {
                    Set_DefaultCamera();
                    Looking_FromUp_m = true;
                    Light_L.SetActive(false);
                    diray = -1;
                }

                if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxisRaw("Change_c") == -1) && !CLEAR && KeyWait == 0)
                {
                    reflection.Changerate();
                    KeyWait = 60;
                }

                // �ʏ펋�_�֐ؑ�//�\���{�^����
                // �Q�[���p�b�h// ���c�N�p2
                if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetAxisRaw("Juji_yoko") == -1) && !CLEAR)
                {
                    Set_DefaultCamera();
                    Looking_FromUp_m = false;
                    diray = 2;
                }

                // �t�H���[�J����//�\���{�^���E
                if ((Input.GetKey(KeyCode.RightArrow) || Input.GetAxisRaw("Juji_yoko") == 1) && !CLEAR)
                {
                    Set_FollowCamera();
                    Looking_FromUp_m = false;
                    diray = 2;
                }
            }

        }
    }

    public void Set_Tower(GameObject _Tower)
    {
        Tower_m = _Tower;
    }
    public void Release_Tower()
    {
        Tower_m = null;
    }

    public void Set_Menu(bool _is)
    {
        Menu_ON = _is;
    }
}
