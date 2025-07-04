using System.Collections.Generic;

[System.Serializable]
public class PromptQuestion
{
    public string question;
    public List<string> promptOptions;
    public int correctIndex;
}

[System.Serializable]
public class PromptQuestionList
{
    public List<PromptQuestion> questions;
}
