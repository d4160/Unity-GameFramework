using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Toast : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Button closeButton;
    public Image background;
    public float fadeDuration = 0.5f;

    public void SetMessage(string message)
    {
        messageText.text = message;
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
        //canvasGroup.DOFade(1, fadeDuration);
    }

    public void FadeOut()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        //canvasGroup.DOFade(0, fadeDuration);
    }
}
