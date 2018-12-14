#include "EntityCallOperationSyncBase.h"
#include "Bundle.h"


EntityBaseEntityCall_OperationSyncBase::EntityBaseEntityCall_OperationSyncBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "OperationSync"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_OperationSyncBase::~EntityBaseEntityCall_OperationSyncBase()
{
}



EntityCellEntityCall_OperationSyncBase::EntityCellEntityCall_OperationSyncBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "OperationSync"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_OperationSyncBase::~EntityCellEntityCall_OperationSyncBase()
{
}

void EntityCellEntityCall_OperationSyncBase::reportFrame(const ENTITY_DATA& arg1)
{
	Bundle* pBundleRet = newCall("reportFrame", entityComponentPropertyID);
	if(!pBundleRet)
		return;

	((DATATYPE_ENTITY_DATA*)EntityDef::id2datatypes[28])->addToStreamEx(*pBundleRet, arg1);
	sendCall(NULL);
}

