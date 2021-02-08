using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
  public static MainGame Instance;

  [SerializeField]
  internal LevelStart childLevelStart;
  [SerializeField]
  internal LevelUpdate childLevelUpdate;

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

  // Level active or not
  [NonSerialized]
  public bool levelState;

  // Game active or not
  [NonSerialized]
  public bool gameState;

  // G
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
    UIButtonMaintoStart.onClick.AddListener(delegate { LevelStart_init(levelNumber_current); });
    UIButtonMaintoSelect.onClick.AddListener(UISelect_transition); 

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
      btn.onClick.AddListener(delegate { levelNumber_current = l; LevelStart_init(l); });
    }
    UIButtonLeveltoMain.onClick.AddListener(UIMainMenu_transition);

    // Popup menu button listeners
    //UIButtonPopuptoStart.onClick.AddListener(delegate { levelNumber_current += 1; LevelStart_init(levelNumber_current); });
    UIButtonPopuptoStart.onClick.AddListener(delegate { LevelStart_init(levelNumber_current); });
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
    mouseState = 0;
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
      childLevelUpdate.LevelListen(levelActive);
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

  private void LevelStart_init(int l) {
    //Debug.Log(l);

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




   // Basic functions
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

  // Manipulate TileMaps
  public void SetTileColour(Color colour, Vector3Int position)
  {
    // Flag the tile, inidicating that it can change colour.
    MapClick.SetTileFlags(position, TileFlags.None);

    // Set the colour.
    MapClick.SetColor(position, colour);
  }
}