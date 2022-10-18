using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    dreamloLeaderBoard dl;
    public PlayerMovement playerScript;
    public LevelSetup levelSetup;
    public TMP_InputField inputField;
    public GameObject leaderboardObj, textParentObj, loadingIconObj, nameWarningObj;

    private float sideMargin = 300f, bottomMargin = 150f;
    private float currentHighestScore = Mathf.Infinity;
    private char[] bannedChars = {'*', '/', '|', '+'};
    
    public List<TextMeshProUGUI> textList;

    public static Leaderboard instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SubmitScore(Button button)
    {
        if (inputField.text == "")
            return;
        if (inputField.text.ContainsAny(bannedChars))
        {
            GrowText();
            return;
        }
        button.interactable = false;
        inputField.interactable = false;
        this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard(playerScript.levelNo);
        Debug.Log(this.dl);

        // Check if the current score is higher than the highest score
        dl.GetScores();
        StartCoroutine(CheckHighestScore());

        dl.AddScore(inputField.text, (int)(100000-(playerScript.timerTime * 100)));
    }

    IEnumerator CheckHighestScore()
    {
        yield return new WaitUntil(() => dl.highScores != "");
        List<dreamloLeaderBoard.Score> scoreList = dl.ToListHighToLow();
        Debug.Log(scoreList.Count);
        if (scoreList.Count == 0)
            currentHighestScore = 0;
        else
            currentHighestScore = scoreList[0].score;
        float currentFastestTime = (100000 - currentHighestScore) / 100f;
        Debug.Log("Current fastest time: " + currentFastestTime);
        Debug.Log("Player time: " + playerScript.timerTime);
        if (playerScript.timerTime < currentFastestTime)
        {
            Debug.Log("Submitting Ghost Data");
            // Submit the ghost data
            levelSetup.SubmitGhostData(playerScript.levelNo);
        }
        levelSetup.BackToMenu();
    }

    public void TextValidation()
    {
        if (inputField.text.ContainsAny(bannedChars))
        {
            nameWarningObj.SetActive(true);
        }
        else
        {
            nameWarningObj.SetActive(false);
            nameWarningObj.GetComponent<TextMeshProUGUI>().margin = new Vector4(300, 0, 300, 150);
        }
    }


    private void GrowText()
    {
        if (sideMargin >= 10)
        {
            sideMargin -= 10;
            bottomMargin -= 5;
        }
        nameWarningObj.GetComponent<TextMeshProUGUI>().margin = new Vector4(sideMargin, 0, sideMargin, bottomMargin);
    }

    public void ShowLeaderboard(int levelNo)
    {
        textParentObj.SetActive(false);
        loadingIconObj.SetActive(true);
        leaderboardObj.SetActive(true);
        this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard(levelNo);
        dl.GetScores();
        Debug.Log(dl);
        StartCoroutine(WaitForScores());
    }

    IEnumerator WaitForScores()
    {
        yield return new WaitUntil(() => dl.highScores != "");
        List<dreamloLeaderBoard.Score> scoreList = dl.ToListHighToLow();
        int count = 0;
        int num = 1;
        foreach (dreamloLeaderBoard.Score currentScore in scoreList)
        {
            textList[count].text = $"{num}. {UnityWebRequest.UnEscapeURL(currentScore.playerName)}";
            Debug.Log(currentScore.playerName + ": " + currentScore.score.ToString());
            float time = (100000f - currentScore.score) / 100f;
            Debug.Log(time);
            textList[count+1].text = $"{time:N2}s";

            num++;
            count += 2;
            if (count >= textList.Count)
                break;
        }

        yield return new WaitForSeconds(0.5f);
        loadingIconObj.SetActive(false);
        textParentObj.SetActive(true);
    }

    public void HideLeaderboard()
    {
        int count = 0;
        int num = 1;
        leaderboardObj.SetActive(false);
        foreach (TextMeshProUGUI text in textList)
        {
            textList[count].text = $"{num}. Blank Name";
            textList[count+1].text = "00.00s";

            num++;
            count += 2;
            if (count >= textList.Count)
                break;
        }
    }
}

public static partial class Extensions
{
    public static bool ContainsAny(this string @this, params char[] values)
    {
        foreach (char value in values)
        {
            if (@this.Contains(value))
            {
                return true;
            }
        }
        return false;
    }
}
