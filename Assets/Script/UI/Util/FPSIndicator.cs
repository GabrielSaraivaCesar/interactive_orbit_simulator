using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class FPSIndicator
{

    private static GameObject _fpsTextElement;
    private static List<int> _fpsList = new List<int>();
    private static TextMeshProUGUI _textMeshPro;

    public static void setUp(GameObject textElement)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 10000;
        _fpsTextElement = textElement;
        _textMeshPro = _fpsTextElement.GetComponent<TextMeshProUGUI>();
    }

    public static void updateFPS()
    {
        _fpsList.Add((int)(1.0f / Time.deltaTime));
        if (_fpsList.Count > 20)
        {
            _fpsList.RemoveAt(0);
        }
        int meanFPS = 0;
        foreach (int fps in FPSIndicator._fpsList)
        {
            meanFPS += fps;
        }
        meanFPS = (int)(meanFPS / FPSIndicator._fpsList.Count);
        _textMeshPro.text = meanFPS.ToString();
    }
}
