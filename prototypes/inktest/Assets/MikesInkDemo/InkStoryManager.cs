using UnityEngine;
using System.Collections;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class InkStoryManager : MonoBehaviour
{
    public static InkStoryManager instance;

    [SerializeField]
	private TextAsset inkJSONAsset = null;

    [SerializeField]
    private GameObject choicePrefab;

    [SerializeField]
    private GameObject choicesUI;

    [SerializeField]
    private GameObject textBoxUI;

    [SerializeField]
    private TMP_Text textBox;
    [SerializeField] private TMP_Text introSpace;
    [SerializeField] GameObject introCutscene;
    [SerializeField] Image introBackground;
    [SerializeField] private TMP_Text charcaterName;
    [SerializeField] GameObject door;
    bool aware = false;
    bool firstTime = true;
    bool known = false;
    int money = 0;
    bool ringHaver = false;
    bool openedDoor = false;

    // We are going to use this to make the coroutine wait until a choice button 
    // is clicked.
    private bool choiceMade = false;

    // We can use this to know whether a knot is currently running. I use it to
    // control whether I can launch TalkToCharacter or not.
    public bool knotActive = false;

    Story inkStory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        inkStory = new Story(inkJSONAsset.text);
        StartCoroutine(IntroSequence());
    }

    IEnumerator LaunchKnot(string knotName) {
        if (knotName.Equals("Tom"))
        {
            charcaterName.text = knotName;
        }
        else
        {
            charcaterName.text = "???";
        }
        knotActive = true;
        // Set ink to use the knotName that was provided
        inkStory.ChoosePathString(knotName);

        textBoxUI.SetActive(true);
        
        while (inkStory.canContinue) {
            LoadStates();
            // As long as there are no choices, keep displaying lines in
            // the text box, and waiting for the player to press space.
            while (inkStory.canContinue) {
                
                string line = inkStory.Continue().Trim();
                Debug.Log(line);
                // Display the line in the text box
                textBox.text = line;

                // Wait for input
                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    yield return null; // Wait for next frame
                }
                yield return null; // This is necessary because the loop continued when space was pressed and registered it as being pressed again.
            }

            // If there are any choices, wait for the choice to be made. We make this
            // a while because there may be two sets of choice in a row.
            while (inkStory.currentChoices.Count > 0) {
                
                choicesUI.SetActive(true);

                // Display all the choices, if there are any!
                for (int i = 0; i < inkStory.currentChoices.Count; i++) {
                    Choice choice = inkStory.currentChoices [i];
                    GameObject buttonObj = Instantiate(choicePrefab, choicesUI.transform);
                    Button button = buttonObj.GetComponent<Button>();
                    TMP_Text choiceText = buttonObj.GetComponentInChildren<TMP_Text>();
		            choiceText.text = choice.text;

                    // Tell the button what to do when we press it
                    button.onClick.AddListener(() => {
                        inkStory.ChooseChoiceIndex(choice.index);
                        inkStory.Continue();
                        choiceMade = true;
                        RemoveChoiceButtons();
                    });
                }
                // Wait for the button function above to be called, which sets
                // choiceMade to true, so we will move on in the coroutine
                while (!choiceMade)
                {
                    yield return null;
                }
                choiceMade = false; // reset this
                choicesUI.SetActive(false);
            }
            if (door!=null)
            {
                bool testForDoor = bool.Parse(inkStory.variablesState["openedDoor"].ToString());
                if (testForDoor)
                {
                    Destroy(door);
                }
            }
        }
        SaveStates();
        // Turn off the UI now that the knot is over
        textBoxUI.SetActive(false);
        knotActive = false;
    }

    void RemoveChoiceButtons() {
		int childCount = choicesUI.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (choicesUI.transform.GetChild (i).gameObject);
		}
    }

    public void TalkToCharacter(string target) {
        inkStory = new Story(inkJSONAsset.text);
        StartCoroutine(LaunchKnot(target));
    }
    public void changeMoney(int amountGained)
    {
        money += amountGained;
    }
    IEnumerator IntroSequence()
    {
        knotActive = true;
        // Set ink to use the knotName that was provided
        inkStory.ChoosePathString("Start");

        while (inkStory.canContinue)
        {
            knotActive = true;
            // As long as there are no choices, keep displaying lines in
            // the text box, and waiting for the player to press space.
            while (inkStory.canContinue)
            {
                string line = inkStory.Continue().Trim();
                introSpace.text = line;
                while (introSpace.color.a < 1)
                {
                    introSpace.color = new Color(introSpace.color.r, introSpace.color.g, introSpace.color.b, introSpace.color.a + 2 * Time.deltaTime);
                    yield return null;
                }
                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    yield return null; 
                }
                while (introSpace.color.a > 0)
                {
                    introSpace.color = new Color(introSpace.color.r, introSpace.color.g, introSpace.color.b, introSpace.color.a - 2 * Time.deltaTime);
                    yield return null;
                }
                yield return null; 
            }
        }
        while (introBackground.color.a > 0)
        {
            introBackground.color = new Color(introBackground.color.r, introBackground.color.g, introBackground.color.b, introBackground.color.a - 2 * Time.deltaTime);
        }
        introBackground.gameObject.SetActive(false);
        knotActive = false;
    }

    void SaveStates()
    {
        aware = bool.Parse(inkStory.variablesState["aware"].ToString());
        firstTime = bool.Parse(inkStory.variablesState["firstTime"].ToString());
        known = bool.Parse(inkStory.variablesState["known"].ToString());
        ringHaver = bool.Parse(inkStory.variablesState["ringHaver"].ToString());
        openedDoor = bool.Parse(inkStory.variablesState["openedDoor"].ToString());
        money = int.Parse(inkStory.variablesState["money"].ToString());
    }
    void LoadStates()
    {
        inkStory.variablesState["firstTime"] = firstTime;
        inkStory.variablesState["known"] = known;
        inkStory.variablesState["ringHaver"] = ringHaver;
        inkStory.variablesState["openedDoor"] = openedDoor;
        inkStory.variablesState["aware"] = aware;
        inkStory.variablesState["money"] = money;
    }

}
