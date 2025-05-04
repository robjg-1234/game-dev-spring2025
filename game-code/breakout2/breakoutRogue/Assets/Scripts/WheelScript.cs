using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WheelScript : MonoBehaviour
{
    [SerializeField] Image wheel;
    public bool spinning = false;
    int finalizedAngle = 0;
    gameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = gameManager.instance;
    }
    public void SpinTheWheel()
    {
        if (!spinning)
        {
            if (gm.UseWheel())
            {
                int spins = Random.Range(0, 4);
                int choice = Random.Range(1, 21);
                StartCoroutine(StartTheSpin(spins, choice));
            }
        }
    }

    IEnumerator StartTheSpin(int spins, int finalChoice)
    {
        int variance = Random.Range(-5, 6);
        float wheelSpinSpeed = 288;
        spinning = true;
        spins = (spins+3)*20 + finalChoice;
        float angleOfTheWheel = wheel.gameObject.transform.rotation.eulerAngles.z;
        float initialPos = angleOfTheWheel;
        float amountTraversed = 0;
        while (spinning)
        {
            angleOfTheWheel += wheelSpinSpeed * Time.deltaTime;
            amountTraversed = angleOfTheWheel- initialPos;
            if (angleOfTheWheel > 360)
            {
                initialPos -= 360;
                angleOfTheWheel -= 360;
            }
            if (amountTraversed > 18)
            {
                initialPos = angleOfTheWheel;
                spins--;
            }
            if (spins < (20 + variance))
            {
                wheelSpinSpeed  = 288 * spins/(20f+ variance);
            }
            if (spins <= 0)
            {
                spinning = false;
            }
            wheel.gameObject.transform.eulerAngles = new Vector3(wheel.gameObject.transform.eulerAngles.x, wheel.gameObject.transform.eulerAngles.y, angleOfTheWheel);
            yield return null;
        }
        finalizedAngle = Mathf.FloorToInt(angleOfTheWheel/18f);
        gm.ActivateWheelEffect(finalizedAngle);
    }
}
