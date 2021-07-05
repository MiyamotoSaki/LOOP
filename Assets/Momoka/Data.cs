using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    private static Data instance;

    [SerializeField] List<int> _status = new List<int>();
    [SerializeField] bool save = false;
    const string _statusKey = "stageStatus";

    public static Data Instance
    {
        get{
            return instance;
        }
    }

    public enum STAGE_STATUS
    {
        NONE,       //���J��
        OPEN,       //�J���ς�(���߂��ꍇ��)
        CLEAR,      //�N���A����
        NEW,        //�J��
    }

    private void Start()
    {
        //�ȈՃV���O���g��
        if (instance)
            Destroy(this.gameObject);
        else
            instance = this;

        //�S�ẴV�[���ɑ���
        DontDestroyOnLoad(this.gameObject);


        //�f�[�^�����[�h
        if(!PlayerPrefs.HasKey(_statusKey))
        {
            string s = null;

            for (int i = 0; i < _status.Count; i++)
            {
                _status[i] = (int)STAGE_STATUS.NONE;
                s += _status[i].ToString() + ",";
            }

            PlayerPrefs.SetString(_statusKey, s);
            PlayerPrefs.Save();
        }
        else
        {
            Load();
        }
    }

    private void Update()
    {
        //�f�o�b�O�p
        if(save)
        {
            Save();
            save = false;
        }
    }


    //�C�ӂ̃X�e�[�W�ԍ������邱�ƂŌ��݂̏�Ԃ��A���Ă���
    //int�ŋA���Ă��邯��STAGE_STATUS�^�ŗp�Ӎς�
    public int GetStageStatus(int stageNum)
    {
        return _status[stageNum];
    }


    //�X�e�[�W�̏�Ԃ��X�V
    //�C�ӂ̃X�e�[�W�̔ԍ��ƍX�V����X�e�[�^�X���K�v
    public void SetStageStatus(int stageNum,STAGE_STATUS status)
    {
        _status[stageNum] = (int)status;

        Save();
    }

    void Load()
    {
        string s = null;

        string loadData = PlayerPrefs.GetString(_statusKey);
        string[] strArray = loadData.Split(',');

        _status.Clear();

        for(int i=0;i<strArray.Length;i++)
        {
            _status.Add(int.Parse(strArray[i]));
            s += _status[i].ToString() + ",";
        }

        PlayerPrefs.SetString(_statusKey, s);
        PlayerPrefs.Save();
    }


    void Save()
    {
        string s = null;

        for (int i = 0; i < _status.Count; i++)
        {
            s += _status[i].ToString() + ",";
        }

        PlayerPrefs.SetString(_statusKey, s);
        PlayerPrefs.Save();
    }
}
