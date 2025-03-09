using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TempPlatform : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] GameObject physicalPlat;
    [SerializeField] SpriteRenderer rend;
    Color defaultColor;
    float cooldown = 0f;
    bool playerSteppedOn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.instance;
        defaultColor = rend.color;
        gameManager.playerDeath += ResetPlatform;
        
    }
    private void OnDestroy()
    {
        gameManager.playerDeath -= ResetPlatform;
    }
    private void Update()
    {
        if (!gameManager.isPaused)
        {
            if (playerSteppedOn)
            {
                if (cooldown < 3f)
                {
                    cooldown += Time.deltaTime;
                    float change = Map(cooldown, 0f, 3f, 0f, 0.5f);
                    rend.color = new Color(change, rend.color.g, rend.color.b);
                }
                else
                {
                    cooldown = 3f;
                    playerSteppedOn = false;
                    physicalPlat.SetActive(false);
                }
            }
            else
            {
                if (!physicalPlat.activeSelf)
                {
                    if (cooldown > 0f)
                    {
                        cooldown -= Time.deltaTime;
                    }
                    else
                    {
                        rend.color = defaultColor;
                        physicalPlat.SetActive(true);
                        cooldown = 0f;
                    }
                }
            }
        }
        
    }
    public void SteppedOn()
    {
        if (physicalPlat)
        {
            playerSteppedOn = true;
        }
    }
    public void ResetPlatform()
    {
        rend.color = defaultColor;
        physicalPlat.SetActive(true);
        cooldown=0f;
        playerSteppedOn = false;

    }
    // Update is called once per frame

    public float Map(float valueOld, float oldMin, float oldMax, float newMin, float newMax)
    {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float valueOldPercent = (valueOld - oldMin) / oldRange;
        return newRange * valueOldPercent + newMin;
    }
}
