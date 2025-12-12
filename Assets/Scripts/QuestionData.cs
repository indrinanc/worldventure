using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public Sprite questionImage;   // Gambar pertanyaan
    public Sprite correctAnswer;   // Gambar jawaban benar
    [Header("Audio Pertanyaan (per soal)")]
    public AudioClip questionAudio;   // suara khusus soal ini
}
