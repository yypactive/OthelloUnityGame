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
        var potentialPosList = GetPotentialPosList();
        var bestVal = - Global.tilesize * Global.tilesize;
        if (potentialPosList.Count > 0)
            {
                foreach (var potentialPos in potentialPosList)
                {
                    // add virtual chess here
                    var currVal = EvaluateCurrBoardState();
                    if (currVal > bestVal)
                    {
                        bestVal = currVal;
                        finalChessPos = potentialPos;
                    }
                }
            }
        else
            Debug.LogError("can not find pos");
        IsRun = false;
        // Debug.Log("pos val: " + bestVal);
        return;
    }

    protected List<Vector2Int> GetPotentialPosList()
    {
        List <Vector2Int> validPosList = new List <Vector2Int>();
        Global.GetValidList(Global.WhoseTurn(), ref validPosList);
        return validPosList;
    }

    protected int EvaluateCurrBoardState()
    {
        var tile = Global.tile;
        var myTurn = -1;
        var enemyTurn = 1;
        var result = 0; 
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                if (tile[i, j] == myTurn)
                {
                    result ++;
                }
                else if (tile[i, j] == enemyTurn)
                {
                    result --;
                }
            }
        }
        return result;
    }

    private void _RealAddNewChess(object state)
    {
        Global.StartNextTurn(finalChessPos.x, finalChessPos.y);
    }
}