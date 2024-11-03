using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MenuButtons : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Dropdown dropdown;
    public List<string> saves;
    public GameObject menuCanvas;

    public void LoadGame()
    {
        //Populate the dropdown with the list of players
        saves = CharacterManager.instance.GetSaveInfo();
        dropdown.ClearOptions();
        dropdown.AddOptions(saves);
    }

    public void StartNewGame()
    {
        string name = nameInput.text;
        if (name == "")
        {
            name = "Player";
        }
        CharacterManager.instance.CreateNewPlayer(name);
    }

    public void StartLoadGame()
    {
        if (saves.Count == 0)
        {
            return;
        }
        int playerID = Convert.ToInt32(saves[dropdown.value].Split(' ')[0]);
        //set MenuCanvas to inactive
        menuCanvas.SetActive(false);
        CharacterManager.instance.LoadPlayer(playerID);
    }

    public void DeleteGame()
    {
        if (saves.Count == 0)
        {
            return;
        }
        int playerID = Convert.ToInt32(saves[dropdown.value].Split(' ')[0]);
        CharacterManager.instance.DeletePlayer(playerID);
        LoadGame();
    }
}
