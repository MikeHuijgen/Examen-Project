using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FPSText;
    private float updateInterval = 1f;
    private float _timeSinceLastUpdate = 0f;

    private float _fps;

    void Update()
    {
        _timeSinceLastUpdate += Time.deltaTime;

        if (_timeSinceLastUpdate >= updateInterval)
        {
            int fps = Mathf.RoundToInt(1f / Time.deltaTime);
            FPSText.text = $"FPS {fps}";
            _timeSinceLastUpdate = 0f;
        }
    }
}
