using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Helper : MonoBehaviour
{
    [System.Serializable]
    public class Line
    {
        [TextArea]
        public string line;
        public float lineTime;
    }
    [SerializeField] Line[] lines;
    [SerializeField] TMP_Text text;
    [SerializeField] int currentLine = -1;

    IEnumerator coroutine;
    private void Start()
    {
        text.text = "F to interact";
    }
    public void Talk()
    {
        currentLine++;

        if(currentLine >= lines.Length)
        {
            currentLine = -1;
            text.text = "F to interact";
            return;
        }

        text.text = lines[currentLine].line;
    }
    IEnumerator TalkRoutine()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            text.text = lines[i].line;
            yield return new WaitForSeconds(lines[i].lineTime);
        }
        text.text = "F to interact";
    }
}
