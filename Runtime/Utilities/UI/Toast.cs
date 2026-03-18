using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button closeButton;
    public Image background;
    public float fadeDuration = 0.5f;

    public void SetTitle(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.gameObject.SetActive(!string.IsNullOrEmpty(title));
        }
    }

    public void SetMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }

    public void SetTheme(Color color)
    {
        background.color = color;
    }

    public void SetDuration(float duration, bool isPermanent)
    {
        if (isPermanent)
        {
            closeButton.gameObject.SetActive(true);
            closeButton.onClick.AddListener(() =>
            {
                FadeOut();
                Destroy(gameObject, fadeDuration);
            });
        }
        else
        {
            closeButton.gameObject.SetActive(false);
        }
    }

    public void FadeIn()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeDuration);
    }

    public void FadeOut()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.DOFade(0, fadeDuration);
    }
}
