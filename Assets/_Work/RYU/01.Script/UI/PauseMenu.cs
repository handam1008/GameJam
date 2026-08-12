using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ESC로 일시정지. 화면이 어두워지고 메뉴가 오른쪽에서 슬라이드 인.
public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image dimBackground;
    [SerializeField] private RectTransform menuPanel;

    [Header("슬라이드")]
    [SerializeField] private float hiddenX = 600f;
    [SerializeField] private float shownX = 0f;
    [SerializeField] private float slideTime = 0.35f;

    [Header("어둡게")]
    [SerializeField] private float dimAlpha = 0.6f;

    [Header("사운드")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;

    [Header("감도")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 3f;

    private bool paused;

    private void Start()
    {

        menuPanel.anchoredPosition = new Vector2(hiddenX, menuPanel.anchoredPosition.y);
        dimBackground.gameObject.SetActive(false);

        SetupSlider(masterSlider, "Master");
        SetupSensitivitySlider();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused) Resume();
            else Pause();
        }
    }

    private void Pause()
    {
        paused = true;
        Time.timeScale = 0f;

        dimBackground.gameObject.SetActive(true);
        
        dimBackground.DOFade(dimAlpha, slideTime).SetUpdate(true);
        menuPanel.DOAnchorPosX(shownX, slideTime).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    
    public void Resume()
    {
        paused = false;

        menuPanel.DOAnchorPosX(hiddenX, slideTime).SetEase(Ease.InCubic).SetUpdate(true);
        dimBackground.DOFade(0f, slideTime).SetUpdate(true)
            .OnComplete(() =>
            {
                dimBackground.gameObject.SetActive(false);
                Time.timeScale = 1f;
            });
    }

   
    public void Quit()
    {
        Application.Quit();
    }

    
    private void SetupSlider(Slider slider, string param)
    {
        if (slider == null || mixer == null)
            return;

      
        if (mixer.GetFloat(param, out float db))
            slider.SetValueWithoutNotify(DbToLinear(db));

        slider.onValueChanged.AddListener(v => mixer.SetFloat(param, LinearToDb(v)));
    }

    private void SetupSensitivitySlider()
    {
        if (sensitivitySlider == null)
            return;

        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.SetValueWithoutNotify(WallRotate.SensitivityMultiplier);

        sensitivitySlider.onValueChanged.AddListener(v =>
        {
            WallRotate.SensitivityMultiplier = v;
            PlayerPrefs.SetFloat(WallRotate.SensitivityPrefKey, v);
        });
    }


    private static float LinearToDb(float v) => v <= 0.0001f ? -80f : Mathf.Log10(v) * 20f;
    private static float DbToLinear(float db) => Mathf.Pow(10f, db / 20f);
}
