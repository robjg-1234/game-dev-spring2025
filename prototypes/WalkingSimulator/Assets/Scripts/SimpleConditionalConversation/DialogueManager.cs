using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
	public static SimpleConditionalConversation scc;

	public static Action<string, string> DialogueAction;

	// NOTE: When you do not use the google sheet option, it is expecting the file
	// to be named "data.csv" and for it to be in the Resources folder in Assets.
	public bool useGoogleSheet = false;
	public string googleSheetDocID = "";

	// Start is called before the first frame update
	void Start()
	{
		if (useGoogleSheet) {
			// This will start the asyncronous calls to Google Sheets, and eventually
			// it will give a value to scc, and also call LoadInitialHistory().
			GoogleSheetSimpleConditionalConversation gs_ssc = gameObject.AddComponent<GoogleSheetSimpleConditionalConversation>();
			gs_ssc.googleSheetDocID = googleSheetDocID;
		} else {
			scc = new SimpleConditionalConversation("data");
			LoadInitialSCCState();
		}
	}
	
	public static void LoadInitialSCCState()
	{
		// Example of setting the initial state:
		// NOTE: If you are putting a number or bool, make sure not to store them
		// as strings.
		//

		scc.setGameStateValue("keyFound", "equals", "false");
        scc.setGameStateValue("safeClosed", "set", "true");
    }
}
