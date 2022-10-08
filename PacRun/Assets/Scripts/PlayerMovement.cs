using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController playerController;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector2 currentInputVector, smoothInputVelocity;

    private float smoothInputSpeed = 0.2f;

    private float fixedMoveSpeed = 4f;

    private bool isTouchingAWall = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, -1f, currentInputVector.y);
        WallCheck();
        float moveSpeed = isTouchingAWall ? fixedMoveSpeed / 4 : fixedMoveSpeed;
        Debug.Log($"Move Speed is {moveSpeed}");
        playerController.Move(move * Time.fixedDeltaTime * moveSpeed);
    }

    void WallCheck()
    {
        isTouchingAWall = false;
        foreach (Collider other in Physics.OverlapCapsule(transform.position + new Vector3(0,0.4f,0), transform.position + new Vector3(0,-0.4f,0), 0.5f))
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                isTouchingAWall = true;
                break;
            }
        }
    }


    //! ALL UNUSED CODE BELOW HERE !!
    void OnCollisionEnter(Collision other)
    {
        ////Debug.Log($"OnCollisionEnter with {other.gameObject.name}");
        if (other.gameObject.CompareTag("Wall"))
        {
            ////isTouchingAWall = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        ////Debug.Log($"OnCollisionExit with {other.gameObject.name}");
        if (other.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(LeftAWall());
        }
    }

    IEnumerator LeftAWall()
    {
        yield return new WaitForSeconds(0.2f);
        ////isTouchingAWall = false;
    }

    public void OnMove(InputValue inputValue)
    {
        ////Debug.Log("OnMove Called");
        ////Vector2 inputVec = inputValue.Get<Vector2>();
        ////playerController.Move(new Vector3(inputVec.x, this.gameObject.transform.localPosition.y, inputVec.y));
    }
}
