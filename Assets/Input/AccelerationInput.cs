// GENERATED AUTOMATICALLY FROM 'Assets/Input/AccelerationInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @AccelerationInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @AccelerationInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""AccelerationInput"",
    ""maps"": [
        {
            ""name"": ""Acceleration"",
            ""id"": ""475b3dd5-e913-4c18-94d9-90a2570094c8"",
            ""actions"": [
                {
                    ""name"": ""Acceleration"",
                    ""type"": ""Value"",
                    ""id"": ""e190afc4-6a00-40b4-9f60-d6f24bef17b7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""DPAD"",
                    ""id"": ""55a1c24c-1895-4d9d-88e0-a126d17831c6"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""cc2ed7ea-9a5b-42ad-9eb4-c21c549223e3"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""14922af9-59d3-4ced-ae41-53632de9225e"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""06e188c0-07b2-482c-b77f-4bff318e8107"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3990d794-67df-4275-9606-e0059886d222"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Acceleration"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Acceleration
        m_Acceleration = asset.FindActionMap("Acceleration", throwIfNotFound: true);
        m_Acceleration_Acceleration = m_Acceleration.FindAction("Acceleration", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Acceleration
    private readonly InputActionMap m_Acceleration;
    private IAccelerationActions m_AccelerationActionsCallbackInterface;
    private readonly InputAction m_Acceleration_Acceleration;
    public struct AccelerationActions
    {
        private @AccelerationInput m_Wrapper;
        public AccelerationActions(@AccelerationInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Acceleration => m_Wrapper.m_Acceleration_Acceleration;
        public InputActionMap Get() { return m_Wrapper.m_Acceleration; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(AccelerationActions set) { return set.Get(); }
        public void SetCallbacks(IAccelerationActions instance)
        {
            if (m_Wrapper.m_AccelerationActionsCallbackInterface != null)
            {
                @Acceleration.started -= m_Wrapper.m_AccelerationActionsCallbackInterface.OnAcceleration;
                @Acceleration.performed -= m_Wrapper.m_AccelerationActionsCallbackInterface.OnAcceleration;
                @Acceleration.canceled -= m_Wrapper.m_AccelerationActionsCallbackInterface.OnAcceleration;
            }
            m_Wrapper.m_AccelerationActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Acceleration.started += instance.OnAcceleration;
                @Acceleration.performed += instance.OnAcceleration;
                @Acceleration.canceled += instance.OnAcceleration;
            }
        }
    }
    public AccelerationActions @Acceleration => new AccelerationActions(this);
    public interface IAccelerationActions
    {
        void OnAcceleration(InputAction.CallbackContext context);
    }
}
