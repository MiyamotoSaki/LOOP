
// //                         // //
// //   Author:�{�{ ����      // //
// //   �V�[���J�ڂƃt�F�[�h  // //
// //                         // //


// // �C���N���[�h�t�@�C���I�Ȃ�� // //
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;


// // �N���X // //
public class CFadeManager : MonoBehaviour
{
    // �t�F�[�h�C��, �t�F�[�h�A�E�g�̃t���O
    public static bool isFadeIn = false;
    public static bool isFadeOut = false;


    // �t�F�[�h���ԁi�P�ʁF�a�j
    public static float FadeTime = 1.0f;


    // �J�ڐ�̃V�[���ԍ�
    private static int NextScene;


    // �t�F�[�h�̃C���^�[�t�F�[�X�擾�p
    public static InterfaceFade iFade;


    // �}�X�N�͈�
    public float CutoutRange;


    // // �t�F�[�h�C���J�n // //
    public static void FadeIn()
    {
        // �t�F�[�h�C���t���O�n�m
        isFadeIn = true;
    }


    // // �t�F�[�h�A�E�g�J�n // //
    public static void FadeOut(int nextscene)
    {
        // ���̃V�[��������
        NextScene = nextscene;

        // �t�F�[�h�A�E�g�t���O�n�e�e
        isFadeOut = true;
    }


    // // ������ // //
    void Start()
    {
        // �t�F�[�h�̃C���^�[�t�F�[�X���擾
        iFade = GetComponent<InterfaceFade>();

        // �}�X�N�͈͂��擾
        iFade.Range = CutoutRange;
    }

    // // �X�V // //
    void Update()
    {
        // �t�F�[�h�C��
        if (isFadeIn)
        {
            // �}�X�N�͈͂����炷
            iFade.Range -= FadeTime * Time.deltaTime;

            // �O���z������~�߂�
            if (iFade.Range < 0.0f)
            {
                iFade.Range = 0.0f;
                isFadeIn = false;
            }
        }


        // �t�F�[�h�A�E�g
        else if (isFadeOut)
        {
            // �}�X�N�͈͂𑝂₷
            iFade.Range += FadeTime * Time.deltaTime;

            // �P���z������~�߂�
            if (iFade.Range > 1.0f)
            {
                iFade.Range = 1.0f;
                isFadeOut = false;
                if(NextScene != 999)
                {
                    SceneManager.LoadScene(NextScene);
                }
                else
                {
                    UnityEngine.Application.Quit();
                }
            }
        }
    }
}
