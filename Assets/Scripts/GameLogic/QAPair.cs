using System.Collections.Generic;

[System.Serializable]
public class QAPair
{
    public string question;
    public List<string> promptOptions;
    public int correctIndex;
}

[System.Serializable]
public class QAPairList
{
    public List<QAPair> questions;
}

