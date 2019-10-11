using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueActor : MonoBehaviour {
	public UnityEngine.UI.Image expressionSprite;
	public UnityEngine.UI.Image speechBubbleSprite;
	public UnityEngine.UI.Text speechBubbleText;

	[SerializeField]
	public ExpressionDictionary expressions;
	public string defaultExpression = "neutral";

    void Start()
    {
        DialogueUI.dialogueEvent += DialogueEventReceiver;
    }

    void DialogueEventReceiver(string[] args)
    {
        if (args[0] == "SetExpression" && args.Length == 3)
        {
            if (args[1] == gameObject.name)
            {
                SetExpression(args[2]);
            }
        }
    }

    public void Hide() {
		this.gameObject.SetActive( false );
	}

	public void Show() {
        if (expressionSprite)
        {
            expressionSprite.sprite = expressions[defaultExpression];
        }
		this.gameObject.SetActive( true );
	}

    public void SetExpression(string expressionName)
    {
        if (!expressionSprite)
        {
            return;
        }
        else if (expressions.ContainsKey(expressionName))
        {
            expressionSprite.sprite = expressions[expressionName];
        }
    }

    public void Speak(string text) {
		if(speechBubbleSprite)
        {
            speechBubbleSprite.gameObject.SetActive(true);
        }
		speechBubbleText.text = text;
	}

	public void StopSpeaking() {
		if(speechBubbleSprite)
        {
            speechBubbleSprite.gameObject.SetActive(false);
        }		
		speechBubbleText.text = "";
	}

    void OnDestroy()
    {
        DialogueUI.dialogueEvent -= DialogueEventReceiver;
    }
}
