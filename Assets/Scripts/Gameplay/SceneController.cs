using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
	
	public static bool paused;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (paused) {
                SoundManager.Resume();
                ResumeGame();
				paused = false;
			} else {
                SoundManager.Stop();
                FreezeGame();
                paused = true;
			}
		}
	}

    public static void FreezeGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);    
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

	public void Exit() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
