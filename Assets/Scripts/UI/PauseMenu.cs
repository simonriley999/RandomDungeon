using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseMenuHolder;
    public GameObject OptionsMenuHolder;
    public void ShowOptionsMenu()
    {
        PauseMenuHolder.SetActive(false);
        OptionsMenuHolder.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        PauseMenuHolder.SetActive(true);
        OptionsMenuHolder.SetActive(false);
    }

}
