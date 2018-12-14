#include "EntityCallHallsBase.h"
#include "Bundle.h"


EntityBaseEntityCall_HallsBase::EntityBaseEntityCall_HallsBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_HallsBase::~EntityBaseEntityCall_HallsBase()
{
}



EntityCellEntityCall_HallsBase::EntityCellEntityCall_HallsBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_HallsBase::~EntityCellEntityCall_HallsBase()
{
}

