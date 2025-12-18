using System;
using System.Collections;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    
    private Animator animator;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        animator = GetComponent<Animator>();
    }
    
    // Method utama untuk transition dengan callback
    public void FadeTransition(Action onFadeInComplete)
    {
        StartCoroutine(FadeSequence(onFadeInComplete));
    }
    
    private IEnumerator FadeSequence(Action onFadeInComplete)
    {
        // 1. Play FadeIn
        animator.SetTrigger("FadeIn");
        
        // 2. Tunggu sampai FadeIn selesai
        yield return new WaitForSeconds(GetAnimationLength("FadeIn"));
        
        // 3. Jalankan callback (ganti scene di sini)
        onFadeInComplete?.Invoke();
        
        // 4. Play FadeOut
        animator.SetTrigger("FadeOut");
    }
    
    // Helper untuk dapat durasi animasi
    private float GetAnimationLength(string animName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animName)
            {
                return clip.length;
            }
        }
        return 1f; // default
    }
}