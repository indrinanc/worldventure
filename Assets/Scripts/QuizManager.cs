using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public enum QuizCategory { Hewan, Warna, Buah }

    [Header("Kategori (hanya untuk label, logika tetap sama)")]
    public QuizCategory category;
.
    [Header("Data Pertanyaan")]
    public List<QuestionData> allQuestions;      // Isi per scene
    public List<Sprite> allAnswerOptions;        // Isi per scene (hewan/warna/buah)

    [Header("UI Soal & Jawaban")]
    public Image questionImageUI;
    public Image[] answerImagesUI;               // 4 slot
    public Button[] answerButtons;               // 4 tombol

    [Header("Pengaturan Kuis")]
    public int maxQuestions = 9;                 // Set per scene

    [Header("Panel Feedback")]
    public GameObject correctPanel;              // BenarPanel isinya 1 gambar statis (set di Inspector)
    public GameObject wrongPanel;

    [Header("Gambar di Panel Salah")]
    public Image wrongChosenImage;               // Menampilkan pilihan user saat salah (opsional)

    [Header("Panel Navigasi (opsional)")]
    public GameObject pilihPanel;                // Panel tujuan setelah kuis selesai (opsional)
    public GameObject kuisPanel;                 // Panel kuis yang dimatikan (opsional)

    private List<QuestionData> questionQueue;
    private QuestionData currentQuestion;
    private int currentQuestionIndex = -1;
    private int correctAnswerIndex = -1;
    private int score = 0;

    void Start()
    {
        SetupQuiz();
        NextQuestion();
    }

    void SetupQuiz()
    {
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

        // ===== Bangun 4 opsi jawaban: 1 benar + 3 salah random (kecuali benar) =====
        List<Sprite> options = new List<Sprite> { currentQuestion.correctAnswer };

        List<Sprite> wrongPool = new List<Sprite>(allAnswerOptions);
        wrongPool.Remove(currentQuestion.correctAnswer);
        ShuffleList(wrongPool);

        for (int i = 0; i < 3 && i < wrongPool.Count; i++)
            options.Add(wrongPool[i]);

        while (options.Count < 4 && wrongPool.Count > 0)
            options.Add(wrongPool[Random.Range(0, wrongPool.Count)]);

        ShuffleList(options);

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

        Debug.Log($"[{category}] Klik jawaban index: {index}");

        if (index == correctAnswerIndex)
        {
            score++;
            SetAnswerButtonsInteractable(false);

            // ✅ BenarPanel cuma tampil gambar statis (tidak perlu set sprite dari kode)
            if (correctPanel != null)
                correctPanel.SetActive(true);
        }
        else
        {
            // ✅ Panel salah menampilkan pilihan user (opsional)
            if (wrongChosenImage != null && answerImagesUI != null && index < answerImagesUI.Length && answerImagesUI[index] != null)
                wrongChosenImage.sprite = answerImagesUI[index].sprite;

            if (wrongPanel != null)
                wrongPanel.SetActive(true);
        }
    }

    public void OnNextButton()
    {
        if (correctPanel != null) correctPanel.SetActive(false);
        NextQuestion();
    }

    public void OnTryAgainButton()
    {
        if (wrongPanel != null) wrongPanel.SetActive(false);
    }

    void EndQuiz()
    {
        Debug.Log($"[{category}] Kuis selesai! Skor akhir: {score} / {(questionQueue == null ? 0 : questionQueue.Count)}");

        if (kuisPanel != null) kuisPanel.SetActive(false);
        if (pilihPanel != null) pilihPanel.SetActive(true);
    }

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
