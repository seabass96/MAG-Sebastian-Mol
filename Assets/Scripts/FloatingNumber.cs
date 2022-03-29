using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingNumber : MonoBehaviour
{
    [Tooltip("The direction the number will move")]
    public Vector2 movemnetVector;
    [Tooltip("The speed the number will move at")]
    public float movmentSpeed;
    public float points; // used to set the text to match the points of the tile
    [Tooltip("The speed at which the number will fade out")]
    public float FadeSpeed;
    [Tooltip("The color of the number")]
    public Color textcol;
    public Text text; //reference to the text itself

    /// <summary>
    /// logic for the numbers movement
    /// </summary>
    private void Movement()
    {
        transform.Translate(movemnetVector * movmentSpeed);
    }

    /// <summary>
    /// logic for the number fading
    /// </summary>
    private void Fade()
    {
        textcol.a -= FadeSpeed * Time.deltaTime;
        text.color = textcol;
        if (textcol.a <= 0) Destroy(gameObject);
    }

    /// <summary>
    /// set up for the number
    /// </summary>
    private void Awake()
    {
        text = transform.GetChild(0).GetComponent<Text>();
        text.color = textcol;
        text.text = points.ToString();
    }

    private void Update()
    {
        Movement();
        Fade();
    }
}
