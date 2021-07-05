
// //                              // //
// //   Author�F�{�{�@����         // //
// //   ���j���[�̃J�[�\������     // //
// //                              // //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------------------------------

public class UI_Clear : MonoBehaviour
{
    public bool isUiClear;      // �N���A�t���O�i�Q�[���N���A���ɂ����true�ɂ���Ɣ������܂��B�j
    Transform ChildTransform;   // �q�I�u�W�F�N�g�̃g�����X�t�H�[��
    int time;
    public UI_Clear_add uI_Clear_Add;

    // ------------------------------------------------------------------------------------------

    void Start()
    {
        isUiClear = false;
        if(LanguageSetting.Get_Is_Japanese())
        {
            ChildTransform = GameObject.Find("ClearImage").transform;
            GameObject.Find("ClearImageEnglish").transform.gameObject.SetActive(false);
        }
        else
        {
            ChildTransform = GameObject.Find("ClearImageEnglish").transform;
            GameObject.Find("ClearImage").transform.gameObject.SetActive(false);
        }
        time = 0;
    }

    // ------------------------------------------------------------------------------------------

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space)) // �����I�ȃN���A�t���OONOFF�����ł��A�K�X�ύX�A�폜���Ă��������B
        {
            if (isUiClear)
            {
                isUiClear = false;
            }
            else
            {
                isUiClear = true;
            }
            time = 0;
        }
        */

        if (isUiClear)
        {
            if (!ChildTransform.gameObject.activeSelf)
            {
                ChildTransform.gameObject.SetActive(true);
            }
        }
        else
        {
            if (ChildTransform.gameObject.activeSelf)
            {
                ChildTransform.gameObject.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (isUiClear)
        {
            time++;
            if(time == 55)
            {
                uI_Clear_Add.Set_ON();
            }
        }
    }

    public void Set_Clear()
    {
        isUiClear = true;
        time = 0;
    }
}
