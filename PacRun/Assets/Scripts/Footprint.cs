using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Footprint : MonoBehaviour
{
    private float waitTime = 0.3f;
    private float fadeTime = 0.7f;

    public Image footprintImage;

    void Awake()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        Fade();
    }

    void Fade()
    {
        LeanTween.value(this.gameObject, 1f, 0f, fadeTime).setOnUpdate((float val) =>
        {
            footprintImage.color = new Color(1, 1, 1, val);
        }).setOnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
