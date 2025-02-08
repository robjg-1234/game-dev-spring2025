using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] TMP_Text scoreLoading;
    public int target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(1,1,1);
    }
    public void SetScoreAndStart(int myTarget)
    {
        target = myTarget;
        StartCoroutine(CountUp());
    }
    public void newTarget(int myTarget)
    {
        target = myTarget;
    }
    IEnumerator CountUp()
    {
        int temp = 0;
        if (target > 200000)
        {
            temp = target / 2;
        }
        while (temp < target)
        {
            if (target < 7500)
            {
                temp++;
                scoreLoading.text = "+" + temp;
                yield return new WaitForSeconds(0.001f);
            }
            else
            {
                temp ++;
                scoreLoading.text = "+" + temp;
                yield return null;
            }
        }
        yield return new WaitForSeconds(1);
        while (scoreLoading.color.a > 0)
        {
            scoreLoading.color = new Color(scoreLoading.color.r, scoreLoading.color.g, scoreLoading.color.b, scoreLoading.color.a - 0.01f);
            yield return null;
        }
        Destroy(gameObject);
    }

}
