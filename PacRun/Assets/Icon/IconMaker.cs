using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconMaker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Make Icon")]
    void CamCapture()
    {
        Camera camera = GetComponent<Camera>();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;

        var bytes = image.EncodeToPNG();
        DestroyImmediate(image);

        File.WriteAllBytes(Application.dataPath + "/Icon/icon.png", bytes);
    }

    [ContextMenu("Make Icon 2")]
    void Screenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Icon/icon.png");
    }
}
