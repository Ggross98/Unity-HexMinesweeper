using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUtils
{

    /// <summary>
    /// 获取距某单元格一定距离内的所有单元格，包括其本身
    /// </summary>
    /// <param name="tiles">地图二维数组</param>
    /// <param name="start">起始单元格</param>
    /// <param name="dist">距离</param>
    /// <returns>能到达单元格的列表，包括初始单元格</returns>
    public static List<MapTile> GetTilesOfDistance(MapTile[,] tiles, MapTile start, int dist )
    {
        List<MapTile> list = new List<MapTile>();

        if (dist < 0) return list;
        if (dist == 0) return new List<MapTile> { start };

        int step = 0;

        List<MapTile> searching = new List<MapTile> { start };

        while(++step <= dist && searching.Count > 0)
        {

            List<MapTile> newlist = new List<MapTile>();

            foreach(MapTile t in searching)
            {
                List<MapTile> adjacent = GetAdjacentTiles(tiles, t);

                foreach(MapTile tt in adjacent)
                {
                    if (!searching.Contains(tt) && !list.Contains(tt)) newlist.Add(tt);
                }

                //searching.Remove(t);
                //list.Add(t);
            }

            list.AddRange(searching);
            searching.Clear();
            searching.AddRange(newlist);
        }
        if (searching.Count > 0) list.AddRange(searching);

        return list;
    }

    /// <summary>
    /// 计算两点之间最短距离
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int GetDistance(MapTile a, MapTile b)
    {
        Vector3Int aPos = new Vector3Int(a.GetX() - a.GetY() / 2, a.GetY(), -a.GetY() / 2 - a.GetX());
        Vector3Int bPos = new Vector3Int(b.GetX() - b.GetY() / 2, b.GetY(), -b.GetY() / 2 - b.GetX());

        return (Mathf.Abs(aPos.x-bPos.x)+Mathf.Abs(aPos.y - bPos.y)+Mathf.Abs (aPos.z - bPos.z))/2;
    }

    /// <summary>
    /// 获取相邻单元格，不包括中心单元格本身
    /// </summary>
    /// <param name="tiles">砖块数组</param>
    /// <param name="a">中心单元格</param>
    /// <returns>其周围最多六个单元格</returns>
    public static List<MapTile> GetAdjacentTiles(MapTile[,] tiles, MapTile a)
    {
        List<MapTile> list = new List<MapTile>();

        int x = a.GetX (), y = a.GetY();

        if (x > 0) list.Add(tiles[x - 1, y]);
        if (x < tiles.GetLength(0) - 1) list.Add(tiles[x + 1, y]);
        if (y > 0) list.Add(tiles[x, y - 1]);
        if (y < tiles.GetLength(1) - 1) list.Add(tiles[x, y + 1]);

        if(y%2 == 1)
        {
            if (x < tiles.GetLength(0) - 1 && y < tiles.GetLength(1) - 1) list.Add(tiles[x + 1, y + 1]);
            if (x < tiles.GetLength(0) - 1 && y > 0) list.Add(tiles[x + 1, y - 1]);
        }
        else
        {
            if (x > 0 && y > 0) list.Add(tiles[x - 1, y - 1]);
            if (x > 0 && y < tiles.GetLength(1) - 1) list.Add(tiles[x - 1, y + 1]);
        }

        return list;
    }
}
