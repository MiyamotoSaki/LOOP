using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CursorMover : MonoBehaviour
{
 [SerializeField]
 private int Velocity;
 private float xMove;

 private int WaitTime = 0;
 private int Step = 1;

    public float FadeTime = 2f;

    public Image fadeImg;
    private float start;
    private float end;

    float time = 0f;

    bool isPlaying = false;
    private bool ClearFade = false;
    private Color fadecolor;

    void Start()
    {
            start = 0f;
            end = 1f;
        
    }
    void Update()
    {
        

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            OutStartFadeAnim();

        }

        CursorMove();
        ChoiceStage();
    }

    public void OutStartFadeAnim()

    {

        if (isPlaying == true)

        {

            return;

        }


        StartCoroutine(fadeinplay());


    }
    private void CursorMove()
    {
        xMove = 0;
        WaitTime--;
        if (Input.GetKey(KeyCode.RightArrow) && WaitTime < 0 && Step < 7)
        {
            xMove = Velocity * Time.deltaTime;
            WaitTime = 30;
            Step++;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && WaitTime < 0 && Step > 1)
        {
            xMove = -Velocity * Time.deltaTime;
            WaitTime = 30;
            Step--;
        }

        this.transform.Translate(new Vector3(xMove, 0, 0));

    }
    private void ChoiceStage()
    {

        if (ClearFade)
        {
            switch (Step)
            {
                case 1:
                    SceneManager.LoadScene("yb_test");
                    break;
                
            }
        }
    }

    IEnumerator fadeinplay()

    {

        isPlaying = true;



        fadecolor = fadeImg.color;

        time = 0f;

        fadecolor.a = Mathf.Lerp(start, end, time);


        while (fadecolor.a <= 0.99f)

        {

            time += Time.deltaTime / FadeTime;

            fadecolor.a = Mathf.Lerp(start, end, time);

            fadeImg.color = fadecolor;

            yield return null;

        }
        ClearFade = true;
        isPlaying = false;

    }
}
