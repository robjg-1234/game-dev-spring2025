using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    [SerializeField] string objName;
    private void OnMouseDown()
    {
        if (objName.Equals("key"))
        {
            DialogueManager.scc.setGameStateValue("keyFound", "set", "true");
            Destroy(gameObject);
        }
        else if (objName.Equals("diamond"))
        {
            DialogueManager.scc.setGameStateValue("diamondGrabbed", "set", "true");
            Destroy(gameObject);
        }
    }
}
