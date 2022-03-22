using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public Animator transition;
    [SerializeField]public float transitionTime;
    public void LoadALevel(int _scenceIndex)
    {
        Debug.Log("LoadLevel");
        StartCoroutine(StartToLoad(_scenceIndex));
    }

    IEnumerator StartToLoad(int _scenceIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_scenceIndex,LoadSceneMode.Single);
        operation.allowSceneActivation = false;//暂时不允许跳转
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        while (!operation.isDone)
        {
            yield return null;
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                transition.SetTrigger("Done");
                break;
            }
        }
        
    }
}
