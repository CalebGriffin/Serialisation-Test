using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAnimator : MonoBehaviour
{
    private float animTime = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        Up();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Up()
    {
        LeanTween.moveLocalY(this.gameObject, 0.1f, animTime).setOnComplete(() => Down());
    }

    private void Down()
    {
        LeanTween.moveLocalY(this.gameObject, -0.1f, animTime).setOnComplete(() => Up());
    }
}
