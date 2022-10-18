using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public Vector2[] positions;
    public float[] rotations;

    public static GhostController instance;
    public PlayerMovement playerScript;

    private int frameNo = 0;
    private int moveCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetGhostData()
    {
        positions = new Vector2[0];
        rotations = new float[0];
        frameNo = 0;
        moveCount = 0;
    }

    public void SetGhostData(Vector2[] positions, float[] rotations)
    {
        this.positions = positions;
        this.rotations = rotations;
    }

    void FixedUpdate()
    {
        if (!playerScript.controlsEnabled)
            return;

        frameNo++;
        if (frameNo / 5 == 0)
        {
            // Run ghost movement and rotation
            MoveAndRotate();
            frameNo = 0;
        }
    }

    void MoveAndRotate()
    {
        if (moveCount >= positions.Length)
        {
            this.gameObject.SetActive(false);
            return;
        }

        LeanTween.cancel(this.gameObject);

        // Move ghost to position
        Vector3 targetPosition = new Vector3(positions[moveCount].x, 0.3f, positions[moveCount].y);
        LeanTween.moveLocal(this.gameObject, targetPosition, 0.1f);

        // Rotate ghost to rotation
        LeanTween.rotateY(this.gameObject, rotations[moveCount], 0.1f);
        moveCount++;
    }
}
