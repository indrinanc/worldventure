using UnityEngine;

[System.Serializable]
public class QuestionData
{
    public Sprite questionImage;
    public Sprite correctAnswer;
    [Header("Audio Pertanyaan (per soal)")]
    public AudioClip questionAudio;
}
