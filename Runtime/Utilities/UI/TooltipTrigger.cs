using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A reusable script that can use the TooltipManager for any UIElement and GameObject.
/// </summary>
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText;
    public TooltipManager tooltipManager;
    public Vector2 tooltipOffset = new Vector2(10f, 10f);

    private bool _shown = false;

    private void Start()
    {
        if (tooltipManager == null)
        {
            tooltipManager = GameObject.FindFirstObjectByType<TooltipManager>();
        }
    }

    void OnDisable()
    {
        if (_shown)
        {
            HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != tooltipManager.tooltipPanel.gameObject)
        {
            HideTooltip();

            if (tooltipManager.LastToolTipTrigger == this)
            {
                tooltipManager.LastToolTipTrigger = null;
            }
        }
        else
        {
            tooltipManager.LastToolTipTrigger = this;
        }
    }

    internal void ShowTooltip()
    {
        tooltipManager.ShowTooltip(gameObject, tooltipText, tooltipOffset);

        _shown = true;
    }

    internal void HideTooltip()
    {
        tooltipManager.HideTooltip(gameObject);

        _shown = false;
    }
}