using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearnPanelManager : MonoBehaviour
{
    [System.Serializable]
    public class LearnItemData
    {
        public Sprite popupMainSprite; 
        public Sprite popupLabelSprite;

        [Header("Audio")]
        public AudioClip voiceClip;
    }

    [Header("Data Belajar")]
    public List<LearnItemData> allItems = new List<LearnItemData>();

    [Header("UI Popup")]
    public GameObject popupPanel;
    public Image popupMainImage;
    public Image popupLabelImage;

    [Header("Audio Player")]
    public AudioSource audioSource; 

    private int currentIndex = -1; 

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    public void ShowItem(int index)
    {
        if (index < 0 || index >= allItems.Count)
        {
            Debug.LogWarning("Index item di luar range: " + index);
            return;
        }

        currentIndex = index;

        LearnItemData data = allItems[index];


        if (popupMainImage != null)
            popupMainImage.sprite = data.popupMainSprite;

        if (popupLabelImage != null)
            popupLabelImage.sprite = data.popupLabelSprite;


        if (popupPanel != null)
            popupPanel.SetActive(true);

        PlayCurrentAudio();
    }

    public void PlayCurrentAudio()
    {
        if (currentIndex < 0 || currentIndex >= allItems.Count) return;

        var data = allItems[currentIndex];
        if (data.voiceClip == null) return;

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource belum di-assign / belum ada di GameObject.");
            return;
        }

        audioSource.Stop();
        audioSource.PlayOneShot(data.voiceClip);
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}
