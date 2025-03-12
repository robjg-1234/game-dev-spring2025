using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Image FadeIn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeInMenu());
    }
    IEnumerator FadeInMenu()
    {
        while (FadeIn.color.a > 0)
        {
            FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, FadeIn.color.a - (1f * Time.deltaTime));
            yield return null;
        }
        FadeIn.color = new Color(FadeIn.color.r, FadeIn.color.g, FadeIn.color.b, 0);
        FadeIn.gameObject.SetActive(false);
    }
}
