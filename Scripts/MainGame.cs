using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
  // *Classes*

  // Parent class reference for children
  public static MainGame Instance;

  // Child classes
  [SerializeField]
  internal LevelStart childLevelStart;
  [SerializeField]
  internal LevelUpdate childLevelUpdate;

  // *Unity game object reference*

  // Main Camera
  public Camera CameraMain; 

  // Main Screen
  public Canvas ScreenMain;

  // All the different Screeens
  public RectTransform UIMenuMain; 
  public RectTransform UIMenuSelect;
  public RectTransform UIMenuSelectPanel;
  public RectTransform UIMenuPopup;
  public RectTransform UIGrid;
  public RectTransform UIScreenDialogue;
  public RectTransform UIGameInterface;

  // Transition Buttons
  public Button UIButtonPopuptoStart;
  public Button UIButtonPopuptoMain;
  public Button UIButtonPopuptoSelect;
  public Button UIButtonMaintoStart;
  public Button UIButtonMaintoSelect;
  public Button UIButtonLeveltoMain;
  public Button UIButtonGametoMain;
  public GameObject UIButtonSelect;

  // ingame UI panel
  public Image UIImage;

  // *Class objects*

  // Random level object
  [Serializable]
  public class LevelRandom
  {
    public string level;
    public int width;
    public int height;
    public int[] mirrorOptions;
    public int[] tileCorrectRestrict;
    public int[] type1RestrictX;
    public int[] type1RestrictY;
    public int[] type2Restict;
    public string name;
  }
  LevelRandom levelRandom = new LevelRandom();

  // Static level object
  [Serializable]
  public class LevelStatic
  {
    public string level;
    public int width;
    public int height;
    public int[] tileType;
    public int[] tileCorrect;
    public int[] tileSelected;
    public int mirror;
    public int[] type1RestrictX;
    public int[] type1RestrictY;
    public int[] type2Restict;
    public string name;
  }
  public LevelStatic levelActive = new LevelStatic();

  // State Enumerations 

  // Game active or not
  public enum GameState
  {
    Game,
    Menu
  }
  public GameState gameState;

  public enum MenuState
  {
    Main,
    Select,
    Popup,
    Game
  }
  public MenuState menuState;

  public enum DialogueState
  {
    Closed,
    Open,
    Listen,
    Writing
  }
  public DialogueState dialogueState;

  public enum MouseTileState
  {
    Wait,
    UnSelect,
    Select
  }
  public MouseTileState mouseTileState;

  // Run time state that when changed will execute special code in the update
  public enum RunState
  {
    Transition,
    Menu,
    Game
  }
  public RunState runState;

  // Game mouse state
  [NonSerialized]
  public int mouseState;

  // Current Level number
  [NonSerialized]
  public int levelNumber_current;

  // Correct tile countdown
  [NonSerialized]
  public int tileCorrect_countdown;

  // Decoded json maps as string array
  [NonSerialized]
  public String[] maps;

  // Tilemaps
  public Tilemap MapClick;
  public Tilemap MapNumber;

  // Random Tilemaps
  public TextAsset mapRandom1;
  public TextAsset mapRandom2;
  public TextAsset mapRandom3;
  public TextAsset mapRandom4;

  // Static Tilemaps
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

    maps = new String[4];

    maps[0] = mapRandom1.text;
    maps[1] = mapRandom2.text;
    maps[2] = mapRandom3.text;
    maps[3] = mapRandom4.text;

    /*maps[0] = map1.text;
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
    maps[14] = map15.text;*/

    //mapDirectory.GetFiles();

    // Main menu button listeners
    UIButtonMaintoStart.onClick.AddListener(delegate { LevelStart_init(levelNumber_current); gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; dialogueState = DialogueState.Open; }); 
    UIButtonMaintoSelect.onClick.AddListener(delegate { gameState = GameState.Menu; ; menuState = MenuState.Select; runState = RunState.Transition; }); 

    // Select level menu button listeners
    for (int i = 0; i<maps.Length; i++)
    {
      // Encapsulate the level number to break dependency
      int l = i;
      // Read the json file into a local string 
      string json = maps[l];

      // Build json string into an object
      levelRandom = JsonUtility.FromJson<LevelRandom>(json);

      // Prefab button added to the select menu panel
      GameObject goButton = Instantiate(UIButtonSelect, UIMenuSelectPanel);
      Button btn = goButton.GetComponent<Button>();
      btn.GetComponentInChildren<Text>().text = levelRandom.name;
      btn.GetComponentInChildren<TextMeshProUGUI>().text = (l + 1).ToString();
      btn.onClick.AddListener(delegate { levelNumber_current = l; LevelStart_init(l); gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; });
    }
    UIButtonLeveltoMain.onClick.AddListener(delegate { gameState = GameState.Menu;  menuState = MenuState.Main; runState = RunState.Transition; });

    // Popup menu button listeners
    //UIButtonPopuptoStart.onClick.AddListener(delegate { levelNumber_current += 1; LevelStart_init(levelNumber_current); });
    UIButtonPopuptoStart.onClick.AddListener(delegate { LevelStart_init(levelNumber_current); gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; });
    UIButtonPopuptoMain.onClick.AddListener(delegate { gameState = GameState.Menu;  menuState = MenuState.Main; runState = RunState.Transition; });
    UIButtonPopuptoSelect.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Select; runState = RunState.Transition; });

    // Ingame menu
    UIButtonGametoMain.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Main; runState = RunState.Transition; });

    // Set states
    gameState = GameState.Menu;
    runState = RunState.Transition;
    mouseState = 0;
  }

  void Update()
  {
    // *State Machines*


    // Menu States
    if (runState == RunState.Transition && menuState == MenuState.Main)
    {
      Debug.Log("Main State");
      UIMenuMain.gameObject.SetActive(true);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
      UIScreenDialogue.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      runState = RunState.Menu;
    }
    else if (runState == RunState.Transition && menuState == MenuState.Select)
    {
      Debug.Log("Select State");
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(true);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(false);
      UIScreenDialogue.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      runState = RunState.Menu;
    }
    else if (runState == RunState.Transition && menuState == MenuState.Popup)
    {
      Debug.Log("Popup State");
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(true);
      UIButtonGametoMain.gameObject.SetActive(false);
      UIScreenDialogue.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      runState = RunState.Menu;
    }
    else if (runState == RunState.Transition && menuState == MenuState.Game)
    {
      Debug.Log("Game State");
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(true);
      MapClick.gameObject.SetActive(true);
      MapNumber.gameObject.SetActive(true);
      runState = RunState.Game;
    }

    // Dialogue States
    if (runState == RunState.Game && dialogueState == DialogueState.Closed)
    {
      UIScreenDialogue.gameObject.SetActive(false);
    }
    else if (runState == RunState.Game && dialogueState == DialogueState.Open)
    {
      UIScreenDialogue.gameObject.SetActive(true);
      dialogueState = DialogueState.Writing;
    }
    
    if (runState == RunState.Game && dialogueState == DialogueState.Writing)
    {
      childLevelUpdate.LevelDialogue();
    }


    if (runState == RunState.Game)
    {
      childLevelUpdate.LevelListen(levelActive);
    }
  }

  // *LevelStart Instantation*
  private void LevelStart_init(int l) {

    // Read the json file into a local string 
    string json = maps[l];

    // Build json string into an object
    //levelActive = JsonUtility.FromJson<LevelStatic>(json);

    levelRandom = JsonUtility.FromJson<LevelRandom>(json);

    // Sets the main camera  to the same size at the levelActive height
    CameraMain.orthographicSize = (float)levelRandom.height * 5 / 6;

    // Shifts the grid postion when a map had odd width.
    UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2, -ScreenMain.GetComponent<RectTransform>().rect.height / 2, 0);
    if (levelRandom.width % 2 == 1)
    {
      UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2 - 0.5f, -ScreenMain.GetComponent<RectTransform>().rect.height / 2 - 0.5f, 0);
    }

    childLevelStart.LevelGenerate(levelRandom);


    //childLevelStart.LevelLoad(levelActive);
  }




   // *Basic functions*
  public Vector3Int TileVector_get(Vector3 levelPos)
  {
    // Return the tile that is clicked
    Vector3Int tilePos = MapClick.WorldToCell(levelPos);

    // A number to remove the top and left border of the grid from tile selection
    int borderY = 1;
    int borderX = 1;
    if (levelActive.height % 2 == 1)
    {
      borderY = (int)0.5;
      borderX = (int)1.5;
    }

    // Make sure it's in the game screen but also exclude the left column and top row from selection, it is adjusted centering it like a graph
    if (tilePos.x >= levelActive.width / 2 + borderX || tilePos.y >= levelActive.height / 2 - borderY || tilePos.x < -levelActive.width / 2 + borderX || tilePos.y < -levelActive.height / 2)
    {
      // An unreachable tile
      tilePos = new Vector3Int(0, 0, -1);
    }

    // Return it as an int vector, this is needed to change colors
    return tilePos;
  }
  public int LevelArrayIndex_create(Vector3 tilePos)
  {
    // Turning all negative tile positions into positive
    tilePos.x += levelActive.width / 2;
    tilePos.y = (tilePos.y + levelActive.height / 2) * levelActive.width;

    int levelIndex = (int)(tilePos.x + tilePos.y);

    // Returning the index of the level
    return levelIndex;
  }

  // *Manipulate TileMaps*
  public void SetTileColour(Color colour, Vector3Int position)
  {
    // Flag the tile, inidicating that it can change colour.
    MapClick.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    MapClick.SetColor(position, colour);
  }
}