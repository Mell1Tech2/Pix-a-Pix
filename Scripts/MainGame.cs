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

  public Image UIImage;

  [Serializable]
  private class Level
  {
    public int width;
    public int height;
    public int[] tileType;
    public int[] tileCorrect;
    public int[] tileSelected;
    public int mirror;
    public string name;
  }
  Level level = new Level();

  // Level active or not
  private bool levelState;
  // Game active or not
  private bool gameState;

  // Current Level number
  private int levelNumber_current;

  // Correct tile countdown
  private int tileCorrect_countdown;

  //private DirectoryInfo mapDirectory;
  private String[] maps;

  // Main game Tilemap
  public Tilemap MapClick;
  public Tilemap MapNumber;

  public GameObject tileNumber;
  public Tile tileBasic;
  public Sprite square1; 
  public Sprite square2;
  public Sprite square3;
  public Sprite square4;

  public Sprite mirror1;
  public Sprite mirror2;
  public Sprite mirror3;
  public Sprite mirror4;
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
  public TextAsset map11;
  public TextAsset map12;
  public TextAsset map13;
  public TextAsset map14;
  public TextAsset map15;

  void Start()
  {
    // Current level set default is 0 
    levelNumber_current = 0;

    // Load map directory and get all map files
    //mapDirectory = new DirectoryInfo(Application.persistentDataPath);

    maps = new String[15];

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
    maps[10] = map11.text;
    maps[11] = map12.text;
    maps[12] = map13.text;
    maps[13] = map14.text;
    maps[14] = map15.text;

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

    // Set states
    levelState = false;
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
      MapClick.gameObject.SetActive(true);
      MapNumber.gameObject.SetActive(true);
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
  }

  // Manipulate Level
  public void LevelLoad(int l) 
  { 
    MapClick.ClearAllTiles();
    MapNumber.ClearAllTiles();

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

    UIImage.sprite = null;
    //Debug.Log(level.mirror);
    if (level.mirror == 1)
    {
      UIImage.sprite = mirror1;
    }
    else if(level.mirror == 2)
    {
      UIImage.sprite = mirror2;
    }
    else if(level.mirror == 3)
    {
      UIImage.sprite = mirror3;
    }
    else if(level.mirror == 4)
    {
      UIImage.sprite = mirror4;
    }

    // Loop for the coordinates
    for (int y = 0; y<level.height; y++)
    {
      for (int x = 0; x<level.width; x++)
      {
        // Reset basic tile
        tileBasic.gameObject = default;

        // Basic Functions

        // The correct x and y index in the array is the height time the max width plus the width since for everything max width you will get a new row
        int levelCell = y * level.width + x;

        // MapClick Logic

        // Set tile sprite setting based of the first column and last row
        if (x == 0 && y == level.height - 1)
        {
          tileBasic.sprite = square4;
        }
        else if (x == 0 && y != level.height - 1)
        {
          tileBasic.sprite = square2;
        }
        else if (x != 0 && y == level.height - 1)
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
        MapClick.SetTile(new Vector3Int(xCentre, yCentre, 0), tileBasic);

        // Checking level for a currently selected tile, will be used in save files
        if (level.tileSelected[levelCell] == 1)
        {
          SetTileColour(Color.red, new Vector3Int(xCentre, yCentre, 0));
        }

        // Reset basic tile
        tileBasic.sprite = null;


        // MapNumber Logic

        // Create a new asset inside the game to break its reference to the origin
        tileBasic.gameObject = Instantiate(tileNumber, new Vector3(-99, 0, 0), Quaternion.identity);

        // Prefab can now be instanced without changing the other tiles prefabs
        TextMeshPro number_value = tileBasic.gameObject.GetComponentInChildren<TextMeshPro>();

        // Logic for tile type 1
        if (level.tileType[levelCell] == 1)
        {
          int tileNumberCorrect = 0;
          for (int xmax = 0; xmax < level.width; xmax++)
          {
            int logic = levelCell + xmax;
            if (0 < logic && logic < level.width * level.height)
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
          number_value.text = tileNumberCorrect.ToString();
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
          number_value.text = tileNumberCorrect.ToString();
        }

        // Logic for tile type 3
        if (level.tileType[levelCell] == 3)
        {
          tileCorrect_countdown = 0;
          for (int y_count = 0; y_count < level.height; y_count++)
          {
            for (int x_count = 0; x_count < level.width; x_count++)
            {
              int logic = y_count * level.width + x_count;
              //Debug.Log(logic);
              if (level.tileCorrect[logic] == 1) { tileCorrect_countdown++; }
            }
          }
          number_value.text = tileCorrect_countdown.ToString();
        }

        // Logic for tile type 4
        if (level.tileType[levelCell] == 4)
        {
          number_value.text = "?";
        }


        MapNumber.SetTile(new Vector3Int(xCentre, yCentre, 0), tileBasic);

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
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
    }

    // Is the mouse button down and not up
    if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0) || !gameState)
    {
      // Check to see if game is paused
      if (!gameState)
      {
        levelState = false;
        UIMenuMain.gameObject.SetActive(true);
        MapClick.gameObject.SetActive(false);
        MapNumber.gameObject.SetActive(false);
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
        if (MapClick.GetColor(tileVector_int) == Color.white) 
        {
          SetTileColour(new Color(200,0,0), tileVector_int);
          level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 1;
          // Debug.Log(tileVector_int);
          tileCorrect_countdown++;
        }
        else
        {
          SetTileColour(Color.white, tileVector_int);
          level.tileSelected[LevelArrayIndex_create(tileVector_int)] = 0;
          // Debug.Log(tileVector_int);
          tileCorrect_countdown--;
        }
         
        //Tile tileUpdate = MapNumber.GetTile<Tile>(new Vector3Int(0, level.height - 1, 0));

        //string countdown = tileUpdate.gameObject.GetComponent<TextMeshPro>().text;

        //Debug.Log(countdown);

        //MapNumber.SetTile(new Vector3Int(0, level.height - 1, 0), tileUpdate);
      }
    }
  }

   // Basic functions
  Vector3Int TileVector_get(Vector3 levelPos)
  {
    // Return the tile that is clicked
    Vector3Int tilePos = MapClick.WorldToCell(levelPos);

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
    MapClick.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    MapClick.SetColor(position, colour);
  }
}