using CBFrame.Core;
using KBEngine;
using SyncFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstructionParseBase : CBComponent
{
    public abstract void InstructionParse(Byte cmd_type, byte[] datas);
    public abstract void InstructionParse(FS_ENTITY_DATA data, FP speedRate);
}
