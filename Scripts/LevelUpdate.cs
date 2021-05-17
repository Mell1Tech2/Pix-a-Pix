using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelUpdate : MonoBehaviour
{
  [SerializeField]
  MainGame parentMainGame;

  private int DialogueCount = 0;

  // *Main game loop*
  public void LevelListen(MainGame.LevelStatic levelActive)
  {
    // Check if you Won by comparing correct tiles with selected tiles, else save the game
    if (levelActive.tileSelected.SequenceEqual(levelActive.tileCorrect))
    {
      // Reset the map local save back to 0
      for (int i = 0; i < levelActive.height * levelActive.width; i++)
      {
        levelActive.tileSelected[i] = 0;
      }
      string json = JsonUtility.ToJson(levelActive);
      parentMainGame.maps[parentMainGame.levelNumber_current] = json;

      // Unload level and show popup menu
      parentMainGame.gameState = MainGame.GameState.Menu;
      parentMainGame.menuState = MainGame.MenuState.Popup;
      parentMainGame.runState = MainGame.RunState.Transition;
    }

    // Is the mouse button down and not up
    if (Input.GetMouseButton(0) && !Input.GetMouseButtonUp(0) || parentMainGame.gameState == MainGame.GameState.Menu)
    {
      // Check to see if game is paused
      if (parentMainGame.gameState == MainGame.GameState.Menu)
      {
        parentMainGame.menuState = MainGame.MenuState.Main;
        parentMainGame.runState = MainGame.RunState.Transition;
      }
      else
      {
        string json = JsonUtility.ToJson(levelActive);
        parentMainGame.maps[parentMainGame.levelNumber_current] = json;
      }

      // Mouse position to camera position 
      Vector3 mouse_pos = Input.mousePosition;
      mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

      // Return tileMap x and y (x, y, z) based on the clicked tile position
      Vector3Int tileVector_int = parentMainGame.TileVector_get(mouse_pos);

      // Set color to red if tile is white else color is white
      if (tileVector_int.z == 0)
      {
        if (parentMainGame.MapClick.GetColor(tileVector_int) == Color.white && parentMainGame.mouseState == 0 || parentMainGame.mouseState == 1)
        {
          parentMainGame.SetTileColour(new Color(200, 0, 0), tileVector_int);
          levelActive.tileSelected[parentMainGame.LevelArrayIndex_create(tileVector_int)] = 1;
          // Debug.Log(tileVector_int);
          parentMainGame.tileCorrect_countdown++;

          parentMainGame.mouseState = 1;
        }
        else if(parentMainGame.MapClick.GetColor(tileVector_int) == new Color(200, 0, 0) && parentMainGame.mouseState == 0 || parentMainGame.mouseState == 2)
        {
          parentMainGame.SetTileColour(Color.white, tileVector_int);
          levelActive.tileSelected[parentMainGame.LevelArrayIndex_create(tileVector_int)] = 0;
          // Debug.Log(tileVector_int);
          parentMainGame.tileCorrect_countdown--;

          parentMainGame.mouseState = 2;
        }
        //Tile tileUpdate = MapNumber.GetTile<Tile>(new Vector3Int(0, level.height - 1, 0));

        //string countdown = tileUpdate.gameObject.GetComponent<TextMeshPro>().text;

        //Debug.Log(countdown);

        //MapNumber.SetTile(new Vector3Int(0, level.height - 1, 0), tileUpdate);
      }
    }
    if (Input.GetMouseButtonUp(0))
    {
      parentMainGame.mouseState = 0;
    }
  }
  // Execute or play the next dialgiue in the active level
  public void LevelDialogue()
  {
    DialogueCount++;
    parentMainGame.UIScreenDialogue.GetComponentInChildren<TextMeshProUGUI>().text = "hi";
    Debug.Log(DialogueCount);
  }
}
