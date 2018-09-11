using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameMgr : MonoBehaviour 
{
    public GameObject CandyPrefab;
    public Sprite[] CandySprite;

    private bool isPlay = false;
    private float time = 60.0f;
    private int countDown = 5;
    private GameObject timer;
    private Text timertxt;
    private GameObject score;
    private Text scoretxt;
    private int scoreNum = 0;
    private bool isEnd = false;


	// Use this for initialization
	void Start () 
    {
        timer = GameObject.Find("time");
        score = GameObject.Find("score");

        timertxt = timer.GetComponent<Text>();
        scoretxt = score.GetComponent<Text>();

        timertxt.text = countDown.ToString();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if(isEnd)
        {
            return;
        }

        if (!isPlay)
        {
            timertxt.text = countDown.ToString();
            if (countDown > 0)
            {
                sleep(1.0f);
                countDown -= 1;
            }
            else
            {
                isPlay = true;
                timertxt.text = time.ToString("F1");
                makeCandy(50, -2.0f);
            }
        }
        else
        {
            scoretxt.text = "score:" + scoreNum.ToString();
            time -= Time.deltaTime;
            timertxt.text = time.ToString("F1");
            if(time <= 0.0f )
            {
                isPlay = false;
                timertxt.text = "Finish!";
                isEnd = true;
            }
        }

    }

    public void calScore(int num)
    {
        int temp = 50 * num * (num + 1) - 300;
        scoreNum += temp + 50 * num;
    }

    public void resetGame()
    {
        Application.LoadLevel("gameScene");
    }

    public bool isPlayFlag()
    {
        return isPlay;
    }

    public void makeCandy(int num, float offsety)
    {
        string[] tags = { "candyBlue", "candyOrange", "candyPerple",
                          "candyRed", "candyYellow" };

        for (int i = 0; i < num; i++)
        {
            GameObject candy = Instantiate(CandyPrefab) as GameObject;
            candy.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(offsety, 7.0f), 0.0f);

            int id = Random.Range(0, 5);

            SpriteRenderer tex = candy.GetComponent<SpriteRenderer>();

            tex.sprite = CandySprite[id];

            candy.tag = tags[id];

            StartCoroutine("sleep", 0.05f);
        }


    }

    private IEnumerator sleep(float wait)
    {
        yield return new WaitForSeconds(wait);
    }

}
