/*
	Generated by KBEngine!
	Please do not modify this file!
	Please inherit this module, such as: (class FrameSyncMgr : public FrameSyncMgrBase)
	tools = kbcmd
*/

#pragma once
#include "KBECommon.h"
#include "Entity.h"
#include "KBETypes.h"
#include "EntityCallFrameSyncMgrBase.h"

class Method;
class Property;
class MemoryStream;

// defined in */scripts/entity_defs/FrameSyncMgr.def
	// Please inherit and implement "class FrameSyncMgr : public FrameSyncMgrBase"
class KBENGINEPLUGINS_API FrameSyncMgrBase : public Entity
{
public:
	EntityBaseEntityCall_FrameSyncMgrBase* pBaseEntityCall;
	EntityCellEntityCall_FrameSyncMgrBase* pCellEntityCall;

	int8 state;
	virtual void onStateChanged(int8 oldValue) {}


	void onGetBase() override;
	void onGetCell() override;
	void onLoseCell() override;

	EntityCall* getBaseEntityCall() override;
	EntityCall* getCellEntityCall() override;


	void onRemoteMethodCall(MemoryStream& stream) override;
	void onUpdatePropertys(MemoryStream& stream) override;
	void callPropertysSetMethods() override;

	FrameSyncMgrBase();
	virtual ~FrameSyncMgrBase();

	void attachComponents() override;
	void detachComponents() override;

};

