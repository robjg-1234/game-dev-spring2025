using UnityEngine;

public class ReplenisherScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    GameManager gm;
    public bool isHit = false;
    float cooldown = 0;
    Color defaultColor;
    private void Start()
    {
        gm = GameManager.instance;
        defaultColor =spriteRenderer.color;
        gm.playerDeath += RestartCooldown;
    }
    private void OnDestroy()
    {
        gm.playerDeath -= RestartCooldown;
    }
    private void Update()
    {
        if (cooldown> 0)
        {
            spriteRenderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.25f);
            cooldown -= Time.deltaTime;
        }
        else
        {
            spriteRenderer.color = defaultColor;
            isHit = false;
        }
    }
    public void StartCooldown()
    {
        isHit = true;
        cooldown = 5f;
    }
    public void RestartCooldown()
    {
        cooldown = 0;
    }
}
