using System;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public UnityEvent OnTutorialFinished = new UnityEvent();
    [SerializeField] private GameObject[] tutorialSlides;
    private int _slideIndex;
    private GameObject _currentActiveSlide;

    private void Start()
    {
        EnableNextSlide();
    }

    public void EnableNextSlide()
    {
        if (_slideIndex == tutorialSlides.Length)
        {
            OnTutorialFinished?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < tutorialSlides.Length; i++)
        {
            if (i != _slideIndex) continue;

            if (_currentActiveSlide != null) _currentActiveSlide.SetActive(false);

            _currentActiveSlide = tutorialSlides[_slideIndex];
            _currentActiveSlide.SetActive(true);
            _slideIndex++;
            break;
        }
    }
}
