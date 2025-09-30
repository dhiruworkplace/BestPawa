using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialFlow : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform handIcon;
    public TextMeshProUGUI messageText;
    public Button nextButton;
    // public TextMeshProUGUI tapToStartText;

    private int stepIndex = 0;
    private Sequence tutorialSequence;

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        // tapToStartText.gameObject.SetActive(false);
        handIcon.gameObject.SetActive(true);

        ShowStep(stepIndex);
    }

    void ShowStep(int step)
    {
        tutorialSequence?.Kill();

        switch (step)
        {
            case 0:
                messageText.text = "Drag to draw boxes";
                AnimateDrag();
                break;
            case 1:
                messageText.text = "Drag back to remove boxes";
                AnimateRemove();
                break;
            case 2:
                messageText.text = "Double tap to remove ALL boxes";
                AnimateDoubleTap();
                break;
            case 3:
                messageText.text = "Swipe to change lane";
                AnimateSwipe();
                break;
            case 4:
                // End of tutorial
                OnTutorialFinished();
                // tapToStartText.gameObject.SetActive(true);
                break;
        }
    }

    void OnNextClicked()
    {
        stepIndex++;
        ShowStep(stepIndex);
    }

    #region Animations
    void AnimateDrag()
    {
        float yPos = 300f; // fixed Y
        Vector3 start = new Vector3(100f, yPos, 0);  // start left
        Vector3 end = new Vector3(-100f, yPos, 0);     // end right

        handIcon.anchoredPosition = start;
        tutorialSequence = DOTween.Sequence();
        tutorialSequence.Append(handIcon.DOAnchorPos(end, .5f).SetEase(Ease.Linear))
                        .Append(handIcon.DOAnchorPos(start, 0f)) // instantly reset back to left
                        .AppendInterval(0.5f)                    // wait a bit before looping
                        .SetLoops(-1, LoopType.Restart).SetUpdate(true);         // restart (not yoyo)
    }



    void AnimateRemove()
    {
        float yPos = 300f; // keep Y fixed
        Vector3 start = new Vector3(-100f, yPos, 0);  // start left
        Vector3 end = new Vector3(100f, yPos, 0);     // end right

        handIcon.anchoredPosition = start;
        tutorialSequence = DOTween.Sequence();
        tutorialSequence.Append(handIcon.DOAnchorPos(end, .5f).SetEase(Ease.Linear))
                        .Append(handIcon.DOAnchorPos(start, 0f)) // instantly reset back to left
                        .AppendInterval(0.5f)                    // wait a bit
                        .SetLoops(-1, LoopType.Restart).SetUpdate(true);         // always restart left→right
    }


    void AnimateDoubleTap()
    {
        Vector3 tapPos = new Vector3(0, 300, 0);
        handIcon.anchoredPosition = tapPos;
        handIcon.localScale = Vector3.one;

        tutorialSequence = DOTween.Sequence();
        tutorialSequence.Append(handIcon.DOScale(0.8f, 0.2f).SetEase(Ease.InOutQuad))
                        .Append(handIcon.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad))
                        .Append(handIcon.DOScale(0.8f, 0.2f).SetEase(Ease.InOutQuad))
                        .Append(handIcon.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad))
                        .AppendInterval(1f)
                        .SetLoops(-1).SetUpdate(true);
    }


    void AnimateSwipe()
    {
        float yPos = 300f; // fixed Y (same as your original swipe)
        Vector3 start = new Vector3(100f, yPos, 0); // start left
        Vector3 end = new Vector3(-100f, yPos, 0);    // move right

        handIcon.anchoredPosition = start;
        tutorialSequence = DOTween.Sequence();
        tutorialSequence.Append(handIcon.DOAnchorPos(end, 0.2f).SetEase(Ease.InOutQuad))
                        .Append(handIcon.DOAnchorPos(start, 0f)) // instantly reset to left
                        .AppendInterval(0.5f)                    // wait before next swipe
                        .SetLoops(-1, LoopType.Restart).SetUpdate(true);         // repeat left→right
    }

    #endregion

    public void OnTutorialFinished()
    {
        // Hide tutorial panel
        gameObject.SetActive(false);
        // Start the game
        GameUIManager.Instance.EnableTapToStart();
    }
}
