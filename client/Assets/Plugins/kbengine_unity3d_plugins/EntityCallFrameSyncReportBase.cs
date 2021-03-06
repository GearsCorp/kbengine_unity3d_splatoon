/*
	Generated by KBEngine!
	Please do not modify this file!
	
	tools = kbcmd
*/

namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	// defined in */scripts/entity_defs/FrameSyncReport.def
	public class EntityBaseEntityCall_FrameSyncReportBase : EntityCall
	{
		public UInt16 entityComponentPropertyID = 0;

		public EntityBaseEntityCall_FrameSyncReportBase(UInt16 ecpID, Int32 eid) : base(eid, "FrameSyncReport")
		{
			entityComponentPropertyID = ecpID;
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_BASE;
		}

	}

	public class EntityCellEntityCall_FrameSyncReportBase : EntityCall
	{
		public UInt16 entityComponentPropertyID = 0;

		public EntityCellEntityCall_FrameSyncReportBase(UInt16 ecpID, Int32 eid) : base(eid, "FrameSyncReport")
		{
			entityComponentPropertyID = ecpID;
			className = "FrameSyncReport";
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL;
		}

		public void reportFrame(FS_ENTITY_DATA arg1)
		{
			Bundle pBundle = newCall("reportFrame", entityComponentPropertyID);
			if(pBundle == null)
				return;

			((DATATYPE_FS_ENTITY_DATA)EntityDef.id2datatypes[28]).addToStreamEx(bundle, arg1);
			sendCall(null);
		}

	}
	}
