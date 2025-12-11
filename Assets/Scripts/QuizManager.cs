using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Data Pertanyaan")]
    public List<QuestionData> allQuestions;      // List pertanyaan untuk kategori ini (hewan/warna/buah)
    public List<Sprite> allAnswerOptions;        // Semua kemungkinan jawaban di kategori ini (misal 9 warna)

    [Header("UI Soal & Jawaban")]
    public Image questionImageUI;                // Gambar soal
    public Image[] answerImagesUI;               // 4 gambar opsi jawaban
    public Button[] answerButtons;               // 4 tombol jawaban (urut sama dgn answerImagesUI)

    [Header("Pengaturan Kuis")]
    public int maxQuestions = 9;                 // Jumlah soal yg dipakai di kategori ini

    [Header("Panel Feedback")]
    public GameObject correctPanel;
    public GameObject wrongPanel;

    [Header("Gambar di Panel Salah")]
    public Image wrongChosenColorImage;   // Image di panel salah, menampilkan warna yang dipilih


// 🔽 TAMBAHAN BARU UNTUK PANEL BENAR
    [Header("Gambar di Panel Benar")]
    public Image correctWordImage;   // Image di bagian atas panel (tulisan warna)
    public Image correctColorImage;  // Image di bagian bawah panel (kotak warna)

    private List<QuestionData> questionQueue;    // Pertanyaan yang sudah diacak dan dipakai
    private QuestionData currentQuestion;
    private int currentQuestionIndex = -1;
    private int correctAnswerIndex = -1;         // index 0–3 posisi jawaban benar di opsi
    private int score = 0;

    void Start()
    {
        SetupQuiz();
        NextQuestion();
    }

    // ------------------------------------------------
    // SETUP AWAL KUIS
    // ------------------------------------------------
    void SetupQuiz()
    {
        // Copy & acak semua pertanyaan
        questionQueue = new List<QuestionData>(allQuestions);
        ShuffleList(questionQueue);

        // Pastikan tidak melebihi jumlah pertanyaan yang ada
        int jumlahSoalDipakai = Mathf.Min(maxQuestions, questionQueue.Count);
        questionQueue = questionQueue.GetRange(0, jumlahSoalDipakai);

        // Matikan panel feedback di awal
        if (correctPanel != null) correctPanel.SetActive(false);
        if (wrongPanel != null) wrongPanel.SetActive(false);

        // Pastikan tombol aktif
        SetAnswerButtonsInteractable(true);
    }

    // ------------------------------------------------
    // TAMPILKAN SOAL BERIKUTNYA
    // ------------------------------------------------
    void NextQuestion()
    {
        currentQuestionIndex++;

        // Kalau sudah tidak ada soal lagi
        if (currentQuestionIndex >= questionQueue.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = questionQueue[currentQuestionIndex];

        // Tampilkan gambar soal
        questionImageUI.sprite = currentQuestion.questionImage;

        // Siapkan gambar untuk Panel Benar
        if (correctWordImage != null)
        {
            // pakai gambar teks warna yang sama dengan soal
            correctWordImage.sprite = currentQuestion.questionImage;
        }

        if (correctColorImage != null)
        {
            // pakai sprite jawaban benar (kotak warna)
            correctColorImage.sprite = currentQuestion.correctAnswer;
        }


        // ==========================
        // BANGUN 4 OPSI JAWABAN
        // ==========================
        List<Sprite> options = new List<Sprite>();

        // 1) Tambah jawaban benar
        options.Add(currentQuestion.correctAnswer);

        // 2) Pool jawaban salah = semua opsi jawaban kategori ini
        List<Sprite> wrongPool = new List<Sprite>(allAnswerOptions);

        //    Hapus jawaban benar dari pool supaya tidak kepilih sebagai salah
        wrongPool.Remove(currentQuestion.correctAnswer);

        //    Acak pool jawaban salah
        ShuffleList(wrongPool);

        // 3) Ambil 3 pertama dari pool sebagai jawaban salah
        int jumlahSalahDibutuhkan = 3;
        for (int i = 0; i < jumlahSalahDibutuhkan && i < wrongPool.Count; i++)
        {
            options.Add(wrongPool[i]);
        }

        // 4) (Fallback) kalau entah kenapa kurang dari 4 opsi, isi random dari wrongPool
        while (options.Count < 4 && wrongPool.Count > 0)
        {
            options.Add(wrongPool[Random.Range(0, wrongPool.Count)]);
        }

        // 5) Acak posisi ke-4 opsi
        ShuffleList(options);

        // 6) Cari di mana posisi jawaban benar setelah diacak
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == currentQuestion.correctAnswer)
            {
                correctAnswerIndex = i;
                break;
            }
        }

        // 7) Pasang sprite ke gambar opsi di UI
        for (int i = 0; i < answerImagesUI.Length && i < options.Count; i++)
        {
            answerImagesUI[i].sprite = options[i];
        }

        // 8) Aktifkan tombol jawaban lagi (kalau sebelumnya dimatikan waktu benar)
        SetAnswerButtonsInteractable(true);
    }

    // ------------------------------------------------
    // DIPANGGIL OLEH TOMBOL JAWABAN (PARAMETER 0..3)
    // ------------------------------------------------
    public void OnAnswerSelected(int index)
    {
        // Kalau tombol lagi dimatikan (misal lagi nunggu user klik "Lanjut"), abaikan
        if (!answerButtons[0].interactable) return;

        if (index == correctAnswerIndex)
        {
            score++;
            SetAnswerButtonsInteractable(false);

            if (correctPanel != null)
            correctPanel.SetActive(true);  // Panel benar muncul, gambar sudah siap
        }

        else
{
            // JAWABAN SALAH
            Debug.Log("Jawaban salah. Coba lagi!");

            // Set gambar warna yang dipilih ke panel salah
            if (wrongChosenColorImage != null)
            {
                // gunakan sprite dari opsi yang diklik
                wrongChosenColorImage.sprite = answerImagesUI[index].sprite;
            }

    // Munculkan panel salah (soal tetap sama, tidak ganti)
    if (wrongPanel != null) wrongPanel.SetActive(true);
}

    }

    // ------------------------------------------------
    // DIPANGGIL OLEH TOMBOL "LANJUT" DI PANEL BENAR
    // ------------------------------------------------
    public void OnNextButton()
    {
        if (correctPanel != null) correctPanel.SetActive(false);
        NextQuestion();
    }

    // ------------------------------------------------
    // DIPANGGIL OLEH TOMBOL "COBA LAGI" / CLOSE DI PANEL SALAH
    // ------------------------------------------------
    public void OnTryAgainButton()
    {
        if (wrongPanel != null) wrongPanel.SetActive(false);
        // Tidak ganti soal, user tinggal pilih jawaban lain
    }

    // ------------------------------------------------
    // KETIKA SEMUA SOAL SELESAI
    // ------------------------------------------------
    void EndQuiz()
    {
        Debug.Log("Kuis selesai! Skor akhir: " + score + " / " + questionQueue.Count);

        // Di sini bisa tampilkan panel hasil akhir / pindah scene, dll.
    }

    // ------------------------------------------------
    // UTIL: ACak list generik
    // ------------------------------------------------
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

    // ------------------------------------------------
    // UTIL: SET AKTIF / NONAKTIF TOMBOL JAWABAN
    // ------------------------------------------------
    void SetAnswerButtonsInteractable(bool value)
    {
        foreach (Button btn in answerButtons)
        {
            if (btn != null)
                btn.interactable = value;
        }
    }
}
