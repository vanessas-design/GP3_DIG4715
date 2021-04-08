using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;

    private PlayerController playerController;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
        playerController.paused = false;
        Destroy(parent);
    }
    
    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        playerController.paused = false;
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
