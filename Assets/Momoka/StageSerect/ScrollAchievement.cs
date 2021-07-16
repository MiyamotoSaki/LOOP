using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollAchievement : MonoBehaviour
{
    Data data;
    [SerializeField] Scroll scroll;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }


    IEnumerator Init()
    {
        yield return new WaitForSeconds(0.1f);
        data = Data.Instance;
        string s;

        s = JsonUtility.ToJson(data);
        Debug.Log(s);

        Debug.Log(data.DataNum);


        //�ォ�猟��
        for (int i = data.DataNum - 1; i > 0; i--)
        {
            s = "stage" + (i + 1).ToString("00");

            if (data.GetStageStatus(i - 1) == (int)Data.STAGE_STATUS.CLEAR)
            {
                //�N���A�݂����玟��open�ɂ���(�����������ĂȂ��ꍇ)
                //���̂������
                //���ƌ��J�����Ƃ��̉�������
                s = "stage" + (i + 1).ToString("00");
                if (data.GetStageStatus(i) != (int)Data.STAGE_STATUS.NONE)
                    continue;

                data.SetStageStatus(i, Data.STAGE_STATUS.NEW);
                GameObject obj = transform.Find(s).Find("lock").gameObject;
                obj.SetActive(true);
                obj.GetComponent<Animator>().SetTrigger("open");
                scroll.Open(i);
            }
            else if (data.GetStageStatus(i) == (int)Data.STAGE_STATUS.NONE)
            {
                transform.Find(s).Find("lock").gameObject.SetActive(true);
            }


        }

        if (data.GetStageStatus(0) == (int)Data.STAGE_STATUS.NONE)
            transform.Find("stage01").Find("lock").gameObject.SetActive(true);
    }
}
