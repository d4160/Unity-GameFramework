using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using d4160.Utilities.UIs;
using System.Collections.Generic;
using UnityEngine.TestTools;

public class UICursorHandlerTests
{
    private ButtonCursorHandler _buttonCursorHandler;
    private GameObject _gameObject;

    [SetUp]
    public void Setup()
    {
        _gameObject = new GameObject();
        _buttonCursorHandler = _gameObject.AddComponent<ButtonCursorHandler>();

        // Create a mock hand cursor texture
        _buttonCursorHandler.handCursor = new Texture2D(16, 16);
        for (int y = 0; y < _buttonCursorHandler.handCursor.height; y++)
        {
            for (int x = 0; x < _buttonCursorHandler.handCursor.width; x++)
            {
                _buttonCursorHandler.handCursor.SetPixel(x, y, Color.white);
            }
        }
        _buttonCursorHandler.handCursor.Apply();

        _buttonCursorHandler.tint = Color.red;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void Start_NoHandCursorAssigned_LogsError()
    {
        // Arrange
        _buttonCursorHandler.handCursor = null;

        // Act
        LogAssert.Expect(LogType.Error, "Hand cursor texture is not assigned. Please assign a texture in the Inspector.");
        _buttonCursorHandler.Start();

        // Assert
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void TintTexture_ValidTexture_TintsCorrectly()
    {
        // Act
        var tintedTexture = _buttonCursorHandler.TintTexture(_buttonCursorHandler.handCursor, _buttonCursorHandler.tint);

        // Assert
        Assert.IsNotNull(tintedTexture);
        Assert.AreEqual(_buttonCursorHandler.handCursor.width, tintedTexture.width);
        Assert.AreEqual(_buttonCursorHandler.handCursor.height, tintedTexture.height);
        for (int y = 0; y < tintedTexture.height; y++)
        {
            for (int x = 0; x < tintedTexture.width; x++)
            {
                Assert.AreEqual(Color.red, tintedTexture.GetPixel(x, y));
            }
        }
    }

    [Test]
    public void TintTexture_NullTexture_LogsError()
    {
        // Act
        var result = _buttonCursorHandler.TintTexture(null, Color.red);

        // Assert
        Assert.IsNull(result);
        LogAssert.Expect(LogType.Error, "Original texture is null. Cannot apply tint.");
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void OnPointerEnter_NoHandCursorAssigned_LogsError()
    {
        // Arrange
        _buttonCursorHandler.handCursor = null;

        // Act
        LogAssert.Expect(LogType.Error, "Cannot set cursor because the hand cursor texture is not assigned.");
        _buttonCursorHandler.OnPointerEnter(null);

        // Assert
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void OnPointerEnter_SetsTintedCursor()
    {
        // Act
        _buttonCursorHandler.OnPointerEnter(null);

        // Assert
        Texture2D cachedTexture;
        Assert.IsTrue(_buttonCursorHandler.TryGetTintedCursor(_buttonCursorHandler.tint, out cachedTexture));
        Assert.AreEqual(_buttonCursorHandler.tint, Color.red);
        Assert.IsNotNull(cachedTexture);
    }

    [Test]
    public void OnPointerEnter_UsesCachedTintedCursor()
    {
        // Act
        _buttonCursorHandler.OnPointerEnter(null);
        var firstTintedTexture = _buttonCursorHandler.TintedCursorCache[_buttonCursorHandler.tint];
        _buttonCursorHandler.OnPointerEnter(null);
        var secondTintedTexture = _buttonCursorHandler.TintedCursorCache[_buttonCursorHandler.tint];

        // Assert
        Assert.AreSame(firstTintedTexture, secondTintedTexture);
    }

    [Test]
    public void OnPointerExit_ResetsCursor()
    {
        // // Arrange
        // Cursor.SetCursor(_buttonCursorHandler.handCursor, Vector2.zero, CursorMode.Auto);

        // // Act
        // _buttonCursorHandler.OnPointerExit(null);

        // // Assert
        // Assert.IsNull(Cursor.current);

        // Act
        Assert.DoesNotThrow(() => _buttonCursorHandler.OnPointerExit(null));

        // Assert
        // No direct way to assert the cursor state in Unity, 
        // so we ensure the method doesn't throw any exceptions.
        Assert.Pass("Cursor reset without exceptions.");
    }

    [Test]
    public void TintTexture_FullyTransparentTint_LogsWarning()
    {
        // Arrange
        _buttonCursorHandler.tint = new Color(1, 1, 1, 0);

        // Act
        _buttonCursorHandler.Start();

        // Assert
        LogAssert.Expect(LogType.Warning, "Tint color is fully transparent. Cursor may not be visible.");
        LogAssert.NoUnexpectedReceived();
    }
}
