using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private GameObject playerObj, goalObj, ghostObj, wallParentObj, wallPrefab, menuCanvas, countdownCanvas, enterNameCanvas;

    private List<GameObject> wallObjs = new List<GameObject>();

    public Button[] buttons;

    [SerializeField] private TextMeshProUGUI countdownText;
    public Slider loadingSlider;

    private bool ghostToggle = true;
    private bool gotGhostData = false;

    private string[] urlArr = 
    {
        "https://api.npoint.io/90f405491ba458f129c5",
        "https://api.npoint.io/c351c113039fd3e19820",
        ////"https://api.npoint.io/e97033d5aa37f57085b3"
        "https://api.npoint.io/14792a6c2883e6c97bb7"
    };

    private string[] ghostUrlArr = 
    {
        "https://getpantry.cloud/apiv1/pantry/5c578090-afce-4b12-9093-2a34de34a1a8/basket/levelOneGhost",
        "https://getpantry.cloud/apiv1/pantry/5c578090-afce-4b12-9093-2a34de34a1a8/basket/levelTwoGhost",
        "https://getpantry.cloud/apiv1/pantry/5c578090-afce-4b12-9093-2a34de34a1a8/basket/levelThreeGhost"
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMenu()
    {
        enterNameCanvas.SetActive(false);
        countdownCanvas.SetActive(false);
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
        menuCanvas.SetActive(true);
        LockAnimator.instance.Reset();
        playerObj.GetComponent<PlayerMovement>().Reset();
        ghostObj.SetActive(true);
        foreach (GameObject wall in wallObjs)
        {
            Destroy(wall);
        }
    }

    public void SetupLevel(int levelIndex)
    {
        buttons[levelIndex-1].interactable = false;

        // Start the loading animation
        playerObj.GetComponent<PlayerMovement>().DisableControls();
        playerObj.GetComponent<PlayerMovement>().SetSelectedLevel(levelIndex);
        countdownText.text = "On Your Marks...";
        menuCanvas.SetActive(false);
        countdownCanvas.SetActive(true);

        // Start the coroutine to download the level
        StartCoroutine(GetRequest(urlArr[levelIndex-1]));
        if (ghostToggle)
            StartCoroutine(GetGhostRequest(ghostUrlArr[levelIndex-1]));
        else
            ghostObj.SetActive(false);
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length -1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Web request error occurred");
                yield break;
            }

            ////Debug.Log($"Test of the page is: {webRequest.downloadHandler.text}");
            string textToParse = webRequest.downloadHandler.text;
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(textToParse);

            MoveThePlayer(saveObject.playerPos);
            MoveTheGoal(saveObject.goalPos);
            SetupTheWalls(saveObject.wallPosArr);

            if (ghostToggle)
                yield return new WaitUntil(() => gotGhostData);

            // Stop the loading animation
            BallManager.instance.GetWallPos();
            BallManager.instance.SpawnBall();
            StartCoroutine(Countdown());
        }
    }

    IEnumerator GetGhostRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length -1;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Web request error occurred");
                yield break;
            }

            string textToParse = webRequest.downloadHandler.text;
            GhostData ghostData = JsonUtility.FromJson<GhostData>(textToParse);

            GhostController.instance.SetGhostData(ghostData.positions, ghostData.rotations);

            gotGhostData = true;
        }
    }

    public void SubmitGhostData(int levelIndex)
    {
        StartCoroutine(ClearGhostData(ghostUrlArr[levelIndex-1]));
    }

    IEnumerator ClearGhostData(string url)
    {
        string json = "{\"positions\":[],\"rotations\":[]}";
        UnityWebRequest webRequest = UnityWebRequest.Put(url, json);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Web request error occurred");
            yield break;
        }

        StartCoroutine(SetGhostRequest(url));
    }

    IEnumerator SetGhostRequest(string url)
    {
        GhostData newGhostData = playerObj.GetComponent<PlayerMovement>().CreateGhostData();
        string json = JsonUtility.ToJson(newGhostData);
        Debug.Log(json);
        UnityWebRequest webRequest = UnityWebRequest.Put(url, json);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Web request error occurred");
            yield break;
        }
        Debug.Log(webRequest.downloadHandler.text);

        Debug.Log("Ghost data submitted");
    }

    public void GhostToggle(Toggle toggle)
    {
        ghostToggle = toggle.isOn;
    }

    void MoveThePlayer(Vector3 to)
    {
        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = to;
        ghostObj.transform.position = to;
        playerObj.GetComponent<CharacterController>().enabled = true;
    }

    void MoveTheGoal(Vector3 to)
    {
        goalObj.transform.position = to;
    }

    void SetupTheWalls(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            GameObject go = Instantiate(wallPrefab, pos, Quaternion.identity, wallParentObj.transform);
            wallObjs.Add(go);
        }
    }

    IEnumerator Countdown()
    {
        LeanTween.value(loadingSlider.gameObject, 0, 100, 3f).setOnUpdate((float val) =>
        {
            loadingSlider.value = val;
        });
        yield return new WaitForSeconds(1f);
        countdownText.text = "Get Set...";
        yield return new WaitForSeconds(1f);
        countdownText.text = "GO!!!";
        yield return new WaitForSeconds(1f);
        countdownCanvas.SetActive(false);
        playerObj.GetComponent<PlayerMovement>().EnableControls();
        playerObj.GetComponent<PlayerMovement>().TimerRunning(true);
    }
}
