﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpdate : MonoBehaviour
{
  [SerializeField]
  MainGame MainGame;
  internal static LevelUpdate instance;
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
      MainGame.maps[MainGame.levelNumber_current] = json;
      // Unload level and show popup menu
      MainGame.levelState = false;
      MainGame.UIMenuPopup.gameObject.SetActive(true);
      MainGame.MapClick.gameObject.SetActive(false);
      MainGame.MapNumber.gameObject.SetActive(false);
      MainGame.UIButtonGametoMain.gameObject.SetActive(false);
    }

    // Is the mouse button down and not up
    if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0) || !MainGame.gameState)
    {
      // Check to see if game is paused
      if (!MainGame.gameState)
      {
        MainGame.levelState = false;
        MainGame.UIMenuMain.gameObject.SetActive(true);
        MainGame.MapClick.gameObject.SetActive(false);
        MainGame.MapNumber.gameObject.SetActive(false);
        MainGame.UIButtonGametoMain.gameObject.SetActive(false);
      }
      else
      {
        string json = JsonUtility.ToJson(levelActive);
        MainGame.maps[MainGame.levelNumber_current] = json;
      }

      // Mouse position to camera position 
      Vector3 mouse_pos = Input.mousePosition;
      mouse_pos = Camera.main.ScreenToWorldPoint(mouse_pos);

      Vector3 offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mouse_pos.z));

      // Return tileMap x and y (x, y, z) based on the clicked tile position
      Vector3Int tileVector_int = MainGame.TileVector_get(mouse_pos);

      // Set color to red if tile is white else color is white
      if (tileVector_int.z == 0)
      {
        if (MainGame.MapClick.GetColor(tileVector_int) == Color.white)
        {
          MainGame.SetTileColour(new Color(200, 0, 0), tileVector_int);
          levelActive.tileSelected[MainGame.LevelArrayIndex_create(tileVector_int)] = 1;
          // Debug.Log(tileVector_int);
          MainGame.tileCorrect_countdown++;
        }
        else
        {
          MainGame.SetTileColour(Color.white, tileVector_int);
          levelActive.tileSelected[MainGame.LevelArrayIndex_create(tileVector_int)] = 0;
          // Debug.Log(tileVector_int);
          MainGame.tileCorrect_countdown--;
        }

        //Tile tileUpdate = MapNumber.GetTile<Tile>(new Vector3Int(0, level.height - 1, 0));

        //string countdown = tileUpdate.gameObject.GetComponent<TextMeshPro>().text;

        //Debug.Log(countdown);

        //MapNumber.SetTile(new Vector3Int(0, level.height - 1, 0), tileUpdate);
      }
    }
  }
}