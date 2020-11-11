using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Tile = UnityEngine.Tilemaps.Tile;

public class MainGame : MonoBehaviour
{ 

  public Camera UICameraMain;

  public Canvas UIMenu;
  public RectTransform UIMenuMain; 
  public RectTransform UIMenuSelect;
  public RectTransform UIMenuSelectPanel;
  public RectTransform UIMenuPopup; 

  public Button UIButtonPopuptoStart;
  public Button UIButtonPopuptoMain;
  public Button UIButtonPopuptoSelect;
  public Button UIButtonMaintoStart;
  public Button UIButtonMaintoSelect;
  public Button UIButtonLeveltoMain;
  public Button UIButtonGametoMain;
  public GameObject UIButtonSelect;

  // the decorated level will be put in this object
  [Serializable]
  private class Level
  {
    public int width;
    public int height;
    public int[] tileType;
    public int[] tileCorrect;
    public int[] tileSelected;
    public string name;
  }
  Level level_json = new Level();
  Level level = new Level();
  //Level complete or not
  private bool levelActive_state;
  //Level paused or not
  private bool gameActive_state;

  private DirectoryInfo mapDirectory;
  private FileInfo[] maps;

  // Main game Tilemap
  public Tilemap GameTilemap_clickable;

  public GameObject tileNumber;
  public Tile tileBasic;
  public Sprite square1;
  public Sprite square2;
  public Sprite square3;
  // Tilemap tiles objects

  void Start()
  {
    // Main menu button listeners
    UIButtonMaintoStart.onClick.AddListener(delegate { LevelLoad(0); });
    UIButtonMaintoSelect.onClick.AddListener(UISelect_transition); 

    // Select level menu btton listeners

    mapDirectory = new DirectoryInfo(Application.dataPath + "/Scripts/Maps");
    maps = mapDirectory.GetFiles();
    List<GameObject> goButton_list = new List<GameObject>();

    for (int l = 0; l<maps.Length; l++) 
    {
      if (maps[l].Extension.Contains("json"))
      {
        //l /= 2;
        //Debug.Log(maps[0]);
        GameObject goButton = Instantiate(UIButtonSelect);
        goButton_list.Add(goButton);
        goButton_list[l].GetComponentInChildren<Button>().onClick.AddListener(delegate { LevelLoad(l); });
        goButton_list[l].transform.SetParent(UIMenuSelectPanel, false); 
      }
    }
    UIButtonLeveltoMain.onClick.AddListener(UIMainMenu_transition);

    // Popup menu button listeners
    UIButtonPopuptoStart.onClick.AddListener(delegate { LevelLoad(0); });
    UIButtonPopuptoMain.onClick.AddListener(UIMainMenu_transition);
    UIButtonPopuptoSelect.onClick.AddListener(UISelect_transition);

    UIButtonGametoMain.onClick.AddListener(StateGame_pause);

    // Transition screens
    UIMenuMain.gameObject.SetActive(true);
    UIMenuSelect.gameObject.SetActive(false);
    UIMenuPopup.gameObject.SetActive(false);
    UIButtonGametoMain.gameObject.SetActive(false);
    levelActive_state = false;
  }

  void Update()
  {
    //Debug.Log(levelActive_state);
    if (levelActive_state == true)
    {
      GameTilemap_clickable.gameObject.SetActive(true);
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(true);
      LevelUpdate();
    }
  }

  // Basic functions
  Vector3Int TileVector_get(Vector3 levelPos)
  {
    // Return the tile that is clicked
    Vector3Int tilePos = GameTilemap_clickable.WorldToCell(levelPos);
    
    // Make sure its in the game screen but also exclude the left coloumn and top row from selection, it is adjusted centering it like a graph
    if (tilePos.x >= level.width / 2 || tilePos.y >= level.height / 2 - 1 || tilePos.x < -level.width / 2 + 1 || tilePos.y < -level.height / 2)
    {
      // An unreachable tile
      tilePos = new Vector3Int(0, 0, -1);
    }

    // Return it as an int vector, this is needed to change colors
    return tilePos;
  }
  int LevelArrayIndex_create(Vector3 tilePos)
  {
    // Turning all negative tile positions into positive
    tilePos.x += level.width / 2;
    tilePos.y = (tilePos.y + level.height / 2) * level.width;

    int levelIndex = (int)(tilePos.x + tilePos.y);

    // Returning the index of the level
    return levelIndex;
  }

  // UI Transitions
  public void UIMainMenu_transition()
  {
    UIMenuMain.gameObject.SetActive(true);
    UIMenuSelect.gameObject.SetActive(false);
    UIMenuPopup.gameObject.SetActive(false);
  }
  public void UISelect_transition()
  {
    UIMenuMain.gameObject.SetActive(false);
    UIMenuSelect.gameObject.SetActive(true);
    UIMenuPopup.gameObject.SetActive(false);
  }
  public void StateGame_pause()
  {
    gameActive_state = false;
  }
  // levelActive_state = false;

  // Manipulate Level
  public void LevelLoad(int l)
  {
    Debug.Log(l);

    //fis = mapDirectory.GetFiles();

    // Read the json file into a local string 
    string json = File.ReadAllText(maps[l].ToString());
    // Debug.Log(json);

    // Build json string into an object
    level = JsonUtility.FromJson<Level>(json);

    // Sets the main camera  to the same size at the level height
    UICameraMain.orthographicSize = level.height / 2;
    //view.orthographicSize = 20;

    // Loop for the coordinates
    for (int y = 0; y<level.height; y++)
    {
      for (int x = 0; x<level.width; x++)
      {
        // The correct x and y index in the array is the height time the max width plus the width since for everything max width you will get a new row
        int levelCell = y * level.width + x;

        // Create a new asset inside the game to break its reference to the origin
        tileBasic.gameObject = Instantiate(tileNumber, new Vector3(0, 0, -20), Quaternion.identity);

        // Prefab can now be instanced without changing the other tiles prefabs
        TextMesh number = tileBasic.gameObject.GetComponentInChildren<TextMesh>();

        // Logic for tile type 1
        if (level.tileType[levelCell] == 1)
        {
          int tileNumberCorrect = 0;
          for (int xmax = 0; xmax<level.width; xmax++)
          {
            int logic = levelCell + xmax;
            if (0 < logic && logic<level.width* level.height)
            {
              //Debug.Log(logic);
              if (level.tileCorrect[logic] == 1) { tileNumberCorrect++; }
            }
          }
          for (int ymax = 0; ymax < level.height; ymax++)
          {
            int logic = levelCell - ymax * level.width;
            if (0 < logic && logic <= level.width * level.height)
            {
              //Debug.Log(logic);
              if (level.tileCorrect[logic] == 1) { tileNumberCorrect++; }
            }
          }
          number.text = tileNumberCorrect.ToString();
        }

        // Logic for tile type 2
        if (level.tileType[levelCell] == 2)
        {
          int tileNumberCorrect = 0;
          for (int xdif = -1; xdif < 2; xdif++)
          {
            for (int ydif = -1; ydif < 2; ydif++)
            {
              int logic = (y + ydif) * level.width + x + xdif;
              if (0 < logic && logic < level.width * level.height)
              {
                if (level.tileCorrect[logic] == 1) { tileNumberCorrect++; }
              }
            }
          }
          number.text = tileNumberCorrect.ToString();
        }

        // Set tile sprite setting based of the first column and last row
        if (x == 0 && y != level.height - 1)
        {
          tileBasic.sprite = square2;
        }
        else if (y == level.height - 1 && x != 0)
        {
          tileBasic.sprite = square3;
        }
        else
        {
          tileBasic.sprite = square1;
        }

        // Centre the tiles rather than using the top right quadrant
        int xCentre = x - level.width / 2;
        int yCentre = y - level.height / 2;

        // This is another instantiate but all the changes need be in place before its creation
        GameTilemap_clickable.SetTile(new Vector3Int(xCentre, yCentre, 0), tileBasic);

        // Checking level for a currently selected tile, will be used in save files
        if (level.tileSelected[levelCell] == 1)
        {
          SetTileColour(Color.red, new Vector3Int(xCentre, yCentre, 0));
        }

        // Destroy the original object after the launch
        Destroy(tileBasic.gameObject, 1);
      };
    };
    levelActive_state = true;
    gameActive_state = true;
  }
  private void LevelUpdate()
  {
    // Is the mouse button down and not up
    if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0))
    {
      // Mouse position to camera position 
      Vector3 mouse_pos = Input.mousePosition;
      mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

      // Return tileMap x and y (x, y, z) based on the clicked tile position
      Vector3Int tileVector_int = TileVector_get(mouse_pos);
      //Debug.Log(idClick);

      // Set color to red if tile is white else color is white
      if (GameTilemap_clickable.GetColor(tileVector_int) == Color.white)
      {
        SetTileColour(Color.red, tileVector_int);
        level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 1;
      }
      else
      {
        SetTileColour(Color.white, tileVector_int);
        level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 0;
      }
    }

    // Check if you Won by comparing correct tiles with selected tiles, else save the game
    if (level.tileSelected.SequenceEqual(level.tileCorrect))
    {
      // Reset the map local save back to 0
      for (int i = 0; i < level.height * level.width; i++)
      {
        level.tileSelected[i] = 0;
      }
      string json = JsonUtility.ToJson(level);
      File.WriteAllText(UnityEngine.Application.dataPath + "/Scripts/Maps/Map1.json", json);
      // Unload level and show popup menu
      levelActive_state = false;
      UIMenuPopup.gameObject.SetActive(true);
      GameTilemap_clickable.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
    }
    // check to see if game is paused
    else if (gameActive_state == false)
    {
      levelActive_state = false;
      UIMenuMain.gameObject.SetActive(true);
      GameTilemap_clickable.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
    }
    else
    {
      string json = JsonUtility.ToJson(level);
      File.WriteAllText(UnityEngine.Application.dataPath + "/Scripts/Maps/Map1.json", json);
      // Debug.Log("Not Won");
    }
  }

  // Manipulate TileMaps
  private void SetTileColour(Color colour, Vector3Int position)
  {
    // Flag the tile, inidicating that it can change colour.
    GameTilemap_clickable.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    GameTilemap_clickable.SetColor(position, colour);
  }
}