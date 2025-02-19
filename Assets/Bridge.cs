using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public GameObject Laser;
    private GameObject C_Laser = null;
    public GameObject pair_Gate;
    public bool Use = false;
    public int Hit_Count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Use && C_Laser != null)
        {
            Hit_Count++;
            C_Laser.GetComponent<beam_other>().Set_End(pair_Gate.gameObject.transform.position);
            C_Laser.GetComponent<beam_other>().Set_Base(transform.position);
        }

        if (Hit_Count == 5 && C_Laser != null)
        {
            Hit_Count = 0;
            Finish_Bridge();
            Use = false;
            pair_Gate.gameObject.GetComponent<Bridge>().SetUse(Use);
        }
    }

    public void HitLaser()
    {
        Make_Pair();
        Hit_Count = 0;
        Use = true;
        pair_Gate.gameObject.GetComponent<Bridge>().SetUse(Use);
    }

    void Make_Pair()
    {
        if (!Use)
        {           
            MakeBridge();
            Use = true;
            pair_Gate.gameObject.GetComponent<Bridge>().SetUse(Use);
        }
    }

    void MakeBridge()
    {
        Quaternion Rot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        C_Laser = Instantiate(Laser, this.transform.position, Rot);
        C_Laser.GetComponent<beam_other>().Set_End(pair_Gate.gameObject.transform.position);
    }

    void Finish_Bridge()
    {
        Destroy(C_Laser);
        C_Laser = null;
    }

    public Vector3 Getpair_pos()
    {
        return pair_Gate.transform.position;
    }

    public bool GetUse()
    {
        return Use;
    }

    public void SetUse(bool _is)
    {
        Use = _is;
    }
}
