using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class park : MonoBehaviour
{
    public GameObject parkPanel;
    public GameObject belajarPanel;
    public GameObject pilihPanel;
    public GameObject kuisPanel;
    public GameObject popupPanel;

    private int parkClickCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void ShowOnly(GameObject panel)
{
    parkPanel.SetActive(false);
    pilihPanel.SetActive(false);
    belajarPanel.SetActive(false);
    kuisPanel.SetActive(false);

    if (panel != null) panel.SetActive(true);
}

public void backButton1()
{
    ShowOnly(parkPanel);
}

public void backButton2()
{
    ShowOnly(pilihPanel);
}

public void belajarButton()
{
    ShowOnly(belajarPanel);
}

public void kuisButton()
{
    ShowOnly(kuisPanel);
}

}

