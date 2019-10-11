using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Shipyard : MonoBehaviour
{
    public DialogueRunner dialogueSystem;
    public GameObject Alex;
    public GameObject MsMeta;
    public GameObject Backdraft;
    public Gamekit3D.InteractOnCollision AlexDialogueTrigger;
    public ScreenOpacityFilter screenOpacityFilter;

    private int numDroneKilled = 0;

    void Start()
    {
        SoundManager.LoopPlay("40s-swing-theme");
        DialogueUI.dialogueEvent += DialogueEventReceiver;
    }

    void Update()
    {
        if (numDroneKilled >= 2)
        {
            AlexDialogueTrigger.active = true;
        }
    }

    public void DroneKill()
    {
        numDroneKilled++;
    }

    void DialogueEventReceiver(string[] args)
    {
        if (args[0] == "Alex_Turn")
        {
            Alex.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        } else if (args[0] == "Alex_Fly")
        {
            Alex.GetComponent<Animator>().SetTrigger("Fly");
        } else if (args[0] == "Meta_Turn")
        {
           MsMeta.transform.eulerAngles += new Vector3(0f, 180f, 0f);
        } else if (args[0] == "Backdraft_Enter")
        {
            Backdraft.SetActive(true);
        } else if (args[0] == "Backdraft_CastFireball")
        {
            Backdraft.GetComponent<Backdraft>().CastFireball();
        }
    }

    void OnDestroy()
    {
        DialogueUI.dialogueEvent -= DialogueEventReceiver;
    }
}
