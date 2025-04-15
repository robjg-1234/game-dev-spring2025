using UnityEngine;

public class NPCScript : MonoBehaviour
{
    [SerializeField] string characterName;
    public void StartConversation()
    {
        InkStoryManager.instance.TalkToCharacter(characterName);
    }
}
