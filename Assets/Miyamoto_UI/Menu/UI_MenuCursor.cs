
// //                              // //
// //   Author�F�{�{�@����         // //
// //   ���j���[�̏���             // //
// //                              // //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ------------------------------------------------------------------------------------------

public class UI_MenuCursor : MonoBehaviour
{
    public Fade fade;
    public Player_State ps;
    RectTransform thisTransform;    // ���g�̃��N�g�g�����X�t�H�[���i���W�ύX�p)
    int CursorPosition;             // �J�[�\���ʒu�i���h���̖��j

    GameObject Parent;              // �e�I�u�W�F�N�g�iMenuCanvas�j�̂��߂̓��ꕨ
    UI_Menu ParentMenu;             // �e�I�u�W�F�N�g�ɂ����X�N���v�g���~����

    bool con_U; //�R���g���[���[���͏�
    bool con_D; //�R���g���[���[���͉�
    bool USE_KEY_BORD;

    int keywait;

    // �T�E���hmiya
    public AudioSource se_move;
    public AudioSource se_select;


	public follow_camera_miya sc_follow_camera;





	// �J�[�\���̏ꏊ�ꗗ
	enum CursorPos
    {
        Retry,
        Stage,
        Return,
        end
    };

    // ------------------------------------------------------------------------------------------

    void Start()
    {
        


        thisTransform = GetComponent<RectTransform>();        // ���N�g�g�����X�t�H�[���擾
        CursorPosition = (int)CursorPos.Retry;                // �J�[�\�������ʒu����

        Parent = GameObject.Find("MenuCanvas");               // �e�I�u�W�F�N�g���擾
        ParentMenu = Parent.GetComponent<UI_Menu>();          // �e�I�u�W�F�N�g�ɂ����X�N���v�g���擾

        keywait = 0;

        ////�܂��䂫�ύX
        ////--------------------
        //fade = GameObject.Find("Image_Fade").GetComponent<Fade>();
        //ps = GameObject.Find("PlayerCenter").GetComponent<Player_State>();
        //se_move = GameObject.Find("SE_MoveCursol").GetComponent<AudioSource>();
        //se_select = GameObject.Find("SE_Select").GetComponent<AudioSource>();
        //sc_follow_camera = GameObject.Find("Player_FollowCamera").GetComponent<follow_camera_miya>();
        ////--------------------
    }


    // ------------------------------------------------------------------------------------------

    void Update()
    {
        

        Check_Cont();


        // �J�[�\���ړ�
        if ((Input.GetKeyDown(KeyCode.DownArrow) || con_D) && keywait == 0)
        {


            // �T�E���hmiya
            if (se_move) se_move.Play();


            CursorPosition++;
            if (CursorPosition > (int)CursorPos.Return)
            {
                CursorPosition = (int)CursorPos.Retry;
            }

            keywait = 25;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || con_U) && keywait == 0)
        {


            // �T�E���hmiya
            if (se_move) se_move.Play();


            CursorPosition--;
            if (CursorPosition < (int)CursorPos.Retry)
            {
                CursorPosition = (int)CursorPos.Return;
            }

            keywait = 25;
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            USE_KEY_BORD = true;
        }

        // �J�[�\���ʒu�ɍ��킹�č��W��ύX
        switch (CursorPosition)
        {

            case (int)CursorPos.Retry:
                thisTransform.anchoredPosition = new Vector2(75.0f, 280.0f);
                break;

            case (int)CursorPos.Stage:
                thisTransform.anchoredPosition = new Vector2(75.0f, 100.0f);
                break;

            case (int)CursorPos.Return:
                thisTransform.anchoredPosition = new Vector2(75.0f, -85.0f);
                break;

            default:
                break;
        }


        // ���肪�����ꂽ��
        if (Input.GetKeyDown(KeyCode.J) || Input.GetButton("OK"))
        {
            // �T�E���hmiya
            if (se_select) se_select.Play();

            switch (CursorPosition)
            {
                case (int)CursorPos.Retry:
                    ParentMenu.Show = false;                                        // ���j���[�������Ȃ��悤�ɂ���
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);     // ���݃V�[����Ǎ����Ȃ���
                    //fade.SetOut();
                    //fade.SetNext(1);
                    CFadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
                    ParentMenu.SetNot();    //�J���Ȃ���Ԃ�
                    break;

                case (int)CursorPos.Stage:
                    ParentMenu.Show = false;        // ���j���[�������Ȃ��悤�ɂ���
                    //fade.SetOut();
                    //fade.SetNext(2);
                    CFadeManager.FadeOut(1);
                    ParentMenu.SetNot();    //�J���Ȃ���Ԃ�
                    // �X�e�[�W�I����ʃV�[���֑J��
                    break;

                case (int)CursorPos.Return:
                    ps.Menu_OFF();
                    ParentMenu.Show = false;        // ���j���[�������Ȃ��悤�ɂ���


					// �t�H���[�J����
					if (sc_follow_camera) sc_follow_camera.Set_isMenu(false);


					break;

                default:
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        if (keywait > 0)
        {
            keywait--;
        }
    }

    private void Check_Cont()
    {
        float UD;
        UD = Input.GetAxis("Vertical_p"); //��Ղ�

        con_U = false;
        con_D = false;

        if (UD > 0.5f)
        {
            con_U = true;
            //keywait = 25;
        }

        if (UD < -0.5f)
        {
            con_D = true;
            //keywait = 25;
        }

        if (!con_U && !con_D  && !USE_KEY_BORD)
        {
            keywait = 0;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            USE_KEY_BORD = false;
        }
    }
}