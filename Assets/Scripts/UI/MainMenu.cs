using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuHolder;
    public GameObject OptionsMenuHolder;
    public void StartGame()
    {
        Debug.Log("StartGame");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void ShowOptionsMenu()
    {
        MainMenuHolder.SetActive(false);
        OptionsMenuHolder.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        MainMenuHolder.SetActive(true);
        OptionsMenuHolder.SetActive(false);
    }
}
