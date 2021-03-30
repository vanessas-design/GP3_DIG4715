using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
    }

    public void PlayGame ()
    {
        print("HowTo");
        SceneManager.LoadScene("HowTo");
    }

    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
