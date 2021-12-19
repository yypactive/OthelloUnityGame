using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    VSHuman,
    VSAI,
}

public class Global : MonoBehaviour {
    //setting
    public static GameMode gameMode;
    // data
    public static int tilesize = 8;
    public static int[,] tile = new int[8, 8];
    public static int[,] validTile = new int[8, 8];
    // signal
    public static bool gameStart = false;
    public static int turnIterator = 0;
    // panel
    public static GameObject state_Panel;
    public static GameObject turn_Panel;
    public static GameObject white_Panel;
    public static GameObject black_Panel;

    public static BaseAIEngine aiEngine = new BaseAIEngine();

    public static TileCheckHelper tileCheckHelper;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void RestartGame(GameMode _gameMode = GameMode.VSHuman)
    {
        state_Panel = GameObject.Find("state_Panel");
        turn_Panel = GameObject.Find("turn_Panel");
        white_Panel = GameObject.Find("white_Panel");
        black_Panel = GameObject.Find("black_Panel");
        // turn iterator
        turnIterator = 0;
        // set mode
        gameMode = _gameMode;
        var modeLab = GameObject.Find("mode_lab");
        if (modeLab != null)
        {
            var text = modeLab.transform.GetComponent<Text>();
            switch (gameMode)
            {
                case GameMode.VSHuman:
                    text.text = "VSHuman"; break;
                case GameMode.VSAI:
                    text.text = "VSAI"; break;
                default:
                    text.text = ""; break;
            }
        }
        // panel
        state_Panel.GetComponentInChildren<statePanel_csharp>().SetState(WhoseTurn());
        state_Panel.GetComponentInChildren<statePanel_csharp>().SetWin(false);
        turn_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(turnIterator);
        white_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(turnIterator);
        black_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(turnIterator);
        //tile array
        // 1 means black, -1 means white
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                Global.tile[i, j] = 0;
            }
        }
        Global.tile[3, 3] = 1;
        Global.tile[4, 4] = 1;
        Global.tile[3, 4] = -1;
        Global.tile[4, 3] = -1;
        tileCheckHelper = new TileCheckHelper(ref Global.tile);
        CheckAllValid(WhoseTurn());
        // refresh
        GameObject.Find("ChessboardBg").GetComponent<bg_csharp>().RefreshAll();
        white_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(2);
        black_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(2);

        // game start
        Debug.LogFormat("game start");
        Global.gameStart = true;
    }

    public void RestartHumanGame()
    {
        Debug.Log("click VSHuman restart btn");
        RestartGame(GameMode.VSHuman);
    }

    public void RestartAIGame()
    {
        Debug.Log("click VSAI restart btn");
        RestartGame(GameMode.VSAI);
    }

    public static bool IsVacant(int _x, int _y)
    {
        int value = tile[_y, _x];
        if (value == 0)
            return true;
        else
            return false;
    }

    public static int WhoseTurn()
    {
        if (turnIterator % 2 == 0)
            // 1 means black, -1 means white
            return 1;
        else
            return -1;
    }

    public static bool IsAIRound()
    {
        return gameMode == GameMode.VSAI && WhoseTurn() == -1;
    }

    public static bool IsValid(int _x, int _y)
    {
        int chess = WhoseTurn();
        return tileCheckHelper.IsValid(_x, _y, chess);
    }

    public static void RefreshTileData(int _x, int _y)
    {
        int chess = WhoseTurn();
        tileCheckHelper.AddNewChess(_x, _y, chess);
        int wnum = 0;
        int bnum = 0;
        for (int i = 0; i < tilesize; ++i)
        {
            for (int j = 0; j < tilesize; ++j)
            {
                if (tile[i,j] == 1)
                {
                    bnum += 1;
                }
                else if (tile[i,j] == -1)
                {
                    wnum += 1;
                }
            }
        }
        white_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(wnum);
        black_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(bnum);
    }

    public static bool CheckAllValid(int _nextchess)
    {
        var hasValid = false;
        int nextchess = _nextchess;
        for (int i = 0; i < tilesize; ++i)
        {
            for (int j = 0; j < tilesize; ++j)
            {
                if (tile[i, j] == 0 && tileCheckHelper.IsValid(j, i, nextchess))
                {
                    validTile[i, j] = 1;
                    hasValid = true;
                }
                else
                    validTile[i, j] = 0;
            }
        }
        return hasValid;
    }

    public static bool IsEnd(int _nextchess)
    {
        return !CheckAllValid(_nextchess);
    }

    public static bool CheckEnd()
    {
        int nextchess = -WhoseTurn();
        if (IsEnd(nextchess))
        {
            gameStart = false;
            state_Panel.GetComponentInChildren<statePanel_csharp>().SetWin(true);
            int wnum = white_Panel.GetComponentInChildren<numPanel_csharp>().GetNum();
            int bnum = black_Panel.GetComponentInChildren<numPanel_csharp>().GetNum();
            if (wnum > bnum)
            {
                state_Panel.GetComponentInChildren<statePanel_csharp>().SetState(-1);
            }
            else if (bnum > wnum)
            {
                state_Panel.GetComponentInChildren<statePanel_csharp>().SetState(1);
            }
            else
            {
                state_Panel.GetComponentInChildren<statePanel_csharp>().SetState(0);
            }
            return true;
        }
        else
            return false;
    }

    public static void StartNextTurn(int x, int y)
    {
        Debug.LogFormat("StartNextTurn: x: {0} y: {1}", x, y);
        // change gloaltile
        Global.RefreshTileData(x, y);
        // refresh
        GameObject.Find("ChessboardBg").GetComponent<bg_csharp>().RefreshAll();
        // check end
        if (Global.CheckEnd()) return;
        // next turn
                // refresh iterator
        turnIterator++;
        // refresh panel
        // Debug.LogFormat("whoseTurn: {0}", WhoseTurn());
        state_Panel.GetComponentInChildren<statePanel_csharp>().SetState(WhoseTurn());
        turn_Panel.GetComponentInChildren<numPanel_csharp>().SetNum(turnIterator / 2);
        // refresh all
        GameObject.Find("ChessboardBg").GetComponent<bg_csharp>().RefreshAll();
        if (IsAIRound())
        {
            aiEngine.TryAddNewChess();
        }
    }
}
