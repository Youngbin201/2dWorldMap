using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CellularAutomata
{
    #region 셀룰러 오토마타
    public static int[,] GenerateMap(CellularAutomataSettings CAS)
    {
        int[,] map = new int[CAS.width, CAS.height];

        map = MapCellular(map, CAS);

        return map;
    }

    public static int[,] MapCellular(int[,] map, CellularAutomataSettings CAS)
    {

        MapRandomFill(map, CAS);

        for (int i = 0; i < CAS.smoothNum; i++) //반복이 많을수록 동굴의 경계면이 매끄러워진다.
            SmoothMap(map, CAS);

        if (CAS.useTrim)
            CellularAutomataDetail(map, CAS.wallThresholdSize);

        return map;
    }

    private static void MapRandomFill(int[,] map, CellularAutomataSettings CAS) //맵을 비율에 따라 벽 혹은 빈 공간으로 랜덤하게 채우는 메소드
    {
        if (CAS.useRandomSeed) CAS.seed = Time.time.ToString(); //시드

        System.Random pseudoRandom = new System.Random(CAS.seed.GetHashCode()); //시드로 부터 의사 난수 생성

        for (int x = 0; x < CAS.width; x++)
        {
            for (int y = 0; y < CAS.height; y++)
            {
                if (x <= 0 || x >= CAS.width - 1 || y <= 0 || y >= CAS.height - 1) map[x, y] = 1; //가장자리는 벽으로 채움
                else map[x, y] = (pseudoRandom.Next(0, 100) < CAS.randomFillPercent) ? 1 : 0; //비율에 따라 벽 혹은 빈 공간 생성
            }
        }
    }

    public static void SmoothMap(int[,] map, CellularAutomataSettings CAS)
    {
        for (int x = 0; x < CAS.width; x++)
        {
            for (int y = 0; y < CAS.height; y++)
            {
                
                    int neighbourWallTiles = GetSurroundingWallCount(x, y, CAS.width, CAS.height, map);
                    if (neighbourWallTiles > 4) map[x, y] = 1; //주변 칸 중 벽이 4칸을 초과할 경우 현재 타일을 벽으로 바꿈
                    else if (neighbourWallTiles < 4) map[x, y] = 0; //주변 칸 중 벽이 4칸 미만일 경우 현재 타일을 빈 공간으로 바꿈
            }
        }
    }

    private static int GetSurroundingWallCount(int gridX, int gridY, int width, int height, int[,] map) // 근처 노드 가져와서 벽 개수 카운트
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        { //현재 좌표를 기준으로 주변 8칸 검사
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                { //맵 범위를 초과하지 않게 조건문으로 검사
                    if (neighbourX != gridX || neighbourY != gridY) wallCount += map[neighbourX, neighbourY]; //벽은 1이고 빈 공간은 0이므로 벽일 경우 wallCount 증가
                }
                else wallCount++; //주변 타일이 맵 범위를 벗어날 경우 wallCount 증가
            }
        }
        return wallCount;
    }
    #endregion

    #region 세포자동자 다듬기

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    static void CellularAutomataDetail(int[,] map, int wallThresholdSize) // 고립 타일 & 작은방 제거
    {
        List<List<Coord>> wallRegions = GetRegions(1, map);


        foreach (List<Coord> wallRegion in wallRegions) // 작은벽 제거
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0, map);

        var dd = roomRegions.OrderByDescending(roomRegion => roomRegion.Count).ToList();

        for (int i = 0; i < dd.Count; i++) //큰방뺴고 전부 제거
        {
            if (i == 0)
            {
                //Debug.Log(roomRegions[i].Count);
            }
            else
            {
                //Debug.Log("m" + roomRegions[i].Count);

                foreach (Coord tile in dd[i])
                {
                    map[tile.tileX, tile.tileY] = 1;

                }
            }


        }

    }

    static List<List<Coord>> GetRegions(int tileType, int[,] map)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[map.GetLength(0), map.GetLength(1)];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y, map);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    static List<Coord> GetRegionTiles(int startX, int startY, int[,] map)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[map.GetLength(0), map.GetLength(1)];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y, map.GetLength(0), map.GetLength(1)) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }

        }

        return tiles;
    }

    static bool IsInMapRange(int x, int y, int sizex, int sizey)
    {
        return x >= 0 && x < sizex && y >= 0 && y < sizey;
    }
    #endregion
}

public class CellularAutomataSettings //오토마타 세팅을 클래스로 
{
    public int width;//크기
    public int height;

    public string seed;//랜덤 or 고정
    public bool useRandomSeed = true;
    public bool useTrim = false; //빈방 매꾸기
    public int wallThresholdSize = 20; //매꿀 방 사이즈


    public int randomFillPercent;
    public int smoothNum;

    public CellularAutomataSettings
    (int _width = 16, int _height = 16, int _randomFillPercent = 28, int _smoothNum = 2, bool _useRandomSeed = true,
    bool _useTrim = false, int _wallThresholdSize = 20, string _seed = "")
    {
        width = _width;
        height = _height;
        randomFillPercent = _randomFillPercent;
        smoothNum = _smoothNum;
        useRandomSeed = _useRandomSeed;
        seed = _seed;
        useTrim = _useTrim;
        wallThresholdSize = _wallThresholdSize;
    }

}
