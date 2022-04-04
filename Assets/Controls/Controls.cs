// GENERATED AUTOMATICALLY FROM 'Assets/Controls/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player01"",
            ""id"": ""3ff73a44-5e28-433d-94e6-1b593f525cac"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""5f168eea-6cc5-42b0-95e1-17af1c273293"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""809fba56-83a4-47ca-bb5f-4b87e9fa235b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""86a3d62a-6f4b-40a7-870d-a0a3e15ca451"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9d175461-1d9a-4195-9bb7-afa1cf1e2009"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player02"",
            ""id"": ""b2433b24-f702-480c-9d0b-64f54da1eb6a"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""6d2783e7-ce1e-4fb4-8940-80eee3afcdad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""27ac363b-d778-4c50-bb9a-c26d4b831b7b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""497944c8-6114-42e6-8dc7-816e8a0bffd9"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3fce9f00-e9b6-4fb4-a585-56cc70742bba"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""AllControls"",
            ""bindingGroup"": ""AllControls"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player01
        m_Player01 = asset.FindActionMap("Player01", throwIfNotFound: true);
        m_Player01_Left = m_Player01.FindAction("Left", throwIfNotFound: true);
        m_Player01_Right = m_Player01.FindAction("Right", throwIfNotFound: true);
        // Player02
        m_Player02 = asset.FindActionMap("Player02", throwIfNotFound: true);
        m_Player02_Left = m_Player02.FindAction("Left", throwIfNotFound: true);
        m_Player02_Right = m_Player02.FindAction("Right", throwIfNotFound: true);
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

    // Player01
    private readonly InputActionMap m_Player01;
    private IPlayer01Actions m_Player01ActionsCallbackInterface;
    private readonly InputAction m_Player01_Left;
    private readonly InputAction m_Player01_Right;
    public struct Player01Actions
    {
        private @Controls m_Wrapper;
        public Player01Actions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_Player01_Left;
        public InputAction @Right => m_Wrapper.m_Player01_Right;
        public InputActionMap Get() { return m_Wrapper.m_Player01; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Player01Actions set) { return set.Get(); }
        public void SetCallbacks(IPlayer01Actions instance)
        {
            if (m_Wrapper.m_Player01ActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_Player01ActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_Player01ActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_Player01ActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_Player01ActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_Player01ActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_Player01ActionsCallbackInterface.OnRight;
            }
            m_Wrapper.m_Player01ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
            }
        }
    }
    public Player01Actions @Player01 => new Player01Actions(this);

    // Player02
    private readonly InputActionMap m_Player02;
    private IPlayer02Actions m_Player02ActionsCallbackInterface;
    private readonly InputAction m_Player02_Left;
    private readonly InputAction m_Player02_Right;
    public struct Player02Actions
    {
        private @Controls m_Wrapper;
        public Player02Actions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_Player02_Left;
        public InputAction @Right => m_Wrapper.m_Player02_Right;
        public InputActionMap Get() { return m_Wrapper.m_Player02; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Player02Actions set) { return set.Get(); }
        public void SetCallbacks(IPlayer02Actions instance)
        {
            if (m_Wrapper.m_Player02ActionsCallbackInterface != null)
            {
                @Left.started -= m_Wrapper.m_Player02ActionsCallbackInterface.OnLeft;
                @Left.performed -= m_Wrapper.m_Player02ActionsCallbackInterface.OnLeft;
                @Left.canceled -= m_Wrapper.m_Player02ActionsCallbackInterface.OnLeft;
                @Right.started -= m_Wrapper.m_Player02ActionsCallbackInterface.OnRight;
                @Right.performed -= m_Wrapper.m_Player02ActionsCallbackInterface.OnRight;
                @Right.canceled -= m_Wrapper.m_Player02ActionsCallbackInterface.OnRight;
            }
            m_Wrapper.m_Player02ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Left.started += instance.OnLeft;
                @Left.performed += instance.OnLeft;
                @Left.canceled += instance.OnLeft;
                @Right.started += instance.OnRight;
                @Right.performed += instance.OnRight;
                @Right.canceled += instance.OnRight;
            }
        }
    }
    public Player02Actions @Player02 => new Player02Actions(this);
    private int m_AllControlsSchemeIndex = -1;
    public InputControlScheme AllControlsScheme
    {
        get
        {
            if (m_AllControlsSchemeIndex == -1) m_AllControlsSchemeIndex = asset.FindControlSchemeIndex("AllControls");
            return asset.controlSchemes[m_AllControlsSchemeIndex];
        }
    }
    public interface IPlayer01Actions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
    }
    public interface IPlayer02Actions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
    }
}
