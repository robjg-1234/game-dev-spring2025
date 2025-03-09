using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
    [SerializeField] TMP_Text personalBest;
    [SerializeField] Image FadeIn;
    public int numberLevel;
    string setPersonalBest;
    private void Start()
    {
        if (numberLevel == 2)
        {
            setPersonalBest = PlayerPrefs.GetString("levelOne");
            personalBest.text = "PB: " + setPersonalBest;
        }
        else if (numberLevel == 3)
        {
            setPersonalBest = PlayerPrefs.GetString("levelTwo");
            personalBest.text = "PB: " + setPersonalBest;
        }
        else if (numberLevel == 4)
        {
            setPersonalBest = PlayerPrefs.GetString("levelThree");
            personalBest.text = "PB: " + setPersonalBest;
        }
    }
    public void SelectLevel()
    {
        if (FadeIn.color.a == 0)
        {
            StartCoroutine(InitiateLevel());
        }
        
    }
    IEnumerator InitiateLevel()
    {
        FadeIn.gameObject.SetActive(true);
        while (FadeIn.color.a < 1)
        {
            FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, FadeIn.color.a + (1f * Time.deltaTime));
            yield return null;
        }
        SceneManager.LoadScene(numberLevel);
    }
}
