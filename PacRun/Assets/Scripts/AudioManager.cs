using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Sprite[] volumeSprites = new Sprite[4];

    public Image volumeImage;

    public void SetVolume(Slider slider)
    {
        SetVolume(slider.value);
    }

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("musicVol", volume);
        
        switch (volume)
        {
            case -80f:
                volumeImage.GetComponent<Image>().sprite = volumeSprites[0];
                break;

            case float x when (x > -80f && x <= -54):
                volumeImage.GetComponent<Image>().sprite = volumeSprites[1];
                break;
            
            case float x when (x > -54 && x <= -28):
                volumeImage.GetComponent<Image>().sprite = volumeSprites[2];
                break;
            
            case float x when (x > -28f && x <= 0f):
                volumeImage.GetComponent<Image>().sprite = volumeSprites[3];
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
