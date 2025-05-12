using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{


    [Header("End Screen")]
    public GameObject endPanel;
    public TextMeshProUGUI endText;
    public Button restartButton;
    public Button exitButton;
    [Header("How To Play")]
    public GameObject tutPanel;
    public Button tutShowButton;
    public Button tutCloseButton;

    bool isTutOn = false;
    private static UiManager instance;
    public static UiManager Instance => instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        restartButton.onClick.AddListener(() => Restart());
        exitButton.onClick.AddListener(() => Exit());
        endPanel.GetComponent<CanvasGroup>().alpha = 0;
        endPanel.transform.localScale = Vector3.zero;
        tutPanel.transform.localScale = Vector3.zero;
        tutShowButton.onClick.AddListener(() => OpenTutPanel());
        tutCloseButton.onClick.AddListener(() => CloseTutPanel());
    }

    private void Start()
    {
        if (!GlobalManager.Instance.isTutorialShown)
        {
            GlobalManager.Instance.isTutorialShown = true;
            OpenTutPanel();
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.H) && !isTutOn)
        {
            OpenTutPanel();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Exit()
    {
        Application.Quit();
    }

    public void OpenEndScreen(string text)
    {
        endText.text = text;
        endPanel.transform.localScale = Vector3.one;
        endPanel.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).SetDelay(1f);
    }

    public void OpenTutPanel()
    {

        if (isTutOn) return;
        isTutOn = true;
        tutPanel.transform.DOScale(1, 0.1f).OnComplete(() =>
        {
            Time.timeScale = 0;
        });

    }
    public void CloseTutPanel()
    {
        Time.timeScale = 1;
        Player.Instance.TurnOffShieldForceFully();
        tutPanel.transform.DOScale(0, 0.1f).OnComplete(() =>
        {
            isTutOn = false;


        });
    }
}
