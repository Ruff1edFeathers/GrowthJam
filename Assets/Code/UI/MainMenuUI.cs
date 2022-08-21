using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
