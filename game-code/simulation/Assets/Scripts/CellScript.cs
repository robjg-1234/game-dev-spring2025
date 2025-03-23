using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class CellScript : MonoBehaviour
{
    [SerializeField] GameObject stage0;
    [SerializeField] GameObject stage1;
    [SerializeField] GameObject stage2;
    [SerializeField] GameObject stage3;
    [SerializeField] GameObject stage4;
    [SerializeField] GameObject volcanoPrefab;
    [SerializeField] Renderer rend;
    [SerializeField] Color water;
    [SerializeField] Color fertileLand;
    [SerializeField] Color forest;
    [SerializeField] Color desert;
    [SerializeField] Color volcano;
    GameObject actualStage;
    int prevAge =0;
    Color currentColor;
    private CellInfo _state = new CellInfo();
    public CellInfo State { get { return _state; } set { _state = value; ChangeState(); } }
    int prevState = 0;
    private void Start()
    {
        ChangeState();
    }
    void ChangeState()
    {
        if (_state.state != prevState)
        {
            if (actualStage != null)
            {
                Destroy(actualStage);
            }
            if (_state.state == 0)
            {
                rend.material.color = desert;
            }
            else if (_state.state == 1)
            {
                rend.material.color = water;
            }
            else if (_state.state == 2)
            {
                rend.material.color = fertileLand;
            }
            else if (_state.state == 3)
            {
                rend.material.color = forest;
                actualStage = Instantiate(stage0, transform.position, Quaternion.identity);
            }
            else if (_state.state == 4)
            {
                rend.material.color = volcano;
                actualStage = Instantiate(volcanoPrefab, transform.position, Quaternion.identity);
            }
            prevState = _state.state;
        }
        else
        {
            if (_state.state == 3 && _state.age != prevAge)
            {
                if (actualStage != null)
                {
                    Destroy(actualStage);
                }
                if (_state.age == 1)
                {
                    rend.material.color = forest;
                    actualStage = Instantiate(stage0, transform.position, Quaternion.identity);
                }
                else if (_state.age == 2)
                {
                    rend.material.color = forest;
                    actualStage = Instantiate(stage1, transform.position, Quaternion.identity);
                }
                else if (_state.age == 3)
                {
                    rend.material.color = forest;
                    actualStage = Instantiate(stage2, transform.position, Quaternion.identity);
                }
                else if (_state.age == 4)
                {
                    rend.material.color = forest;
                    actualStage = Instantiate(stage3, transform.position, Quaternion.identity);
                }
                else if (_state.age == 5)
                {
                    rend.material.color = forest;
                    actualStage = Instantiate(stage4, transform.position, Quaternion.identity);
                }
                prevAge = _state.age;
            }
        }
        if (rend.material.color != Color.red)
        {
            currentColor = rend.material.color;
        }
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
        rend.material.color = Color.red;
    }
}
