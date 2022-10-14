using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public Vector2[] positions;
    public float[] rotations;

    private int frameNo = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        frameNo++;
        if (frameNo / 10 == 0)
        {
            // Run ghost movement and rotation
            frameNo = 0;
        }

    }
}
