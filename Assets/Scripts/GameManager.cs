using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct TileInfo //information for the tile coupled together
{
    public Color color;
    public int Point;
}


public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    internal GameObject[,] levelTiles;
    [Tooltip("number of rows in the lvl")]
    public int rows;
    [Tooltip("number of columns in the lvl")]
    public int columns;
    [Tooltip("all the possible combination of color and points for the lvl")]
    public TileInfo[] colorsPoints;
    [Tooltip("number of points needed to complete the lvl")]
    public int PointsToCompleteLevel;
    [Tooltip("amount of moves the player can make in the lvl")]
    public int amountOfMoves;
    [Tooltip("the sound that plays when the player fails a match")]
    public GameObject FaildMatchSound;

    [Header("Tile Settings")]
    [Tooltip("The tile game object used in the game")]
    public GameObject Tile;
    [Tooltip("the minimum amount of matched needed to make a complete chain")]
    public int AmountOfMatchedTilesNeeded = 3;
    [Tooltip("the speed at which the tile gets destroys")]
    public float speedOfTileDestruction;
    [Tooltip("the speed at which the tiles fall down at")]
    public float fallingSpeedOfTiles;

    private List<GameObject> SelectedTilesSorted = new List<GameObject>(); //a sorted list of selected tiles 
    private bool IsMatching = false; //used to see if the matching process is still running

    internal List<GameObject> SelectedTiles = new List<GameObject>(); //a list of tile the player has selected
    internal int points; //amount of points the player has earned in the lvl
    internal int amountOfStars = 3; //amount of stars in the lvl the player will looses these if requirements are not met
    internal bool LvlComplete = false; //used to signify if the lvl has been completed



    private void Start()
    {
        StartLevel();      
    }

    private void Update()
    {
        isLevelComplete();
    }

    /// <summary>
    /// sets up the level at the start looks to see if there are any possible matches
    /// </summary>
    private void StartLevel()
    {
        GenLevel();
        ShuffleIfNoMatches();
    }

    /// <summary>
    /// recursion used to shuffle the board till there is a possible match
    /// </summary>
    /// <returns>itself until the processes is done</returns>
    private bool ShuffleIfNoMatches()
    {
        if(!CheckForMatchesInLevel())
        {
            ShuffleTiles();
            return CheckForMatchesInLevel();
        }

        return true;
    }

    /// <summary>
    /// the match button logic that accepts the chosen tiles and sees if they are a match
    /// </summary>
    public void MatchButton()
    {
        if (LookForMatchInSelectedTiles())
        {
            StartCoroutine(DestorySelectedAndMoveDown());
            MovesLeftTracker();
        }
        else
        {
            UnselectSelectedTiles();
            PlayFailSound();
        }
    }

    /// <summary>
    /// a sound plays when the tiles dont match
    /// </summary>
    private void PlayFailSound()
    {
        GameObject sound = Instantiate(FaildMatchSound);
        Destroy(sound, 1);
    }

    /// <summary>
    /// checks to see if the lvl is complete and saves how many stars the player got in this lvl
    /// </summary>
    private void isLevelComplete()
    {
        if (points >= PointsToCompleteLevel && !IsMatching)
        {
            StopCoroutine(DestorySelectedAndMoveDown());
            //save the stars if they got more than last time
            if (PlayerPrefs.HasKey("LVL" + SceneManager.GetActiveScene().buildIndex))
            {
                if (PlayerPrefs.GetInt("LVL" + SceneManager.GetActiveScene().buildIndex) < amountOfStars)
                {
                    PlayerPrefs.SetInt("LVL" + SceneManager.GetActiveScene().buildIndex, amountOfStars);
                }
            }
         
            LvlComplete = true;
        }
    }

    /// <summary>
    /// tracks how many move the play can make before loosing a star
    /// </summary>
    private void MovesLeftTracker()
    {
        if (amountOfMoves > 0)
        {
            amountOfMoves--;
        }
        else
        {
            amountOfStars = 2;
        }
    }

    /// <summary>
    /// unselect the selected tiles used if there was no match
    /// </summary>
    private void UnselectSelectedTiles()
    {
        foreach (GameObject item in SelectedTiles)
        {
            item.GetComponent<Tile>().UnselectTile();
        }
        SelectedTiles.Clear();
    }

    /// <summary>
    /// destroys the selected tiles in order and moves the other tiles down to fill in the blank spaces
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestorySelectedAndMoveDown()
    {
        IsMatching = true;
        GameObject[] selectedOrderTiles = new GameObject[SelectedTiles.Count];
        SelectedTiles.CopyTo(selectedOrderTiles);
        int counter = SelectedTiles.Count;


        //NOTE this is not the fastest sorting algorithm out there but for the size it works pretty well 
        //if the lists where going to be longer i would have to use a real algorithm not just one i made up 
        //sort it into height order list, im trying to use google as little as possible
        while (counter != 0)
        {
            GameObject smallestY = SelectedTiles[0];

            for (int i = 0; i < SelectedTiles.Count; i++)
            {
                if (SelectedTiles[i] != smallestY)
                {
                    if (SelectedTiles[i].transform.position.y < smallestY.transform.position.y)
                    {
                        smallestY = SelectedTiles[i];
                    }
                }
            }

            SelectedTiles.Remove(smallestY);
            SelectedTilesSorted.Add(smallestY);
            counter--;
        }

        //destroy the selected tiles
        foreach (GameObject item in selectedOrderTiles)
        {
            item.GetComponent<Tile>().DestroyTile();
            points += item.GetComponent<Tile>().points;
            yield return new WaitForSeconds(speedOfTileDestruction);
        }

        IsMatching = false;

        //move the rest of the tiles down
        SelectedTilesSorted.Reverse();
        foreach (GameObject item in SelectedTilesSorted)
        {
            item.GetComponent<Tile>().TakeColorFromAbove();
            yield return new WaitForSeconds(fallingSpeedOfTiles);
        }

        //clear lists read for the next match
        SelectedTiles.Clear();
        SelectedTilesSorted.Clear();

        //see if the board needs to be re shuffled
        ShuffleIfNoMatches();


        yield return null;
    }

    /// <summary>
    /// generated the level
    /// </summary>
    private void GenLevel()
    {
        //create the level based on size
        levelTiles = new GameObject[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int ii = 0; ii < columns; ii++)
            {
                GameObject inst = Instantiate(Tile, new Vector2(i, ii), Quaternion.identity);
                Tile instSquare = inst.GetComponent<Tile>();
                levelTiles[i, ii] = inst;
                int rand = Random.Range(0, colorsPoints.Length);
                instSquare.SetUp(i, ii, colorsPoints[rand].color, this, colorsPoints[rand].Point);
            }
        }
    }

    /// <summary>
    /// shuffles each tiles color
    /// </summary>
    public void ShuffleTiles()
    {
        foreach (GameObject item in levelTiles)
        {
            item.GetComponent<Tile>().ShuffleColor();
        }
    }

    /// <summary>
    /// used to add and delete from the list of selected tiles
    /// </summary>
    /// <param name="tile">teh tiles that is being added/deleted</param>
    public void AddToListOfSelectedTiles(GameObject tile)
    {
        if (SelectedTiles.Contains(tile))
        {
            SelectedTiles.Remove(tile);
        }
        else
        {
            SelectedTiles.Add(tile);
        }
    }

    /// <summary>
    /// scans the lvl for at least 1 match 
    /// </summary>
    /// <returns></returns>
    private bool CheckForMatchesInLevel()
    {
        List<GameObject> currentMatchedTiles = new List<GameObject>();
        List<GameObject> matchedList = new List<GameObject>();

        for (int i = 1; i < rows - 1; i++)
        {
            for (int ii = 1; ii < columns - 1; ii++)
            {
                //add current tile to matched list
                matchedList.Add(levelTiles[i, ii]);

                //add all the tiles around the main tile
                currentMatchedTiles.Add(levelTiles[i + 1, ii]);
                currentMatchedTiles.Add(levelTiles[i + 1, ii - 1]);
                currentMatchedTiles.Add(levelTiles[i, ii - 1]);
                currentMatchedTiles.Add(levelTiles[i - 1, ii - 1]);
                currentMatchedTiles.Add(levelTiles[i - 1, ii]);
                currentMatchedTiles.Add(levelTiles[i - 1, ii + 1]);
                currentMatchedTiles.Add(levelTiles[i, ii + 1]);
                currentMatchedTiles.Add(levelTiles[i + 1, ii + 1]);

                //loop through each tile and see if it matches color with the first one
                for (int iii = 0; iii < currentMatchedTiles.Count; iii++)
                {
                    //if the color of the chose tile matches the color of the surrounding tiles
                    if (matchedList[0].GetComponent<Tile>().GetColor() == currentMatchedTiles[iii].GetComponent<Tile>().GetColor())
                    {
                        matchedList.Add(currentMatchedTiles[iii]);
                        if (matchedList.Count == AmountOfMatchedTilesNeeded)
                        {
                            return true;
                        }
                    }
                }

                matchedList.Clear(); //no matches where found in this one
                currentMatchedTiles.Clear(); //clear the tiles were lookign at
            }
        }

        return false;
    }

    /// <summary>
    /// looks at the list of selected tiles to see if there is a match
    /// </summary>
    /// <returns></returns>
    private bool LookForMatchInSelectedTiles()
    {
        bool ListIsGood = true;

        //maker sure there is a minimum number of matches
        if (SelectedTiles.Count < AmountOfMatchedTilesNeeded)
        {
            return false;
        }

        //see if colors match
        Color ChosenColor = SelectedTiles[0].GetComponent<SpriteRenderer>().color;
        foreach (GameObject item in SelectedTiles)
        {
            if (ChosenColor != item.GetComponent<SpriteRenderer>().color)
            {
                return false;
            }
        }

        //are tiles touching
        for (int i = 0; i < SelectedTiles.Count; i++)
        {
            if (i > 0 && ListIsGood == true)
            {
                Transform oldTile = SelectedTiles[i - 1].transform;
                Transform newTile = SelectedTiles[i].transform;

                // i don't like this at all but i just have to check somehow
                if (oldTile.position.x + 1 == newTile.position.x && oldTile.position.y == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x + 1 == newTile.position.x && oldTile.position.y - 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x == newTile.position.x && oldTile.position.y - 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x - 1 == newTile.position.x && oldTile.position.y - 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x - 1 == newTile.position.x && oldTile.position.y == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x - 1 == newTile.position.x && oldTile.position.y + 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x == newTile.position.x && oldTile.position.y + 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else if (oldTile.position.x + 1 == newTile.position.x && oldTile.position.y + 1 == newTile.position.y)
                {
                    ListIsGood = true;
                }
                else
                {
                    ListIsGood = false;
                    continue;
                }
            }
        }

        return ListIsGood;
    }


}
