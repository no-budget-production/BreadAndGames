using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    public int playerNumber;

    public string xboxHorizontalLeftStick = "Horizontal_PX";
    public string xboxVerticalLeftStick = "Vertical_PX";
    public string xboxHorizontalRightStick = "HorizontalLook_PX";
    public string xboxVerticalLookRightStick = "VerticalLook_PX";
    public string xboxButtonA = "Button_A_PX";
    public string xboxButtonB = "Button_B_PX";
    public string xboxButtonX = "Button_X_PX";
    public string xboxButtonY = "Button_Y_PX";
    public string xboxButtonStart = "Button_Start_PX";
    public string xboxButtonBack = "Button_Back_PX";
    public string xboxButtonRightStick = "Button_RightStick_PX";
    public string xboxButtonLeftStick = "Button_LeftStick_PX";
    public string xboxTriggerLeft = "Trigger_Left_PX";
    public string xboxTriggerRight = "Trigger_Right_PX";
    public string xboxBumperLeft = "Bumper_Left_PX";
    public string xboxBumperRight = "Bumper_Right_PX";
    public string xboxPadLeftRight = "Pad_Left_PX";
    public string xboxPadUpDown = "Pad_Right_PX";

    void Start()
    {
        xboxHorizontalLeftStick += playerNumber;
        xboxVerticalLeftStick += playerNumber;
        xboxHorizontalRightStick += playerNumber;
        xboxVerticalLookRightStick += playerNumber;
        xboxButtonA += playerNumber;
        xboxButtonB += playerNumber;
        xboxButtonX += playerNumber;
        xboxButtonY += playerNumber;
        xboxButtonStart += playerNumber;
        xboxButtonBack += playerNumber;
        xboxButtonRightStick += playerNumber;
        xboxButtonLeftStick += playerNumber;
        xboxTriggerLeft += playerNumber;
        xboxTriggerRight += playerNumber;
        xboxBumperLeft += playerNumber;
        xboxBumperRight += playerNumber;
        xboxPadLeftRight += playerNumber;
        xboxPadUpDown += playerNumber;
    }
}
