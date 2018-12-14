#include "OperationSyncBase.h"
#include "KBVar.h"
#include "Entity.h"
#include "EntityDef.h"
#include "ScriptModule.h"
#include "Property.h"
#include "Method.h"
#include "DataTypes.h"
#include "CustomDataTypes.h"
#include "MemoryStream.h"


void OperationSyncBase::createFromStream(MemoryStream& stream)
{
	EntityComponent::createFromStream(stream);
	pBaseEntityCall = new EntityBaseEntityCall_OperationSyncBase(entityComponentPropertyID, ownerID);
	pCellEntityCall = new EntityCellEntityCall_OperationSyncBase(entityComponentPropertyID, ownerID);
}

void OperationSyncBase::onRemoteMethodCall(uint16 methodUtype, MemoryStream& stream)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("OperationSync");
	Method* pMethod = sm->idmethods[methodUtype];

	switch(pMethod->methodUtype)
	{
		case 27:
		{
			FRAME_DATA onFrameMessage_arg1;
			((DATATYPE_FRAME_DATA*)pMethod->args[0])->createFromStreamEx(stream, onFrameMessage_arg1);
			onFrameMessage(onFrameMessage_arg1);
			break;
		}
		default:
			break;
	};
}

void OperationSyncBase::onUpdatePropertys(uint16 propUtype, MemoryStream& stream, int maxCount)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("OperationSync");

	while(stream.length() > 0 && maxCount-- != 0)
	{
		uint16 componentPropertyUType = 0;
		uint16 properUtype = propUtype;

		if (properUtype == 0)
		{
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
		}

		Property* pProp = sm->idpropertys[properUtype];

		switch(pProp->properUtype)
		{
			case 17:
			{
				int8 oldval_seatNo = seatNo;
				seatNo = stream.readInt8();

				if(pProp->isBase())
				{
					if(pOwner->inited())
						onSeatNoChanged(oldval_seatNo);
				}
				else
				{
					if(pOwner->inWorld())
						onSeatNoChanged(oldval_seatNo);
				}

				break;
			}
			default:
				break;
		};
	}
}

void OperationSyncBase::callPropertysSetMethods()
{
	ScriptModule* sm = EntityDef::moduledefs["OperationSync"];
	TMap<uint16, Property*>& pdatas = sm->idpropertys;

	int8 oldval_seatNo = seatNo;
	Property* pProp_seatNo = pdatas[4];
	if(pProp_seatNo->isBase())
	{
		if(pOwner->inited() && !pOwner->inWorld())
			onSeatNoChanged(oldval_seatNo);
	}
	else
	{
		if(pOwner->inWorld())
		{
			if(pProp_seatNo->isOwnerOnly() && !pOwner->isPlayer())
			{
			}
			else
			{
				onSeatNoChanged(oldval_seatNo);
			}
		}
	}

}

OperationSyncBase::OperationSyncBase():
	EntityComponent(),
	pBaseEntityCall(NULL),
	pCellEntityCall(NULL),
	seatNo((int8)FCString::Atoi64(TEXT("0")))
{
}

OperationSyncBase::~OperationSyncBase()
{
	if(pBaseEntityCall)
		delete pBaseEntityCall;

	if(pCellEntityCall)
		delete pCellEntityCall;

}
