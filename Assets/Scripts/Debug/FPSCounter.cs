using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float refreshRate = 0.5f;

    private int _frameCount;
    private float _timer;

    private void Update()
    {
        _frameCount++;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= refreshRate)
        {
            float fps = _frameCount / _timer;

            fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";

            _frameCount = 0;
            _timer = 0f;
        }
    }
}