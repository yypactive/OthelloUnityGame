// #define TEST

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

    protected override void IterateChessPos()
    {
        // main procedure
        finalChessPos = MaxMinValueSearch(searchDeepth);
        return;
    }

    protected Vector2Int MaxMinValueSearch(int deep)
    {
        var bestVal = int.MinValue;
        var bestValPosList = new List<Vector2Int>();
        var currTurn = Global.WhoseTurn(); 
        var potentialPosList = GetPotentialPosList(Global.tileCheckHelper, currTurn);
        foreach (var pos in potentialPosList)
        {
            // TODO
            // add chess
            var newTile = Global.tile.Clone() as int[,];
            var tileCheckHelper = new TileCheckHelper(ref newTile);
            tileCheckHelper.AddNewChess(pos.x, pos.y, currTurn);
            var currVal = MinValueSearch(deep - 1, int.MinValue, int.MaxValue, tileCheckHelper);
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
        Debug.LogFormat("#AIEngine# bestVal: {0}", bestVal);
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        return finalPos;
    }
    private int MinValueSearch(int deep, int alpha, int beta, TileCheckHelper tileCheckHelper)
    {
        // recursion end
        // need Test
        var currTurn = (searchDeepth - deep) % 2 == 1 ? Global.WhoseTurn() : - Global.WhoseTurn();
        var currVal = EvaluateCurrBoardState(ref tileCheckHelper.tile, currTurn);
#if TEST
        Debug.LogFormat("[AI] deep: {0} currTurn {1} currVal: {2} myTurnNum: {3} enemyTurnNum: {4}", 
            deep, currTurn, currVal, myTurnNum, enemyTurnNum);
#endif
        if (deep <= 0)
        {
            
            return currVal;
        }

        var bestVal = int.MinValue;
        var nextTurn = - currTurn;
        var potentialPosList = GetPotentialPosList(tileCheckHelper, nextTurn);
        // recursion end
        if (potentialPosList.Count == 0)
        {
            return currVal;
        }
        foreach (var pos in potentialPosList)
        {
            var newTile = tileCheckHelper.tile.Clone() as int[,];
            var newTileCheckHelper = new TileCheckHelper(ref newTile);
            newTileCheckHelper.AddNewChess(pos.x, pos.y, nextTurn);
            var newVal = MinValueSearch(deep - 1, -beta, -alpha, newTileCheckHelper) * (-1);
#if TEST
            Debug.LogFormat("[AI] deep: {0} Pos: {1} newVal: {2}", deep, pos, newVal);
#endif
            if (newVal > bestVal)
            {
                bestVal = newVal;
            }
            alpha = Math.Max(bestVal, alpha);

            // alpha-beta cut
            if (newVal > beta || (searchDeepth-deep>1) && newVal == beta)
            {
                return newVal;
            }
        }
        return bestVal;
    }
}