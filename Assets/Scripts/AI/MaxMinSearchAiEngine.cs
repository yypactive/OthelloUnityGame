using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxMinSearchAiEngine: BaseAIEngine
{
    // 10 is okay
    protected static readonly int searchDeepth = 10;

    // reference
    // https://github.com/lihongxun945/myblog/issues/13

    protected override void IterateChessPos()
    {
        // main procedure
        finalChessPos = MaxMinValueSearch(searchDeepth);
        IsRun = false;
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
        }
        Debug.LogFormat("bestVal: {0}", bestVal);
        var ran = new System.Random();
        var finalPos = bestValPosList[ran.Next(bestValPosList.Count - 1)];
        return finalPos;
    }
    private int MinValueSearch(int deep, int alpha, int beta, TileCheckHelper tileCheckHelper)
    {
        // recursion end
        // need Test
        var currTurn = (searchDeepth - deep) % 2 == 1 ? Global.WhoseTurn() : - Global.WhoseTurn();
        var currVal = EvaluateCurrBoardState(ref tileCheckHelper.tile);
        // Debug.LogFormat("[AI] deep: {0} currTurn: {1} currVal: {2}", deep, currTurn, currVal);
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
            var newVal = MinValueSearch(deep - 1, -beta, -alpha, newTileCheckHelper) * (-1);
            // Debug.LogFormat("[AI] deep: {0} Pos: {1} val: {2}", deep - 1, pos, newVal);
            if (newVal > bestVal)
            {
                bestVal = newVal;
            }
            alpha = Math.Max(bestVal, alpha);

            // alpha-beta cut
            if (newVal >= beta)
            {
                return newVal;
            }
        }
        return bestVal;
    }
}