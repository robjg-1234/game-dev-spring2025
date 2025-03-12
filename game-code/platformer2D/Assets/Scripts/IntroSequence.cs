using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] Image fadeIn;
    [SerializeField] Image FirstPage;
    [SerializeField] TMP_Text Description;
    bool movingOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetString("levelOne", "0:00:00");
        PlayerPrefs.SetString("levelTwo", "0:00:00");
        PlayerPrefs.SetString("levelThree", "0:00:00");
        PlayerPrefs.SetFloat("levelOneTime", 99);
        PlayerPrefs.SetFloat("levelTwoTime", 99);
        PlayerPrefs.SetFloat("levelThreeTime", 99);
        PlayerPrefs.Save();
        StartCoroutine(IntroCustscene());
    }
    public void GoToNextScreen()
    {
        if (!movingOn)
        {
            StartCoroutine(MoveOn());
            movingOn = true;
        }

    }
    IEnumerator IntroCustscene()
    {
        while (FirstPage.color.a < 1)
        {
            Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, Description.color.a + (0.5f * Time.deltaTime));
            FirstPage.color = new Color(1, 1, 1, FirstPage.color.a + (0.5f * Time.deltaTime));
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (FirstPage.color.a > 0)
        {
            Description.color = new Color(Description.color.r, Description.color.g, Description.color.b, Description.color.a - (0.5f * Time.deltaTime));
            FirstPage.color = new Color(FirstPage.color.r, FirstPage.color.g, FirstPage.color.b, FirstPage.color.a - (0.5f * Time.deltaTime));
            yield return null;
        }
        while (fadeIn.color.a > 0)
        {
            fadeIn.color = new Color(fadeIn.color.r, fadeIn.color.g, fadeIn.color.b, fadeIn.color.a - (0.5f * Time.deltaTime));
            yield return null;
        }
        fadeIn.gameObject.SetActive(false);
    }
    IEnumerator MoveOn()
    {
        fadeIn.gameObject.SetActive(true);
        while (fadeIn.color.a < 1)
        {
            fadeIn.color = new Color(fadeIn.color.r, fadeIn.color.g, fadeIn.color.b, fadeIn.color.a + (0.5f * Time.deltaTime));
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
