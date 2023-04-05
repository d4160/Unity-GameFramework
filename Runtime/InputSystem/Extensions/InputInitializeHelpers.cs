using UnityEngine;
using UnityEngine.InputSystem;

namespace d4160.InputSystem
{
    public static class InputInitializeHelpers
    {
        /// <summary>
        /// Used to force in the same machine 2 schemas with the same KeyboardMouse
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PlayerInput ForceInitializeKeyboardMouse(this PlayerInput input)
        {
            var instance = PlayerInput.Instantiate(input.gameObject, controlScheme: "KeyboardMouse", pairWithDevices: new InputDevice[] { Keyboard.current, Mouse.current });

            instance.transform.parent = input.transform.parent;
            instance.transform.position = input.transform.position;

            Object.Destroy(input.gameObject);
            return instance;
        }
    }
}