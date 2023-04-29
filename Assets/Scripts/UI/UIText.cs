using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIText : MonoBehaviour
{
    private TMP_Text m_text;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    public void UpdateUI(string text)
    {
        m_text.text = text;
    }

    public void UpdateUI(int number)
    {
        m_text.text = number.ToString();
    }

    public void UpdateUI(float number)
    {
        m_text.text = number.ToString();
    }
}
