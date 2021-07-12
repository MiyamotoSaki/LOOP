using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class LanguageSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ui = null;
    [SerializeField]
    private GameObject m_toggle_japanese = null;
    [SerializeField]
    private GameObject m_toggle_english = null;

    static bool m_is_japanese = true;

    static int REFLECTION_MODE = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_toggle_japanese.GetComponent<Toggle>().isOn)
        {
            m_is_japanese = true;
        }
        if (m_toggle_english.GetComponent<Toggle>().isOn)
        {
            m_is_japanese = false;
        }
    }

    public static bool Get_Is_Japanese()
    {
        return m_is_japanese;
    }
    public static void Set_Is_Japanese(bool _true)
    {
        m_is_japanese = _true;
    }

    public void Show_Window()
    {
        m_ui.SetActive(true);
    }
    public void Close_Window()
    {
        m_ui.SetActive(false);
    }

    public static int Get_MODE()
    {
        return REFLECTION_MODE;
    }

    public static void Set_MODE(int i)
    {
        REFLECTION_MODE = i;
    }
}
