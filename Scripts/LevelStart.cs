using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelStart: MonoBehaviour
{
  [SerializeField]
  MainGame parentMainGame;

  // Tiles
  public GameObject tileNumber;
  public Tile tileBasic;
  public Sprite square1;
  public Sprite square2;
  public Sprite square3;
  public Sprite square4;

  // Menu panel sprites
  public Sprite mirror1;
  public Sprite mirror2;
  public Sprite mirror3;
  public Sprite mirror4;
  private int tileCorrect_countdown;

  // *Randomly generate a new static level*
  public void LevelGenerate(MainGame.LevelRandom levelRandom)
  {

    // An instantiated static level
    MainGame.LevelStatic levelStatic = new MainGame.LevelStatic
    {
      level = "static",
      height = levelRandom.height,
      width = levelRandom.width,
      tileType = new int[levelRandom.height * levelRandom.width],
      tileCorrect = new int[levelRandom.height * levelRandom.width],
      tileSelected = new int[levelRandom.height * levelRandom.width],
      type1RestrictX = levelRandom.type1RestrictX,
      type1RestrictY = levelRandom.type1RestrictY,
      type2Restict = levelRandom.type2Restict,
      mirror = 1,
      name = "Beginner"
    };

    // Create a Tile Pattern array and random binary number
    int[] tilePattern = new int[levelRandom.height * levelRandom.width];
    int randomNumber = UnityEngine.Random.Range(1, (int)Math.Pow(2, Mathf.Floor(levelRandom.height / 2) * Mathf.Floor(levelRandom.width / 2)) - 2);
    String randomPattern = Convert.ToString(randomNumber, 2);

    // Set the Tile Pattern with the random number Pattern
    for (int i = 0; i < randomPattern.Length; i++)
    {
      tilePattern[i] = Convert.ToInt32(randomPattern.ToCharArray()[i]) - 48;
      //Debug.Log(tilePattern[i]);
    }

    // Mirror 1 Pattern duplication
    if (levelStatic.mirror == 1)
    {
      for (int y = 0; y < Mathf.Floor(levelRandom.height / 2); y++)
      {
        for (int x = 0; x < Mathf.Floor(levelRandom.width / 2); x++)
        {
          int levelActiveCell = (int)(y * Mathf.Floor(levelRandom.width / 2) + x);

          // 4 Quadrant duplication
          if (tilePattern[levelActiveCell] == 1)
          {
            // Bottom left
            levelActiveCell = y * levelRandom.width + x + 1;
            levelStatic.tileCorrect[levelActiveCell] = 1;
            // Bottom right
            levelActiveCell = y * levelRandom.width - x + levelRandom.width - 1;
            levelStatic.tileCorrect[levelActiveCell] = 1;
            // Top left
            levelActiveCell = (-(1 + y - levelRandom.height) * levelRandom.width - levelRandom.height + x + 1);
            levelStatic.tileCorrect[levelActiveCell] = 1;
            // Top right
            levelActiveCell = (-1 + levelRandom.height * levelRandom.width) - ((y + 1) * levelRandom.width + x);
            levelStatic.tileCorrect[levelActiveCell] = 1;
          }
        }
      }
    }

    // Set grid tile type
    for (int y = 0; y < levelStatic.height; y++)
    {
      for (int x = 0; x < levelStatic.width; x++)
      {
        int activeCell = y * levelStatic.width + x;

        if (x == 0 || y == levelStatic.height - 1) { levelStatic.tileType[activeCell] = 1; }
        else { levelStatic.tileType[activeCell] = 2; }
      }
    }

    LevelLoad(levelStatic);
  }

  // *Procedual generation of a static level*
  public void LevelLoad(MainGame.LevelStatic levelStatic)
  {
    parentMainGame.MapClick.ClearAllTiles();
    parentMainGame.MapNumber.ClearAllTiles();


    parentMainGame.UIImage.sprite = null;
    //Debug.Log(levelActive.mirror);
    if (levelStatic.mirror == 1)
    {
      parentMainGame.UIImage.sprite = mirror1;
    }
    else if (levelStatic.mirror == 2)
    {
      parentMainGame.UIImage.sprite = mirror2;
    }
    else if (levelStatic.mirror == 3)
    {
      parentMainGame.UIImage.sprite = mirror3;
    }
    else if (levelStatic.mirror == 4)
    {
      parentMainGame.UIImage.sprite = mirror4;
    }

    String checkStaticType = "";
    String checkStaticCorrect = "";

    // Loop for the coordinates
    for (int y = 0; y < levelStatic.height; y++)
    {
      for (int x = 0; x < levelStatic.width; x++)
      {
        // Reset basic tile
        tileBasic.gameObject = default;

        // Basic Functions

        // The correct x and y index in the array is the height time the max width plus the width since for everything max width you will get a new row
        int activeCell = y * levelStatic.width + x;

        checkStaticType += levelStatic.tileType[activeCell];
        checkStaticCorrect += levelStatic.tileCorrect[activeCell];


        // **MapClick Logic**

        // Set tile sprite setting based of the first column and last row
        if (x == 0 && y == levelStatic.height - 1)
        {
          tileBasic.sprite = square4;
        }
        else if (x == 0 && y != levelStatic.height - 1)
        {
          tileBasic.sprite = square2;
        }
        else if (x != 0 && y == levelStatic.height - 1)
        {
          tileBasic.sprite = square3;
        }
        else
        {
          tileBasic.sprite = square1;
        }

        // Centre the tiles rather than using the top right quadrant
        int xCentre = x - levelStatic.width / 2;
        int yCentre = y - levelStatic.height / 2;

        // This is another instantiate but all the changes need be in place before its creation
        parentMainGame.MapClick.SetTile(new Vector3Int(xCentre, yCentre, 0), tileBasic);

        // Checking levelActive for a currently selected tile, will be used in save files
        if (levelStatic.tileSelected[activeCell] == 1)
        {
          parentMainGame.SetTileColour(new Color(200, 0, 0), new Vector3Int(xCentre, yCentre, 0));
        }

        // Reset basic tile
        tileBasic.sprite = null;


        // **MapNumber Logic**

        // Create a new asset inside the game to break its reference to the origin
        tileBasic.gameObject = Instantiate(tileNumber, new Vector3(-99, 0, 0), Quaternion.identity);

        // Prefab can now be instanced without changing the other tiles prefabs
        TextMeshPro number_value = tileBasic.gameObject.GetComponentInChildren<TextMeshPro>();

        // Logic for tile type 1
        if (levelStatic.tileType[activeCell] == 1)
        {
          int tileNumberCorrect = 0;
          if (x == 0)
          {
            for (int xmax = 0; xmax < levelStatic.width; xmax++)
            {
              int logic = activeCell + xmax;
              if (0 < logic && logic < levelStatic.width * levelStatic.height)
              {
                //Debug.Log(logic);
                if (levelStatic.tileCorrect[logic] == 1) { tileNumberCorrect++; }
              }
            }
          }
          if (y == levelStatic.height - 1)
          {
            for (int ymax = 0; ymax < levelStatic.height; ymax++)
            {
              
              int logic = activeCell - ymax * levelStatic.width;
              if (0 < logic && logic <= levelStatic.width * levelStatic.height)
              {
                //Debug.Log(logic);
                if (levelStatic.tileCorrect[logic] == 1) { tileNumberCorrect++; }
              }
            }
          }
          //Applies the static level type 1 restrictions
          if (Array.Exists<int>(levelStatic.type1RestrictX, element => element.Equals(tileNumberCorrect)) &&
          Array.Exists<int>(levelStatic.type1RestrictY, element => element.Equals(tileNumberCorrect))) { tileNumberCorrect = 0; }
          //Debug.Log(tileNumberCorrect);

          number_value.text = tileNumberCorrect.ToString();
        }

        // Logic for tile type 2
        if (levelStatic.tileType[activeCell] == 2)
        {
          int tileNumberCorrect = 0;
          for (int xdif = -1; xdif < 2; xdif++)
          {
            for (int ydif = -1; ydif < 2; ydif++)
            {
              int logic = (y + ydif) * levelStatic.width + x + xdif;
              if (0 < logic && logic < levelStatic.width * levelStatic.height)
              {
                if (levelStatic.tileCorrect[logic] == 1) { tileNumberCorrect++; }
              }
            }
          }
          // Applies the static level type 2 restictions
          //if (Array.Exists<int>(levelStatic.type2Restict, element => element.Equals(tileNumberCorrect))) { tileNumberCorrect = 0; }

          number_value.text = tileNumberCorrect.ToString();
        }

        // Logic for tile type 3
        if (levelStatic.tileType[activeCell] == 3)
        {
          tileCorrect_countdown = 0;
          for (int y_count = 0; y_count < levelStatic.height; y_count++)
          {
            for (int x_count = 0; x_count < levelStatic.width; x_count++)
            {
              int logic = y_count * levelStatic.width + x_count;
              //Debug.Log(logic);
              if (levelStatic.tileCorrect[logic] == 1) { tileCorrect_countdown++; }
            }
          }
          number_value.text = tileCorrect_countdown.ToString();
        }

        // Logic for tile type 4
        if (levelStatic.tileType[activeCell] == 4)
        {
          //number_value.text = "?";
        }

        // Turns zeros to question marks
        if (number_value.text == "0")
        { 
          //number_value.text = "?";
        }

        parentMainGame.MapNumber.SetTile(new Vector3Int(xCentre, yCentre, 0), tileBasic);

        parentMainGame.levelActive = levelStatic;

        // Destroy the original object after the launch
        Destroy(tileBasic.gameObject, 1);
      };
    };

    Debug.Log(checkStaticType);
    Debug.Log(checkStaticCorrect);
    Debug.Log("Type" + checkStaticType.Length + " Correct" + checkStaticCorrect.Length);
  }
}
