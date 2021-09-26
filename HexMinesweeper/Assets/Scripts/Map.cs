using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private int width, height; //格子数目的长*宽

    private int tileWidth, tileHeight; //每个格子的长*宽

    private MapTile[,] tiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private Transform tileParent;

    private void Awake()
    {
        if(tileParent == null)
        {
            Debug.Log("Map object lost!");
        }
    }

    public void SetTileSize(int w, int h)
    {
        tileWidth = w;
        tileHeight = h;
    }

    /// <summary>
    /// 生成指定尺寸的地图
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void Generate(int w, int h)
    {

        //清除已有格子
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(tiles[i,j]!= null)
                {
                    Destroy(tiles[i, j].gameObject);
                    tiles[i, j] = null;
                }

                
            }
        }

        width = w;
        height = h;

        tileWidth = Game.tileWidth;
        tileHeight = Game.tileHeight;

        if (width <= 0 || height <= 0)
        {
            Debug.Log("Map size error!");
        }
        else
        {
            tiles = new MapTile[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject obj = Instantiate(tilePrefab, tileParent);
                    obj.name = "tile" + i + ", " + j;

                    MapTile newtile = obj.GetComponent<MapTile>();
                    if (newtile != null)
                    {
                        tiles[i, j] = newtile;
                        newtile.Init(i, j);

                        newtile.SetUIPosition(GetUIPosition(i, j));
                    }
                    else
                    {
                        Debug.Log("Map tile prefab error!");
                    }
                }
            }
        }

        HideAll();
    }

    public bool CheckWin()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (tiles[i, j] == null) return false;

                if (tiles[i, j].IsMine() && tiles[i, j].status != MapTile.TileStatus.MARKED) return false;

                if (!tiles[i, j].IsMine() && tiles[i, j].status == MapTile.TileStatus.MARKED) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 用一个数组来生成格子
    /// </summary>
    /// <param name="typeArray">0：不是炸弹；其他数字：炸弹</param>
    public void Generate(int[,] typeArray)
    {
        
        Generate(typeArray.GetLength(0), typeArray.GetLength(1));


        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j].SetMine(typeArray[i, j] != 0);
            }
        }
    }

    /// <summary>
    /// 随机生成地图。第一个格子不为雷
    /// </summary>
    /// <param name="starter"></param>
    public void Init( MapTile starter, int count )
    {
        if (count >= width * height) count = width * height - 1;
        if (count < 0) count = 0;

        List<MapTile> list = GetTileList();

        List<MapTile> mine = new List<MapTile>();

        if (list.Contains(starter)) list.Remove(starter);

        int c = count;

        while (c > 0)
        {
            int index = Random.Range(0, list.Count);
            mine.Add(list[index]);
            list.RemoveAt(index);

            c--;
        }

        foreach(MapTile t in mine){
            t.SetMine(true);
        }

        foreach(MapTile tile in GetTileList())
        {
            int t = 0;
            foreach(MapTile _tile in GetAdjacentTiles(tile))
            {
                if (_tile.IsMine()) t++;
            }
            tile.SetCount(t);
        }

        //HideAll();
    }

    public MapTile GetTile(int x, int y)
    {
        if (x < 0 || x > width - 1 || y < 0 || y > height - 1) return null;

        return tiles[x, y];
    }

    public List<MapTile> GetTileList()
    {
        List<MapTile> list = new List<MapTile>();

        if (tiles != null)
        {
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    list.Add(tiles[i, j]);
                }
            }
        }

        return list;
    }
    
    /// <summary>
    /// 揭示格子的内容：雷或数字。
    /// </summary>
    /// <param name="tile"></param>
    public void UnmaskTile(MapTile tile)
    {
        if (tile.status == MapTile.TileStatus.UNMASKED) return;

        if (tile.IsMine())
        {
            tile.SetStatus(MapTile.TileStatus.MARKED);
        }
        else
        {
            tile.SetStatus(MapTile.TileStatus.UNMASKED);
            
            //若周围六格五雷，递归揭示
            if(tile.GetCount() == 0)
            {
                foreach(MapTile t in GetAdjacentTiles(tile))
                {
                    UnmaskTile(t);
                }
            }
        }
    }

    /// <summary>
    /// 游戏结束时展示格子情况。
    /// 若已显示或正确标记：不进行操作
    /// 若隐藏或存疑：展示其内容
    /// 若已爆炸：不进行操作
    /// 若错误标记：显示标记
    /// </summary>
    /// <param name="tile"></param>
    public void CheckTile(MapTile tile)
    {

        if (tile.status == MapTile.TileStatus.EXPLODED) return;


        if(tile.status == MapTile.TileStatus.HIDDEN || tile.status == MapTile.TileStatus.QUESTIONED)
        {
            UnmaskTile(tile);
        }
        else
        {
            if (tile.status == MapTile.TileStatus.MARKED && !tile.IsMine())
                tile.SetStatus(MapTile.TileStatus.WRONG_MARKED);
        }
    }

    public void CheckAll()
    {
        foreach(MapTile tile in GetTileList())
        {
            CheckTile(tile);
        }
    }

    /// <summary>
    /// 左键单击有雷格子时爆炸
    /// </summary>
    /// <param name="tile"></param>
    public void Explode(MapTile tile)
    {
        tile.SetStatus(MapTile.TileStatus.EXPLODED);
    }

    public void HideAll()
    {
        foreach(MapTile tile in GetTileList())
        {
            tile.SetStatus(MapTile.TileStatus.HIDDEN);
        }
    }

    public void UnmaskAll()
    {
        foreach (MapTile tile in GetTileList())
        {
            UnmaskTile(tile);
        }
    }
    
    /// <summary>
    /// 计算六边形格子的坐标
    /// </summary>
    public Vector2 GetUIPosition(int x, int y)
    {
        float posX, posY;
        posX = (x  + 0.5f) * tileWidth;
        if (y % 2 == 1) posX += 0.5f * tileWidth;

        posY = (y*3f/4f  + 0.5f) * tileHeight;

        return new Vector2(posX, posY);
    }
    
    /// <summary>
    /// 获得一个格子周围最多六个格子。不包括自身
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public List<MapTile> GetAdjacentTiles(MapTile tile)
    {
        return MapUtils.GetAdjacentTiles(tiles, tile);
    }
}
