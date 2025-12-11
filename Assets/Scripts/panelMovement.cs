using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(-100)]
public class park : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject belajarPanel;
    public GameObject pilihPanel;
    public GameObject kuisPanel;
    public GameObject benarPanel;
    public GameObject salahPanel;
    public GameObject popupPanel;

void Start()
    {
        menuPanel.SetActive(true);
        belajarPanel.SetActive(false);
        pilihPanel.SetActive(false);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

public class SceneLogger : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Scene aktif: " + SceneManager.GetActiveScene().name);
        }
    }


    public void backButton1()
    {
        menuPanel.SetActive(true);
        belajarPanel.SetActive(false);
        pilihPanel.SetActive(false);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

    public void backButton2()
    {
        menuPanel.SetActive(false);
        belajarPanel.SetActive(false);
        pilihPanel.SetActive(true);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

    public void backButton3()
    {
        menuPanel.SetActive(false);
        belajarPanel.SetActive(true);
        pilihPanel.SetActive(false);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }


    public void belajarButton()
    {
        menuPanel.SetActive(false);
        belajarPanel.SetActive(true);
        pilihPanel.SetActive(false);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

        public void pilihButton()
    {
        menuPanel.SetActive(false);
        belajarPanel.SetActive(false);
        pilihPanel.SetActive(true);
        kuisPanel.SetActive(false);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }

    public void kuisButton()
    {
        menuPanel.SetActive(false);
        belajarPanel.SetActive(false);
        pilihPanel.SetActive(false);
        kuisPanel.SetActive(true);
        benarPanel.SetActive(false);
        salahPanel.SetActive(false);
        popupPanel.SetActive(false);
    }
     public void backButton0(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    

}

