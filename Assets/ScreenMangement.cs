using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ScreenMangement : MonoBehaviour
{
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Transform players;
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private List<Camera> cameras = new List<Camera>();

    [SerializeField] private LayerMask[] playerMasks = new LayerMask[4];
    private void Start()
    {
        SetupNewPlayer();
    }
    void Update()
    {
        if (totalControllers > totalPlayers)
        {
            SetupNewPlayer();
        }
        else if (totalControllers < totalPlayers)
        {
            RemovePlayers();
            CheckNewPlayers(players, cleanList: true);
            SetupCamera(players.childCount);
        }
    }
    private void SetupNewPlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefabs[Random.Range(0, 2)], players);
        newPlayer.transform.position = startingPosition;

        if (newPlayer.TryGetComponent(out Input input))
        {
            input.playerControl.controllerID = totalPlayers;
            input.playerControl.lookSensitivity = input.playerControl.lookSensitivity * 4;

            string layerName = "Default";
            switch (input.playerControl.controllerID)
            {
                case 0: layerName = "Player1"; break;
                case 1: layerName = "Player2"; break;
                case 2: layerName = "Player3"; break;
                case 3: layerName = "Player4"; break;
            }
            input.tag = layerName;
            input.Body.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        CheckNewPlayers(players, cleanList: true);
        SetupCamera(players.childCount);
    }
    private void SetupCamera(int playerNum)
    {
        switch (playerNum)
        {
            case 1: 
                cameras[0].rect = new Rect(0f, 0f, 1f, 1f); 
                break;
            case 2:
                cameras[0].rect = new Rect(0f, 0.5f, 1, 0.5f); 
                cameras[1].rect = new Rect(0f, 0f, 1f, 0.5f);
                break;
            case 3:
                cameras[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
                cameras[1].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
            case 4:
                cameras[0].rect = new Rect(0f, 0.5f, 0.5f, 0.5f);
                cameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                cameras[2].rect = new Rect(0f, 0f, 0.5f, 0.5f);
                cameras[3].rect = new Rect(0.5f, 0f, 0.5f, 0.5f);
                break;
        }
        cameras[playerNum - 1].cullingMask = playerMasks[playerNum - 1];
    }

    private void CheckNewPlayers(Transform parent, bool cleanList = true)
    {
        if (cleanList && cameras.Count > 0) cameras = new List<Camera>();

        foreach (Transform child in parent)
        {
            if(child.TryGetComponent(out Camera camera))
            {
                cameras.Add(camera);
            }
            if(child.transform.childCount > 0)
            {
                CheckNewPlayers(child, cleanList: false);
            }
        }
    }
    private void RemovePlayers()
    {
        for (int p = 1; p < players.childCount; p++)
        {
            if (p < totalControllers) continue;
            Destroy(players.GetChild(p).gameObject);
        }
    }
    private int totalPlayers => players.childCount - 1;
    private int totalControllers => ReInput.controllers.joystickCount;


}
