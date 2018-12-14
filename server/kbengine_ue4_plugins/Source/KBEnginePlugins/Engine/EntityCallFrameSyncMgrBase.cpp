#include "EntityCallFrameSyncMgrBase.h"
#include "Bundle.h"


EntityBaseEntityCall_FrameSyncMgrBase::EntityBaseEntityCall_FrameSyncMgrBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_FrameSyncMgrBase::~EntityBaseEntityCall_FrameSyncMgrBase()
{
}



EntityCellEntityCall_FrameSyncMgrBase::EntityCellEntityCall_FrameSyncMgrBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_FrameSyncMgrBase::~EntityCellEntityCall_FrameSyncMgrBase()
{
}

