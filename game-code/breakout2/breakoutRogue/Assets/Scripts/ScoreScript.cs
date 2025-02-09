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
        target += myTarget;
    }
    IEnumerator CountUp()
    {
        int temp = 0;
        while (temp < target)
        {
            if ((target - temp) > 100000)
            {
                temp += 50000;
                scoreLoading.text = "+" + temp;
                yield return null;
            }
            else if ((target - temp) > 10000)
            {
                temp += 1000;
                scoreLoading.text = "+" + temp;
                yield return null;
            }
            else if ((target - temp) > 2500)
            {
                temp += 100;
                scoreLoading.text = "+" + temp;
                yield return null;
            }
            else if ((target - temp) > 500)
            {
                temp += 10;
                scoreLoading.text = "+" + temp;
                yield return null;
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
            scoreLoading.color = new Color(scoreLoading.color.r, scoreLoading.color.g, scoreLoading.color.b, scoreLoading.color.a - 5f * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

}
