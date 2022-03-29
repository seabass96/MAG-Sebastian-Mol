using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameManager manager; //reference to the game manager
    public Text PointsText; //the text that shows the points
    public Text MovesText;// the text that shows the moves 
    public GameObject GUI;// reference to the games user interface
    public GameObject LevelCompleatScreen; //reference to the lvl complete screen
    public GameObject[] stars; //reference to all the stars in the lvl complete screen
    public float TimeBtweenStarAnimation;


    /// <summary>
    /// update the points text 
    /// </summary>
    private void UpdatePoints()
    {
        PointsText.text = manager.PointsToCompleteLevel + "/" + manager.points.ToString();
    }

    /// <summary>
    /// update the moves text 
    /// </summary>
    private void UpdateMoves()
    {
        MovesText.text = "Moves: " + manager.amountOfMoves.ToString();
    }

    /// <summary>
    /// checks to see if the lvl is complete
    /// </summary>
    public void IsLevelCompleated()
    {
        if (manager.LvlComplete)
        {
            GetComponent<Animator>().SetTrigger("Complete");
            GUI.SetActive(false);
        }
    }

    /// <summary>
    /// activates the stars at the end of the lvl
    /// </summary>
    public void ActivateStars()
    {
        StartCoroutine(ActivateStarsInCannon(manager.amountOfStars));
    }

    /// <summary>
    /// logic for the stars to activate in a row and not all at once
    /// </summary>
    /// <param name="amountOfStars">how many stars should be activated</param>
    /// <returns></returns>
    private IEnumerator ActivateStarsInCannon(int amountOfStars)
    {
        for (int i = 0; i < amountOfStars; i++)
        {
            stars[i].SetActive(true);
            yield return new WaitForSeconds(TimeBtweenStarAnimation);
        }
        yield return null;
    }

    /// <summary>
    /// loads the next lvl
    /// </summary>
    public void LoadNextLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// return to main menu
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        UpdatePoints();
        UpdateMoves();
        IsLevelCompleated();
    }
}
