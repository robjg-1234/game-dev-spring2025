using UnityEngine;

public class BrickScript : MonoBehaviour
{
    [SerializeField] Renderer rend;
    public int difficulty;
    GameManager gameManager;
    Color defaultColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.instance;
    }
    private void OnDestroy()
    {
        gameManager.updateScore(1* difficulty + 1);
        gameManager.changeLayer(difficulty);
        if (rend.material.color == Color.white)
        {
            gameManager.choosePower();
        }
        else if (rend.material.color == Color.magenta)
        {
            gameManager.activateDebuff();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void setColor(int number)
    {
        if (number < 6)
        {
            if (number == 0)
            {
                rend.material.color = Color.cyan;
            }
            else if (number == 1)
            {
                rend.material.color = Color.blue;
            }
            else if (number == 2)
            {
                rend.material.color = Color.green;
            }
            else if (number == 3)
            {
                rend.material.color = Color.yellow;
            }
            else if (number == 4)
            {
                rend.material.color = new Color(1f, 0.3940722f, 0f, 1f);
            }
            else if (number == 5)
            {
                rend.material.color = Color.red;
            }
            difficulty = number;
            defaultColor = rend.material.color;
        }
        else if(number == 6)
        {
            rend.material.color = Color.white;
        }
        else if(number == 7)
        {
            rend.material.color = Color.magenta;
        }
        else
        {
            rend.material.color = defaultColor;
        }
        
    }
}
