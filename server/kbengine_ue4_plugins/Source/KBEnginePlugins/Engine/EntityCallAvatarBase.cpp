#include "EntityCallAvatarBase.h"
#include "Bundle.h"


EntityBaseEntityCall_AvatarBase::EntityBaseEntityCall_AvatarBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	componentFrameSync = new EntityBaseEntityCall_FrameSyncReportBase(15, id);
	type = ENTITYCALL_TYPE_BASE;
}

EntityBaseEntityCall_AvatarBase::~EntityBaseEntityCall_AvatarBase()
{
	if(componentFrameSync)
		delete componentFrameSync;

}

void EntityBaseEntityCall_AvatarBase::endOfStatistics()
{
	Bundle* pBundleRet = newCall("endOfStatistics", 0);
	if(!pBundleRet)
		return;

	sendCall(NULL);
}

void EntityBaseEntityCall_AvatarBase::enterStartGame()
{
	Bundle* pBundleRet = newCall("enterStartGame", 0);
	if(!pBundleRet)
		return;

	sendCall(NULL);
}

void EntityBaseEntityCall_AvatarBase::registerHalls()
{
	Bundle* pBundleRet = newCall("registerHalls", 0);
	if(!pBundleRet)
		return;

	sendCall(NULL);
}

void EntityBaseEntityCall_AvatarBase::statisticalResult(uint8 arg1, float arg2)
{
	Bundle* pBundleRet = newCall("statisticalResult", 0);
	if(!pBundleRet)
		return;

	pBundleRet->writeUint8(arg1);
	pBundleRet->writeFloat(arg2);
	sendCall(NULL);
}

void EntityBaseEntityCall_AvatarBase::weaponChanged(int32 arg1)
{
	Bundle* pBundleRet = newCall("weaponChanged", 0);
	if(!pBundleRet)
		return;

	pBundleRet->writeInt32(arg1);
	sendCall(NULL);
}



EntityCellEntityCall_AvatarBase::EntityCellEntityCall_AvatarBase(int32 eid, const FString& ename) : EntityCall(eid, ename)
{
	componentFrameSync = new EntityCellEntityCall_FrameSyncReportBase(15, id);
	type = ENTITYCALL_TYPE_CELL;
}

EntityCellEntityCall_AvatarBase::~EntityCellEntityCall_AvatarBase()
{
	if(componentFrameSync)
		delete componentFrameSync;

}

