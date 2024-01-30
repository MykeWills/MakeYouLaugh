using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class Input : MonoBehaviour
{
    [Serializable]
    public struct PlayerControl
    {
        public int controllerID;
        public float gravity;
        public float speed;
        public float checkDistance;
        public Vector2 lookSensitivity;
        [Space]
        public GameObject AButtonUI;
        public Camera playerCamera;
    }
    public GameObject interactiveObject;
    public PlayerControl playerControl;
    [Space]
    public int stateIndex = 0;
    public bool isGrounded = true;
    public bool objectInFront = false;
    [Space]
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Animator anim;

    void Update()
    {
        Move();
        Look();
        InteractWithObject();
    }

    private void InteractWithObject()
    {
        if(Physics.Raycast(playerControl.playerCamera.transform.position, playerControl.playerCamera.transform.forward, out hit, playerControl.checkDistance))
        {
            objectInFront = hit.collider.tag == interactiveTag;

            if (objectInFront && playerInput.GetButtonDown("Interact"))
            {
                if (hit.collider.gameObject.TryGetComponent(out InteractivePushObject interactivePush))
                {
                    Vector3 direction = transform.right;
                    anim.SetTrigger("Push");
                    interactivePush.PushObject(direction);
                }
                else if (hit.collider.gameObject.TryGetComponent(out InteractiveDoorObject interactiveDoor))
                {
                    anim.SetTrigger("Push");
                    interactiveDoor.OpenObject();
                }

            }
            else if (objectInFront)
            {
                interactiveObject = hit.transform.gameObject;
                if (interactiveObject) ShowInteractionUI(interactiveObject.transform, active: true);
            }
        }
        else ShowInteractionUI(active: false);
    }
    private void ShowInteractionUI(Transform interactionObj = null, bool active = false)
    {
        playerControl.AButtonUI.SetActive(active);

        if (!active) return;

        if (active) 
        {
            Vector3 statusPos = playerControl.playerCamera.WorldToScreenPoint(interactionObj.position);

            int playerNumber = ScreenMangement.Instance.totalPlayers;
            int controllerID = playerControl.controllerID;
            switch (playerNumber)
            {
                case 0: 
                    statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width); 
                    statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height); 
                    break;

                case 1:
                    switch (controllerID)
                    {
                        case 0:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, Screen.height / 2, Screen.height);
                            break;
                        case 1:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height / 2);
                            break;
                    }
                    break;
                case 2:
                    switch (controllerID)
                    {
                        case 0:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, Screen.height / 2, Screen.height);
                            break;
                        case 1:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width / 2);
                            statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height / 2);
                            break;
                        case 2:
                            statusPos.x = Mathf.Clamp(statusPos.x, Screen.width / 2, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height / 2);
                            break;
                    }
                    break;
                case 3:
                    switch (controllerID)
                    {
                        case 0:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width/2);
                            statusPos.y = Mathf.Clamp(statusPos.y, Screen.height / 2, Screen.height);
                            break;
                        case 1:
                            statusPos.x = Mathf.Clamp(statusPos.x, Screen.width / 2, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, Screen.height / 2, Screen.height);
                            break;
                        case 2:
                            statusPos.x = Mathf.Clamp(statusPos.x, 0, Screen.width / 2);
                            statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height / 2);
                            break;
                        case 3:
                            statusPos.x = Mathf.Clamp(statusPos.x, Screen.width / 2, Screen.width);
                            statusPos.y = Mathf.Clamp(statusPos.y, 0, Screen.height / 2);
                            break;
                    }
                    break;
            }

            playerControl.AButtonUI.transform.position = statusPos;
            playerControl.AButtonUI.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void Move()
    {
        input.x = playerInput.GetAxis("MoveX");
        input.y = playerInput.GetAxis("MoveY");

        Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        stateIndex = GetState();

        anim.SetInteger("StateIndex", stateIndex);

        moveDirection.y -= playerControl.gravity * time;

        moveSpeed = isRunning ? playerControl.speed * 2 : playerControl.speed;

        isGrounded = (cc.Move(transform.TransformDirection(moveDirection) * time * moveSpeed) & CollisionFlags.Below) != 0;
    }
    private void Look()
    {
        switch (axis)
        {
            // XY rotation
            case RotationAxes.XY: XLook(); YLook(); break;
            // X rotation
            case RotationAxes.X: XLook(); break;
            // Y rootation
            case RotationAxes.Y: YLook(); break;
        }
    }
    private void XLook()
    {
        // rotation input times smoothing & sensitivity
        lookRotation[0] = playerInput.GetAxisRaw("LookX") * time * playerControl.lookSensitivity.x;
        // rotate only player transform
        transform.Rotate(0, lookRotation[0], 0);
    }
    private void YLook()
    {
        // rotation input times smoothing, sensitivity and inversion
        float y = playerInput.GetAxisRaw("LookY") * time * playerControl.lookSensitivity.y;
        lookRotation[1] += y;
        // clamp rotation of the Y between 55/-55
        lookRotation[1] = Mathf.Clamp(lookRotation[1], -70, 70);
        // rotate the head up or down
        Vector3 newRot = new Vector3(-lookRotation[1], 0, 0);
        head.transform.localEulerAngles = newRot;
    }
    private int GetState()
    {
        int state = stateIndex;
        bool thresholdX = input.x > -0.1f && input.x < 0.1f;
        bool thresholdY = input.y > -0.1f && input.y < 0.1f;

        if (input.x > 0 && thresholdY) state = isRunning ? 6 : 4;
        else if (input.x < 0 && thresholdY) state = isRunning ? 5 : 3;

        else if (thresholdX && input.y > 0) state = isRunning ? 2 : 1;
        else if (thresholdX && input.y < 0) state = isRunning ? 8 : 7;

        else if (input.x > 0 && input.y > 0) state = isRunning ? 2 : 1;
        else if (input.x < 0 && input.y < 0) state = isRunning ? 2 : 1;

        else if (input.x < 0 && input.y > 0) state = isRunning ? 2 : 1;
        else if (input.x > 0 && input.y < 0) state = isRunning ? 2 : 1;

        else if (input.x == 0 && input.y == 0) state = 0;
        return state;
    }

    public int Layer
    {
        get { return anim.gameObject.layer; }
        set { anim.gameObject.layer = value; }
    }
    public Transform Body
    {
        get { return body; }
    }



    private enum RotationAxes { XY, X, Y };
    private RotationAxes axis = RotationAxes.XY;
    private Vector2 input = new Vector2();
    private RaycastHit hit = new RaycastHit();
    private float moveSpeed;
    private float[] lookRotation = new float[2];
    private const string interactiveTag = "Interactive";
    private Player playerInput => ReInput.players.GetPlayer(playerControl.controllerID);

    private bool isRunning => playerInput.GetButton("Run");
    private float time => Time.deltaTime;
    private CharacterController cc => GetComponent<CharacterController>();
}
