/*
 * Original code supplied by Secret Lab Pty. Ltd. and Yarn Spinner contributors
 * under MIT License and later edited and adapted.
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

/// Displays dialogue lines to the player, and sends
/// user choices back to the dialogue system.

/** Note that this is just one way of presenting the
 * dialogue to the user. The only hard requirement
 * is that you provide the RunLine, RunOptions, RunCommand
 * and DialogueComplete coroutines; what they do is up to you.
 */
public class DialogueUI : Yarn.Unity.DialogueUIBehaviour {

	public Yarn.Unity.DialogueRunner dialogueRunner;

	/// A UI element that appears after lines have finished appearing
	public GameObject continuePrompt;
	public UnityEngine.Events.UnityEvent onStart, onEnd;

	/* 
    ==================
    Added variables
    ==================
     */
	public ActorDictionary actors;
    private DialogueActor speakingActor = null;

    /// The buttons that let the user choose an option
    public List<Image> optionButtons;
    private int currentOption = 0;

    /// For c# event, additional modification by LeoTheCat
    public delegate void eventDelegate(string[] args);
    public static event eventDelegate dialogueEvent;

    void Awake() {
		foreach( var button in optionButtons ) {
			button.gameObject.SetActive( false );
		}

		// Hide the continue prompt if it exists
		if( continuePrompt != null )
        {
            continuePrompt.SetActive(false);
        }

        //Hide the actors and their text
        HideAllActors();
    }

	/// Show a line of dialogue
    /// If the line is empty (ignore spaces), the actor will stop speaking (speach bubble go away)
    /// If the line is #Hide, the actor will hide (hide expression sprites and speach bubble)
	public override IEnumerator RunLine(Yarn.Line line) {
        //Get the current speaking actor actor by name
        DialogueActor actor = actors[line.text.Substring( 0, line.text.IndexOf( ':' ) )];
		line.text = line.text.Substring( line.text.IndexOf( ':' ) + 1 );
        line.text = line.text.Trim();
        if (actor == null) {
			Debug.LogError( "[DialogueUI] Invalid actor name: " + line.text.Substring( 0, line.text.IndexOf( ':' ) ) );
			yield return null;
		}
		// Switch actors if necessary
		if(actor != speakingActor) {
			if(speakingActor)
            {
                speakingActor.StopSpeaking();
            }
            actor.Show();
			speakingActor = actor;
		}

		if(line.text == "") {
            speakingActor.StopSpeaking();
            yield break;
		} else if (line.text.ToLower() == "#hide")
        {
            speakingActor.Hide();
            yield break;
        } else {
			speakingActor.Speak( line.text );
		}

		// Show the continue prompt if we have one
		if(continuePrompt)
        {
           continuePrompt.SetActive(true);
        }

        bool done = false;
		while(!done) {
			if (Input.GetKeyDown(KeyCode.Return))
            {
                done = true;
            }
			yield return null;
		}

		if(continuePrompt)
        {
           continuePrompt.SetActive(false);
        }	
	}

	/// Show a list of options, and wait for the player to make a selection.
	public override IEnumerator RunOptions(Yarn.Options optionsCollection, Yarn.OptionChooser optionChooser) {
		// Do a little bit of safety checking
		if(optionsCollection.options.Count > optionButtons.Count) {
			Debug.LogWarning( "There are more options to present than there are buttons to present them in. This will cause problems." );
		}

		// Display each option in a button, and make it visible
		int i = 0;
		foreach(var optionString in optionsCollection.options) {
			optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].GetComponent<Outline>().enabled = false;
			optionButtons[i].GetComponentInChildren<Text>().text = optionString;
			i++;
		}

        bool choiceMade = false;
		// Wait until the chooser has been used 
		while(!choiceMade) {
            optionButtons[currentOption].GetComponent<Outline>().enabled = false;
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentOption = Mathf.Max(0, currentOption - 1);
            } else if (Input.GetKeyDown(KeyCode.S))
            {
                currentOption = Mathf.Min(optionsCollection.options.Count - 1, currentOption + 1);
            } else if (Input.GetKeyDown(KeyCode.Return))
            {
                optionChooser(currentOption);
                choiceMade = true;
            }
            optionButtons[currentOption].GetComponent<Outline>().enabled = true;
            yield return null;
		}

		// Hide all the buttons
		foreach(var button in optionButtons) {
			button.gameObject.SetActive(false);
		}
    }

	/// Run an internal command.
    /// Commands starting with # are sent into this coroutine instead of being processed as a regular yarn command
    /// #Wait [time] to suspend the conversation for a certain amount of time
    /// #Anythingelse will be sent out as a c# event, other scripts should subscribe to DialogueUI.dialogueEvent
	public override IEnumerator RunCommand(Yarn.Command command) {
        Debug.Log("Command: " + command.text);
        if (command.text[0] == '#')
        {
            string info = command.text.Remove(0, 1);
            string[] args = info.Split(' ');
            if (args[0].Trim().ToLower() == "wait")
            {
                float time = 0f;
                if (float.TryParse(args[1].Trim(), out time))
                {
                    yield return new WaitForSeconds(time);
                }
                yield break;
            }
            if (dialogueEvent != null)
            {
                dialogueEvent(args);
            }
        }
		yield break;
	}

	/// Called when the dialogue system has started running.
	public override IEnumerator DialogueStarted() {
        ConstrianAllActors();
        if (onStart != null)
        {
            onStart.Invoke();
        }		

		yield break;
	}

	/// Called when the dialogue system has finished running.
	public override IEnumerator DialogueComplete() {
        speakingActor = null;
        ReleaseAllActors();
        HideAllActors();

        if ( onEnd != null )
        {
            onEnd.Invoke();
        }	

		yield break;
	}

    private void ConstrianAllActors()
    {
        foreach (DialogueActor actor in actors.Values)
        {
            if (actor != null)
            {
                Actor x = actor.GetComponentInParent<Actor>();
                if (x != null)
                {
                    x.ConstrainControl();
                    x.ResetVelocity();
                }
            }
        }
    }

    private void ReleaseAllActors()
    {
        foreach (DialogueActor actor in actors.Values)
        {
            if (actor != null)
            {
                Actor x = actor.GetComponentInParent<Actor>();
                if (x != null)
                {
                    x.ReleaseControl();
                }
            }
        }
    }

	private void HideAllActors() {
		foreach(var pair in actors) {
            if (pair.Value != null)
            {
                pair.Value.Hide();
            }
		}
	}
}


