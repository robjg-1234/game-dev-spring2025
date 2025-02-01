using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class titleScreenScript : MonoBehaviour
{
    [SerializeField] TMP_Text stageDescription;
    int stage = 0;

    public void nextPage()
    {
        stage++;
        if (stage ==1)
        {
            stageDescription.text = "Scoring Stage\r\nAfter the building stage ends, you can start the game by pressing space, which will drop the ball in a random direction. At this point, you will play similarly to break out to try and score as much as you can in each round. However, you only have 15 bounces. After that, your paddle gets destroyed for the round. The score is kept in between rounds but not in between waves. At the end of the wave, if you manage to pass it, the objective changes: 1000 (Wave 1), 15000 (Wave 2), and 50000 (Wave 3).\r\nDifferent bricks have different abilities, so read carefully. After the ball is destroyed, the round ends, bricks reappear, and you return to the building stage. Good Luck!";
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }
    public void skipIntro()
    {
        SceneManager.LoadScene(1);
    }
}
