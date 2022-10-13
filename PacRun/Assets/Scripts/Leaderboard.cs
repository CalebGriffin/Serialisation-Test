using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    dreamloLeaderBoard dl;
    public PlayerMovement playerScript;
    public LevelSetup levelSetup;
    public TMP_InputField inputField;
    public GameObject leaderboardObj, textParentObj, nameWarningObj;

    private float sideMargin = 300f, bottomMargin = 150f;

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
        if (inputField.text.Contains("*"))
        {
            GrowText();
            return;
        }
        this.dl = dreamloLeaderBoard.GetSceneDreamloLeaderboard(playerScript.levelNo);
        Debug.Log(this.dl);
        dl.AddScore(inputField.text, (int)(100000-(playerScript.timerTime * 100)));
        levelSetup.BackToMenu();
    }

    public void TextValidation()
    {
        if (inputField.text.Contains("*"))
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
