#include "FrameSyncBase.h"
#include "KBVar.h"
#include "Entity.h"
#include "EntityDef.h"
#include "ScriptModule.h"
#include "Property.h"
#include "Method.h"
#include "DataTypes.h"
#include "CustomDataTypes.h"
#include "MemoryStream.h"


void FrameSyncBase::createFromStream(MemoryStream& stream)
{
	EntityComponent::createFromStream(stream);
	pBaseEntityCall = new EntityBaseEntityCall_FrameSyncBase(entityComponentPropertyID, ownerID);
	pCellEntityCall = new EntityCellEntityCall_FrameSyncBase(entityComponentPropertyID, ownerID);
}

void FrameSyncBase::onRemoteMethodCall(uint16 methodUtype, MemoryStream& stream)
{
}

void FrameSyncBase::onUpdatePropertys(uint16 propUtype, MemoryStream& stream, int maxCount)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("FrameSync");

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

	}
}

void FrameSyncBase::callPropertysSetMethods()
{
	ScriptModule* sm = EntityDef::moduledefs["FrameSync"];
	TMap<uint16, Property*>& pdatas = sm->idpropertys;

}

FrameSyncBase::FrameSyncBase():
	EntityComponent(),
	pBaseEntityCall(NULL),
	pCellEntityCall(NULL)
{
}

FrameSyncBase::~FrameSyncBase()
{
	if(pBaseEntityCall)
		delete pBaseEntityCall;

	if(pCellEntityCall)
		delete pCellEntityCall;

}
