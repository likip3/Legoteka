using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    public Button withHelperButton;
    public Button withoutHelperButton;
    public Button startButton;

    public string withHelperSceneName;
    public string withoutHelperSceneName;

    public Sprite selectedWithHelperSprite;
    public Sprite selectedWithoutHelperSprite;
    public Sprite defaultWithHelperSprite;
    public Sprite defaultWithoutHelperSprite;

    private bool isSceneWithHelperSelected = false;
    private bool isSceneWithoutHelperSelected = false;

    private void Start()
    {
        SelectScene1();
        withHelperButton.onClick.AddListener(SelectScene1);
        withoutHelperButton.onClick.AddListener(SelectScene2);
        startButton.onClick.AddListener(StartSelectedScene);
    }

    private void SelectScene1()
    {
        isSceneWithHelperSelected = true;
        isSceneWithoutHelperSelected = false;

        withHelperButton.image.sprite = selectedWithHelperSprite;
        withoutHelperButton.image.sprite = defaultWithoutHelperSprite;
    }

    private void SelectScene2()
    {
        isSceneWithHelperSelected = false;
        isSceneWithoutHelperSelected = true;

        withHelperButton.image.sprite = defaultWithHelperSprite;
        withoutHelperButton.image.sprite = selectedWithoutHelperSprite;
    }

    private void StartSelectedScene()
    {
        if (isSceneWithHelperSelected)
            SceneManager.LoadScene(withHelperSceneName);
        else if (isSceneWithoutHelperSelected)
            SceneManager.LoadScene(withoutHelperSceneName);
    }
}