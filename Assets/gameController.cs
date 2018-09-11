using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour 
{

    private GameObject firstSelObj = null;
    private GameObject lastSelObj = null;
    private List<GameObject> delObjList;
    private string culTag;
    private GameObject scenemgr;

    // Use this for initialization
    void Start () 
	{
        scenemgr = GameObject.Find("sceneMgr");
    }
	
	// Update is called once per frame
	void Update () 
	{
        bool isplay = scenemgr.GetComponent<gameMgr>().isPlayFlag();

        if (isplay)
        {
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

        if(delnum >= 3)
        {
            for (int i = 0; i < delnum; i++)
            {
                Destroy(delObjList[i]);
            }

            // 削除した分だけ追加
            scenemgr.GetComponent<gameMgr>().makeCandy(delnum, 6.5f);
            scenemgr.GetComponent<gameMgr>().calScore(delnum);
        }
        else
        {
            for (int i = 0; i < delnum; i++)
            {
                selObjChangeCol(delObjList[i], 1.0f);
            }
        }

        firstSelObj = null;
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
                    if(lastSelObj != obj)
                    {
                        // 一定の距離内なら
                        float dist = Vector2.Distance(lastSelObj.transform.position, obj.transform.position);
                        if (dist < 1.5f)
                        {
                            addDelList(obj);
                        }
                    }
                }
            }
        }
    }

    private Collider2D GetHitRay()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider;
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
