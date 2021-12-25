using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tile_csharp : MonoBehaviour {
    //object
    public GameObject tile;
    public GameObject whiteChess;
    public GameObject blackChess;
    // const 
    static float chessSpace = 0.0F;
    static float chessScalex = 1.0F;
    static float chessScaley = 1.0F;
    // info
    float posx;
    float posy; 
    public int x;
    public int y;
    public int value = 0;
    public int validValue = 0;
    // Use this for initialization
    void Start () {
        // const 
        float tilesize = Mathf.Min(
            tile.transform.localScale.x,
            tile.transform.localScale.y
            );
        posx = tile.transform.position.x;
        posy = tile.transform.position.y;
        chessSpace = tilesize / 10.0F;

        chessScalex = 0.8F;
        chessScaley = 0.8F;
        // value
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void OnButtonClick()
    {
        // check Game start
        if (!Global.gameStart) return;
        // check tile is vacant
        if (!Global.IsVacant(x, y)) return;
        // check chess is valid
        if (!Global.IsValid(x, y)) return;
        // check is ai round
        if (Global.IsAIRound()) return;
        Global.StartNextTurn(x, y);
    }

    public void InitValue(int _x, int _y, int _value)
    {
        x = _x;
        y = _y;
        value = _value;
    }

    public int GetPositionX()
    {
        return x;
    }

    public int GetPositionY()
    {
        return y;
    }

    public void RefreshTile(int _value, int _validValue, bool isCurretnChess)
    {
        var tileImage = gameObject.GetComponent<Image>();
        if (tileImage != null)
        {
            Color color;
            // Update color
            if (isCurretnChess)
            {
                ColorUtility.TryParseHtmlString("#d5c85D", out color);
            }
            else if (_validValue == 1)
            {
                ColorUtility.TryParseHtmlString("#E5585D", out color);
            }
            else
            {
                ColorUtility.TryParseHtmlString("#9AB4BA", out color);
            }
            tileImage.color = color;
        }
        if (value == _value && validValue == _validValue)
            return;
        if (value != 0)
        // remove old chess
        {
            for (int i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }

        if (_value != 0)
        // draw new chess
        {
            // 1 means black, -1 means white
            if (_value == -1)
            {
                GameObject cloneChess = GameObject.Instantiate(whiteChess);
                cloneChess.transform.parent = tile.transform;
                cloneChess.transform.Translate(
                    new Vector3(posx + chessSpace, posy + chessSpace, 0)
                    );
                cloneChess.transform.localScale = new Vector3(chessScalex, chessScaley, 1);
            }
            else if (_value == 1)
            {
                GameObject cloneChess = GameObject.Instantiate(blackChess);
                cloneChess.transform.parent = tile.transform;
                cloneChess.transform.Translate(
                    new Vector3(posx + chessSpace, posy + chessSpace, 0)
                    );
                cloneChess.transform.localScale = new Vector3(chessScalex, chessScaley, 1);
            }
        }
        value = _value;
        validValue = _validValue;

    }

    public bool IsVacant ()
    {
        if (value == 0)
            return true;
        else
            return false;
    }


}
