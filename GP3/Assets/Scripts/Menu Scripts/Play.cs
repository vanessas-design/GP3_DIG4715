using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameObject.FindWithTag("Button1"));
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("KitchenLevel");
    }
}
