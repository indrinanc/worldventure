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
    }

    [Header("Data Belajar")]
    public List<LearnItemData> allItems = new List<LearnItemData>();

    [Header("UI Popup")]
    public GameObject popupPanel;  
    public Image popupMainImage;
    public Image popupLabelImage;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false); // Popup tidak muncul diawal
    }

    // Dipanggil oleh button: ShowItem( index )
    public void ShowItem(int index)
    {
        if (index < 0 || index >= allItems.Count)
        {
            Debug.LogWarning("Index item di luar range: " + index);
            return;
        }

        LearnItemData data = allItems[index];

        // Update gambar popup
        if (popupMainImage != null)
            popupMainImage.sprite = data.popupMainSprite;

        if (popupLabelImage != null)
            popupLabelImage.sprite = data.popupLabelSprite;

        // Tampilkan popup
        popupPanel.SetActive(true);
    }

}
