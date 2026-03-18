using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using d4160.Singleton;

public enum ToastPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

public enum ToastTheme
{
    Info,
    Dark,
    White,
    Success,
    Warning,
    Error
}

public class ToastManager : Singleton<ToastManager>
{
    public GameObject toastPrefab;
    public Canvas canvas;

    private Dictionary<ToastTheme, Color> themeColors;

    protected override void Awake()
    {
        base.Awake();
        Color successColor, errorColor, infoColor, warningColor;
        ColorUtility.TryParseHtmlString("#2E7D32", out successColor); // Material Green 800
        ColorUtility.TryParseHtmlString("#C62828", out errorColor); // Material Red 800
        ColorUtility.TryParseHtmlString("#0277BD", out infoColor); 
        ColorUtility.TryParseHtmlString("#F9A825", out warningColor); 

        themeColors = new Dictionary<ToastTheme, Color>
        {
            { ToastTheme.Info, infoColor },
            { ToastTheme.Dark, new Color(0.15f, 0.15f, 0.15f) },
            { ToastTheme.White, Color.white },
            { ToastTheme.Success, successColor },
            { ToastTheme.Warning, warningColor },
            { ToastTheme.Error, errorColor }
        };
    }

    public void ShowToast(string title, string message, ToastPosition position = ToastPosition.TopCenter, ToastTheme theme = ToastTheme.Dark, float duration = 2.0f, bool isPermanent = false)
    {
        GameObject toastInstance = Instantiate(toastPrefab, canvas.transform);
        Toast toastComponent = toastInstance.GetComponent<Toast>();

        toastComponent.SetTitle(title);
        toastComponent.SetMessage(message);
        toastComponent.SetTheme(themeColors[theme]);
        toastComponent.SetDuration(duration, isPermanent);

        toastInstance.SetActive(true);

        RectTransform toastRect = toastInstance.GetComponent<RectTransform>();
        toastRect.anchoredPosition = GetPosition(position);

        toastComponent.FadeIn();

        if (!isPermanent)
        {
            StartCoroutine(DestroyToastAfterDuration(toastInstance, duration));
        }
    }

    private Vector2 GetPosition(ToastPosition position)
    {
        switch (position)
        {
            case ToastPosition.TopLeft: return new Vector2(-canvas.GetComponent<RectTransform>().rect.width / 2 + 50, canvas.GetComponent<RectTransform>().rect.height / 2 - 50);
            case ToastPosition.TopCenter: return new Vector2(0, canvas.GetComponent<RectTransform>().rect.height / 2 - 50);
            case ToastPosition.TopRight: return new Vector2(canvas.GetComponent<RectTransform>().rect.width / 2 - 50, canvas.GetComponent<RectTransform>().rect.height / 2 - 50);
            case ToastPosition.MiddleLeft: return new Vector2(-canvas.GetComponent<RectTransform>().rect.width / 2 + 50, 0);
            case ToastPosition.MiddleCenter: return new Vector2(0, 0);
            case ToastPosition.MiddleRight: return new Vector2(canvas.GetComponent<RectTransform>().rect.width / 2 - 50, 0);
            case ToastPosition.BottomLeft: return new Vector2(-canvas.GetComponent<RectTransform>().rect.width / 2 + 50, -canvas.GetComponent<RectTransform>().rect.height / 2 + 50);
            case ToastPosition.BottomCenter: return new Vector2(0, -canvas.GetComponent<RectTransform>().rect.height / 2 + 50);
            case ToastPosition.BottomRight: return new Vector2(canvas.GetComponent<RectTransform>().rect.width / 2 - 50, -canvas.GetComponent<RectTransform>().rect.height / 2 + 50);
            default: return new Vector2(0, 0);
        }
    }

    private IEnumerator DestroyToastAfterDuration(GameObject toastInstance, float duration)
    {
        yield return new WaitForSeconds(duration);

        Toast toastComponent = toastInstance.GetComponent<Toast>();
        toastComponent.FadeOut();
        yield return new WaitForSeconds(toastComponent.fadeDuration);

        Destroy(toastInstance);
    }
}
