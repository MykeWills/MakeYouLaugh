using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ScreenMangement : MonoBehaviour
{
    private static ScreenMangement screenMangement;


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
        // TotalControllers +1 because Keyboard and mouse is always hooked up.
        if (totalControllers + 1 > totalPlayers)
        {
            SetupNewPlayer();
        }
        else if (totalControllers < totalPlayers - 1)
        {
            RemovePlayers();
        }

        if(totalPlayers < currentPlayers)
        {
            CheckNewPlayers(players, cleanList: true);
            SetupCamera(players.childCount);
        }
            
    }
    private void SetupNewPlayer()
    {
        GameObject newPlayer = Instantiate(playerPrefabs[Random.Range(0, 2)], players);
        newPlayer.transform.position = startingPosition;


        if (newPlayer.transform.GetChild(0).TryGetComponent(out Input input))
        {
            input.playerControl.controllerID = totalPlayers - 1;
            input.playerControl.lookSensitivity = input.playerControl.lookSensitivity * (totalControllers > 1 ? 4 : 1);

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
        currentPlayers = totalPlayers;
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
    private int currentPlayers = 0;
    public int totalPlayers => players.childCount;
    private int totalControllers => ReInput.controllers.joystickCount;

    public static ScreenMangement Instance
    {
        get
        {
            if(screenMangement == null)
            {
                screenMangement = FindObjectOfType<ScreenMangement>();
            }
            return screenMangement;
        }
    }
}
