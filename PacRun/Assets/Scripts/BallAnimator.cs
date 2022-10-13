using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnimator : MonoBehaviour
{
    private float animTime = 0.6f;

    private float rotateSpeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        Up();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void Up()
    {
        LeanTween.moveY(this.gameObject, 0.4f, animTime).setOnComplete(() => Down());
    }

    private void Down()
    {
        LeanTween.moveY(this.gameObject, -0.2f, animTime).setOnComplete(() => Up());
    }

    private void Rotate()
    {
        this.gameObject.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
    }
}
