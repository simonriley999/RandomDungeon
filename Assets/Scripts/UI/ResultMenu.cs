using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenu : MonoBehaviour
{
    [SerializeField]public float transitionTime;
    Animator anime;

    void BackToMainMenu(int _scenceIndex)
    {
        StartCoroutine(CBackToMainMenu(_scenceIndex));
    }

    IEnumerator CBackToMainMenu(int _scenceIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_scenceIndex,LoadSceneMode.Single);
        operation.allowSceneActivation = false;//暂时不允许跳转
        anime.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        while (!operation.isDone)
        {
            yield return null;
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                anime.SetTrigger("Done");
                break;
            }
        }
    }
}
