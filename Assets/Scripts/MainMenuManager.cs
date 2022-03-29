using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject levelSelectScreen; //reference to the lvl select screen
    public GameObject mainMenuScreen; //reference to the main menu screen

    /// <summary>
    /// starts the game at the first lvl
    /// </summary>
    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// shows the lvl select screen
    /// </summary>
    public void LevelSelectButton()
    {
        GetComponent<Animator>().SetBool("ShowLvlSelect", true);
    }

    /// <summary>
    /// returns to main menu from the lvl select screen
    /// </summary>
    public void LevelSelectBackButton()
    {
        GetComponent<Animator>().SetBool("ShowLvlSelect", false);
    }

    /// <summary>
    /// quits the game
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// loads a given lvl
    /// </summary>
    /// <param name="lvl">the lvl that is going to be loaded</param>
    public void Loadlevel(int lvl)
    {
        SceneManager.LoadScene(lvl);
    }
}
