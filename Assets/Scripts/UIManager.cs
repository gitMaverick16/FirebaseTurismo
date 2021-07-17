using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject gameUI;
    public GameObject lvl1UI;
    public GameObject lvl2UI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        gameUI.SetActive(false);
        lvl1UI.SetActive(false);
        lvl2UI.SetActive(false);
    }
    public void LoginScreen() //Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
        //registerUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        ClearScreen();
        //loginUI.SetActive(false);
        registerUI.SetActive(true);
    }
    public void gameScreen()
    {
        ClearScreen();
        gameUI.SetActive(true);
    }
    public void LevelOneScreen()
    {
        ClearScreen();
        lvl1UI.SetActive(true);
    }
    public void LevelTwoScreen()
    {
        ClearScreen();
        lvl2UI.SetActive(true);
    }
}
