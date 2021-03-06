﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour 
{
    public GameObject effprefabbomb;
    public GameObject[] effprefabcandy;

    private GameObject firstSelObj = null;
    private GameObject lastSelObj = null;
    private List<GameObject> delObjList;
    private string culTag;
    private GameObject scenemgr;
    private gameMgr gamemgrComp;
    private bool isDragging = false;

    // Use this for initialization
    void Start () 
	{
        scenemgr = GameObject.Find("sceneMgr");
        gamemgrComp = scenemgr.GetComponent<gameMgr>();
    }
	
	// Update is called once per frame
	void Update () 
	{
        bool isplay = gamemgrComp.isPlayFlag();

        if (isplay)
        {
            if(Input.GetMouseButtonDown(0))
            {
                onClick();
            }

            if (Input.GetMouseButton(0) && firstSelObj == null)
            {
                dragStart();
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                dragEnd();
            }
            else
            if (firstSelObj != null)
            {
                drag();
            }
        }
    }

    private void onClick()
    {
        // マウスの位置にあるオブジェクト取得
        Collider2D hitcol = GetHitRay();
        if (hitcol != null)
        {
            GameObject obj = hitcol.gameObject;

            // ボムチェック
            if (obj.tag.StartsWith("bomb", System.StringComparison.Ordinal) && !isDragging)
            {
                clearBomb(obj);
            }
        }
    }

    private void dragStart()
    {
        // マウスの位置にあるオブジェクト取得
        Collider2D hitcol = GetHitRay();
        if(hitcol != null)
        {
            GameObject obj = hitcol.gameObject;

            // 飴かどうかチェック
            if(obj.tag.StartsWith("candy", System.StringComparison.Ordinal))
            {
                firstSelObj = obj;
                culTag = obj.tag;

                // 削除リストの初期化
                delObjList = new List<GameObject>();

                // 削除リストへの追加
                addDelList(obj);

                isDragging = true;
            }
        }
    }

    private void dragEnd()
    {
        if(delObjList == null)
        {
            return;
        }

        int delnum = delObjList.Count;

        // 3個以上繋がっていたら
        if(delnum >= 3)
        {
            removeCandy(delnum, 0);

            // リストを完全にクリア
            delObjList.Clear();
            delObjList.TrimExcess();
        }
        else
        {
            for (int i = 0; i < delnum; i++)
            {
                selObjChangeCol(delObjList[i], 1.0f);
            }
        }

        firstSelObj = null;
        isDragging = false;
    }

    private void drag()
    {
        // マウスの位置にあるオブジェクト取得
        Collider2D hitcol = GetHitRay();
        if (hitcol != null)
        {
            GameObject obj = hitcol.gameObject;

            // 飴かどうかチェック
            if (obj.tag.StartsWith("candy", System.StringComparison.Ordinal))
            {
                // 同じ種類かチェック
                if( culTag == obj.tag)
                {
                    // 最後に選択したものと別なら
                    /*
                    if(lastSelObj != obj)
                    {
                        // 一定の距離内なら
                        float dist = Vector2.Distance(lastSelObj.transform.position, obj.transform.position);
                        if (dist < 1.5f)
                        {
                            addDelList(obj);
                        }
                    }
                    */
                    // リストに登録されてなければ
                    if (!isExistObjAtDelList(obj))
                    {

                        addDelList(obj);
                    }
                }
            }
        }
    }

    private bool isExistObjAtDelList(GameObject obj)
    {
        if (delObjList == null || obj == null)
        {
            return false;
        }

        int delnum = delObjList.Count;
        float dist = 0.0f;
        float tmpdist = 0.0f;

        for (int i = 0; i < delnum; i++)
        {
            if (delObjList[i].Equals(obj))
            {
                return true;
            }
            else
            {
                // もし比較で違っていたら、距離を算出し、その集合との最短距離をだす
                if (i == 0)
                {
                    dist = Vector2.Distance(delObjList[i].transform.position, obj.transform.position);
                }
                else
                {
                    tmpdist = Vector2.Distance(delObjList[i].transform.position, obj.transform.position);
                    if (tmpdist < dist)
                    {
                        dist = tmpdist;
                    }
                }
            }
        }

        // 算出した最短距離が一定の距離内なら
        if (dist < 1.5f)
        {
            return false;
        }

        return true;
    }

    private Collider2D GetHitRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider;
    }

    // @brief キャンディーを削除
    // @param type 0=ドラッグで削除、1=ボムで削除
    private void removeCandy(int num, int type)
    {
        // リストの最後のオブジェクトの位置を保持
        if( num <= 0)
        {
            return;
        }

        GameObject tempobj = delObjList[num - 1];

        Vector3 temppos = Vector3.zero;
        temppos.y = 7.0f;
        if (type == 0 && tempobj != null)
        {
            temppos = tempobj.transform.position;
        }

        // ブロックの生成確率
        int makeblocknum = UnityEngine.Random.Range(0, 10);
        int removeblockcnt = 0;
        int colid = 0;
        string[] tags = { "candyBlue", "candyOrange", "candyPerple",
                          "candyRed", "candyYellow", "block"};

        for (int i = 0; i < num; i++)
        {
            // ブロックの数をカウント
            if (delObjList[i].tag.StartsWith("block", System.StringComparison.Ordinal))
            {
                removeblockcnt++;
            }

            GameObject effcandy = null;
            for (int j = 0; j < 6; j++)
            {
                if (delObjList[i].tag.StartsWith(tags[j], System.StringComparison.Ordinal))
                {
                    effcandy = Instantiate(effprefabcandy[j]) as GameObject;
                    break;
                }
            }

            //GameObject effcandy = Instantiate(effprefabcandy) as GameObject;
            Destroy(effcandy, effcandy.GetComponent<ParticleSystem>().main.duration);
            effcandy.transform.position = delObjList[i].transform.position;
            effcandy.transform.Rotate(new Vector3(20.0f, 0.0f, 0.0f));
            effcandy.GetComponent<AudioSource>().Play();

            // 飴リストからも削除
            gamemgrComp.delCandyfromList(delObjList[i]);

            Destroy(delObjList[i]);
        }

        //ボム、ブロック生成処理
        if (type == 0)
        {
            // ボム
            if (num >= 7)
            {
                gamemgrComp.makeOptionalObj(1, temppos, 0);
            }
            else
            // ブロック 2/5の確率で生成
            if(makeblocknum <= 2)
            {
                gamemgrComp.makeOptionalObj(1, temppos, 1);
            }
        }

        // 削除した分だけ追加
        gamemgrComp.makeCandy(num, 6.5f);

        // スコアやフィーバー計算
        gamemgrComp.calScore(num);
        // ブロックは通常の3倍溜まる
        gamemgrComp.calFever(num + (removeblockcnt * 2));
    }

    private void clearBomb(GameObject obj)
    {
        List<GameObject> candys = gamemgrComp.getCandyList();

        if (candys != null)
        {
            // 削除リストの初期化
            delObjList = new List<GameObject>();

            int listnum = candys.Count;

            //キャンディーリストを全検索して、飴とブロックを対象にピックアップ
            for (int i = 0; i < listnum; i++)
            {
                // 一定の距離内なら
                float dist = Vector2.Distance(candys[i].transform.position, obj.transform.position);
                if (dist < 1.8f)
                {
                    // 削除リストへの追加
                    addDelList(candys[i]);
                }
            }
        }

        if (delObjList != null)
        {
            removeCandy(delObjList.Count, 1);

            // リストを完全にクリア
            delObjList.Clear();
            delObjList.TrimExcess();
        }

        GameObject effbomb = Instantiate(effprefabbomb) as GameObject;
        Destroy(effbomb, effbomb.GetComponent<ParticleSystem>().main.duration);
        effbomb.transform.position = obj.transform.position;
        effbomb.transform.Rotate(new Vector3(110.0f, 0.0f, 0.0f));
        effbomb.GetComponent<AudioSource>().Play();

        Destroy(obj);
    }

    private  void addDelList(GameObject obj)
    {
        lastSelObj = obj;
        delObjList.Add(obj);

        selObjChangeCol(obj, 0.5f);
    }

    private void selObjChangeCol(GameObject obj, float alpha)
    {
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        Color basecol = sprite.color;
        sprite.color = new Color(basecol.r, basecol.g, basecol.b, alpha);
    }
}
