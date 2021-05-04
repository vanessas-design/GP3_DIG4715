using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    private GameObject winCinematic;

    private GameObject dialogueObject;
    private AudioSource dialogueSource;
    private GameObject dialogueTextObject;
    private Text dialogueText;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
        musicSource.clip = macGuffin;
        musicSource.Play();
        Invoke("PlayBG", 3.239f);
        Instantiate(winCinematic);
        dialogueObject = GameObject.FindWithTag("dialogue");
        dialogueSource= dialogueObject.GetComponent<AudioSource>();
        dialogueTextObject = GameObject.FindWithTag("DialogueText");
        dialogueText = dialogueTextObject.GetComponent<Text>();
        dialogueText.text = "Hey mom. It’s Blake. I know I was just here last week, but I wanted to come by and say Hi again. I don’t know if it was actually your doing but...I know you’re with me. I saw you, in my dreams. You were leading me through our memories, and I realized at the end of the of the dream what you were trying to tell me, what you were leading me to do.";
        Invoke("Textshift1", 22.0f);
        Invoke("Textshift2", 31.0f);
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
        musicSource.clip = backgroundWin;
        musicSource.Play();
    }
    private void Textshift1()
    {
        dialogueText.text = "You always made me feel so loved and cared for, and in almost every moment of my life, you taught me lessons that have made me the person I am today.";
    }
    private void Textshift2()
    {
        dialogueText.text = "And in my dream, you were still teaching me. It wasn’t until the end that I realized, you were teaching me how to let go, how to heal. Thank you mom. I love you. And I know I’ll see you again one day.";
    }
    private void CinematicEnd()
    {
        Destroy(GameObject.FindWithTag("Cinematic"));
    }
}