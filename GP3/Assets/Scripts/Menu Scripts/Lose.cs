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
    [SerializeField]
    private GameObject lossCinematic;

    private GameObject dialogueObject;
    private AudioSource dialogueSource;
    private GameObject dialogueTextObject;
    private Text DialogueText;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
        musicSource.clip = musicClipLose;
        musicSource.Play();
        Invoke("PlayBG", 3.239f);
        Instantiate(lossCinematic);
        dialogueObject = GameObject.FindWithTag("dialogue");
        dialogueSource= dialogueObject.GetComponent<AudioSource>();
        dialogueTextObject = GameObject.FindWithTag("DialogueText");
        DialogueText = dialogueTextObject.GetComponent<Text>();
        DialogueText.text = "Mom...I’m sorry...it’s just too soon. Too soon since you’ve been...gone. I don’t know what to do with these thoughts...these memories....I can’t just keep going like that, without you, with just memories of what once was. I don’t know what to do, how to get it to feel like you’re still here. Everyone says you’re not really gone...but they don’t know what it’s like to lose someone like you in an instant...I miss you, mom.";
        Invoke("CinematicEnd", dialogueSource.clip.length);
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
    private void CinematicEnd()
    {
        Destroy(GameObject.FindWithTag("Cinematic"));
    }
}
