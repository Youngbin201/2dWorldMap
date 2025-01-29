using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;

public enum NeighborDirection
{
    Up , Down , Right , Middle ,Left , UpRight , UpLeft , DownRight, DownLeft , Isolate
}

public static class RuleTile
{

    public static Tuple<Tile , NeighborDirection> GetRuleTile(int[,] map, int x , int y ,RuleTileSO ruleTileSO , int sizeX , int sizeY)
    {
        NeighborDirection nei = FindMostNeiborTile(GetSurroundingValues(map , x,y , sizeX , sizeY) , ruleTileSO);


        
        for (int i = 0; i < ruleTileSO.ruleTiles.Count; i++)
        {
            if(ruleTileSO.ruleTiles[i].neighbor == nei)
            {
                return new Tuple<Tile, NeighborDirection>(ruleTileSO.ruleTiles[i].tile , nei);
            }
        }

        return null;
    }

    //주변 값 가져오기
    static int[] GetSurroundingValues(int[,] map, int x, int y , int sizeX , int sizeY)
    {
        int[] Sv = new int[9];
        
        int index = 0;

        for (int yy = y+1; yy >= y-1; yy--)
        {
            for (int xx = x-1; xx <= x+1; xx++)
            {
                if(xx>= 0 && yy>= 0 && xx<sizeX && yy<sizeY)
                Sv[index] = map[xx,yy];

                index++;
            }
        }

        return Sv;
    }



    // 배열 간 유사도 측정 함수
    static int CalculateArraySimilarity(int[] nei, int[] dir)
    {
        int similarity = 0;

        for (int i = 0; i < nei.Length; i++)
        {
            if (nei[i] == dir[i] && dir[i] != 2)
            {
                similarity++;
            }
        }

        return similarity;
    }

    // 가장 유사한 배열 찾기 함수
    static NeighborDirection FindMostNeiborTile(int[] targetArray ,RuleTileSO ruleTileSO )
    {
        int maxSimilarity = 0;
        NeighborDirection mostSimilar = NeighborDirection.Middle;

        List<(int[], NeighborDirection)> dict = GetRuleDict(ruleTileSO);

    
         foreach (var array in dict)
        {
            int similarity = CalculateArraySimilarity(targetArray, array.Item1);
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                mostSimilar = array.Item2;
            }
        }
        return mostSimilar;
    }

    
    #region Util
    public static List<(int[], NeighborDirection)> GetRuleDict(RuleTileSO ruleTileSO)
    {
        //모든 방향 값
        List<(int[], NeighborDirection)> dict = new List<(int[], NeighborDirection)>
        {
            (new int[9]{2,0,2,0,2,1,2,1,2},NeighborDirection.UpLeft),
            (new int[9]{2,0,2,1,2,1,2,1,2},NeighborDirection.Up),
            (new int[9]
        {2,0,2
        ,1,2,0
        ,2,1,2},
                NeighborDirection.UpRight
            ),
            (
                new int[9]
        {2,1,2
        ,0,2,1
        ,2,1,2},
                NeighborDirection.Left
            ),
            (
                new int[9]
        {2,1,2
        ,1,2,1
        ,2,1,2},
                NeighborDirection.Middle
            ),
            (
                new int[9]
        {2,1,2
        ,1,2,0
        ,2,1,2},
                NeighborDirection.Right
            ),
            (
                new int[9]
        {2,1,2
        ,0,2,1
        ,2,0,2},
                NeighborDirection.DownLeft
            ),
            (
                new int[9]
        {2,1,2
        ,1,2,1
        ,2,0,2},
                NeighborDirection.Down
            ),
            (
                new int[9]
        {2,1,2
        ,1,2,0
        ,2,0,2},
                NeighborDirection.DownRight
            ),
            (
                new int[9]
        {2,0,2
        ,0,2,0
        ,2,0,2},
                NeighborDirection.Isolate
            )
        };

        //사용할 값
        List<(int[], NeighborDirection)> dict2 = new List<(int[], NeighborDirection)>();

        for (int i = 0; i < ruleTileSO.ruleTiles.Count; i++)
        {
            for (int j = 0; j < dict.Count; j++)
            {
                if(ruleTileSO.ruleTiles[i].neighbor == dict[j].Item2)
                {
                    dict2.Add(dict[j]);
                    break;
                }
            }
        }
        return dict2;
    }
    #endregion
    
}
