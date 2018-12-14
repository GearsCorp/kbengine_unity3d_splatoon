using KBEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using GameLogic.Configs;
using KBEngine;

namespace SyncFrame
{
    public enum CMD
    {
        MOUSE    = 1,
        KEYBOARD = 2,
        JOYSTICK = 3, 
        USER = 3,
        TEST = 4,
        MAX =255,
    }

    public abstract class FrameBase
    {
        public FS_ENTITY_DATA e = new FS_ENTITY_DATA();
       // public FixedPointStream s = new FixedPointStream();

        public abstract FS_ENTITY_DATA Serialize();

        public abstract void PareseFrom(FS_ENTITY_DATA e);
    }


    public class FrameMouse : FrameBase
    {
        public FPVector point = FPVector.zero;
        public FrameMouse(CMD cmd, FPVector p)
        {
            e.cmd_type = (Byte)cmd;
            point = p;
        }

        public FrameMouse()
        {
        }

        public override FS_ENTITY_DATA Serialize()
        {
            //s.writeTSVector(point);
            //e.datas = new byte[s.wpos];
            //Array.Copy(s.data(),e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(FS_ENTITY_DATA e)
        {
            //this.e = e;
            //s.setBuffer(e.datas);
            //point = s.readTSVector();
        }

    }
   
    public class FrameKeyboard : FrameBase
    {
        public List<KeyCode> keys = new List<KeyCode>();

        public FrameKeyboard(CMD cmd, List<KeyCode> keys )
        {
            e.cmd_type = (Byte)cmd;
            this.keys = keys;
        }

        public FrameKeyboard()
        {
        }

        public override FS_ENTITY_DATA Serialize()
        {
            //for (int i = 0; i < keys.Count; i++)
            //{
            //    s.writeUint16((UInt16)keys[i]);
            //}
            //e.datas = new byte[s.wpos];
            //Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(FS_ENTITY_DATA e)
        {
            //this.e = e;
            //s.setBuffer(e.datas);

            //KeyCode k = KeyCode.None;

            //while ((k = (KeyCode)s.readUint16()) != 0)
            //{
            //    keys.Add(k);
            //}
        }
    }

    public class FrameTest:FrameBase
    {
        public FP fp_1;
        public FP fp_2;


        public FrameTest(CMD cmd, FP arg1,FP arg2)
        {
            e.cmd_type = (Byte)cmd;
            fp_1 = arg1;
            fp_2 = arg2;
        }

        public FrameTest()
        {
        }

        public override FS_ENTITY_DATA Serialize()
        {
            //s.writeFP(fp_1);
            //s.writeFP(fp_2);
            //e.datas = new byte[s.wpos];
            //Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(FS_ENTITY_DATA e)
        {
            //this.e = e;
            //s.setBuffer(e.datas);
            //fp_1 = s.readFP();
            //fp_2 = s.readFP();
        }
    }

    public class FrameJoystick : FrameBase
    {
        public UINT16 OperationType = OperationID.None; 
        public FP Angle = 0;
        public FrameJoystick(CMD cmd, UINT16 typeId, FP angle)
        {
            e.cmd_type = (Byte)cmd;
            OperationType = typeId;
            Angle = angle;
        }

        public override FS_ENTITY_DATA Serialize()
        {
            //s.writeUint16(OperationType);
            //s.writeFP(Angle);
            //e.datas = new byte[s.wpos];
            //Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(FS_ENTITY_DATA e)
        {
            //this.e = e;
            //s.setBuffer(e.datas);
            //OperationType = s.readUint16();

            //if (s.rpos != e.datas.Length)
            //{
            //    Angle = s.readFP();
            //}
        }
    }


    public class FrameUser : FrameBase
    {
        public FPVector movement = FPVector.zero;


        public FrameUser(CMD cmd, FPVector p)
        {
            e.cmd_type = (Byte)cmd;
            movement = p;
        }

        public FrameUser()
        {
        }

        public override FS_ENTITY_DATA Serialize()
        {
            //s.writeTSVector(movement);
            //e.datas = new byte[s.wpos];
            //Array.Copy(s.data(), e.datas, s.wpos);
            return e;
        }

        public override void PareseFrom(FS_ENTITY_DATA e)
        {
            //this.e = e;
            //s.setBuffer(e.datas);
            //movement = s.readTSVector();
        }
    }

    public class FrameProto
    {
        static public FS_ENTITY_DATA encode(FrameBase sendMsg)
        {
            return sendMsg.Serialize();
        }
        
        static public FrameBase decode(FS_ENTITY_DATA readMsg)
        {
            FrameBase f;

            switch ((CMD)readMsg.cmd_type)
            {
                case CMD.MOUSE:
                    {
                        f = new FrameMouse();
                    }
                    break;
                case CMD.KEYBOARD:
                    {
                        f = new FrameKeyboard();
                    }
                    break;
                case CMD.USER:
                    {
                        f = new FrameUser();
                    }
                    break;
                case CMD.TEST:
                    {
                        f = new FrameTest();
                    }
                    break;
                default:
                    {
                        f = null;
                    }
                    return f;
            }

            f.PareseFrom(readMsg);

            return f;
        }

    }


    //指令数据操作
    public class InstructionOperationBase
    {
        public FS_ENTITY_DATA e = new FS_ENTITY_DATA();
     //   public FixedPointStream s = new FixedPointStream();

        //public void PackagingData(CMD cmd,)
        //{
        //    e
        //}

    }

}

