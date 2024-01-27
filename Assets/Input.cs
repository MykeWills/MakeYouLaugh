using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class Input : MonoBehaviour
{

    public struct PlayerControl
    {
        private Player playerInput;

        private float moveSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
    }

    [SerializeField] private PlayerControl playerControl;



    [SerializeField] private Animator anim;
    private int stateIndex = 0;

    void Update()
    {
        
    }

    private void Move()
    {

    }
}
