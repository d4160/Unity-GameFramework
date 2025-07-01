using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace d4160.Utilities.UIs
{
    [DisallowMultipleComponent]
    public class ButtonCursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Texture2D handCursor; // Assign a hand cursor texture in the Inspector
        public Color tint = Color.white; // Tint color for the cursor

        private Dictionary<Color, Texture2D> _tintedCursorCache = new Dictionary<Color, Texture2D>();

        public Dictionary<Color, Texture2D> TintedCursorCache => _tintedCursorCache;


        public void Start()
        {
            // Validate that the hand cursor texture is assigned
            if (handCursor == null)
            {
                Debug.LogError("Hand cursor texture is not assigned. Please assign a texture in the Inspector.");
            }

            // Validate that the tint color is not completely transparent
            if (tint.a == 0)
            {
                Debug.LogWarning("Tint color is fully transparent. Cursor may not be visible.");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (handCursor == null)
            {
                Debug.LogError("Cannot set cursor because the hand cursor texture is not assigned.");
                return;
            }

            try
            {
                Texture2D tintedCursor;

                // Check if the tinted texture is already cached
                if (_tintedCursorCache.TryGetValue(tint, out tintedCursor))
                {
                    // Use the cached tinted texture
                    Cursor.SetCursor(tintedCursor, Vector2.zero, CursorMode.Auto);
                }
                else
                {
                    // Tint the cursor texture, cache it, and set it
                    tintedCursor = TintTexture(handCursor, tint);
                    _tintedCursorCache[tint] = tintedCursor;
                    Cursor.SetCursor(tintedCursor, Vector2.zero, CursorMode.Auto);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while setting the cursor: {ex.Message}");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            try
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An error occurred while resetting the cursor: {ex.Message}");
            }
        }

        public Texture2D TintTexture(Texture2D originalTexture, Color tintColor)
        {
            // Validate the original texture
            if (originalTexture == null)
            {
                Debug.LogError("Original texture is null. Cannot apply tint.");
                return null;
            }

            if (tintColor == Color.white)
            {
                return originalTexture;
            }

            // Create a new texture to hold the tinted version
            Texture2D tintedTexture = new Texture2D(originalTexture.width, originalTexture.height);

            for (int y = 0; y < originalTexture.height; y++)
            {
                for (int x = 0; x < originalTexture.width; x++)
                {
                    Color originalColor = originalTexture.GetPixel(x, y);
                    Color tintedColor = originalColor * tintColor;
                    tintedTexture.SetPixel(x, y, tintedColor);
                }
            }

            tintedTexture.Apply(); // Apply changes to the new texture
            return tintedTexture;
        }

        public bool TryGetTintedCursor(Color tintColor, out Texture2D tintedTexture)
        {
            return _tintedCursorCache.TryGetValue(tintColor, out tintedTexture);
        }
    }
}
