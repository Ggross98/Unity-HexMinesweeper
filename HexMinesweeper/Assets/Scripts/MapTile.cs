using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapTile : MonoBehaviour, IPointerClickHandler
{

    #region 逻辑

    private bool mine = false;

    public void SetMine(bool b) { mine = b;  }

    public bool IsMine() { return mine;  }

    public enum TileStatus { HIDDEN, UNMASKED, MARKED, WRONG_MARKED, EXPLODED, QUESTIONED }
    public TileStatus status = TileStatus.HIDDEN;


    private int count = 0;

    public int GetCount() { return count; }

    public void SetCount(int i) { count = i; }

    #endregion



    #region 坐标表示
    private Vector2Int pos;

    public Vector2Int GetPos() { return pos;  }

    public void SetPos(Vector2Int v) { pos = v; }

    public int GetX() { return pos.x;  }

    public int GetY() { return pos.y;  }

    public void SetX(int x) { pos.x = x; }

    public void SetY(int y) { pos.y = y; }

    #endregion

    [SerializeField]
    private Image background;

    [SerializeField]
    private Text countText;
    
    public void Init(int x, int y)
    {
        SetPos(new Vector2Int(x, y));
    }
    
    public void SetUIPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void SetColor(Color c)
    {
        background.color = c;
    }
    
    public void SetStatus(MapTile.TileStatus ts)
    {
        status = ts;

        switch (ts)
        {
            case TileStatus.HIDDEN:
                background.color = Color.yellow;
                countText.gameObject.SetActive(false);
                break;

            case TileStatus.UNMASKED:
                background.color = Color.blue;
                countText.gameObject.SetActive(true);
                SetText(count);
                break;

            case TileStatus.MARKED:
                background.color = Color.black;
                countText.gameObject.SetActive(false);
                break;

            case TileStatus.WRONG_MARKED:
                background.color = Color.red;
                countText.gameObject.SetActive(false);
                break;

            case TileStatus.EXPLODED:
                background.color = Color.magenta;
                countText.gameObject.SetActive(false);
                break;

            case TileStatus.QUESTIONED:
                background.color = Color.magenta;
                countText.gameObject.SetActive(true);
                countText.text = "?";
                break;

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Game.Instance.status == Game.GameStatus.WAITING)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Game.Instance.StartGame(this);
                Game.Instance.LeftClickTile(this);
            }
        }
        else if (Game.Instance.status == Game.GameStatus.PLAYING)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Game.Instance.LeftClickTile(this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Game.Instance.RightClickTile(this);
            }
        }

        
    }

    public void SetText(int count)
    {
        if (count == 0) countText.text = "";
        else countText.text = count + "";
    }
}
