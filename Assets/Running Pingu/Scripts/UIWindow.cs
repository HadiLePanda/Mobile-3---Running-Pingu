using UnityEngine;

public class UIWindow : MonoBehaviour
{
    [Header("Window References")]
    public GameObject panel;
    public RectTransform windowTransform;
    public CanvasGroup canvasGroup;
    public bool closeOnStart = true;

    [Header("Animation Settings")]
    public LeanTweenType openAnimationType = LeanTweenType.easeInOutQuad;
    public LeanTweenType closeAnimationType = LeanTweenType.easeInOutQuad;
    public LeanTweenType fadeAnimationType = LeanTweenType.linear;
    public float animationTime = 0.5f;

    private int fadeTweenId;
    private int scaleTweenId;

    protected virtual void Start()
    {
        if (closeOnStart)
            CloseInstantly();
    }

    public virtual void Open()
    {
        // Cancel any existing tweens
        LeanTween.cancel(fadeTweenId);
        LeanTween.cancel(scaleTweenId);

        // make it invisible before opening
        windowTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;

        // block raycasts
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
        // enable panel
        panel.SetActive(true);

        // fade in and scale up
        fadeTweenId = LeanTween.alphaCanvas(canvasGroup, 1f, animationTime).setEase(fadeAnimationType)
            .setOnComplete(() => FinishedOpening()).id;
        scaleTweenId = LeanTween.scale(windowTransform, Vector3.one, animationTime).setEase(openAnimationType).id;
    }

    public virtual void Close()
    {
        // block raycasts
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        // cancel any existing tweens
        LeanTween.cancel(fadeTweenId);
        LeanTween.cancel(scaleTweenId);

        // fade out and scale down
        fadeTweenId = LeanTween.alphaCanvas(canvasGroup, 0f, animationTime).setEase(fadeAnimationType)
            .setOnComplete(() => FinishedClosing()).id;
        scaleTweenId = LeanTween.scale(windowTransform, Vector3.zero, animationTime).setEase(closeAnimationType).id;
    }

    public void CloseInstantly()
    {
        // Cancel any existing tweens
        LeanTween.cancel(fadeTweenId);
        LeanTween.cancel(scaleTweenId);

        panel.SetActive(false);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void FinishedOpening()
    {
        // turn visible for raycast interaction
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        windowTransform.localScale = Vector3.one;
    }
    private void FinishedClosing()
    {
        // turn off panel
        panel.SetActive(false);

        // turn invisible for raycast interaction
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        windowTransform.localScale = Vector3.one;
    }
}
