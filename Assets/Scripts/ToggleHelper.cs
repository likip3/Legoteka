using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToggleHelper : MonoBehaviour {
    [SerializeField] 
    RectTransform uiHandleRectTransform;

    [SerializeField] 
    Color backgroundActiveColor;

    [SerializeField] 
    Image backgroundImage;
    Color backgroundDefaultColor;
    Toggle toggle;
    Vector2 handlePosition;

    public string activeSceneName;
    public string inactiveSceneName;

    void Awake() {
        toggle = GetComponent<Toggle>();

        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
            OnSwitch (true);
    }

    void OnSwitch (bool on) {
        uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;

        backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor;
    }

    public void StartButtonClicked()
    {
        string sceneToLoad = toggle.isOn ? activeSceneName : inactiveSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}