using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;
using System;

public class ModelMindGameManager : MonoBehaviour
{
    [Header("Settings")]
    public string jsonFileName = "ModelMind.json";
    public float questionTime = 10f;

    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text[] optionTexts; // Size 3
    public Button[] optionButtons; // Size 3
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject comingSoonPanel;

    private List<QAPair> _allPairs;
    private int currentQuestionIndex = 0;
    private int selectedOption = -1;
    private int score = 0;
    private float timer;
    private bool inputAllowed = true;

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
            QAPairList data = JsonUtility.FromJson<QAPairList>(json);
            _allPairs = data.questions;
        }
        catch (Exception ex)
        {
            Debug.LogError("JSON parse error: " + ex.Message);
            yield break;
        }

        if (_allPairs == null || _allPairs.Count < 1)
        {
            Debug.LogWarning("Not enough questions. Showing Coming Soon.");
            comingSoonPanel.SetActive(true);
            yield break;
        }

        SetupNextRound();
    }

    private void SetupNextRound()
    {
        if (currentQuestionIndex >= _allPairs.Count)
        {
            EndGame();
            return;
        }

        inputAllowed = true;
        selectedOption = -1;
        timer = questionTime;

        QAPair pair = _allPairs[currentQuestionIndex];
        questionText.text = pair.question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionTexts[i].text = pair.promptOptions[i];
            int index = i;
            optionButtons[i].interactable = true;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }

        StartCoroutine(CountdownAndSubmit());
    }

    private void OnOptionSelected(int index)
    {
        if (!inputAllowed) return;

        selectedOption = index;
        inputAllowed = false;

        foreach (var btn in optionButtons)
            btn.interactable = false;
    }

    private IEnumerator CountdownAndSubmit()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString("0");
            yield return null;
        }

        SubmitAnswer();
    }

    private void SubmitAnswer()
    {
        QAPair pair = _allPairs[currentQuestionIndex];

        if (selectedOption == pair.correctIndex)
        {
            score++;
        }

        scoreText.text = $"Ldksj {score}";
        currentQuestionIndex++;

        Invoke(nameof(SetupNextRound), 1.5f);
    }

    private void EndGame()
    {
        questionText.text = "[ksy lekIr!";
        timerText.text = "";
        foreach (var btn in optionButtons)
            btn.gameObject.SetActive(false);
    }
}
