using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameMgr : MonoBehaviour 
{
    public GameObject CandyPrefab;
    public GameObject bombPrefab;
    public GameObject blockPrefab;
    public Sprite[] CandySprite;

    private bool isPlay = false;
    private float time = 60.0f;
    private int countDown = 3;
    private GameObject timer;
    private Text timertxt;
    private GameObject score;
    private Text scoretxt;
    private bool isEnd = false;
    private float counter = 0.0f;

    private List<GameObject> candyList;
    private GameObject gage;
    private Image gageimg;
    private int feverCnt = 0;
    private bool isFever = false;
    private int feverAnimeCnt = 0;
    private float feverLimitTime = 0.0f;

    private float endtimer = 0.0f;

    private shareData sharemgr;
    private GameObject bgm;


    // Use this for initialization
    void Start () 
    {
        timer = GameObject.Find("time");
        score = GameObject.Find("score");

        timertxt = timer.GetComponent<Text>();
        scoretxt = score.GetComponent<Text>();

        timertxt.text = countDown.ToString();

        candyList = new List<GameObject>();

        gage = GameObject.Find("gage");
        gageimg = gage.GetComponent<Image>();
        gageimg.fillAmount = 0.0f;

        sharemgr = shareData.Instance;
        sharemgr.scoreNum = 0;

        endtimer = 0.0f;

        bgm = GameObject.Find("bgm");
    }

    // Update is called once per frame
    void Update () 
    {
        if (isEnd)
        {
            endtimer += Time.deltaTime;
            if (endtimer >= 2.0f)
            {
                score.active = false;
                StartCoroutine("waitDraw");
            }


        }
        else
        {
            if (!isPlay)
            {
                timertxt.text = countDown.ToString();
                if (countDown > 0)
                {
                    counter += Time.deltaTime;
                    if (counter >= 1.0f)
                    {
                        counter = 0.0f;
                        countDown -= 1;
                    }
                }
                else
                {
                    bgm.GetComponent<AudioSource>().Play();

                    isPlay = true;
                    timertxt.text = time.ToString("F1");
                    makeCandy(50, -2.0f);
                }
            }
            else
            {
                scoretxt.text = "score:" + sharemgr.scoreNum.ToString();
                time -= Time.deltaTime;
                timertxt.text = time.ToString("F1");
                if (time <= 0.0f)
                {
                    isPlay = false;
                    timertxt.text = "Finish!";
                    isEnd = true;

                    endtimer = 0.0f;
                }

                // フィーバー関連
                if (isFever)
                {
                    // アニメーション
                    feverAnimeCnt++;
                    if (feverAnimeCnt >= 2)
                    {
                        feverAnimeCnt = 0;
                        gageimg.enabled = (gageimg.enabled) ? false : true;
                    }


                    // フィーバータイム終了判定
                    feverLimitTime = Time.deltaTime / 10.0f;
                    gageimg.fillAmount -= feverLimitTime;

                    if (gageimg.fillAmount <= 0.0f)
                    {
                        gageimg.fillAmount = 0.0f;
                        feverLimitTime = 0.0f;
                        feverCnt = 0;
                        isFever = false;
                        gageimg.enabled = true;
                    }
                }

            }
        }
    }

    public void calFever(int num)
    {
        if (!isFever)
        {
            feverCnt += num;
            gageimg.fillAmount = (float)feverCnt / 30.0f;
            if (feverCnt >= 30)
            {
                feverCnt = 30;
                isFever = true;
                feverAnimeCnt = 0;
                feverLimitTime = 0;

                time += 1.0f;
                gageimg.fillAmount = 1.0f;
            }
        }
        else
        {
            time += 0.25f * (float)num;
        }
    }

    public void calScore(int num)
    {
        int temp = 50 * num * (num + 1) - 300;
        sharemgr.scoreNum += (!isFever) ? temp + 50 * num : (temp + 50 * num) * 3;
    }

    public void resetGame()
    {
        //GameObject rest = GameObject.Find("buttonse");
        //rest.GetComponent<AudioSource>().Play();

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

            // リストに追加
            candyList.Add(candy);

            StartCoroutine("sleep", 0.05f);
        }
    }

    public void makeOptionalObj(int num, Vector3 pos, int type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject obj = null;

            // 0=bomb, 1=blockを生成
            if (type == 0)
            {
                obj = Instantiate(bombPrefab) as GameObject;
                obj.tag = "bomb";
            }
            else
            {
                obj = Instantiate(blockPrefab) as GameObject;
                obj.tag = "block";

                // ブロックをリストに追加
                candyList.Add(obj);
            }

            obj.transform.position = pos;
            StartCoroutine("sleep", 0.05f);
        }
    }

    public List<GameObject> getCandyList()
    {
        return candyList;
    }

    public void delCandyfromList(GameObject obj)
    {
        if(candyList == null || obj == null)
        {
            return;
        }

        candyList.Remove(obj);
    }

    private IEnumerator sleep(float wait)
    {
        yield return new WaitForSeconds(wait);
    }

    private IEnumerator waitDraw()
    {
        yield return new WaitForEndOfFrame();

        sharemgr.gamebg.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        sharemgr.gamebg.Apply();
        SceneManager.LoadScene("resultscene");
    }

}
