using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    dreamloLeaderBoard dl;
    public PlayerMovement playerScript;
    public TMP_InputField inputField;
    public GameObject leaderboardObj;

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

    public void SubmitScore()
    {
        if (inputField.text == "")
            return;
        this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard(playerScript.levelNo);
        Debug.Log(this.dl);
        dl.AddScore(inputField.text, (int)(100000-(playerScript.timerTime * 100)));
    }

    public void ShowLeaderboard(int levelNo)
    {
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
        foreach (dreamloLeaderBoard.Score currentScore in scoreList)
        {
            textList[count].text = $"{count+1}. {currentScore.playerName}";
            Debug.Log(currentScore.playerName + ": " + currentScore.score.ToString());
            float time = (100000f - currentScore.score) / 100f;
            Debug.Log(time);
            textList[count+1].text = $"{time:N2}";

            count += 2;
            if (count >= textList.Count)
                break;
        }

        leaderboardObj.SetActive(true);
    }

    public void HideLeaderboard()
    {
        int count = 0;
        leaderboardObj.SetActive(false);
        foreach (TextMeshProUGUI text in textList)
        {
            textList[count].text = $"{count+1}. Blank Name";
            textList[count+1].text = "00.00";

            count += 2;
            if (count >= textList.Count)
                break;
        }
    }
}
