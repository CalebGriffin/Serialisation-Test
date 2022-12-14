using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAnimator : MonoBehaviour
{
    private float animTime = 0.8f;

    public static LockAnimator instance;

    // Start is called before the first frame update
    void Start()
    {
        Up();
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
    
    private void Up()
    {
        LeanTween.moveY(this.gameObject, 0.1f, animTime).setOnComplete(() => Down());
    }

    private void Down()
    {
        LeanTween.moveY(this.gameObject, -0.1f, animTime).setOnComplete(() => Up());
    }

    [ContextMenu("Disappear")]
    public void Disappear()
    {
        LeanTween.scale(this.gameObject, Vector3.zero, animTime).setOnComplete(() => 
        {
            this.gameObject.SetActive(false);
        });
    }

    public void Reset()
    {
        this.gameObject.transform.localScale = Vector3.one;
        this.gameObject.SetActive(true);
    }
}
