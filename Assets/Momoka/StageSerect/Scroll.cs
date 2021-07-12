using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll : MonoBehaviour
{
    enum STATE
    {
        STAY,
        UP,
        DOWN,
    }

    enum STAGE_LEVEL
    {
        FIRST,
        EASY,
        NORMAL,
        HARD,
        EXTRA,
        END,
    }

    enum MOVE_MODE
    {
        NORMAL,
        SKIP,
    }

    enum SOUND
    {
        MOVE,
        OK,
        OPEN,
        CANCEL,
    }

    bool con_L; //�R���g���[���[���͍�
    bool con_R; //�R���g���[���[���͉E
    bool con_U; //�R���g���[���[���͏�
    bool con_D; //�R���g���[���[���͉�


    [SerializeField] int num;
    [SerializeField] bool up;
    [SerializeField] bool down;
    [SerializeField] bool a;    //������
    [SerializeField] bool d;    //������
    [SerializeField] float correction = 0.0f;   //�����l
    [SerializeField] int currentStagenum = 0;
    [SerializeField] int hard;
    [SerializeField] int normal;
    [SerializeField] int easy;

    [SerializeField] GameObject pop;

    [SerializeField] Image level;
    [SerializeField] Image lvNum;

    [SerializeField] Animator batU;
    [SerializeField] Animator batD;

    [SerializeField] Sprite[] numTex = new Sprite[10];

    [SerializeField] List<Sprite> lvTex = new List<Sprite>();
    [SerializeField] List<AudioSource> sound = new List<AudioSource>();
    [SerializeField] List<int> moveStageNum = new List<int>();
    [SerializeField] List<int> stageList = new List<int>();

    Color color = new Color(1, 1, 1, 1);
    Scrollbar sb;
    Data data;

    STAGE_LEVEL stageLevel = STAGE_LEVEL.EASY;
    STATE status = STATE.STAY;
    float nextPos = 0;
    float addValue = 0.1f;
    bool isPop = false;

    const float correctionAdd = 0.0001f; //�����ړ���
    const float scrollSpeed = 1.0f;
    const string scrollkey = "scrollkey";

    public int CurrentStagenum
    {
        get{return currentStagenum;}
    }

    void CreateStageList()
    {
        //�X�e�[�W�����X�g�ɃZ�b�g
        for (int i = 1; i <= easy; i++)
            stageList.Add(10 + i);

        for (int i = 1; i <= normal; i++)
            stageList.Add(20 + i);

        for (int i = 1; i <= hard; i++)
            stageList.Add(30 + i);
    }

    void UpdateCurrentStageNum()
    {
        //�I��ł���X�e�[�W�X�V
        currentStagenum = Mathf.FloorToInt(
               (nextPos - correction * (float)num) / (1.0f / (float)num - correction)
               + 0.001f);

        //�e�L�X�g�̔ԍ����X�V
        int w = stageList[currentStagenum] % 10;
        lvNum.sprite = numTex[w];
    }

    private void Start()
    {
        CFadeManager.FadeIn();

        sound[(int)SOUND.MOVE].Stop();
        sound[(int)SOUND.OK].Stop();
        sound[(int)SOUND.OPEN].Stop();
        sound[(int)SOUND.CANCEL].Stop();

        //�X�N���[���̏����|�W�V�������Z�b�g
        //�X�e�[�W�ɓ������L��������ꍇ���̃X�e�[�W����I���ł���
        if (PlayerPrefs.HasKey(scrollkey))
            nextPos = correction * num + PlayerPrefs.GetInt(scrollkey) * (1.0f / (float)num - correction);
        else
            nextPos = correction * num;

        //�X�N���[���o�[�̃f�[�^�ǂݍ���
        sb = GetComponent<Scrollbar>();
        sb.value = nextPos;
 
        //�X�e�[�W�̃X�e�[�^�X��ǂݍ���
        data = Data.Instance;

        //�X�e�[�W���X�g�ɃX�e�[�W�̓�Փx(10�̈�)�A�ԍ�(1�̈�)������
        CreateStageList();

        //���݂̃X�e�[�W�ԍ����X�V
        UpdateCurrentStageNum();

        //�X�e�[�W���x�����Z�b�g
        ChangeStageLevel();

        isPop = false;
    }

    void MoveSetting()
    {
        sound[(int)SOUND.MOVE].Play();

        //�ړ��̒l��ύX
        addValue = (nextPos - sb.value) / scrollSpeed;

        //���̒l���������
        //���̒l�������牺
        if (addValue > 0)
        {
            batU.SetTrigger("fly");
            status = STATE.UP;
        }
        else
        {
            batD.SetTrigger("fly");
            status = STATE.DOWN;
        }

      

        //�ԍ����������񌩂��Ȃ�����
        color.a = 0;
        lvNum.color = color;
        level.color = color;

        UpdateCurrentStageNum();

        //�X�e�[�W�̓�Փx���X�V
        ChangeStageLevel();
    }


    /////////////////////////////////////////////////////////////////////////////


    void Update()
    {
        if (isPop)
            return;

        Check_Cont();

        //�㉺
        if (up || Input.GetKeyDown(KeyCode.W) ||con_U)
        {
            if (status == STATE.STAY)
            {
                //�ŏ�K�����Ȃ�ʏ퓹���A���ɐi��
                //�ŏ�K�Ȃ�ŉ��w�Ɉړ�
                if (currentStagenum < stageList.Count - 1)
                {
                    nextPos = correction * num + (currentStagenum + 1) * (1.0f / (float)num - correction);
                }
                else
                {
                    nextPos = correction * num + 0 * (1.0f / (float)num - correction);
                }

                MoveSetting();
            }

            up = false;
        }

        if (down || Input.GetKeyDown(KeyCode.S) || con_D)
        {
            if (status == STATE.STAY)
            {
                //��F�ʏ�ړ�
                //���F��ԏ�ɒ���    
                if (currentStagenum > 0)
                {

                    nextPos = correction * num + (currentStagenum - 1) * (1.0f / (float)num - correction);
                }
                else
                {
                    nextPos = correction * num + (stageList.Count - 1) * (1.0f / (float)num - correction);

                }

                MoveSetting();
            }


            down = false;
        }

        //��Փx�ړ�
        if (Input.GetKeyDown(KeyCode.D) || con_R)
        {
            if (status == STATE.STAY)
            {
                //�X�e�[�W�̓�Փx�X�L�b�v
                MoveStageLevel();

                MoveSetting();

            }
        }

        if (Input.GetKeyDown(KeyCode.A) || con_L)
        {
            if (status == STATE.STAY)
            {
                MoveStageLevel(false);

                MoveSetting();
            }

        }

        //�������p
        if (a)
        {
            sb.value += correctionAdd;
            a = false;
            correction += correctionAdd;
        }
        if (d)
        {
            sb.value -= correctionAdd;
            d = false;
            correction -= correctionAdd;
        }


        switch (status)
        {
            case STATE.UP:
                MoveUp();
                break;

            case STATE.DOWN:
                MoveDown();
                break;

            default:
                //�ԍ���\��
                if (color.a < 1)
                {
                    color.a += 1.5f * Time.deltaTime;
                    lvNum.color = color;
                    level.color = color;
                }

                break;
        }

        if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("OK"))
        {
            int yn = data.GetStageStatus(currentStagenum);

            Debug.Log(moveStageNum[currentStagenum]);

            sound[(int)SOUND.OK].Play();

            if (yn == (int)Data.STAGE_STATUS.NONE)
                Debug.Log("���J��");
            else if (yn == (int)Data.STAGE_STATUS.OPEN)
            {
               // CFadeManager.FadeOut(moveStageNum[currentStagenum]);

                pop.SetActive(true);
                isPop = true;
            }
                
            else if (yn == (int)Data.STAGE_STATUS.CLEAR)
            {
                // CFadeManager.FadeOut(moveStageNum[currentStagenum]);

                pop.SetActive(true);
                isPop = true;
            }

            PlayerPrefs.SetInt(scrollkey, currentStagenum);
            PlayerPrefs.Save();
        }

        if (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("NO"))
        {
            sound[(int)SOUND.CANCEL].Play();
            CFadeManager.FadeOut(0);
            isPop = true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void MoveUp()
    {
        if (sb.value >= nextPos)
        {
            sb.value = nextPos;
            status = STATE.STAY;
            return;
        }

        sb.value += addValue * Time.deltaTime;
    }

    void MoveDown()
    {
        if (sb.value <= nextPos||Mathf.Abs(addValue)<0.01f)
        {
            sb.value = nextPos;
            status = STATE.STAY;
            return;
        }

        sb.value += addValue * Time.deltaTime;
    }


    ///////////////////////////////////////////////////////////////////////////////////////

    public void Open(int stagenum)
    {
        if (stagenum > stageList.Count - 1)
        {
            Debug.Log("�ő�");
            return;
        }


        if (currentStagenum < stagenum)
        {
            batU.SetTrigger("fly");
            status = STATE.UP;
            nextPos = correction * num + stagenum * (1.0f / (float)num - correction);

            addValue = (nextPos - sb.value) / scrollSpeed;

            //�I��ł���X�e�[�W�X�V
            currentStagenum = Mathf.FloorToInt(
                   (nextPos - correction * (float)num) / (1.0f / (float)num - correction)
                   + 0.001f);

            //�ԍ��e�L�X�g�̕ύX
            int w;
            w = stageList[currentStagenum] % 10;
            lvNum.sprite = numTex[w];

            ChangeStageLevel();

            data.SetStageStatus(stagenum, Data.STAGE_STATUS.OPEN);

            sound[(int)SOUND.OPEN].Play();

        }
    }

    public void ChangeStageLevel()
    {
        int lv0, lv1, lv2, lv3;
        lv0 = 0;
        lv1 = easy;
        lv2 = lv1 + normal;
        lv3 = lv2 + hard - 1;


        if (currentStagenum == lv0)
        {
            stageLevel = STAGE_LEVEL.FIRST;
        }
        else if (currentStagenum < lv1)
        {
            stageLevel = STAGE_LEVEL.EASY;
        }
        else if (currentStagenum < lv2)
        {
            stageLevel = STAGE_LEVEL.NORMAL;
        }
        else if (currentStagenum < lv3)
        {
            stageLevel = STAGE_LEVEL.HARD;
        }
        else if(currentStagenum == lv3)
        {
            stageLevel = STAGE_LEVEL.END;
        }

        int sl = 0;

        switch (stageLevel)
        {
            case STAGE_LEVEL.NORMAL:
                sl = 1;
                break;
            case STAGE_LEVEL.HARD:
                sl = 2;
                break;
            case STAGE_LEVEL.END:
                sl = 2;
                break;
        }

        level.sprite = lvTex[sl];
    }

    void MoveStageLevel(bool isUp = true)
    {
        int lv1, lv2, lv3, lv4;
        lv1 = 0;
        lv2 = easy;
        lv3 = lv2 + normal;
        lv4 = lv3 + hard - 1;

        if (isUp)
        {
            switch (stageLevel)
            {
                case STAGE_LEVEL.FIRST:
                    nextPos = correction * num + lv2 * (1.0f / (float)num - correction);
                    break;

                case STAGE_LEVEL.EASY:
                    nextPos = correction * num + lv2 * (1.0f / (float)num - correction);
                    break;

                case STAGE_LEVEL.NORMAL:
                    nextPos = correction * num + lv3 * (1.0f / (float)num - correction);
                    break;

                case STAGE_LEVEL.HARD:
                    nextPos = correction * num + lv4 * (1.0f / (float)num - correction);
                    break;

                case STAGE_LEVEL.END:
                    nextPos = correction * num + lv1 * (1.0f / (float)num - correction);
                    break;
            }
        }
        else
        {

            if (stageLevel == STAGE_LEVEL.FIRST)
                nextPos = correction * num + lv4 * (1.0f / (float)num - correction);
            else if (stageLevel==STAGE_LEVEL.EASY)
                nextPos = correction * num + lv1 * (1.0f / (float)num - correction);
            else if (stageLevel == STAGE_LEVEL.NORMAL)
                nextPos = correction * num + lv1 * (1.0f / (float)num - correction);
            else if (stageLevel == STAGE_LEVEL.HARD)
                nextPos = correction * num + lv2 * (1.0f / (float)num - correction);
            else if (stageLevel == STAGE_LEVEL.END)
                nextPos = correction * num + lv3 * (1.0f / (float)num - correction);
        }
    }


    public void JudgeYesOrNo(bool isEnter)
    {
        if (isEnter)
        {
            sound[(int)SOUND.OK].Play();
            CFadeManager.FadeOut(moveStageNum[currentStagenum]);
        }
        else
        {
            sound[(int)SOUND.CANCEL].Play();
            pop.SetActive(false);
            isPop = false;
        }
    }

    private void Check_Cont()
    {
        float LR;
        float UD;
        LR = Input.GetAxis("Horizontal_p"); //�E�Ղ�
        UD = Input.GetAxis("Vertical_p"); //��Ղ�

        con_L = false;
        con_R = false;
        con_U = false;
        con_D = false;

        if (LR > 0.5f)
        {
            con_R = true;
        }

        if (LR < -0.5f)
        {
            con_L = true;
        }

        if (UD > 0.5f)
        {
            con_U = true;
        }

        if (UD < -0.5f)
        {
            con_D = true;
        }
    }
}
