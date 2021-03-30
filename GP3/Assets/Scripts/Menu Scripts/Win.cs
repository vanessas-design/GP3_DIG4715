using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioClip macGuffin;

    [SerializeField]
    private AudioClip backgroundWin;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
        musicSource.clip = macGuffin;
        musicSource.Play();
        Invoke("PlayBG", 3.239f);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void PlayBG()
    {
        musicSource.Stop();
        musicSource.loop = true;
        musicSource.clip = backgroundWin;
        musicSource.Play();
    }
}