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

  // Different Screeens
  public RectTransform UIMenuMain;
  public RectTransform UIMenuSelect;
  public RectTransform UIMenuSelectPanel;
  public RectTransform UIMenuPopup;
  public RectTransform UIGrid;
  public RectTransform UIGameInterfaceMirror;
  public RectTransform UIGameInterfaceRow;
  public RectTransform UIGameInterfaceColumn;
  public RectTransform UIScreenDialogue;
  public RectTransform UIGameHighlightMirror;
  public RectTransform UIGameHighlightRow;
  public RectTransform UIGameHighlightColumn;

  // Transition Buttons
  public Button UIButtonPopuptoStart;
  public Button UIButtonPopuptoMain;
  public Button UIButtonPopuptoSelect;
  public Button UIButtonMaintoStart;
  public Button UIButtonMaintoSelect;
  public Button UIButtonLeveltoMain;
  public Button UIButtonGametoMain;
  public Button UIButtonDialoguePanel;
  public GameObject UIButtonSelect;

  // Ingame UI panel
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
    public string[] dialogueScript;
    public string[] flash;
  }
  public LevelStatic levelActive = new LevelStatic();

  public string[] dialogueScript;

  // *State Enumerations*

  // Game active or not
  public enum GameState
  {
    Game,
    Menu
  }
  public GameState gameState;

  // UI Menu state
  public enum MenuState
  {
    Main,
    Select,
    Popup,
    Game
  }
  public MenuState menuState;

  // Level
  public enum LevelState
  {
    Listen,
    Tutorial,
    Random
  }
  public LevelState levelState;

  // Flash
  public enum FlashState
  {
    Listen,
    Reset,
    Mirror,
    Count,
    Column,
    Row
  }
  public FlashState flashState;

  // Dialogue
  public int dialogueScriptNum; 

  public enum DialogueState
  {
    Listen,
    Closed,
    Open,
    Pause,
    Next,
  }
  public DialogueState dialogueState;

  // Mouse
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
    LevelSelection,
    Tutorial,
    Running
  }
  public RunState runState;

  // Game mouse state
  [NonSerialized]
  public int mouseState;

  // Current Level number
  [NonSerialized]
  public int randomLevelNumber_current;
  public int tutorialLevelNumber_current;
  public int tileCorrect_countdown;

  // Decoded json maps as string array
  [NonSerialized]
  public String[] MapAssetTutorial;
  public String[] MapAssetRandom;

  // Tilemaps
  public Tilemap MapClick;
  public Tilemap MapNumber;

  // Random Tilemaps
  public TextAsset AssetRandom1;
  public TextAsset AssetRandom2;
  public TextAsset AssetRandom3;
  public TextAsset AssetRandom4;

  // Static Tilemaps
  public TextAsset AssetTutorial1;

  // Flash Global variable
  int flash = 1;
  float t = 0;



  void Start()
  {
    // Current level set default is 0 
    randomLevelNumber_current = 0;
    tutorialLevelNumber_current = 0; 

    MapAssetRandom = new String[4];

    MapAssetTutorial = new String[1];

    MapAssetRandom[0] = AssetRandom1.text;
    MapAssetRandom[1] = AssetRandom2.text;
    MapAssetRandom[2] = AssetRandom3.text;
    MapAssetRandom[3] = AssetRandom4.text;

    MapAssetTutorial[0] = AssetTutorial1.text;


    dialogueScript = new string[2];

    //mapDirectory.GetFiles();

    // Main menu button listeners
    UIButtonMaintoStart.onClick.AddListener(delegate { levelState = LevelState.Tutorial; gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; });
    UIButtonMaintoSelect.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Select; runState = RunState.Transition; });

    // Select level menu button listeners
    for (int i = 0; i < MapAssetRandom.Length; i++)
    {
      // Encapsulate the level number to break dependency
      int l = i;
      // Read the json file into a local string 
      string json = MapAssetRandom[l];

      // Build json string into an object
      levelRandom = JsonUtility.FromJson<LevelRandom>(json);

      // Prefab button added to the select menu panel
      GameObject goButton = Instantiate(UIButtonSelect, UIMenuSelectPanel);
      Button btn = goButton.GetComponent<Button>();
      btn.GetComponentInChildren<Text>().text = levelRandom.name;
      btn.GetComponentInChildren<TextMeshProUGUI>().text = (l + 1).ToString();
      btn.onClick.AddListener(delegate { randomLevelNumber_current = l; levelState = LevelState.Random; gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; });
    }
    UIButtonLeveltoMain.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Main; runState = RunState.Transition; });

    // Popup menu button listeners
    //UIButtonPopuptoStart.onClick.AddListener(delegate { levelNumber_current += 1; LevelStart_init(levelNumber_current); });
    UIButtonPopuptoStart.onClick.AddListener(delegate { LevelStart_init(randomLevelNumber_current); gameState = GameState.Game; menuState = MenuState.Game; runState = RunState.Transition; });
    UIButtonPopuptoMain.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Main; runState = RunState.Transition; });
    UIButtonPopuptoSelect.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Select; runState = RunState.Transition; });

    // Ingame menu
    UIButtonGametoMain.onClick.AddListener(delegate { gameState = GameState.Menu; menuState = MenuState.Main; runState = RunState.Transition; });

    UIButtonDialoguePanel.onClick.AddListener(delegate { dialogueState = DialogueState.Next; });

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
      UIGameInterfaceMirror.gameObject.SetActive(false);
      UIGameInterfaceRow.gameObject.SetActive(false);
      UIGameInterfaceColumn.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      flashState = FlashState.Reset;
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
      UIGameInterfaceMirror.gameObject.SetActive(false);
      UIGameInterfaceRow.gameObject.SetActive(false);
      UIGameInterfaceColumn.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      flashState = FlashState.Reset;
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
      UIGameInterfaceMirror.gameObject.SetActive(false);
      UIGameInterfaceRow.gameObject.SetActive(false);
      UIGameInterfaceColumn.gameObject.SetActive(false);
      MapClick.gameObject.SetActive(false);
      MapNumber.gameObject.SetActive(false);
      flashState = FlashState.Reset;
      runState = RunState.Menu;
    }
    else if (runState == RunState.Transition && menuState == MenuState.Game)
    {
      Debug.Log("Game State");
      UIMenuMain.gameObject.SetActive(false);
      UIMenuSelect.gameObject.SetActive(false);
      UIMenuPopup.gameObject.SetActive(false);
      UIButtonGametoMain.gameObject.SetActive(true);
      UIGameInterfaceMirror.gameObject.SetActive(true);
      UIGameInterfaceRow.gameObject.SetActive(true);
      UIGameInterfaceColumn.gameObject.SetActive(true);
      MapClick.gameObject.SetActive(true);
      MapNumber.gameObject.SetActive(true);
      runState = RunState.LevelSelection;
    }

    // Level States
    if (runState == RunState.LevelSelection && levelState == LevelState.Listen)
    {

    }
    else if (runState == RunState.LevelSelection && levelState == LevelState.Tutorial)
    {
      LevelStart_init(tutorialLevelNumber_current);
      dialogueState = DialogueState.Open;
      runState = RunState.Running;
      levelState = LevelState.Listen;
    }
    else if (runState == RunState.LevelSelection && levelState == LevelState.Random)
    {
      LevelStart_init(randomLevelNumber_current); 
      runState = RunState.Running;
      levelState = LevelState.Listen;
    }

    // Dialogue States
    if (runState == RunState.Running && dialogueState == DialogueState.Listen)
    {

    }
    else if (runState == RunState.Running && dialogueState == DialogueState.Closed)
    {
      UIScreenDialogue.gameObject.SetActive(false);
    }
    else if (runState == RunState.Running && dialogueState == DialogueState.Open)
    {
      UIScreenDialogue.gameObject.SetActive(true);
      dialogueState = DialogueState.Pause;
    }
    // First dialogue box
    if (runState == RunState.Running && dialogueState == DialogueState.Pause)
    {
      dialogueScriptNum = 0;
      if (dialogueScriptNum < levelActive.dialogueScript.Length)
      {
        StateSnippetFlashDialogue();
        dialogueState = DialogueState.Listen;
        runState = RunState.Tutorial; 
      }
    }
    // Every dialogue box after
    else if (runState == RunState.Tutorial && dialogueState == DialogueState.Next)
    {
      dialogueScriptNum++;
      if (dialogueScriptNum < levelActive.dialogueScript.Length - 1)
      {
        StateSnippetFlashDialogue();
        dialogueState = DialogueState.Listen;
      }
      else
      {
        dialogueState = DialogueState.Closed;
        runState = RunState.Running;
      }
    }
    
    if (flashState == FlashState.Reset)
    {
      UIGameHighlightMirror.gameObject.SetActive(false);
      UIGameHighlightRow.gameObject.SetActive(false);
      UIGameHighlightColumn.gameObject.SetActive(false);
    }



    // Flashing animation for mirror button
    if (flashState == FlashState.Mirror)
    {
      if (flash == 1)
      {
        t += Time.deltaTime * 100;
        UIGameHighlightMirror.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t > 100)
        {
          flash = 0;
        }
      }
      else
      {
        t -= Time.deltaTime * 100;
        UIGameHighlightMirror.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t < 0)
        {
          flash = 1;
        }
      }
    }
    if (flashState == FlashState.Column)
    {
      if (flash == 1)
      {
        t += Time.deltaTime * 100;
        UIGameHighlightColumn.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t > 100)
        {
          flash = 0;
        }
      }
      else
      {
        t -= Time.deltaTime * 100;
        UIGameHighlightColumn.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t < 0)
        {
          flash = 1;
        }
      }
    }
    if (flashState == FlashState.Row)
    {
      if (flash == 1)
      {
        t += Time.deltaTime * 100;
        UIGameHighlightRow.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t > 100)
        {
          flash = 0;
        }
      }
      else
      {
        t -= Time.deltaTime * 100;
        UIGameHighlightRow.transform.GetComponent<Image>().color = new Color32(255, 255, 255, ((byte)t));
        if (t < 0)
        {
          flash = 1;
        }
      }
    }

    if (flashState == FlashState.Count)
    {
      if (flash == 1)
      {
        t += Time.deltaTime * 100;
        SetTileColour(new Color32(255, 255, 255, ((byte)t)), new Vector3Int(-levelActive.width / 2, levelActive.height / 2 - 1 + levelActive.height % 2, 0));
        if (t > 100)
        {
          flash = 0;
        }
      }
      else
      {
        t -= Time.deltaTime * 100;
        SetTileColour(new Color32(255, 255, 255, ((byte)t)), new Vector3Int(-levelActive.width / 2, levelActive.height / 2 - 1 + levelActive.height % 2, 0));
        if (t < 0)
        {
          flash = 1;
        }
      }
    }

    // Run Game State
    if (runState == RunState.Running)
    {
      childLevelUpdate.LevelListen(levelActive);
    }

    //Debug.Log(t);
  }

  // *LevelStart Instantation*
  private void LevelStart_init(int l) {

    if (levelState == LevelState.Random)
    {
      // Read the json file into a local string 
      string json = MapAssetRandom[l];

      // Build json string into an object
      levelRandom = JsonUtility.FromJson<LevelRandom>(json);

      // Sets the main camera to the same size at the levelActive height
      CameraMain.orthographicSize = (float)levelRandom.height;
      //Debug.Log(UIGrid.localPosition);
      //UIGameHighlight.sizeDelta = new Vector2(levelRandom.width * 200, levelRandom.height * 200);   

      // Shifts the grid postion when a map had odd width
      UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2, -ScreenMain.GetComponent<RectTransform>().rect.height / 2, 0);
      if (levelRandom.width % 2 == 1)
      {
        UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2 - 0.5f, -ScreenMain.GetComponent<RectTransform>().rect.height / 2 - 0.5f, 0);
      }

      // Generate this random level
      childLevelStart.LevelGenerate(levelRandom);
    }
    else if (levelState == LevelState.Tutorial)
    {
      // Read the json file into a local string 
      string json = MapAssetTutorial[l];

      // Build json string into an object
      levelActive = JsonUtility.FromJson<LevelStatic>(json);

      // Set dialogue script 
      dialogueScript = levelActive.dialogueScript;

      // Sets the main camera to the same size at the levelActive height
      CameraMain.orthographicSize = (float)levelActive.height;
      //Debug.Log(UIGrid.localPosition);
      //UIGameHighlight.sizeDelta = new Vector2(levelRandom.width * 200, levelRandom.height * 200);   

      // Shifts the grid postion when a map had odd width
      UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2, -ScreenMain.GetComponent<RectTransform>().rect.height / 2, 0);
      if (levelActive.width % 2 == 1)
      {
        UIGrid.localPosition = new Vector3(-ScreenMain.GetComponent<RectTransform>().rect.width / 2 - 0.5f, -ScreenMain.GetComponent<RectTransform>().rect.height / 2 - 0.5f, 0);
      }

      // Generate this Tutorial level
      childLevelStart.LevelLoad(levelActive);
    }
  }


  // *Basic functions*
  public Vector3Int TileVector_get(Vector3 levelPos)
  {
    // Return the tile that is clicked
    Vector3Int tilePos = MapClick.WorldToCell(levelPos);

    //Debug.Log(tilePos);

    // A number to remove the top and left border of the grid from tile selection
    int borderY = 1;
    int borderX = 1; 
    if (levelActive.height % 2 == 1)
    {
      borderY = (int)0.5;
      borderX = (int)1.5;
    }
    int extraShiftX = 0;
    if (levelActive.width % 2 == 1) 
    {
      extraShiftX = 1;
    }

      // Make sure it's in the game screen but also exclude the left column and top row from selection, it is adjusted centering it like a graph
      if (tilePos.x >= levelActive.width / 2 + extraShiftX || tilePos.y >= levelActive.height / 2 - borderY || tilePos.x < -levelActive.width / 2 + borderX || tilePos.y < -levelActive.height / 2)
    {
      // An unreachable tile
      tilePos = new Vector3Int(0, 0, -1);
    }

    // Return it as an int vector, this is needed to change colors
    return tilePos;
  }
  public int LevelArrayIndex_covert(Vector3 tilePos)
  {
    // Turning all negative tile positions into positive
    tilePos.x += levelActive.width / 2;
    tilePos.y = (tilePos.y + levelActive.height / 2) * levelActive.width;

    int levelIndex = (int)(tilePos.x + tilePos.y);

    // Returning the index of the level
    return levelIndex;
  }
  /*public int MouseTileVector_covert(Vector3Int levelIndex)
  {
    Vector3 tilePos;
    // Turning all negative tile positions into positive
    tilePos.x += levelActive.width / 2;
    tilePos.y = (tilePos.y + levelActive.height / 2) * levelActive.width;

    int levelIndex = (int)(tilePos.x + tilePos.y);

    // Returning the index of the level
    return tilePos;
  }*/

  // *Manipulate TileMaps*
  public void SetTileNumber(Tile tileBasic, Vector3Int position)
  {
    // Overide the existing tile
    MapNumber.SetTile(position, tileBasic);

    // Refrest the tile
    MapNumber.RefreshTile(position);

    // Destroy the original object after the launch
    Destroy(tileBasic.gameObject, 1);

  }

  public void SetTileColour(Color colour, Vector3Int position) 
  {
    // Flag the tile, inidicating that it can change colour.
    MapClick.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    MapClick.SetColor(position, colour);
  }

  // *State Snippet*
  public void StateSnippetFlashDialogue() { 
    childLevelUpdate.LevelDialogue(levelActive.dialogueScript[dialogueScriptNum]); 

    if (levelActive.flash[dialogueScriptNum] == "mirror")
    {
      UIGameHighlightMirror.gameObject.SetActive(true);
      flashState = FlashState.Mirror;
    }
    else if(levelActive.flash[dialogueScriptNum] == "column")
    {
      UIGameHighlightColumn.gameObject.SetActive(true);
      flashState = FlashState.Column;
    }
    else if(levelActive.flash[dialogueScriptNum] == "row")
    {
      UIGameHighlightRow.gameObject.SetActive(true);
      flashState = FlashState.Row;
    }
    else if(levelActive.flash[dialogueScriptNum] == "count")
    {
      flashState = FlashState.Count;
    }
    else if (levelActive.flash[dialogueScriptNum] == "listen")
    {
      SetTileColour(new Color32(255, 255, 255, 100), new Vector3Int(-levelActive.width / 2, levelActive.height / 2 - 1 + levelActive.height % 2, 0));
      flashState = FlashState.Reset;
    }
  }
}