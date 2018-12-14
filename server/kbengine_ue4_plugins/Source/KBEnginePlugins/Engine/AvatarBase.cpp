#include "AvatarBase.h"
#include "KBVar.h"
#include "EntityDef.h"
#include "ScriptModule.h"
#include "Property.h"
#include "Method.h"
#include "DataTypes.h"
#include "CustomDataTypes.h"
#include "MemoryStream.h"
#include "EntityComponent.h"
#include "Scripts/Components/FrameSyncReport.h"


void AvatarBase::onGetBase()
{
	if(pBaseEntityCall)
		delete pBaseEntityCall;

	pBaseEntityCall = new EntityBaseEntityCall_AvatarBase(id(), className());
}

void AvatarBase::onGetCell()
{
	if(pCellEntityCall)
		delete pCellEntityCall;

	pCellEntityCall = new EntityCellEntityCall_AvatarBase(id(), className());
}

void AvatarBase::onLoseCell()
{
	delete pCellEntityCall;
	pCellEntityCall = NULL;
}

EntityCall* AvatarBase::getBaseEntityCall()
{
	return pBaseEntityCall;
}

EntityCall* AvatarBase::getCellEntityCall()
{
	return pCellEntityCall;
}

void AvatarBase::onRemoteMethodCall(MemoryStream& stream)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("Avatar");
	uint16 methodUtype = 0;
	uint16 componentPropertyUType = 0;

	if (sm->useMethodDescrAlias)
	{
		componentPropertyUType = stream.readUint8();
		methodUtype = stream.read<uint8>();
	}
	else
	{
		componentPropertyUType = stream.readUint16();
		methodUtype = stream.read<uint16>();
	}

	Method* pMethod = sm->idmethods[methodUtype];

	if(componentPropertyUType > 0)
	{
		Property* pComponentPropertyDescription = sm->idpropertys[componentPropertyUType];

		switch(pComponentPropertyDescription->properUtype)
		{
			case 15:
				componentFrameSync->onRemoteMethodCall(methodUtype, stream);
				break;
		}

		return;
	}

	switch(pMethod->methodUtype)
	{
		case 14:
		{
			onEnding();
			break;
		}
		case 10:
		{
			int32 onPlayerQuitMatch_arg1 = stream.readInt32();
			onPlayerQuitMatch(onPlayerQuitMatch_arg1);
			break;
		}
		case 12:
		{
			onReadyForBattle();
			break;
		}
		case 9:
		{
			MATCHING_INFOS_LIST onResPlayersInfo_arg1;
			((DATATYPE_MATCHING_INFOS_LIST*)pMethod->args[0])->createFromStreamEx(stream, onResPlayersInfo_arg1);
			onResPlayersInfo(onResPlayersInfo_arg1);
			break;
		}
		case 13:
		{
			onReturnHalls();
			break;
		}
		case 15:
		{
			uint8 onStatisticalResult_arg1 = stream.readUint8();
			float onStatisticalResult_arg2 = stream.readFloat();
			onStatisticalResult(onStatisticalResult_arg1, onStatisticalResult_arg2);
			break;
		}
		case 11:
		{
			onTeamMateChange();
			break;
		}
		default:
			break;
	};
}

void AvatarBase::onUpdatePropertys(MemoryStream& stream)
{
	ScriptModule* sm = *EntityDef::moduledefs.Find("Avatar");

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
			Property* pComponentPropertyDescription = sm->idpropertys[componentPropertyUType];

			switch(pComponentPropertyDescription->properUtype)
			{
				case 15:
					componentFrameSync->onUpdatePropertys(properUtype, stream, -1);
					break;
			}

			return;
		}

		Property* pProp = sm->idpropertys[properUtype];

		switch(pProp->properUtype)
		{
			case 15:
				componentFrameSync->createFromStream(stream);
				break;
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
			case 11:
			{
				int8 oldval_gameStateC = gameStateC;
				gameStateC = stream.readInt8();

				if(pProp->isBase())
				{
					if(inited())
						onGameStateCChanged(oldval_gameStateC);
				}
				else
				{
					if(inWorld())
						onGameStateCChanged(oldval_gameStateC);
				}

				break;
			}
			case 13:
			{
				uint8 oldval_modelID = modelID;
				modelID = stream.readUint8();

				if(pProp->isBase())
				{
					if(inited())
						onModelIDChanged(oldval_modelID);
				}
				else
				{
					if(inWorld())
						onModelIDChanged(oldval_modelID);
				}

				break;
			}
			case 8:
			{
				float oldval_moveSpeed = moveSpeed;
				moveSpeed = stream.readFloat();

				if(pProp->isBase())
				{
					if(inited())
						onMoveSpeedChanged(oldval_moveSpeed);
				}
				else
				{
					if(inWorld())
						onMoveSpeedChanged(oldval_moveSpeed);
				}

				break;
			}
			case 3:
			{
				FString oldval_name = name;
				name = stream.readUnicode();

				if(pProp->isBase())
				{
					if(inited())
						onNameChanged(oldval_name);
				}
				else
				{
					if(inWorld())
						onNameChanged(oldval_name);
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
			case 4:
			{
				uint8 oldval_roleType = roleType;
				roleType = stream.readUint8();

				if(pProp->isBase())
				{
					if(inited())
						onRoleTypeChanged(oldval_roleType);
				}
				else
				{
					if(inWorld())
						onRoleTypeChanged(oldval_roleType);
				}

				break;
			}
			case 40002:
			{
				stream.readUint32();
				break;
			}
			case 9:
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
			case 6:
			{
				uint8 oldval_teamID = teamID;
				teamID = stream.readUint8();

				if(pProp->isBase())
				{
					if(inited())
						onTeamIDChanged(oldval_teamID);
				}
				else
				{
					if(inWorld())
						onTeamIDChanged(oldval_teamID);
				}

				break;
			}
			case 5:
			{
				int32 oldval_weaponID = weaponID;
				weaponID = stream.readInt32();

				if(pProp->isBase())
				{
					if(inited())
						onWeaponIDChanged(oldval_weaponID);
				}
				else
				{
					if(inWorld())
						onWeaponIDChanged(oldval_weaponID);
				}

				break;
			}
			default:
				break;
		};
	}
}

void AvatarBase::callPropertysSetMethods()
{
	ScriptModule* sm = EntityDef::moduledefs["Avatar"];
	TMap<uint16, Property*>& pdatas = sm->idpropertys;

	componentFrameSync->callPropertysSetMethods();

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

	int8 oldval_gameStateC = gameStateC;
	Property* pProp_gameStateC = pdatas[5];
	if(pProp_gameStateC->isBase())
	{
		if(inited() && !inWorld())
			onGameStateCChanged(oldval_gameStateC);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_gameStateC->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onGameStateCChanged(oldval_gameStateC);
			}
		}
	}

	uint8 oldval_modelID = modelID;
	Property* pProp_modelID = pdatas[6];
	if(pProp_modelID->isBase())
	{
		if(inited() && !inWorld())
			onModelIDChanged(oldval_modelID);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_modelID->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onModelIDChanged(oldval_modelID);
			}
		}
	}

	float oldval_moveSpeed = moveSpeed;
	Property* pProp_moveSpeed = pdatas[7];
	if(pProp_moveSpeed->isBase())
	{
		if(inited() && !inWorld())
			onMoveSpeedChanged(oldval_moveSpeed);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_moveSpeed->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onMoveSpeedChanged(oldval_moveSpeed);
			}
		}
	}

	FString oldval_name = name;
	Property* pProp_name = pdatas[8];
	if(pProp_name->isBase())
	{
		if(inited() && !inWorld())
			onNameChanged(oldval_name);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_name->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onNameChanged(oldval_name);
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

	uint8 oldval_roleType = roleType;
	Property* pProp_roleType = pdatas[9];
	if(pProp_roleType->isBase())
	{
		if(inited() && !inWorld())
			onRoleTypeChanged(oldval_roleType);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_roleType->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onRoleTypeChanged(oldval_roleType);
			}
		}
	}

	int8 oldval_state = state;
	Property* pProp_state = pdatas[10];
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

	uint8 oldval_teamID = teamID;
	Property* pProp_teamID = pdatas[11];
	if(pProp_teamID->isBase())
	{
		if(inited() && !inWorld())
			onTeamIDChanged(oldval_teamID);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_teamID->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onTeamIDChanged(oldval_teamID);
			}
		}
	}

	int32 oldval_weaponID = weaponID;
	Property* pProp_weaponID = pdatas[12];
	if(pProp_weaponID->isBase())
	{
		if(inited() && !inWorld())
			onWeaponIDChanged(oldval_weaponID);
	}
	else
	{
		if(inWorld())
		{
			if(pProp_weaponID->isOwnerOnly() && !isPlayer())
			{
			}
			else
			{
				onWeaponIDChanged(oldval_weaponID);
			}
		}
	}

}

AvatarBase::AvatarBase():
	Entity(),
	pBaseEntityCall(NULL),
	pCellEntityCall(NULL),
	componentFrameSync(new FrameSyncReport()),
	gameStateC((int8)FCString::Atoi64(TEXT("0"))),
	modelID((uint8)FCString::Atoi64(TEXT("0"))),
	moveSpeed(FCString::Atof(TEXT("6.5"))),
	name(TEXT("")),
	roleType((uint8)FCString::Atoi64(TEXT("0"))),
	state((int8)FCString::Atoi64(TEXT("0"))),
	teamID((uint8)FCString::Atoi64(TEXT("0"))),
	weaponID((int32)FCString::Atoi64(TEXT("0")))
{
	componentFrameSync->pOwner = this;
	componentFrameSync->ownerID = id_;
	componentFrameSync->entityComponentPropertyID = 15;

}

AvatarBase::~AvatarBase()
{
	if(componentFrameSync)
		delete componentFrameSync;

	if(pBaseEntityCall)
		delete pBaseEntityCall;

	if(pCellEntityCall)
		delete pCellEntityCall;

}

void AvatarBase::attachComponents()
{
	componentFrameSync->onAttached(this);
}

void AvatarBase::detachComponents()
{
	componentFrameSync->onDetached(this);
}

