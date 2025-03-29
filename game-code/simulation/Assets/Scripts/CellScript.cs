using UnityEngine;

public class CellScript : MonoBehaviour
{
    [SerializeField] GameObject shop;
    [SerializeField] Renderer rend;
    [SerializeField] Color waterLow;
    [SerializeField] Color land;
    [SerializeField] Color waterMedium;
    [SerializeField] Color waterHigh;
    ShipScript ship;
    GameObject activeShop;
    Color currentColor;
    int prevTide = 0;
    private CellInfo _state = new CellInfo();
    public CellInfo State { get { return _state; } set { _state = value; ChangeState(); } }
    void ChangeState()
    {
        if (_state.state> 0)
        {
            if (_state.tideLevel != prevTide)
            {
                if (_state.tideLevel == 1)
                {
                    rend.material.color = waterLow;
                }
                else if (_state.tideLevel == 2)
                {
                    rend.material.color = waterMedium;
                }
                else
                {
                    rend.material.color = waterHigh;
                }
                currentColor = rend.material.color;
                prevTide = _state.tideLevel;
            }
            
        }
        else
        {
            if (rend.material.color != land)
            {
                rend.material.color = land;
                currentColor = rend.material.color;
            }
            if (_state.isShop && activeShop==null)
            {
                activeShop = Instantiate(shop, transform.position, Quaternion.identity);
            }
        }
    }
    public void SetShip(ShipScript newShip)
    {
        ship = newShip;
        _state.occupied = true;
    }
    public void UnSetShip()
    {
        ship = null;
        _state.occupied = false;
    }
    public void SetCoordinates(int x, int y)
    {
        _state.x = x; _state.y = y;
    }
    public void UnSelect()
    {
        rend.material.color = currentColor;
    }
    public void SelectCell()
    {
        rend.material.color = Color.gray;
    }
    public ShipScript GetShip()
    {
        if (ship != null)
        {
            return ship;
        }
        else
        {
            return null;
        }
    }
}
