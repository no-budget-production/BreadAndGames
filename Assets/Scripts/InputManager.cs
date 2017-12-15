using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Entity
{ 
    public string setPlayerNumber;

    // XBox Controller
    private string _xboxHorizontalLeftStick = "Horizontal_PX";
    private string _xboxVerticalLeftStick = "Vertical_PX";
    private string _xboxHorizontalRightStick = "HorizontalLook_PX";
    private string _xboxVerticalLookRightStick = "VerticalLook_PX";

    private string _xboxButtonA = "Button_A_PX";
    private string _xboxButtonB = "Button_B_PX";
    private string _xboxButtonX = "Button_X_PX";
    private string _xboxButtonY = "Button_Y_PX";
    private string _xboxButtonStart = "Button_Start_PX";
    private string _xboxButtonBack = "Button_Back_PX";
    private string _xboxButtonRightStick = "Button_RightStick_PX";
    private string _xboxButtonLeftStick = "Button_LeftStick_PX";

    private string _xboxTriggerLeft = "Trigger_Left_PX";
    private string _xboxTriggerRight = "Trigger_Right_PX";
    private string _xboxBumperLeft = "Bumper_Left_PX";
    private string _xboxBumperRight = "Bumper_Right_PX";

    private string _xboxPadLeftRight = "Pad_Left_PX";
    private string _xboxPadUpDown = "Pad_Right_PX";

    public string XBoxHorizontalLeftStick{ get { return _xboxHorizontalLeftStick; } }
    public string XBoxVerticalLeftStick { get { return _xboxVerticalLeftStick; } }
    public string XBoxHorizontalRightStick { get { return _xboxHorizontalRightStick; } }
    public string XBoxVerticalLookRightStick { get { return _xboxVerticalLookRightStick; } }

    public string XBoxButtonA { get { return _xboxButtonA; } }
    public string XBoxButtonB { get { return _xboxButtonB; } }
    public string XBoxButtonX { get { return _xboxButtonX; } }
    public string XBoxButtonY { get { return _xboxButtonY; } }
    public string XBoxButtonStart { get { return _xboxButtonStart; } }
    public string XBoxButtonBack { get { return _xboxButtonBack; } }
    public string XBoxButtonRightStick { get { return _xboxButtonRightStick; } }
    public string XBoxButtonLeftStick { get { return _xboxButtonLeftStick; } }

    public string XBoxTriggerLeft { get { return _xboxTriggerLeft; } }
    public string XBoxTriggerRight { get { return _xboxTriggerRight; } }
    public string XBoxBumperLeft { get { return _xboxBumperLeft; } }
    public string XBoxBumperRight { get { return _xboxBumperRight; } }

    public string XBoxPadLeftRight { get { return _xboxPadLeftRight; } }
    public string XBoxPadUpDown { get { return _xboxPadUpDown; } }

    void Awake()
    {
        SetPlayerInputs();
    }

    void SetPlayerInputs()
    {
        _xboxHorizontalLeftStick += setPlayerNumber;
        _xboxVerticalLeftStick += setPlayerNumber;
        _xboxHorizontalRightStick += setPlayerNumber;
        _xboxVerticalLookRightStick += setPlayerNumber;
        _xboxButtonA += setPlayerNumber;
        _xboxButtonB += setPlayerNumber;
        _xboxButtonX += setPlayerNumber;
        _xboxButtonY += setPlayerNumber;
        _xboxButtonStart += setPlayerNumber;
        _xboxButtonBack += setPlayerNumber;
        _xboxButtonRightStick += setPlayerNumber;
        _xboxButtonLeftStick += setPlayerNumber;
        _xboxTriggerLeft += setPlayerNumber;
        _xboxTriggerRight += setPlayerNumber;
        _xboxBumperLeft += setPlayerNumber;
        _xboxBumperRight += setPlayerNumber;
        _xboxPadLeftRight += setPlayerNumber;
        _xboxPadUpDown += setPlayerNumber;
    }
}
