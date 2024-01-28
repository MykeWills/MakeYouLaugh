using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class Input : MonoBehaviour
{
    private enum RotationAxes { XY, X, Y };
    private RotationAxes axis = RotationAxes.XY;

    [Serializable]
    public struct PlayerControl
    {
        public int controllerID;
        public float gravity;
        public float speed;

        public Vector2 lookSensitivity;
    }

    public PlayerControl playerControl;

    private float[] lookRotation = new float[2];
    [SerializeField] private Transform head;
    [SerializeField] private Animator anim;
    private float moveSpeed;
    public int stateIndex = 0;
    private Player playerInput => ReInput.players.GetPlayer(playerControl.controllerID);
    private bool isRunning => playerInput.GetButton("Run");
    public bool isGrounded = true;
    Vector2 input = new Vector2();
    private float time => Time.deltaTime;
    private CharacterController cc => GetComponent<CharacterController>();
    void Update()
    {
        Move();
        Look();
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
        bool thresholdX = input.x > 0.1f && input.x < 0.1f;
        bool thresholdY = input.y > 0.1f && input.y < 0.1f;

        if (input.x > 0 && thresholdY) state = isRunning ? 6 : 4;
        else if (input.x < 0 && thresholdY) state = isRunning ? 5 : 3;

        else if (thresholdX && input.y > 0) state = isRunning ? 2 : 1;
        else if (thresholdX && input.y < 0) state = isRunning ? 8 : 7;

        else if (input.x > 0 && input.y > 0) state = isRunning ? 2 : 1;
        else if (input.x < 0 && input.y < 0) state = isRunning ? 2 : 1;

        else if (input.x < 0 && input.y > 0) state = isRunning ? 2 : 1;
        else if (input.x > 0 && input.y < 0) state = isRunning ? 2 : 1;

        else if (thresholdX && thresholdY) state = 0;
        return state;
    }
}
