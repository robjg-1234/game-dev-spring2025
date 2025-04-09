using UnityEngine;
using Ink.Runtime;
public class textManager : MonoBehaviour
{
    [SerializeField] TextAsset textAsset = null;
    Story story;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        story = new Story(textAsset.text);
        while(story.canContinue)
        {
            if (story.currentChoices.Count > 0)
            {
                foreach(Choice c in story.currentChoices)
                {
                    Debug.Log(c);
                }
                int choiceIndex = Random.Range(0, story.currentChoices.Count);
                story.ChooseChoiceIndex(choiceIndex);
                
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
