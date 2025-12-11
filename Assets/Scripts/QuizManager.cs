using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Data Pertanyaan")]
    public List<QuestionData> allQuestions;

    [Header("Semua Opsi Jawaban Kategori Ini")]
    public List<Sprite> allAnswerOptions;

    [Header("UI Referensi")]
    public Image questionImageUI;
    public Image[] answerImagesUI;

    [Header("Pengaturan Kuis")]
    public int maxQuestions = 9;

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

    //---------------------------------------------------------
    // SETUP QUIZ
    //---------------------------------------------------------
    void SetupQuiz()
    {
        // Copy semua pertanyaan
        questionQueue = new List<QuestionData>(allQuestions);

        // Acak urutan
        ShuffleList(questionQueue);

        // Tentukan jumlah soal yang dipakai
        int jumlahSoalDipakai = Mathf.Min(maxQuestions, questionQueue.Count);

        // Ambil sesuai jumlah
        questionQueue = questionQueue.GetRange(0, jumlahSoalDipakai);
    }

    //---------------------------------------------------------
    // MENAMPILKAN SOAL BERIKUTNYA
    //---------------------------------------------------------
    void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questionQueue.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = questionQueue[currentQuestionIndex];

        // Tampilkan gambar soal
        questionImageUI.sprite = currentQuestion.questionImage;

        // Bangun jawaban (1 benar + 3 salah)
        List<Sprite> options = new List<Sprite>();

        // Tambah jawaban benar
        options.Add(currentQuestion.correctAnswer);

        // Buat pool jawaban salah
        List<Sprite> wrongPool = new List<Sprite>(allAnswerOptions);
        wrongPool.Remove(currentQuestion.correctAnswer);
        ShuffleList(wrongPool);

        // Tambah 3 jawaban salah
        for (int i = 0; i < 3 && i < wrongPool.Count; i++)
        {
            options.Add(wrongPool[i]);
        }

        // Jika kurang dari 4 opsi, isi seadanya (fallback)
        while (options.Count < 4)
            options.Add(wrongPool[Random.Range(0, wrongPool.Count)]);

        // Acak posisi opsi
        ShuffleList(options);

        // Cari index jawaban benar
        for (int i = 0; i < 4; i++)
        {
            if (options[i] == currentQuestion.correctAnswer)
            {
                correctAnswerIndex = i;
                break;
            }
        }

        // Pasang sprite ke UI
        for (int i = 0; i < 4; i++)
            answerImagesUI[i].sprite = options[i];
    }

    public void OnAnswerSelected(int index)
    {
        if (index == correctAnswerIndex)
            score++;

        NextQuestion();
    }

    void EndQuiz()
    {
        Debug.Log("Kuis selesai! Skor akhir: " + score + " / " + questionQueue.Count);

    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
