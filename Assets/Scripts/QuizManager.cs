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
    public Image[] answerImagesUI;  
    public Button[] answerButtons; 

    [Header("Pengaturan Kuis")]
    public int maxQuestions = 9;

    [Header("Panel Feedback")]
    public GameObject correctPanel;  
    public GameObject wrongPanel;    

    [Header("Gambar di Panel Benar")]
    public Image correctAnswerImage;  

    [Header("Gambar di Panel Salah")]
    public Image wrongChosenImage;     

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

        if (correctAnswerImage == null && correctPanel != null)
        {
            correctAnswerImage = FindBestContentImage(correctPanel.transform);
        }

        if (wrongChosenImage == null && wrongPanel != null)
        {
            var img = FindImageByNameContains(wrongPanel.transform, "Warna");
            if (img == null) img = FindImageByNameContains(wrongPanel.transform, "Chosen");
            if (img == null) img = FindBestContentImage(wrongPanel.transform);

            wrongChosenImage = img;
        }

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
        var images = root.GetComponentsInChildren<Image>(true);

        foreach (var img in images)
        {
            if (img == null) continue;

            string n = img.gameObject.name.ToLower();
            if (n.Contains("bg") || n.Contains("background") || n.Contains("panel")) continue;

            return img;
        }

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

        EnsurePanelsAndImages(); 
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

        if (IsQuizPanelActive())
            PlayClip(currentQuestion.questionAudio);

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

            EnsurePanelsAndImages();
            if (correctAnswerImage != null && currentQuestion != null)
                correctAnswerImage.sprite = currentQuestion.correctAnswer;

            PlayClip(correctClip);

            if (correctPanel != null)
                correctPanel.SetActive(true);
        }
        else
        {
            EnsurePanelsAndImages();

            Sprite chosen = null;
            if (currentOptions != null && index >= 0 && index < currentOptions.Count)
                chosen = currentOptions[index];

            if (wrongChosenImage != null && chosen != null)
                wrongChosenImage.sprite = chosen;

            PlayClip(wrongClip);

            if (wrongPanel != null)
                wrongPanel.SetActive(true);

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
        if (wrongPanel != null) wrongPanel.SetActive(false);

        SetAnswerButtonsInteractable(true);
    }

    void EndQuiz()
    {
        if (audioSource != null) audioSource.Stop();

        if (kuisPanel != null) kuisPanel.SetActive(false);
        if (pilihPanel != null) pilihPanel.SetActive(true);

        SetupQuiz();
    }

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
