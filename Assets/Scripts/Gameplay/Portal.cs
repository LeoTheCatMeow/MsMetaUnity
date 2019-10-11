using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string destination;
    public ScreenOpacityFilter opacityFilter;

    private bool loadingInProcess;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && destination != null)
        {
            StartCoroutine(LoadNextLevel(destination));
        }
    }

    IEnumerator LoadNextLevel(string name)
    {
        SceneController.FreezeGame();
        loadingInProcess = true;
        opacityFilter.FadeOut();
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadSceneAsync(destination);
    }

    void OnDestroy()
    {
        if (loadingInProcess)
        {
            SceneController.ResumeGame();
        } 
    }
}
