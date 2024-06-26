// #define TEST

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIEngine
{
    public static int waitTime = 10;
    public bool IsRun {get; protected set;}
    private SynchronizationContext mainThreadSynContext;
    protected Vector2Int finalChessPos;

    protected int[,] currTile = new int[8, 8];

    protected int myTurnNum;
    protected int enemyTurnNum;

    protected long startTime = 0;
    
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
        startTime = UI.GetCurrClientMilliTimeStamp();
        Debug.LogFormat("#AIEngine# startTime: {0}", startTime);
        InitCurrStatus();
        IterateChessPos();
        IsRun = false;
        var endTime = UI.GetCurrClientMilliTimeStamp();
        Debug.LogFormat("#AIEngine# endTime: {0} deltaTime {1}", endTime, endTime - startTime);
        mainThreadSynContext.Post(
            new SendOrPostCallback(_RealAddNewChess), null);
        return;
    }

    protected virtual void InitCurrStatus()
    {
        IsRun = true;
    }

    protected virtual bool CheckEnd()
    {
        var currTime = UI.GetCurrClientMilliTimeStamp();
        return currTime - startTime > waitTime * 1000;
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
                    var currVal = EvaluateCurrBoardState(ref newTile, Global.WhoseTurn());
                    if (currVal > bestVal)
                    {
                        bestVal = currVal;
                        finalChessPos = potentialPos;
                    }
#if TEST
                    Debug.LogFormat("[AI] Potential Pos: {0} val: {1}", potentialPos, currVal);
#endif
                }
            }
        else
            Debug.LogError("can not find pos");
#if TEST
        Debug.LogFormat("[AI] finalChessPos: {0}", finalChessPos);
#endif
        return;
    }

    int GetPosSortVal(Vector2Int a)
    {
       var half = Global.tilesize - 1;
       var x = Math.Abs((a.x << 1) - half);
       var y = Math.Abs((a.y << 1) - half);
       var val = x + y + Math.Max(x, y);
       return val;
    }

    int SortPotentialPos(Vector2Int a, Vector2Int b)
    {   
        var asize = GetPosSortVal(a);
        var bsize = GetPosSortVal(b);
        return bsize.CompareTo(asize);
    } 

    protected List<Vector2Int> GetPotentialPosList(TileCheckHelper tileCheckHelper, int turn)
    {       
        List <Vector2Int> validPosList = new List <Vector2Int>();
        tileCheckHelper.GetValidList(turn, ref validPosList);
        validPosList.Sort(SortPotentialPos);

#if TEST
            Debug.LogFormat("[AI] pos List: {0}",  String.Join(" ", validPosList));
#endif

        return validPosList;
    }

    protected int EvaluateCurrBoardState(ref int[,] tile, int myTurn)
    {
        var result = 0;
        var enemyTurn = - myTurn;
        myTurnNum = 0;
        enemyTurnNum = 0;
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