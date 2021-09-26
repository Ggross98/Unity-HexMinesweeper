using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : Singleton<Game>
{
    [SerializeField]
    public static int width = 19, height = 11;
    
    public static int tileWidth = 80, tileHeight = 93;
    
    public static int mineCount = 45;

    [SerializeField]
    private Map map;

    private float time;

    public enum GameStatus { WAITING, PLAYING, OVER};
    public GameStatus status = GameStatus.WAITING;


    private int leftMineCount;


    #region UI components
    [SerializeField]
    private Text statusText, mineText, timeText;


    #endregion


    private void Start()
    {
        if(map == null)
        {
            Debug.LogError("Map not found!");
        }

        InitGame();
    }

    public void InitGame()
    {
        map.SetTileSize(tileWidth, tileHeight);
        map.Generate(width, height);

        status = GameStatus.WAITING;
        statusText.text = "准备就绪";
        time = 0;
        leftMineCount = mineCount;
    }

    public void StartGame(MapTile starter)
    {
        map.Init(starter, mineCount);

        status = GameStatus.PLAYING;
        statusText.text = "游戏中";

    }


    public void GameOver()
    {
        status = GameStatus.OVER;
        statusText.text = "游戏失败";
        map.CheckAll();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LeftClickTile(MapTile tile)
    {
        if(tile.status == MapTile.TileStatus.HIDDEN || tile.status == MapTile.TileStatus.QUESTIONED)
        {
            if (tile.IsMine())
            {
                map.Explode(tile);
                GameOver();
            }
            else
            {
                map.UnmaskTile(tile);
            }
        }
    }

    public bool CheckWin()
    {
        if (leftMineCount != 0) return false;

        return map.CheckWin();
    }

    public void GameWin()
    {
        status = GameStatus.OVER;
        statusText.text = "游戏胜利";
    }

    public void RightClickTile(MapTile tile)
    {
        //右键标记地雷
        if(tile.status == MapTile.TileStatus.HIDDEN)
        {
            tile.SetStatus(MapTile.TileStatus.MARKED);

            leftMineCount--;

            if (CheckWin())
                GameWin();
        }
        else if(tile.status == MapTile.TileStatus.MARKED)
        {
            leftMineCount++;
            tile.SetStatus(MapTile.TileStatus.QUESTIONED);
        }
        else if (tile.status == MapTile.TileStatus.QUESTIONED)
        {
            tile.SetStatus(MapTile.TileStatus.HIDDEN);
        }
    }

    private void Update()
    {

        switch (status)
        {
            case GameStatus.WAITING:

                timeText.text = Utils.FormatTwoTime(0);
                mineText.text = "剩余: ";

                break;

            case GameStatus.PLAYING:


                time += Time.deltaTime;
                timeText.text = Utils.FormatTwoTime((int)time);

                mineText.text = "剩余: " + leftMineCount;
                break;

            case GameStatus.OVER:
                mineText.text = "剩余: " + 0;
                break;
        }

        if (Input.GetKey(KeyCode.Escape))
            QuitGame();
    }

}
