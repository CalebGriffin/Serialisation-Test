using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LevelSetup : MonoBehaviour
{
    [SerializeField] private GameObject playerObj, goalObj, wallParentObj, wallPrefab, menuCanvas, countdownCanvas, enterNameCanvas;
    [SerializeField] private TextMeshProUGUI countdownText;

    private string[] urlArr = 
    {
        "https://api.npoint.io/90f405491ba458f129c5"
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
        menuCanvas.SetActive(true);
        playerObj.GetComponent<PlayerMovement>().Reset();
    }

    public void SetupLevel(int levelIndex)
    {
        // Start the loading animation
        playerObj.GetComponent<PlayerMovement>().DisableControls();
        playerObj.GetComponent<PlayerMovement>().SetSelectedLevel(levelIndex);
        menuCanvas.SetActive(false);
        countdownCanvas.SetActive(true);

        // Start the coroutine to download the level
        StartCoroutine(GetRequest(urlArr[levelIndex-1]));
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

            // Stop the loading animation
            BallManager.instance.GetWallPos();
            BallManager.instance.SpawnBall();
            StartCoroutine(Countdown());
        }
    }

    void MoveThePlayer(Vector3 to)
    {
        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = to;
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
            Instantiate(wallPrefab, pos, Quaternion.identity, wallParentObj.transform);
        }
    }

    IEnumerator Countdown()
    {
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