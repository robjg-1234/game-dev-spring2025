using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject ball;
    public bool ballAlive = true;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ballAlive)
        {
            Instantiate(ball);
            ballAlive = true;
        }
    }
}
