using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioClip musicClipLose;

    [SerializeField]
    private AudioClip backgroundLose;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
        musicSource.clip = musicClipLose;
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
        musicSource.clip = backgroundLose;
        musicSource.Play();
    }
}
