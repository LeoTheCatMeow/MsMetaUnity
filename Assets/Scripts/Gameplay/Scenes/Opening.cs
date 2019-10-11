using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Opening : MonoBehaviour
{
    public DialogueRunner dialogueSystem;
    public GameObject MsMeta;
    public SpiderDrone drone;
    public GameObject MP1;
    public GameObject MP2;
    
    void Start()
    {
        SoundManager.LoopPlay("Modern-theme");
        DialogueUI.dialogueEvent += DialogueEventReceiver;
    }

    void DialogueEventReceiver(string[] args)
    {
        if (args[0] == "Play40s")
        {
            SoundManager.LoopPlay("40s-swing-theme");
        }
        else if (args[0] == "Drone_Start_Attack")
        {
            drone.hostile = true;
        }
        else if (args[0] == "MP_Turn")
        {
            MP1.transform.eulerAngles += new Vector3(0f, 180f, 0f);
            MP2.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        }
        else if (args[0] == "Meta_Turn")
        {
            MsMeta.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        }
    }

    void OnDestroy()
    {
        DialogueUI.dialogueEvent -= DialogueEventReceiver;
    }
}
