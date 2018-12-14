#include "EntityCallFrameSyncBase.h"
#include "Bundle.h"


EntityBaseEntityCall_FrameSyncBase::EntityBaseEntityCall_FrameSyncBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "FrameSync"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_FrameSyncBase::~EntityBaseEntityCall_FrameSyncBase()
{
}



EntityCellEntityCall_FrameSyncBase::EntityCellEntityCall_FrameSyncBase(uint16 ecpID, int32 eid) : 
EntityCall(eid, "FrameSync"),
entityComponentPropertyID(0)
{
	entityComponentPropertyID = ecpID;
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_FrameSyncBase::~EntityCellEntityCall_FrameSyncBase()
{
}

