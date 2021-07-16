using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    private static Data instance;

    [SerializeField] List<int> _status = new List<int>();
    [SerializeField] bool save = false;
    const string _statusKey = "stageStatus";
    const string scrollkey = "scrollkey";
    int currentStageNum;
    [SerializeField] int stageNum;

    const int EStart = 0;
    const int NStart = 5;
    const int HStart = 12;

    public static Data Instance
    {
        get
        {
            return instance;
        }
    }

    public int CurrentStageNum
    {
        get { return currentStageNum; }
    }

    public int DataNum
    {
        get { return _status.Count; }
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

        //PlayerPrefs.DeleteKey(_statusKey);


        //�f�[�^�����[�h
        if (!PlayerPrefs.HasKey(_statusKey))
        {
            string s = null;

            for (int i = 0; i < stageNum; i++)
            {
                if (i == EStart || i == NStart || i == HStart)
                {
                    _status.Add((int)STAGE_STATUS.OPEN);
                    s += _status[i].ToString() + ",";
                }
                else
                {
                    _status.Add((int)STAGE_STATUS.NONE);
                    s += _status[i].ToString() + ",";
                }
            }

            PlayerPrefs.SetString(_statusKey, s);
            PlayerPrefs.Save();
        }
        else
        {
            Load();
        }

        if (PlayerPrefs.HasKey(scrollkey))
            currentStageNum = PlayerPrefs.GetInt(scrollkey);
        else
            currentStageNum = 0;

    }

    private void Update()
    {
        //�f�o�b�O�p
        if (save)
        {
            Save();
            save = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
            Delete();
    }


    //�C�ӂ̃X�e�[�W�ԍ������邱�ƂŌ��݂̏�Ԃ��A���Ă���
    //int�ŋA���Ă��邯��STAGE_STATUS�^�ŗp�Ӎς�
    public int GetStageStatus(int stageNum)
    {
        return _status[stageNum];
    }


    //�X�e�[�W�̏�Ԃ��X�V
    //�C�ӂ̃X�e�[�W�̔ԍ��ƍX�V����X�e�[�^�X���K�v
    public void SetStageStatus(int stageNum, STAGE_STATUS status)
    {
        _status[stageNum] = (int)status;

        Save();
    }


    /// <summary>
    /// ���݂̃X�e�[�W���N���A�����Ƃ��ɌĂ�
    /// </summary>
    public void StageClear()
    {
        currentStageNum = PlayerPrefs.GetInt(scrollkey);

        _status[currentStageNum] = (int)STAGE_STATUS.CLEAR;

        Save();
    }

    void Load()
    {
        string s = null;

        string loadData = PlayerPrefs.GetString(_statusKey);
        string[] strArray = loadData.Split(',');

        _status.Clear();

        for (int i = 0; i < strArray.Length - 1; i++)
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


    void Delete()
    {
        PlayerPrefs.DeleteKey(_statusKey);
        PlayerPrefs.DeleteKey(scrollkey);

        _status.Clear();

        //�f�[�^�����[�h
        string s = null;

        for (int i = 0; i < stageNum; i++)
        {
            if (i == EStart || i == NStart || i == HStart)
            {
                _status.Add((int)STAGE_STATUS.OPEN);
                s += _status[i].ToString() + ",";
            }
            else
            {
                _status.Add((int)STAGE_STATUS.NONE);
                s += _status[i].ToString() + ",";
            }
        }

        PlayerPrefs.SetString(_statusKey, s);
        PlayerPrefs.Save();

        CFadeManager.FadeOut(1);
    }
}