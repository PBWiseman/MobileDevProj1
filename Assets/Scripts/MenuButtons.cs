using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuButtons : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void LoadGame()
    {
        Debug.Log("Load Game");
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
        int playerID = 0; //TODO: get ID
        CharacterManager.instance.LoadPlayer(playerID);
    }
}
