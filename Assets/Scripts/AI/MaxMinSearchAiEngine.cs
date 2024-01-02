#define TEST

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaxMinSearchAiEngine: BaseAIEngine
{

#if TEST
    protected static readonly int searchDeepth = 2;
#else
    // 8 is okay for android phone
    protected static readonly int searchDeepth = 9;
#endif
    // reference
    // https://github.com/lihongxun945/myblog/issues/13


    protected static readonly int minVal = -100000;
    protected static readonly int maxVal = 100000;

    protected override void IterateChessPos()
    {
        // main procedure
        finalChessPos = MaxMinValueSearch(searchDeepth);
        return;
    }

    protected Vector2Int MaxMinValueSearch(int deep)
    {
        var bestVal = minVal;
        var bestValPosList = new List<Vector2Int>();
        var currTurn = Global.WhoseTurn(); 
        var potentialPosList = GetPotentialPosList(Global.tileCheckHelper, currTurn);
#if TEST
        Debug.LogFormat("[AI] deep: {0} pos List: {1}", deep, String.Join(" ", potentialPosList));
#endif
        if (potentialPosList.Count == 0)
        {
            // do sth
        }
        else
        {
            foreach (var pos in potentialPosList)
            {
                // TODO
                // add chess
                var newTile = Global.tile.Clone() as int[,];
                var tileCheckHelper = new TileCheckHelper(ref newTile);
                tileCheckHelper.AddNewChess(pos.x, pos.y, currTurn);
                var currVal = -MaxValueSearch(deep - 1, -bestVal, minVal, tileCheckHelper);
                if (currVal == bestVal)
                    bestValPosList.Add(pos);
                else if (currVal > bestVal)
                {
                    bestVal = currVal;
                    bestValPosList.Clear();
                    bestValPosList.Add(pos);
                }
                // TODO
                // remove chess
                if (CheckEnd())
                    break;
            }
        }
        Debug.LogFormat("[AI] deep: {0} bestVal: {1}", deep, bestVal);
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        return finalPos;
    }
    private int MaxValueSearch(int deep, int alpha, int beta, TileCheckHelper tileCheckHelper)
    {
        // recursion end
        // need Test
        var currTurn = (searchDeepth - deep) % 2 == 0 ? Global.WhoseTurn() : - Global.WhoseTurn();
        var currVal = EvaluateCurrBoardState(ref tileCheckHelper.tile, currTurn);
        // deep end
        if (deep <= 0)
        {
#if TEST
            Debug.LogFormat("[AI] deep: {0} currTurn {1} currVal: {2} myTurnNum: {3} enemyTurnNum: {4}",
                deep, currTurn, currVal, myTurnNum, enemyTurnNum);
#endif
            return currVal;
        }

        var bestVal = minVal;
#if TEST
        var bestPos = new Vector2Int();
#endif
        var nextTurn = - currTurn;
        var potentialPosList = GetPotentialPosList(tileCheckHelper, nextTurn);
#if TEST
        Debug.LogFormat("[AI] deep: {0} pos List: {1}", deep, String.Join(" ", potentialPosList));
#endif
        // recursion end
        if (potentialPosList.Count == 0)
        {
            return currVal;
        }
        else
        {
            foreach (var pos in potentialPosList)
            {
                var newTile = tileCheckHelper.tile.Clone() as int[,];
                var newTileCheckHelper = new TileCheckHelper(ref newTile);
                newTileCheckHelper.AddNewChess(pos.x, pos.y, nextTurn);
                beta = Math.Max(bestVal, beta);
                var newVal = -MaxValueSearch(deep - 1, -beta, -alpha, newTileCheckHelper);
                if (newVal > bestVal)
                {
                    bestVal = newVal;
#if TEST    
                    bestPos = pos;
#endif
                }

                // alpha-beta cut
                if (newVal > alpha || (searchDeepth-deep>1) && newVal == alpha)
                {
                    bestVal = maxVal;
                    break;
                }
            }
        }

#if TEST
            Debug.LogFormat("[AI] deep: {0} Pos: {1} bestVal: {2}", deep, bestPos, bestVal);
#endif
        return bestVal;
    }
}