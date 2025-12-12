using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearnPanelManager : MonoBehaviour
{
    [System.Serializable]
    public class LearnItemData
    {
        public Sprite popupMainSprite;    // Gambar besar untuk popup
        public Sprite popupLabelSprite;   // Gambar label (nama)

        [Header("Audio")]
        public AudioClip voiceClip;       // Audio: "Sapi Cow Moo"
    }

    [Header("Data Belajar")]
    public List<LearnItemData> allItems = new List<LearnItemData>();

    [Header("UI Popup")]
    public GameObject popupPanel;
    public Image popupMainImage;
    public Image popupLabelImage;

    [Header("Audio Player")]
    public AudioSource audioSource;       // Taruh AudioSource di GameObject ini

    private int currentIndex = -1;        // simpan item yang sedang terbuka

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Kalau lupa drag audioSource, auto ambil dari GameObject yang sama
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // Dipanggil oleh button: ShowItem(index)
    public void ShowItem(int index)
    {
        if (index < 0 || index >= allItems.Count)
        {
            Debug.LogWarning("Index item di luar range: " + index);
            return;
        }

        currentIndex = index;

        LearnItemData data = allItems[index];

        // Update gambar popup
        if (popupMainImage != null)
            popupMainImage.sprite = data.popupMainSprite;

        if (popupLabelImage != null)
            popupLabelImage.sprite = data.popupLabelSprite;

        // Tampilkan popup
        if (popupPanel != null)
            popupPanel.SetActive(true);

        // Auto play audio saat popup muncul
        PlayCurrentAudio();
    }

    // Dipanggil oleh tombol di popup: "Play Lagi"
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

    // (Opsional) tombol close popup
    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}
