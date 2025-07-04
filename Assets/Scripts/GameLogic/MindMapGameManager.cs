using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

[Serializable]
public class ReverseBotQuestion
{
    public string output;
    public List<string> options;
    public int correctIndex;
}

[Serializable]
public class ReverseBotQuestionList
{
    public List<ReverseBotQuestion> questions;
}

public class MindMapGameManager : MonoBehaviour
{
    [Header("Settings")]
    public string jsonFileName = "MindMapAI.json";
    public float questionTime = 10f;

    [Header("UI References")]
    public TMP_Text outputText;
    public TMP_Text[] optionTexts; // Size = 3
    public Button[] optionButtons; // Size = 3
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject comingSoonPanel;

    private List<ReverseBotQuestion> allQuestions;
    private int currentQuestionIndex = 0;
    private int selectedOption = -1;
    private int score = 0;
    private float timer;
    private bool inputAllowed = true;
    private Coroutine countdownCoroutine;


    private void Start()
    {
        StartCoroutine(LoadQuestions());
    }

    private IEnumerator LoadQuestions()
    {
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string json = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
        using var www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load JSON: " + www.error);
            yield break;
        }
        json = www.downloadHandler.text;
#else
        if (File.Exists(path)) json = File.ReadAllText(path);
        else
        {
            Debug.LogError("JSON not found: " + path);
            yield break;
        }
#endif

        try
        {
            ReverseBotQuestionList data = JsonUtility.FromJson<ReverseBotQuestionList>(json);
            allQuestions = data.questions;

            Debug.Log($"✅ Loaded {allQuestions.Count} questions.");
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON parse error: " + ex.Message);
            yield break;
        }

        // 🔐 Null or empty check before proceeding
        if (allQuestions == null || allQuestions.Count < 1)
        {
            Debug.LogWarning("❌ No questions found. Showing Coming Soon panel.");
            comingSoonPanel.SetActive(true);
            yield break;
        }

        SetupNextRound();
    }

    private void SetupNextRound()
    {
        if (currentQuestionIndex >= allQuestions.Count)
        {
            EndGame();
            return;
        }

        inputAllowed = true;
        selectedOption = -1;
        timer = questionTime;

        ReverseBotQuestion q = allQuestions[currentQuestionIndex];
        outputText.text = q.output;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionTexts[i].text = q.options[i];
            int index = i;
            optionButtons[i].interactable = true;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }

        countdownCoroutine = StartCoroutine(CountdownAndSubmit());
    }

    private void OnOptionSelected(int index)
    {
        if (!inputAllowed) return;

        selectedOption = index;
        inputAllowed = false;

        // Disable buttons
        foreach (var btn in optionButtons)
            btn.interactable = false;

        // Stop timer coroutine if it's still running
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        StartCoroutine(DelayedSubmit());
    }

    private IEnumerator CountdownAndSubmit()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString("0");
            yield return null;
        }

        StartCoroutine(DelayedSubmit());
    }

    private IEnumerator DelayedSubmit()
    {
        yield return new WaitForSeconds(0.5f);
        SubmitAnswer();
    }

    private void SubmitAnswer()
    {
        ReverseBotQuestion q = allQuestions[currentQuestionIndex];

        if (selectedOption == q.correctIndex)
        {
            score++;
        }

        scoreText.text = $"vad {score}";
        currentQuestionIndex++;

        Invoke(nameof(SetupNextRound), 1.5f);
    }

    private void EndGame()
    {
        outputText.text = "खेल समाप्त!";
        timerText.text = "";
        foreach (var btn in optionButtons)
            btn.gameObject.SetActive(false);
    }
}
