using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject kreditPanel;
    public GameObject pilihPanel;
    public GameObject asetPanel;

    void Start()
    {
        homePanel.SetActive(true);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(false);
        asetPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public class SceneLogger : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("Scene aktif: " + SceneManager.GetActiveScene().name);
        }
    }


    public void backButton()
    {
        homePanel.SetActive(true);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(false);
        asetPanel.SetActive(false);
    }

    public void kreditButton()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(true);
        pilihPanel.SetActive(false);
        asetPanel.SetActive(false);
    }

    public void pilihButton()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(true);
        asetPanel.SetActive(false);
    }
    public void asetButton()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(false);
        asetPanel.SetActive(true);
    }
    public void backButton1()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(true);
        pilihPanel.SetActive(false);
        asetPanel.SetActive(false);
    }

     public void zooButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

     public void parkButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

     public void marketButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
}

