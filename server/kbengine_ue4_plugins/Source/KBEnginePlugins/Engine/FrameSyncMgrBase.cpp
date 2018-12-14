#include "FrameSyncMgrBase.h"
#include "KBVar.h"
#include "EntityDef.h"
#include "ScriptModule.h"
#include "Property.h"
#include "Method.h"
#include "DataTypes.h"
#include "CustomDataTypes.h"
#include "MemoryStream.h"
#include "EntityComponent.h"


void FrameSyncMgrBase::onGetBase()
{
	if(pBaseEntityCall)
		delete pBaseEntityCall;

	pBaseEntityCall = new EntityBaseEntityCall_FrameSyncMgrBase(id(), className());
}

void FrameSyncMgrBase::onGetCell()
{
	if(pCellEntityCall)
		delete pCellEntityCall;

	pCellEntityCall = new EntityCellEntityCall_FrameSyncMgrBase(id(), className());
}

void FrameSyncMgrBase::onLoseCell()
{
	delete pCellEntityCall;
	pCellEntityCall = NULL;
}

EntityCall* FrameSyncMgrBase::getBaseEntityCall()
{
	return pBaseEntityCall;
}

EntityCall* FrameSyncMgrBase::getCellEntityCall()
{
	return pCellEntityCall;
}

void FrameSyncMgrBase::onRemoteMethodCall(MemoryStream& stream)
{
}

void FrameSyncMgrBase::onUpdatePropertys(MemoryStream& stream)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("FrameSyncMgr");

	while(stream.length() > 0)
	{
		uint16 componentPropertyUType = 0;
		uint16 properUtype = 0;

		if (sm->usePropertyDescrAlias)
		{
			componentPropertyUType = stream.readUint8();
			properUtype = stream.read<uint8>();
		}
		else
		{
			componentPropertyUType = stream.readUint16();
			properUtype = stream.read<uint16>();
		}

		if(componentPropertyUType > 0)
		{
			KBE_ASSERT(false);

			return;
		}

		Property* pProp = sm->idpropertys[properUtype];

		switch(pProp->properUtype)
		{
			case 40001:
			{
				FVector oldval_direction = direction;
				direction = stream.readVector3();

				if(pProp->isBase())
				{
					if(inited())
						onDirectionChanged(oldval_direction);
				}
				else
				{
					if(inWorld())
						onDirectionChanged(oldval_direction);
				}

				break;
			}
			case 40000:
			{
				FVector oldval_position = position;
				position = stream.readVector3();

				if(pProp->isBase())
				{
					if(inited())
						onPositionChanged(oldval_position);
				}
				else
				{
					if(inWorld())
						onPositionChanged(oldval_position);
				}

				break;
			}
			case 40002:
			{
				stream.readUint32();
				break;
			}
			case 23:
			{
				int8 oldval_state = state;
				state = stream.readInt8();

				if(pProp->isBase())
				{
					if(inited())
						onStateChanged(oldval_state);
				}
				else
				{
					if(inWorld())
						onStateChanged(oldval_state);
				}

				break;
			}
			default:
				break;
		};
	}
}

void FrameSyncMgrBase::callPropertysSetMethods()
{
	ScriptModule* sm = EntityDef::moduledefs["FrameSyncMgr"];
	TMap<uint16, Property*>& pdatas = sm->idpropertys;

	FVector oldval_direction = direction;
	Property* pProp_direction = pdatas[2];
	if(pProp_direction->isBase())
	{
		if(inited() && !inWorld())
			onDirectionChanged(oldval_direction);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_direction->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onDirectionChanged(oldval_direction);
			}
		}
	}

	FVector oldval_position = position;
	Property* pProp_position = pdatas[1];
	if(pProp_position->isBase())
	{
		if(inited() && !inWorld())
			onPositionChanged(oldval_position);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_position->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onPositionChanged(oldval_position);
			}
		}
	}

	int8 oldval_state = state;
	Property* pProp_state = pdatas[4];
	if(pProp_state->isBase())
	{
		if(inited() && !inWorld())
			onStateChanged(oldval_state);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_state->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onStateChanged(oldval_state);
			}
		}
	}

}

FrameSyncMgrBase::FrameSyncMgrBase():
	Entity(),
	pBaseEntityCall(NULL),
	pCellEntityCall(NULL),
	state((int8)FCString::Atoi64(TEXT("0")))
{
}

FrameSyncMgrBase::~FrameSyncMgrBase()
{
	if(pBaseEntityCall)
		delete pBaseEntityCall;

	if(pCellEntityCall)
		delete pCellEntityCall;

}

void FrameSyncMgrBase::attachComponents()
{
}

void FrameSyncMgrBase::detachComponents()
{
}

