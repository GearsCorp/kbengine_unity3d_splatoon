#include "EntityCallFrameSyncReportBase.h"
#include "Bundle.h"


EntityBaseEntityCall_FrameSyncReportBase::EntityBaseEntityCall_FrameSyncReportBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "FrameSyncReport"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_FrameSyncReportBase::~EntityBaseEntityCall_FrameSyncReportBase()
{
}



EntityCellEntityCall_FrameSyncReportBase::EntityCellEntityCall_FrameSyncReportBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "FrameSyncReport"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_FrameSyncReportBase::~EntityCellEntityCall_FrameSyncReportBase()
{
}

void EntityCellEntityCall_FrameSyncReportBase::reportFrame(const FS_ENTITY_DATA& arg1)
{
	Bundle* pBundleRet = newCall("reportFrame", entityComponentPropertyID);
	if(!pBundleRet)
		return;

	((DATATYPE_FS_ENTITY_DATA*)EntityDef::id2datatypes[28])->addToStreamEx(*pBundleRet, arg1);
	sendCall(NULL);
}

