using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCheckHelper
{
    public int[,] tile = null;

    public TileCheckHelper(ref int[,] _tile) {
        tile = _tile;
    }

    public bool IsValid(int _x, int _y, int _chess)
    {
        // check chess is valid
        if (_chess != 1 && _chess != -1) return false;
        // check position
        for (int dx = -1; dx < 2; ++dx)
        {
            for (int dy = -1; dy < 2; ++dy)
            {
                if (dx == 0 && dy == 0) continue;
                if (CheckPosition(_x, _y, _chess,
                    dx, dy) > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int CheckPosition(int _x, int _y, int _chess, int _dx, int _dy)
    {
        int result = 0;
        int other = -_chess;
        int x = _x + _dx;
        int y = _y + _dy;
        while (x < Global.tilesize && x >= 0 && y < Global.tilesize && y >= 0)
        {
            if (tile[y, x] == 0)
                return 0;
            else if (tile[y, x] == other)
                result++;
            else if (tile[y, x] == _chess)
                return result;
            x += _dx;
            y += _dy;
        }
        return 0;
    }

    public void RefreshTileData(int _x, int _y, int _chess)
    {
        for (int dx = -1; dx < 2; ++dx)
        {
            for (int dy = -1; dy < 2; ++dy)
            {
                if (dx == 0 && dy == 0) continue;
                int len = CheckPosition(_x, _y, _chess, dx, dy);
                for (int i = 1; i <= len; ++i)
                {
                    tile[_y + i * dy, _x + i * dx] = _chess;
                }
            }
        }
    }

    public void AddNewChess(int _x, int _y, int _chess)
    {
        RefreshTileData(_x, _y, _chess);
        tile[_y, _x] = _chess;
    }

    // just for ai
    public bool GetValidList(int _nextchess, ref List<Vector2Int> validList)
    {
        var hasValid = false;
        int nextchess = _nextchess;
        validList.Clear();
        for (int i = 0; i < Global.tilesize; ++i)
        {
            for (int j = 0; j < Global.tilesize; ++j)
            {
                if (tile[i, j] == 0 && IsValid(j, i, nextchess))
                {
                    validList.Add(new Vector2Int(j, i));
                    hasValid = true;
                }
            }
        }
        return hasValid;
    }                            
}