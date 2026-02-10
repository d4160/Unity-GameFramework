using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Ensure you have DOTween imported in your project
using System.Collections;
using TMPro;
using System;

/// <summary>
/// Manages the display and positioning of tooltips in the UI.
/// </summary>
public class TooltipManager : MonoBehaviour
{
    /// <summary>
    /// The RectTransform of the tooltip panel.
    /// </summary>
    public RectTransform tooltipPanel;

    /// <summary>
    /// The Text component of the tooltip.
    /// </summary>
    public TextMeshProUGUI tooltipText;

    /// <summary>
    /// The Canvas component of the UI.
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// The duration of the animation for showing and hiding the tooltip.
    /// </summary>
    public float animationDuration = 0.3f;

    /// <summary>
    /// The offset of the tooltip from the target position.
    /// </summary>
    public Vector2 tooltipOffset = new Vector2(10f, 10f);

    /// <summary>
    /// Multiplier for separation with different screens;
    /// </summary>
    public bool offsetScaleWithScreen = true;

    /// <summary>
    /// Multiplier for separation with different screens;
    /// </summary>
    public float offsetScaleFactor = 1f;

    private RectTransform _canvasRectTransform;
    private Tweener _hidingTween;

    public TooltipTrigger LastToolTipTrigger { get; internal set; }

    private void Start()
    {
        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned in the inspector.");
            return;
        }

        //if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        if (tooltipPanel == null)
        {
            Debug.LogError("TooltipPanel is not assigned in the inspector.");
            return;
        }

        var helper = tooltipPanel.GetComponent<TooltipTriggerHelper>();

        if (!helper)
        {
            helper = tooltipPanel.gameObject.AddComponent<TooltipTriggerHelper>();
        }

        helper.TooltipManager = this;
    }

    /// <summary>
    /// Shows the tooltip with the specified text near the given UI element.
    /// </summary>
    /// <param name="uiElement">The UI element to show the tooltip above.</param>
    /// <param name="tooltipContent">The content of the tooltip.</param>
    /// <example>
    /// <code>
    /// public class MyButton : MonoBehaviour
    /// {
    ///     public TooltipManager tooltipManager;
    ///     public string tooltipText = "This is a tooltip!";
    ///     
    ///     private void OnMouseOver()
    ///     {
    ///         tooltipManager.ShowTooltip(GetComponent<RectTransform>(), tooltipText);
    ///     }
    /// }
    /// </code>
    /// </example>
    public void ShowTooltip(GameObject target, string tooltipContent, Vector2 triggerOffset = default)
    {
        if (target == null)
        {
            Debug.LogError("Target is null.");
            return;
        }

        if (_hidingTween != null)
        {
            _hidingTween.Kill();
            _hidingTween = null;
        }

        Vector2 screenPosition = GetScreenPosition(target) + tooltipOffset + GetFixedOffset(triggerOffset);
        StartCoroutine(UpdateTooltipPosition(screenPosition, tooltipContent));
    }

    private Vector2 GetFixedOffset(Vector2 offset)
    {
        if (offsetScaleWithScreen)
        {
            Vector2 screenSizeBase = new Vector2(1920, 1080);

            float factorX = Screen.width / screenSizeBase.x;
            float factorY = Screen.height / screenSizeBase.y;
            return new Vector2(factorX * offset.x, factorY * offset.y) * offsetScaleFactor;
        }

        return offset;

    }

    private Vector2 GetScreenPosition(GameObject target)
    {
        if (target.GetComponent<RectTransform>() != null)
        {
            // UI element
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            //rectTransform.
            return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
        }
        else
        {
            // 3D/2D GameObject
            if (Camera.main == null)
            {
                Debug.LogError("Main camera is not assigned in the scene.");
                return Vector2.zero;
            }
            return Camera.main.WorldToScreenPoint(target.transform.position);
        }
    }

    private IEnumerator UpdateTooltipPosition(Vector2 screenPosition, string tooltipContent)
    {
        tooltipText.text = tooltipContent;
        tooltipPanel.gameObject.SetActive(true);

        yield return null; // Wait for the end of the frame to get correct layout

        Vector2 anchoredPosition;

        try
        {
            //Vector2 uiElementScreenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiElement.position);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPosition, canvas.worldCamera, out anchoredPosition))
            {
                Debug.LogError("Failed to convert screen point to local point.");
                yield break;
            }
        }
        catch {
            anchoredPosition = screenPosition;
        }

        tooltipPanel.anchoredPosition = anchoredPosition;

        AdjustPositionToStayOnScreen();

        //tooltipPanel.DOAnchorPos(anchoredPosition, animationDuration).SetEase(Ease.OutSine);
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyButton : MonoBehaviour
    /// {
    ///     public TooltipManager tooltipManager;
    ///     
    ///     private void OnMouseExit()
    ///     {
    ///         tooltipManager.HideTooltip();
    ///     }
    /// }
    /// </code>
    /// </example>
    public void HideTooltip()
    {
        if (tooltipPanel == null)
        {
            Debug.LogError("Tooltip panel is not assigned in the inspector.");
            return;
        }

        //_hidingTween = tooltipPanel.DOAnchorPos(tooltipPanel.anchoredPosition, animationDuration).SetEase(Ease.InSine).OnComplete(() =>
        // {
        //     tooltipPanel.gameObject.SetActive(false);
        // });
    }

    private void AdjustPositionToStayOnScreen()
    {
        Vector3[] corners = new Vector3[4];
        tooltipPanel.GetWorldCorners(corners);

        Vector3 offset = Vector3.zero;

        if (corners[2].x > Screen.width)
        {
            offset.x = Screen.width - corners[2].x;
        }
        if (corners[0].x < 0)
        {
            offset.x = -corners[0].x;
        }
        if (corners[2].y > Screen.height)
        {
            offset.y = Screen.height - corners[2].y;
        }
        if (corners[0].y < 0)
        {
            offset.y = -corners[0].y;
        }

        tooltipPanel.anchoredPosition += new Vector2(offset.x, offset.y);
    }
}