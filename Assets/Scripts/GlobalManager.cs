using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GlobalManager : MonoBehaviour
{

    public Button playButton;
    public Button quitButton;

    public GameObject title;
    public bool isTutorialShown = false;

    private static GlobalManager instance;
    public static GlobalManager Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        quitButton.onClick.AddListener(()=> Exit());
        playButton.onClick.AddListener(()=> StartGame());
        // Title GameObject-এ ইনফিনিটি ফ্লোটিং অ্যানিমেশন
        if (title != null)
        {
            title.transform.DOLocalMoveY(title.transform.localPosition.y + 30f, 1.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    void Exit()
    {
        Application.Quit();
    }

  
}
