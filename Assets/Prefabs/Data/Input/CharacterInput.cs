// GENERATED AUTOMATICALLY FROM 'Assets/Prefabs/Data/Input/CharacterInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CustomInput
{
    public class @CharacterInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @CharacterInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInput"",
    ""maps"": [
        {
            ""name"": ""Character"",
            ""id"": ""f8c787fd-bb8a-4175-b372-008f40a26bf4"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ee7e0446-21f0-403f-8e6a-d686324d8eae"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""7c8f497d-fdd8-44c2-b1f3-1987b366087d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""cc743d31-ac53-4311-a132-da1084d28290"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Action"",
                    ""type"": ""Button"",
                    ""id"": ""0ca6b44c-e996-4860-b486-190df820c536"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SubAction"",
                    ""type"": ""Button"",
                    ""id"": ""66f7455e-a25b-4a26-a361-365b6c975138"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Talk"",
                    ""type"": ""Button"",
                    ""id"": ""119b7394-5f85-4208-bbca-b838e11ad8d3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveAssist"",
                    ""type"": ""Button"",
                    ""id"": ""77340b09-b5e4-4bff-bc98-25bf4c2c2e7b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Movement"",
                    ""id"": ""f4e4fea1-1602-4d2d-af8b-b7d5e82eec33"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6a1636e7-13a7-4417-9b07-d8e6f886e0d6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""16a4c854-9de1-4e87-a40e-b08fafd4d860"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5a976b73-780c-45c6-8ffb-33293677154a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""faa4f04d-d465-4cfb-8b96-7cdb3329344f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1de2509b-181f-453a-bc68-a6aaeda88bb8"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""247d3961-dfc1-4ec0-8dd9-0e0337c1fbe1"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Movement"",
                    ""id"": ""70d2e337-b46e-4521-a1b4-11b913df210b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""38e95ba5-23d7-4f26-904b-8f502fc2303e"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""080a989a-0c78-4c91-9a97-3e6a263ce051"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2be412ae-d4ea-4212-a1b3-53f654944f01"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a4df7904-fd1c-4951-aacb-cb29af1d2e0e"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f30f2958-17c5-4127-bd99-34e03b90c65d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""480191d6-bd4b-4b90-ae0b-546b95801825"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee1fd249-5b41-4206-8a42-ea08b0fba089"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3d015c2-9b33-4106-9f9c-3c111a6a4ccd"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""SubAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27b39b11-8310-490b-9c97-dcfb5695b03f"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""SubAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a9ec65e-d52a-4abc-8037-058b4d175907"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""SubAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d156d2a-125b-4454-aa32-4c760569c84f"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Talk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53918ab4-0278-47c0-a5b9-ea10f32c6aaf"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Talk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a41d239a-2909-4507-ac88-94a7d8eb4c75"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Talk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bcfc8777-1fc8-4aeb-94cd-2a3e8ed73bc9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Keyboard1;Gamepad;Keyboard2"",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31995395-b8fe-4bd2-9cad-b3306bdbaeb0"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f43e1aa9-fb33-401c-b265-23f9fac09810"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90b0c78e-2cda-4980-bb28-f301058ba345"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""178d7c1a-df68-42d8-bb78-805d844b411a"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a4f9ee3-8591-47fd-87c2-d19a86df145e"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54ae28e5-1ad7-48c9-ab09-07acb66e49be"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a62c4546-f398-4c6b-9833-6e7356ab4557"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""432fa433-0e2e-45c6-8bd9-d7cc09ce992d"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard1"",
                    ""action"": ""MoveAssist"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c18677c-d850-4095-b49a-67afdaff330b"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard2"",
                    ""action"": ""MoveAssist"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efb4a8d9-14df-488b-b44a-fdc183457860"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard1;Keyboard2"",
                    ""action"": ""MoveAssist"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard1"",
            ""bindingGroup"": ""Keyboard1"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard2"",
            ""bindingGroup"": ""Keyboard2"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Character
            m_Character = asset.FindActionMap("Character", throwIfNotFound: true);
            m_Character_Move = m_Character.FindAction("Move", throwIfNotFound: true);
            m_Character_Up = m_Character.FindAction("Up", throwIfNotFound: true);
            m_Character_Down = m_Character.FindAction("Down", throwIfNotFound: true);
            m_Character_Action = m_Character.FindAction("Action", throwIfNotFound: true);
            m_Character_SubAction = m_Character.FindAction("SubAction", throwIfNotFound: true);
            m_Character_Talk = m_Character.FindAction("Talk", throwIfNotFound: true);
            m_Character_MoveAssist = m_Character.FindAction("MoveAssist", throwIfNotFound: true);
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

        // Character
        private readonly InputActionMap m_Character;
        private ICharacterActions m_CharacterActionsCallbackInterface;
        private readonly InputAction m_Character_Move;
        private readonly InputAction m_Character_Up;
        private readonly InputAction m_Character_Down;
        private readonly InputAction m_Character_Action;
        private readonly InputAction m_Character_SubAction;
        private readonly InputAction m_Character_Talk;
        private readonly InputAction m_Character_MoveAssist;
        public struct CharacterActions
        {
            private @CharacterInput m_Wrapper;
            public CharacterActions(@CharacterInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Character_Move;
            public InputAction @Up => m_Wrapper.m_Character_Up;
            public InputAction @Down => m_Wrapper.m_Character_Down;
            public InputAction @Action => m_Wrapper.m_Character_Action;
            public InputAction @SubAction => m_Wrapper.m_Character_SubAction;
            public InputAction @Talk => m_Wrapper.m_Character_Talk;
            public InputAction @MoveAssist => m_Wrapper.m_Character_MoveAssist;
            public InputActionMap Get() { return m_Wrapper.m_Character; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CharacterActions set) { return set.Get(); }
            public void SetCallbacks(ICharacterActions instance)
            {
                if (m_Wrapper.m_CharacterActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Up.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnUp;
                    @Up.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnUp;
                    @Up.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnUp;
                    @Down.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDown;
                    @Down.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDown;
                    @Down.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDown;
                    @Action.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAction;
                    @Action.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAction;
                    @Action.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAction;
                    @SubAction.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSubAction;
                    @SubAction.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSubAction;
                    @SubAction.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnSubAction;
                    @Talk.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnTalk;
                    @Talk.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnTalk;
                    @Talk.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnTalk;
                    @MoveAssist.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMoveAssist;
                    @MoveAssist.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMoveAssist;
                    @MoveAssist.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMoveAssist;
                }
                m_Wrapper.m_CharacterActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Up.started += instance.OnUp;
                    @Up.performed += instance.OnUp;
                    @Up.canceled += instance.OnUp;
                    @Down.started += instance.OnDown;
                    @Down.performed += instance.OnDown;
                    @Down.canceled += instance.OnDown;
                    @Action.started += instance.OnAction;
                    @Action.performed += instance.OnAction;
                    @Action.canceled += instance.OnAction;
                    @SubAction.started += instance.OnSubAction;
                    @SubAction.performed += instance.OnSubAction;
                    @SubAction.canceled += instance.OnSubAction;
                    @Talk.started += instance.OnTalk;
                    @Talk.performed += instance.OnTalk;
                    @Talk.canceled += instance.OnTalk;
                    @MoveAssist.started += instance.OnMoveAssist;
                    @MoveAssist.performed += instance.OnMoveAssist;
                    @MoveAssist.canceled += instance.OnMoveAssist;
                }
            }
        }
        public CharacterActions @Character => new CharacterActions(this);
        private int m_Keyboard1SchemeIndex = -1;
        public InputControlScheme Keyboard1Scheme
        {
            get
            {
                if (m_Keyboard1SchemeIndex == -1) m_Keyboard1SchemeIndex = asset.FindControlSchemeIndex("Keyboard1");
                return asset.controlSchemes[m_Keyboard1SchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        private int m_Keyboard2SchemeIndex = -1;
        public InputControlScheme Keyboard2Scheme
        {
            get
            {
                if (m_Keyboard2SchemeIndex == -1) m_Keyboard2SchemeIndex = asset.FindControlSchemeIndex("Keyboard2");
                return asset.controlSchemes[m_Keyboard2SchemeIndex];
            }
        }
        public interface ICharacterActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnUp(InputAction.CallbackContext context);
            void OnDown(InputAction.CallbackContext context);
            void OnAction(InputAction.CallbackContext context);
            void OnSubAction(InputAction.CallbackContext context);
            void OnTalk(InputAction.CallbackContext context);
            void OnMoveAssist(InputAction.CallbackContext context);
        }
    }
}
