#include "EntityCallRoomBase.h"
#include "Bundle.h"


EntityBaseEntityCall_RoomBase::EntityBaseEntityCall_RoomBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_RoomBase::~EntityBaseEntityCall_RoomBase()
{
}



EntityCellEntityCall_RoomBase::EntityCellEntityCall_RoomBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_RoomBase::~EntityCellEntityCall_RoomBase()
{
}

