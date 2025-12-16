using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public enum QuizCategory { Hewan, Warna, Buah }

    [Header("Kategori (hanya untuk label, logika tetap sama)")]
    public QuizCategory category;

    [Header("Data Pertanyaan")]
    public List<QuestionData> allQuestions;
    public List<Sprite> allAnswerOptions;

    [Header("UI Soal & Jawaban")]
    public Image questionImageUI;
    public Image[] answerImagesUI;     // 4 slot
    public Button[] answerButtons;     // 4 tombol

    [Header("Pengaturan Kuis")]
    public int maxQuestions = 9;

    [Header("Panel Feedback")]
    public GameObject correctPanel;    // HARUS root panel (Benar Farm Panel)
    public GameObject wrongPanel;      // HARUS root panel (Salah Farm Panel)

    [Header("Gambar di Panel Benar")]
    public Image correctAnswerImage;   // Image yang menampilkan hewan benar (kotak putih di tengah)

    [Header("Gambar di Panel Salah")]
    public Image wrongChosenImage;     // Image yang menampilkan pilihan user (contoh: Warna Salah)

    [Header("Panel Navigasi (opsional)")]
    public GameObject pilihPanel;
    public GameObject kuisPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private List<QuestionData> questionQueue;
    private QuestionData currentQuestion;
    private int currentQuestionIndex = -1;
    private int correctAnswerIndex = -1;
    private int score = 0;

    private List<Sprite> currentOptions;
    private bool quizStarted = false;

    void Awake()
    {
        // Pastikan referensi panel benar/salah mengarah ke ROOT panelnya
        EnsurePanelsAndImages();
    }

    void Start()
    {
        SetupQuiz();
        TryStartQuizIfPanelActive();
    }

    void Update()
    {
        if (!quizStarted)
            TryStartQuizIfPanelActive();
    }

    void EnsurePanelsAndImages()
    {
        // ====== AUTO FIND PANEL ROOT berdasarkan nama di Hierarchy ======
        // Nama sesuai screenshot kamu:
        if (correctPanel == null)
        {
            var go = GameObject.Find("Benar Farm Panel");
            if (go != null) correctPanel = go;
        }

        if (wrongPanel == null)
        {
            var go = GameObject.Find("Salah Farm Panel");
            if (go != null) wrongPanel = go;
        }

        // ====== AUTO FIND IMAGE yang benar untuk panel BENAR ======
        // Hindari salah ambil BG. Cari Image yang bukan BG/Background dan yang biasanya "kotak" tengah.
        if (correctAnswerImage == null && correctPanel != null)
        {
            correctAnswerImage = FindBestContentImage(correctPanel.transform);
        }

        // ====== AUTO FIND IMAGE untuk panel SALAH ======
        // Di screenshot ada child "Warna Salah" (kemungkinan image pilihan user)
        if (wrongChosenImage == null && wrongPanel != null)
        {
            // Prioritas: cari yang namanya mengandung "Warna" atau "Chosen"
            var img = FindImageByNameContains(wrongPanel.transform, "Warna");
            if (img == null) img = FindImageByNameContains(wrongPanel.transform, "Chosen");
            if (img == null) img = FindBestContentImage(wrongPanel.transform);

            wrongChosenImage = img;
        }

        // AudioSource auto ambil
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    Image FindImageByNameContains(Transform root, string contains)
    {
        var images = root.GetComponentsInChildren<Image>(true);
        foreach (var img in images)
        {
            if (img == null) continue;
            if (img.gameObject.name.ToLower().Contains(contains.ToLower()))
                return img;
        }
        return null;
    }

    Image FindBestContentImage(Transform root)
    {
        // Heuristik: ambil Image yang bukan BG/Background/Panel dan bukan Image yang ada di object root itu sendiri (biasanya container)
        var images = root.GetComponentsInChildren<Image>(true);

        foreach (var img in images)
        {
            if (img == null) continue;

            string n = img.gameObject.name.ToLower();
            if (n.Contains("bg") || n.Contains("background") || n.Contains("panel")) continue;

            // Kalau ada sprite kosong pun tetap boleh (kotak putih sering sprite-nya putih/empty),
            // yang penting bukan BG.
            return img;
        }

        // fallback: kalau semua dianggap BG, ambil yang pertama
        return images != null && images.Length > 0 ? images[0] : null;
    }

    void SetupQuiz()
    {
        quizStarted = false;
        currentQuestionIndex = -1;
        correctAnswerIndex = -1;
        score = 0;
        currentQuestion = null;
        currentOptions = null;

        EnsurePanelsAndImages(); // pastikan lagi setelah scene load

        if (allQuestions == null || allQuestions.Count == 0)
        {
            Debug.LogError($"[{name}] allQuestions kosong. Isi dulu di Inspector untuk kategori {category}.");
            return;
        }

        if (allAnswerOptions == null || allAnswerOptions.Count < 4)
        {
            Debug.LogError($"[{name}] allAnswerOptions minimal 4 sprite. Saat ini: {(allAnswerOptions == null ? 0 : allAnswerOptions.Count)}");
            return;
        }

        questionQueue = new List<QuestionData>(allQuestions);
        ShuffleList(questionQueue);

        int jumlahSoalDipakai = Mathf.Min(maxQuestions, questionQueue.Count);
        questionQueue = questionQueue.GetRange(0, jumlahSoalDipakai);

        if (correctPanel != null) correctPanel.SetActive(false);
        if (wrongPanel != null) wrongPanel.SetActive(false);

        SetAnswerButtonsInteractable(true);
    }

    void TryStartQuizIfPanelActive()
    {
        if (quizStarted) return;

        bool quizPanelActive = (kuisPanel == null) || kuisPanel.activeInHierarchy;
        if (!quizPanelActive) return;

        if (questionQueue == null || questionQueue.Count == 0) return;

        quizStarted = true;
        NextQuestion();
    }

    void NextQuestion()
    {
        if (correctPanel != null) correctPanel.SetActive(false);
        if (wrongPanel != null) wrongPanel.SetActive(false);

        currentQuestionIndex++;

        if (questionQueue == null || currentQuestionIndex >= questionQueue.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = questionQueue[currentQuestionIndex];

        if (questionImageUI != null)
            questionImageUI.sprite = currentQuestion.questionImage;

        // audio pertanyaan hanya saat kuisPanel aktif
        if (IsQuizPanelActive())
            PlayClip(currentQuestion.questionAudio);

        // ===== build opsi =====
        List<Sprite> options = new List<Sprite> { currentQuestion.correctAnswer };

        List<Sprite> wrongPool = new List<Sprite>(allAnswerOptions);
        wrongPool.Remove(currentQuestion.correctAnswer);
        ShuffleList(wrongPool);

        for (int i = 0; i < 3 && i < wrongPool.Count; i++)
            options.Add(wrongPool[i]);

        while (options.Count < 4 && wrongPool.Count > 0)
            options.Add(wrongPool[Random.Range(0, wrongPool.Count)]);

        ShuffleList(options);

        currentOptions = new List<Sprite>(options);
        correctAnswerIndex = options.IndexOf(currentQuestion.correctAnswer);

        for (int i = 0; i < 4; i++)
        {
            if (answerImagesUI != null && i < answerImagesUI.Length && answerImagesUI[i] != null)
                answerImagesUI[i].sprite = options[i];
        }

        SetAnswerButtonsInteractable(true);
    }

    public void OnAnswerSelected(int index)
    {
        if (index < 0 || index > 3) return;
        if (answerButtons == null || answerButtons.Length == 0) return;
        if (!answerButtons[0].interactable) return;

        if (index == correctAnswerIndex)
        {
            score++;
            SetAnswerButtonsInteractable(false);

            // ✅ FIX #1: BENAR panel tampilkan HEWAN BENAR (pakai correctAnswer)
            EnsurePanelsAndImages();
            if (correctAnswerImage != null && currentQuestion != null)
                correctAnswerImage.sprite = currentQuestion.correctAnswer;

            PlayClip(correctClip);

            if (correctPanel != null)
                correctPanel.SetActive(true);
        }
        else
        {
            // ✅ FIX #2: Pastikan panel salah benar-benar muncul (root panel)
            EnsurePanelsAndImages();

            // Ambil sprite yang dipilih user
            Sprite chosen = null;
            if (currentOptions != null && index >= 0 && index < currentOptions.Count)
                chosen = currentOptions[index];

            // Tampilkan sprite pilihan user di panel salah
            if (wrongChosenImage != null && chosen != null)
                wrongChosenImage.sprite = chosen;

            PlayClip(wrongClip);

            // Tampilkan panel salah dulu, baru nonaktifkan tombol (biar jelas responsnya)
            if (wrongPanel != null)
                wrongPanel.SetActive(true);

            // matikan tombol jawaban sampai user tekan "Coba Lagi"
            SetAnswerButtonsInteractable(false);
        }
    }

    public void OnNextButton()
    {
        if (correctPanel != null) correctPanel.SetActive(false);
        NextQuestion();
    }

    public void OnTryAgainButton()
    {
        // Tutup panel salah
        if (wrongPanel != null) wrongPanel.SetActive(false);

        // Aktifkan lagi tombol jawaban agar user bisa mencoba lagi
        SetAnswerButtonsInteractable(true);
    }

    void EndQuiz()
    {
        if (audioSource != null) audioSource.Stop();

        if (kuisPanel != null) kuisPanel.SetActive(false);
        if (pilihPanel != null) pilihPanel.SetActive(true);

        SetupQuiz();
    }

    // -------------------- AUDIO HELPER --------------------
    void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.Stop();
        audioSource.PlayOneShot(clip);
    }

    bool IsQuizPanelActive()
    {
        return (kuisPanel == null) || kuisPanel.activeInHierarchy;
    }

    // -------------------- UTILS --------------------
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    void SetAnswerButtonsInteractable(bool value)
    {
        if (answerButtons == null) return;

        foreach (Button btn in answerButtons)
            if (btn != null) btn.interactable = value;
    }
}
