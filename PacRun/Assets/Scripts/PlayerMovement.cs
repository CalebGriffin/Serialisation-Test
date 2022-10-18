using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController playerController;
    public GameObject enterNameCanvas;
    public Button submitButton;
    public TMP_InputField inputField;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private List<Vector2> positions = new List<Vector2>();
    private List<float> rotations = new List<float>();

    private Vector2 currentInputVector, smoothInputVelocity;

    private float smoothInputSpeed = 0.1f;

    private float fixedMoveSpeed = 4f;

    public float timerTime = 0f;

    private float fixedTimeBetweenFootsteps = 0.15f;
    private int footstepFrameNo = 0;

    public GameObject[] footstepPrefabs;
    private bool leftFoot = true;

    private bool isTouchingAWall = false;
    public bool controlsEnabled = false;

    private bool timerRunning = false;

    private int ballsCollected = 0;
    private int ballTarget = 5;
    public int levelNo = 0;
    private int frameNo = 0;

    [SerializeField] private TextMeshProUGUI ballsText, timeText;

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
        UpdateTimer();
    }

    void UpdateTimer()
    {
        if (!timerRunning)
            return;
        timerTime += Time.deltaTime;
        timeText.text = $"{timerTime:N2}s";
    }

    void FixedUpdate()
    {
        HandleInput();

        if (!controlsEnabled)
            return;
        frameNo++;
        if (frameNo / 5 == 0)
        {
            SavePositionAndRotation();
            frameNo = 0;
        }

    }

    void HandleInput()
    {
        if (!controlsEnabled)
            return;
        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input.magnitude > 0)
        {
            footstepFrameNo++;
            float timeBetweenFootsteps = isTouchingAWall ? fixedTimeBetweenFootsteps * 4 : fixedTimeBetweenFootsteps;
            if (footstepFrameNo > 50 * timeBetweenFootsteps)
            {
                SpawnFootprint();
                footstepFrameNo = 0;
            }
        }
        currentInputVector = Vector2.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, -1f, currentInputVector.y);
        WallCheck();
        float moveSpeed = isTouchingAWall ? fixedMoveSpeed / 4 : fixedMoveSpeed;
        //Debug.Log($"Move Speed is {moveSpeed}");
        RotatePlayer(input);
        playerController.Move(move * Time.fixedDeltaTime * moveSpeed);
    }

    void SavePositionAndRotation()
    {
        positions.Add(new Vector2(transform.position.x, transform.position.z));
        rotations.Add(transform.rotation.eulerAngles.y);
    }

    public GhostData CreateGhostData()
    {
        GhostData ghostData = new GhostData
        {
            positions = positions.ToArray(),
            rotations = rotations.ToArray()
        };
        return ghostData;
    }

    void RotatePlayer(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        LeanTween.rotateLocal(this.gameObject, new Vector3(0,angle,0), 0.1f);
    }

    void WallCheck()
    {
        isTouchingAWall = false;
        foreach (Collider other in Physics.OverlapCapsule(transform.position + new Vector3(0,0.4f,0), transform.position + new Vector3(0,-0.4f,0), 0.45f))
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                isTouchingAWall = true;
                break;
            }
        }
    }

    void SpawnFootprint()
    {
        GameObject prefab = leftFoot ? footstepPrefabs[0] : footstepPrefabs[1];
        GameObject.Instantiate(prefab, new Vector3(transform.position.x, -0.49f, transform.position.z), Quaternion.Euler(90, transform.rotation.eulerAngles.y - 180f, 180));
        leftFoot = !leftFoot;
    }

    public void TimerRunning(bool shouldBeRunning)
    {
        timerRunning = shouldBeRunning;
    }

    public void SetSelectedLevel(int levelNo)
    {
        this.levelNo = levelNo;
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Ball":
                Destroy(other.gameObject);
                BallCollected();
                break;
            
            case "Goal":
                TimerRunning(false);
                DisableControls();
                StartCoroutine(GameOver());
                break;
            
            default:
                break;
        }
    }

    void BallCollected()
    {
        ballsCollected++;
        ballsText.text = $"{ballsCollected:D1}/{ballTarget}";
        if (ballsCollected >= ballTarget)
        {
            GoalAchieved();
            return;
        }
        BallManager.instance.SpawnBall();
    }

    void GoalAchieved()
    {
        LockAnimator.instance.Disappear();
    }

    public void DisableControls()
    {
        controlsEnabled = false;
    }

    public void EnableControls()
    {
        controlsEnabled = true;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);

        submitButton.interactable = true;
        inputField.interactable = true;
        enterNameCanvas.SetActive(true);
    }

    public void Reset()
    {
        ballsCollected = 0;
        levelNo = 0;
        timerTime = 0f;
        positions.Clear();
        rotations.Clear();
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
