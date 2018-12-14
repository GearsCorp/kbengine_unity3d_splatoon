namespace KBEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FrameSyncReport : FrameSyncReportBase
    {
        public override void onFrameMessage(FS_FRAME_DATA arg1)
        {
            SpaceData.Instance.frameList.Enqueue(arg1);
     //      Debug.Log(System.DateTime.Now + "." + System.DateTime.Now.Millisecond+",onFrameMessage:" + arg1.frameid + arg1.operation.Count );

        }

        public override void onAttached(Entity ownerEntity)
        {
            if (ownerEntity.isPlayer())
            {
                SpaceData.Instance.localPlayer = this;
                KBEngine.Event.registerIn("reportFrame", this, "reportFrame");
            }
        }

        public virtual void reportFrame(FS_ENTITY_DATA operation)
        {
            operation.entityid = ownerID;
            operation.cmd_type = 1;
            cellEntityCall.reportFrame(operation);
            //Debug.Log(System.DateTime.Now + "." + System.DateTime.Now.Millisecond+",reportFrame:" + operation.entityid + operation.cmd_type + operation.datas);
        }
    }
}
