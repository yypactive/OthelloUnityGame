using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class bg_csharp : MonoBehaviour {
    // object
    public GameObject tile;

    static float offsetx = 0.0F;
    static float offsety = 0.0F;
    static float space = 0.0F;
    static float scalex = 1.0F;
    static float scaley = 1.0F;
    static float tilesize = 0.0F;
    float spaceRatio = 0.1F;

    // Use this for initialization
    void Start () {
        // set const
        GameObject bg = gameObject;
        RectTransform rect = bg.GetComponent < RectTransform > ();
        float bgw = rect.rect.width;
        float bgh = rect.rect.height;
        float boardSize = Mathf.Min(bgw *0.9f, bgh * 0.9f);
        // Debug.LogFormat("bgw: {0}\t bgh: {1}\t size: {2}", bgw, bgh, boardSize);
        offsetx = (bgw - boardSize) / 2;
        offsety = (bgh - boardSize) / 2;
        tilesize = boardSize / (Global.tilesize + (Global.tilesize - 1) * spaceRatio);
        space = tilesize * spaceRatio;
        // Debug.LogFormat("offsetx: {0}\toffsety: {1}\t tilesize: {2}\t space: {3}", 
        //    offsetx, offsety, tilesize, space);


        //tile array
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                Global.tile[i, j] = 0;
            }
        }

        // clone tile
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                GameObject cloneTile = GameObject.Instantiate(tile);
                RectTransform tileRect = cloneTile.GetComponent<RectTransform>();
                
                scalex = tilesize / tileRect.rect.width;
                scaley = tilesize / tileRect.rect.height;

                cloneTile.transform.SetParent(bg.transform);
                var pos = new Vector3(offsetx + j * (tilesize + space) + 0.05f * bgw
                    , offsety + i * (tilesize + space) - 0.45f * bgh
                    , 0);
                // cloneTile.transform.Translate(pos);
                cloneTile.transform.localPosition = pos;
                var scaleVal = new Vector3(scalex, scaley, 1);
                cloneTile.transform.localScale = scaleVal;
                cloneTile.name = "tile" + Convert.ToString(i) + Convert.ToString(j);
                // init csharp
                cloneTile.GetComponentInChildren<tile_csharp>().InitValue(j, i, 0);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RefreshAll()
    {
        for (int k = 0; k < transform.childCount; ++k)
        {
            GameObject tmptile = transform.GetChild(k).gameObject;
            tile_csharp tcsharp = tmptile.GetComponentInChildren<tile_csharp>();
            int x = tcsharp.GetPositionX();
            int y = tcsharp.GetPositionY();
            bool isCurrentChess = Global.currChess.x == x && Global.currChess.y == y;
            tcsharp.RefreshTile(Global.tile[y,x], Global.validTile[y,x], isCurrentChess);
            // Debug.LogFormat("finish tile: {0} {1}", y, x);
        }
    }
}

