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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        homePanel.SetActive(true);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(false);
    }

    // Update is called once per frame
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
    }

    public void kreditButton()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(true);
        pilihPanel.SetActive(false);
    }

    public void pilihButton()
    {
        homePanel.SetActive(false);
        kreditPanel.SetActive(false);
        pilihPanel.SetActive(true);
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

