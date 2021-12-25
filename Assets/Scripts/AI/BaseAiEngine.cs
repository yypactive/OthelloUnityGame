using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIEngine
{
    public static int waitTime = 5;
    public bool IsRun {get; protected set;}
    private SynchronizationContext mainThreadSynContext;
    protected Vector2Int finalChessPos;

    protected int[,] currTile = new int[8, 8];

    protected int aiStartTurn;

    protected static int myTurn = -1;
    protected static int enemyTurn = 1;
    

    public BaseAIEngine ()
    {
    }

    public void TryAddNewChess()
    {
        mainThreadSynContext = SynchronizationContext.Current;
        Thread thread = new Thread(FindChessPosThread);
        thread.Start();
    }

    private void FindChessPosThread()
    {
        var startTime = UI.GetCurrClientMilliTimeStamp();
        Debug.LogFormat("#BaseAIEngine# startTime: {0}", startTime);
        InitCurrStatus();
        while (true)
        {
            IterateChessPos();
            var currTime = UI.GetCurrClientMilliTimeStamp();
            if (!IsRun || currTime - startTime > waitTime * 1000)
            {
                var endTime = UI.GetCurrClientMilliTimeStamp();
                Debug.LogFormat("#BaseAIEngine# endTime: {0} deltaTime {1}", endTime, endTime - startTime);
                mainThreadSynContext.Post(
                    new SendOrPostCallback(_RealAddNewChess), null);
                return;
            }
        }
    }

    protected virtual void InitCurrStatus()
    {
        IsRun = true;
    }

    protected virtual void IterateChessPos()
    {
        var potentialPosList = GetPotentialPosList(Global.tileCheckHelper, Global.WhoseTurn());
        var bestVal = - Global.tilesize * Global.tilesize;
        if (potentialPosList.Count > 0)
            {
                foreach (var potentialPos in potentialPosList)
                {
                    // add virtual chess here
                    var newTile = Global.tile.Clone() as int[,];
                    var tileCheckHelper = new TileCheckHelper(ref newTile);
                    tileCheckHelper.AddNewChess(potentialPos.x, potentialPos.y, Global.WhoseTurn());
                    var currVal = EvaluateCurrBoardState(ref newTile);
                    if (currVal > bestVal)
                    {
                        bestVal = currVal;
                        finalChessPos = potentialPos;
                    }
                    Debug.LogFormat("[AI] Potential Pos: {0} val: {1}", potentialPos, currVal);
                }
            }
        else
            Debug.LogError("can not find pos");
        IsRun = false;
        Debug.LogFormat("[AI] finalChessPos: {0}", finalChessPos);
        return;
    }

    protected List<Vector2Int> GetPotentialPosList(TileCheckHelper tileCheckHelper, int turn)
    {
        List <Vector2Int> validPosList = new List <Vector2Int>();
        tileCheckHelper.GetValidList(turn, ref validPosList);
        return validPosList;
    }

    protected int EvaluateCurrBoardState(ref int[,] tile)
    {
        var result = 0; 
        var myTurnNum = 0;
        var enemyTurnNum = 0;
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                if (tile[i, j] == myTurn)
                {
                    result ++;
                    myTurnNum ++;
                }
                else if (tile[i, j] == enemyTurn)
                {
                    result --;
                    enemyTurnNum ++;
                }
            }
        }
        // Debug.LogFormat("[AI] myTurnNum: {0} enemyTurnNum: {1}", myTurnNum, enemyTurnNum);
        return result;
    }

    private void _RealAddNewChess(object state)
    {
        Global.StartNextTurn(finalChessPos.x, finalChessPos.y);
    }
}