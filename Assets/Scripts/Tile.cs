using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Tooltip("the speed the tile rotates at when it is begin destroyed")]
    public float DeathRotateSpeed;
    [Tooltip("the speed at which the tile fades out")]
    public float DeathColorFadeSpeed;
    [Tooltip("how long before the tile is destroyed")]
    public float durationOfDeath;
    [Tooltip("the alpha of the highlight color")]
    public float HighlightAlpha = 0.4f;
    [Tooltip("the color of the tiles highlight")]
    public Color HighlightColor;
    [Tooltip("reference to the object that gets spawned when the tile is destroyed")]
    public GameObject floatingPoints;
    [Tooltip("reference to the sound that plays when the tile is destroyed")]
    public GameObject destroySound;

    private float timer;//used to create a timer that controls the animation 
    private Color color; //the color of the tile
    private int posX; //tiles x position
    private int posY; //tiles y position
    private bool pressed = false; //keeps track if the tile has been selected or not
    private bool isDestroyed; //keeps track of whether the tile has been destroyed or not
    private GameManager manager; //reference to the game manager

    internal int points; //how many points the tile is worth

    /// <summary>
    /// initialize the tile
    /// </summary>
    /// <param name="posX">its x coord</param>
    /// <param name="posY">its y coord</param>
    /// <param name="color"> its color </param>
    /// <param name="manager">reference to the manager</param>
    /// <param name="points">how many points the tile is worth</param>
    public void SetUp(int posX, int posY, Color color, GameManager manager, int points)
    {
        //set up variables
        this.posX = posX;
        this.posY = posY;
        this.color = color;
        this.manager = manager;
        this.points = points;

        //apply changes
        GetComponent<SpriteRenderer>().color = color;

        //general set up
        isDestroyed = false;
        timer = durationOfDeath;
        pressed = false;
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseEnter()
    {
        if (!isDestroyed) SelectTile();
    }

    private void OnMouseExit()
    {
        if (!pressed) UnselectTile();
    }

    private void OnMouseDown()
    {
        if (!isDestroyed)
        {
            pressed = !pressed; //toggle
            manager.AddToListOfSelectedTiles(this.gameObject);//add to list or remove from list
        }         
    }

    /// <summary>
    /// graphical actions that happen when the tile is selected 
    /// </summary>
    public void SelectTile()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(HighlightColor.r, HighlightColor.g, HighlightColor.b, HighlightAlpha);
    }

    /// <summary>
    /// graphical actions that happen when the tile is unselected 
    /// </summary>
    public void UnselectTile()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        pressed = false;
    }


    /// <summary>
    /// actions that happen when the tile is destroyed 
    /// </summary>
    public void DestroyTile()
    {
        isDestroyed = true;
        InstanciateFloatingNumber();
        PlaySound();
    }

    private void PlaySound()
    {
        GameObject sound = Instantiate(destroySound);
        Destroy(sound, 1);
    }

    /// <summary>
    /// create a floating number that represents how many points the tile was worth
    /// </summary>
    private void InstanciateFloatingNumber()
    {
        GameObject inst = Instantiate(floatingPoints, transform.position, Quaternion.identity);
        inst.GetComponent<FloatingNumber>().points = points;
        inst.SetActive(true);
    }

    /// <summary>
    /// looks at the tile above this one and takes its color
    /// </summary>
    public void TakeColorFromAbove()
    {
        if(isDestroyed)
        {
            //look above
            //if no tile above gen random color
            if ((int)transform.position.y + 1 > manager.columns - 1)
            {
                ShuffleColor();
            }
            else
            {
                //take color
                Tile aboveTile = manager.levelTiles[(int)transform.position.x, (int)transform.position.y + 1].GetComponent<Tile>();
                SetUp(posX, posY, aboveTile.color, manager, aboveTile.points);

                //destroy above tile
                manager.levelTiles[(int)transform.position.x, (int)transform.position.y + 1].GetComponent<Tile>().QuickDestroy();
                
            }
        }
    }

    /// <summary>
    /// destroys the tile quickly this is used when moving the tiles down
    /// </summary>
    public void QuickDestroy()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        isDestroyed = true;
        TakeColorFromAbove();
    }

    /// <summary>
    /// random gen a new color for the tile
    /// </summary>
    public void ShuffleColor()
    {
        TileInfo info = manager.colorsPoints[Random.Range(0, manager.colorsPoints.Length)];
        SetUp(posX, posY, info.color, manager, info.Point);
    }

    /// <summary>
    /// returns the color of the tile
    /// </summary>
    /// <returns>the color</returns>
    public Color GetColor()
    {
        return color;
    }

    private void Update()
    {
        if(isDestroyed)
        {
            if(timer <= 0)
            {
                GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
            else
            {
                timer -= Time.deltaTime;
                transform.Rotate(new Vector3(0, 0, DeathRotateSpeed * Time.deltaTime));
                GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, GetComponent<SpriteRenderer>().color.a - (DeathColorFadeSpeed * Time.deltaTime));
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
        }

    }
}
