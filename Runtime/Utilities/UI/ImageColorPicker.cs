/* Image based color picker for Unity UI by SecretAnorak Feb 2018
 * 1 - Simply add this component to any UI Image to expose an "OnColorPicked" Event.
 * 2 - Ensure your source image (Sprite) is marked as "Read/Write Enabled" in the Texture Import Settings.
 * 3 - Scale the image as you like but DO NOT use preserve aspect on your image. The image must fill the entire RectTransform for this script to work.*/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImageColorPicker : MonoBehaviour, IPointerClickHandler
{
    public Color selectedColor;

    [Serializable]
    public class ColorEvent : UnityEvent<Color> { }
    public ColorEvent OnColorPicked = new ColorEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        selectedColor = GetColor(GetPointerUVPosition());
        OnColorPicked.Invoke(selectedColor);
    }

    private Color GetColor(Vector2 pos)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Color selected = texture.GetPixelBilinear(pos.x, pos.y);
        selected.a = 1; // force full alpha
        return selected;
    }

    Vector2 GetPointerUVPosition()
    {
        Vector3[] imageCorners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(imageCorners);
        float texWidth = imageCorners[2].x - imageCorners[0].x;
        float texHeight = imageCorners[2].y - imageCorners[0].y;
        float uvX = (Input.mousePosition.x - imageCorners[0].x) / texWidth;
        float uvY = (Input.mousePosition.y - imageCorners[0].y) / texHeight;
        return new Vector2(uvX, uvY);
    }
}