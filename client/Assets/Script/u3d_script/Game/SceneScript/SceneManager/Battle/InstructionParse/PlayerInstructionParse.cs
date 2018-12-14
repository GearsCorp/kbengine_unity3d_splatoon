using System;
using System.Collections;
using System.Collections.Generic;
using KBEngine;
using SyncFrame;
using UnityEngine;

public class PlayerInstructionParse : InstructionParseBase {

    public override void InstructionParse(Byte cmd_type, byte[] datas)
    {
        switch(cmd_type)
        {
            case (Byte)CMD.MOUSE:
                break;
            case (Byte)CMD.KEYBOARD:
                break;
            case (Byte)CMD.JOYSTICK:
                break;
        }
    }

    public override void InstructionParse(FS_ENTITY_DATA data, FP speedRate)
    {
      
        switch (data.cmd_type)
        {
            case (Byte)CMD.MOUSE:
                break;
            case (Byte)CMD.KEYBOARD:
                break;
            case (Byte)CMD.JOYSTICK:
                AnalyseDataFromJoystick(data, speedRate);
                break;
        }
    }

    public void AnalyseDataFromJoystick(FS_ENTITY_DATA data, FP speedRate)
    {
        FrameJoystick msg = FrameProto.decode(data) as FrameJoystick;
        TriggerEvent<FP,FP>(msg.OperationType, speedRate, msg.Angle);
    }


}
