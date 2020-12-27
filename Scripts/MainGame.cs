using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Tile = UnityEngine.Tilemaps.Tile;

public class MainGame : MonoBehaviour
{ 

  public Camera CameraMain;

  public Canvas ScreenMain;
  public RectTransform UIGrid;
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
  Level level = new Level();
  // Level active or not
  private bool levelState;
  // Game active or not
  private bool gameState;

  private bool gameState_started;
  // Current Level number
  private int levelNumber_current;

  //private DirectoryInfo mapDirectory;
  private String[] maps;

  // Main game Tilemap
  public Tilemap GameTilemap_clickable;

  public GameObject tileNumber;
  public Tile tileBasic;
  public Sprite square1; 
  public Sprite square2;
  public Sprite square3;
  // Tilemap tiles objects

  public TextAsset map1;
  public TextAsset map2;
  public TextAsset map3;
  public TextAsset map4;
  public TextAsset map5;
  public TextAsset map6;
  public TextAsset map7;
  public TextAsset map8;
  public TextAsset map9;
  public TextAsset map10;

  void Start()
  {
    // Current level set default is 0 
    levelNumber_current = 0;

    // Load map directory and get all map files
    //mapDirectory = new DirectoryInfo(Application.persistentDataPath);

    maps = new String[10];

    maps[0] = map1.text;
    maps[1] = map2.text;
    maps[2] = map3.text;
    maps[3] = map4.text;
    maps[4] = map5.text;
    maps[5] = map6.text;
    maps[6] = map7.text;
    maps[7] = map8.text;
    maps[8] = map9.text;
    maps[9] = map10.text;

    //mapDirectory.GetFiles();

    // Main menu button listeners
    UIButtonMaintoStart.onClick.AddListener(delegate { LevelLoad(levelNumber_current); });
    UIButtonMaintoSelect.onClick.AddListener(UISelect_transition); 

    // Select level menu button listeners
    for (int i = 0; i<maps.Length; i++)
    {
      // Encapsulate the level number to break dependency
      int l = i;
      // Read the json file into a local string 
      string json = maps[l];

      // Build json string into an object
      level = JsonUtility.FromJson<Level>(json);

      // Prefab button added to the select menu panel
      GameObject goButton = Instantiate(UIButtonSelect, UIMenuSelectPanel);
      Button btn = goButton.GetComponent<Button>();
      btn.GetComponentInChildren<Text>().text = level.name;
      btn.GetComponentInChildren<TextMeshProUGUI>().text = (l + 1).ToString();
      btn.onClick.AddListener(delegate { levelNumber_current = l; LevelLoad(l); });
    }
    UIButtonLeveltoMain.onClick.AddListener(UIMainMenu_transition);

    // Popup menu button listeners
    UIButtonPopuptoStart.onClick.AddListener(delegate { levelNumber_current += 1; LevelLoad(levelNumber_current); });
    UIButtonPopuptoMain.onClick.AddListener(UIMainMenu_transition);
    UIButtonPopuptoSelect.onClick.AddListener(UISelect_transition);

    // Ingame menu
    UIButtonGametoMain.onClick.AddListener(StateGame_pause);

    // Transition screens
    UIMenuMain.gameObject.SetActive(true);
    UIMenuSelect.gameObject.SetActive(false);
    UIMenuPopup.gameObject.SetActive(false);
    UIButtonGametoMain.gameObject.SetActive(false);

    // set states
    levelState = false;
    gameState_started = false;
  }

  void Update()
  {
    //Debug.Log(levelActive_state);
    if (levelState == true)
    {
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(true);
      GameTilemap_clickable.gameObject.SetActive(true);
      LevelUpdate();
    }
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
    gameState = false;
    gameState_started = false;
  }

  // Manipulate Level
  public void LevelLoad(int l) 
  {
    GameTilemap_clickable.ClearAllTiles();

    //Debug.Log(l);

    // Read the json file into a local string 
    string json = maps[l];
    // Debug.Log(json);

    // Build json string into an object
    level = JsonUtility.FromJson<Level>(json);

    // Sets the main camera  to the same size at the level height
    CameraMain.orthographicSize = (float)level.height * 5 / 6; 

    //Debug.Log(ScreenMain.GetComponent<RectTransform>().rect.width);
    //Debug.Log(ScreenMain.GetComponent<RectTransform>().rect.height);

    UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2, -ScreenMain.GetComponent<RectTransform>().rect.height / 2, 0);
    if (level.width % 2 == 1)
    {
      UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2 - 0.5f, -ScreenMain.GetComponent<RectTransform>().rect.height / 2 - 0.5f, 0);
    }

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
    levelState = true;
    gameState = true;
  }

  private void LevelUpdate()
  {

    // Check if you Won by comparing correct tiles with selected tiles, else save the game
    if (level.tileSelected.SequenceEqual(level.tileCorrect))
    {
      // Reset the map local save back to 0
      for (int i = 0; i < level.height * level.width; i++)
      {
        level.tileSelected[i] = 0;
      }
      string json = JsonUtility.ToJson(level);
      maps[levelNumber_current] = json;
      // Unload level and show popup menu
      levelState = false;
      UIMenuPopup.gameObject.SetActive(true);
      GameTilemap_clickable.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
    }
    // Is the mouse button down and not up
    if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0) || !gameState)
    {
      // check to see if game is paused
      if (!gameState)
      {
        levelState = false;
        UIMenuMain.gameObject.SetActive(true);
        GameTilemap_clickable.gameObject.SetActive(false);
        UIButtonGametoMain.gameObject.SetActive(false);
      }
      else
      {
        string json = JsonUtility.ToJson(level);
        maps[levelNumber_current] = json;
      }
      // Mouse position to camera position 
      Vector3 mouse_pos = Input.mousePosition;
      mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

      // Return tileMap x and y (x, y, z) based on the clicked tile position
      Vector3Int tileVector_int = TileVector_get(mouse_pos);

      // Set color to red if tile is white else color is white
      if (tileVector_int.z == 0) { 
        if (GameTilemap_clickable.GetColor(tileVector_int) == Color.white) 
        {
          SetTileColour(new Color(200,0,0), tileVector_int);
          level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 1;
          // Debug.Log(tileVector_int);
        }
        else
        {
          SetTileColour(Color.white, tileVector_int);
          level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 0;
          // Debug.Log(tileVector_int);
        }
      }
    }
  }

   // Basic functions
  Vector3Int TileVector_get(Vector3 levelPos)
  {
    // Return the tile that is clicked
    Vector3Int tilePos = GameTilemap_clickable.WorldToCell(levelPos);

    // A number to remove the top and left border of the grid from tile selection
    int borderY = 1;
    int borderX = 1;
    if (level.height % 2 == 1)
    {
      borderY = (int)0.5;
      borderX = (int)1.5;
    }

    // Make sure it's in the game screen but also exclude the left column and top row from selection, it is adjusted centering it like a graph
    if (tilePos.x >= level.width / 2 + borderX || tilePos.y >= level.height / 2 - borderY || tilePos.x < -level.width / 2 + borderX || tilePos.y < -level.height / 2)
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

  // Manipulate TileMaps
  private void SetTileColour(Color colour, Vector3Int position)
  {
    // Flag the tile, inidicating that it can change colour.
    GameTilemap_clickable.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    GameTilemap_clickable.SetColor(position, colour);
  }
}