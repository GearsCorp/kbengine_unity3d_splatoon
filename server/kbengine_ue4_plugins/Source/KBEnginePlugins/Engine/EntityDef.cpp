#include "EntityDef.h"
#include "DataTypes.h"
#include "CustomDataTypes.h"
#include "ScriptModule.h"
#include "Property.h"
#include "Method.h"
#include "KBVar.h"
#include "Entity.h"

#include "Scripts/Avatar.h"
#include "Scripts/Components/FrameSyncReport.h"
#include "Scripts/Account.h"

TMap<FString, uint16> EntityDef::datatype2id;
TMap<FString, DATATYPE_BASE*> EntityDef::datatypes;
TMap<uint16, DATATYPE_BASE*> EntityDef::id2datatypes;
TMap<FString, int32> EntityDef::entityclass;
TMap<FString, ScriptModule*> EntityDef::moduledefs;
TMap<uint16, ScriptModule*> EntityDef::idmoduledefs;

bool EntityDef::initialize()
{
	initDataTypes();
	initDefTypes();
	initScriptModules();
	return true;
}

bool EntityDef::reset()
{
	clear();
	return initialize();
}

void EntityDef::clear()
{
	TArray<DATATYPE_BASE*> deleted_datatypes;
	for (auto& item : EntityDef::datatypes)
	{
		int32 idx = deleted_datatypes.Find(item.Value);
		if (idx != INDEX_NONE)
			continue;

		deleted_datatypes.Add(item.Value);
		delete item.Value;
	}

	for (auto& item : EntityDef::moduledefs)
		delete item.Value;

	datatype2id.Empty();
	datatypes.Empty();
	id2datatypes.Empty();
	entityclass.Empty();
	moduledefs.Empty();
	idmoduledefs.Empty();
}

void EntityDef::initDataTypes()
{
	datatypes.Add(TEXT("UINT8"), new DATATYPE_UINT8());
	datatypes.Add(TEXT("UINT16"), new DATATYPE_UINT16());
	datatypes.Add(TEXT("UINT32"), new DATATYPE_UINT32());
	datatypes.Add(TEXT("UINT64"), new DATATYPE_UINT64());

	datatypes.Add(TEXT("INT8"), new DATATYPE_INT8());
	datatypes.Add(TEXT("INT16"), new DATATYPE_INT16());
	datatypes.Add(TEXT("INT32"), new DATATYPE_INT32());
	datatypes.Add(TEXT("INT64"), new DATATYPE_INT64());

	datatypes.Add(TEXT("FLOAT"), new DATATYPE_FLOAT());
	datatypes.Add(TEXT("DOUBLE"), new DATATYPE_DOUBLE());

	datatypes.Add(TEXT("STRING"), new DATATYPE_STRING());
	datatypes.Add(TEXT("VECTOR2"), new DATATYPE_VECTOR2());

	datatypes.Add(TEXT("VECTOR3"), new DATATYPE_VECTOR3());

	datatypes.Add(TEXT("VECTOR4"), new DATATYPE_VECTOR4());
	datatypes.Add(TEXT("PYTHON"), new DATATYPE_PYTHON());

	datatypes.Add(TEXT("UNICODE"), new DATATYPE_UNICODE());
	datatypes.Add(TEXT("ENTITYCALL"), new DATATYPE_ENTITYCALL());

	datatypes.Add(TEXT("BLOB"), new DATATYPE_BLOB());
}

Entity* EntityDef::createEntity(int utype)
{
	Entity* pEntity = NULL;

	switch(utype)
	{
		case 1:
			pEntity = new Avatar();
			break;
		case 3:
			pEntity = new Account();
			break;
		default:
			SCREEN_ERROR_MSG("EntityDef::createEntity() : entity(%d) not found!", utype);
			break;
	};

	return pEntity;
}

void EntityDef::initScriptModules()
{
	ScriptModule* pAvatarModule = new ScriptModule("Avatar", 1);
	EntityDef::moduledefs.Add(TEXT("Avatar"), pAvatarModule);
	EntityDef::idmoduledefs.Add(1, pAvatarModule);

	Property* pAvatar_position = new Property();
	pAvatar_position->name = TEXT("position");
	pAvatar_position->properUtype = 40000;
	pAvatar_position->properFlags = 4;
	pAvatar_position->aliasID = 1;
	KBVar* pAvatar_position_defval = new KBVar(FVector());
	pAvatar_position->pDefaultVal = pAvatar_position_defval;
	pAvatarModule->propertys.Add(TEXT("position"), pAvatar_position); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_position->aliasID, pAvatar_position);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(position / 40000).");

	Property* pAvatar_direction = new Property();
	pAvatar_direction->name = TEXT("direction");
	pAvatar_direction->properUtype = 40001;
	pAvatar_direction->properFlags = 4;
	pAvatar_direction->aliasID = 2;
	KBVar* pAvatar_direction_defval = new KBVar(FVector());
	pAvatar_direction->pDefaultVal = pAvatar_direction_defval;
	pAvatarModule->propertys.Add(TEXT("direction"), pAvatar_direction); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_direction->aliasID, pAvatar_direction);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(direction / 40001).");

	Property* pAvatar_spaceID = new Property();
	pAvatar_spaceID->name = TEXT("spaceID");
	pAvatar_spaceID->properUtype = 40002;
	pAvatar_spaceID->properFlags = 16;
	pAvatar_spaceID->aliasID = 3;
	KBVar* pAvatar_spaceID_defval = new KBVar((uint32)FCString::Atoi64(TEXT("")));
	pAvatar_spaceID->pDefaultVal = pAvatar_spaceID_defval;
	pAvatarModule->propertys.Add(TEXT("spaceID"), pAvatar_spaceID); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_spaceID->aliasID, pAvatar_spaceID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(spaceID / 40002).");

	Property* pAvatar_componentFrameSync = new Property();
	pAvatar_componentFrameSync->name = TEXT("componentFrameSync");
	pAvatar_componentFrameSync->properUtype = 15;
	pAvatar_componentFrameSync->properFlags = 157;
	pAvatar_componentFrameSync->aliasID = 4;
	pAvatarModule->propertys.Add(TEXT("componentFrameSync"), pAvatar_componentFrameSync); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_componentFrameSync->aliasID, pAvatar_componentFrameSync);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(componentFrameSync / 15).");

	Property* pAvatar_gameStateC = new Property();
	pAvatar_gameStateC->name = TEXT("gameStateC");
	pAvatar_gameStateC->properUtype = 11;
	pAvatar_gameStateC->properFlags = 4;
	pAvatar_gameStateC->aliasID = 5;
	KBVar* pAvatar_gameStateC_defval = new KBVar((int8)FCString::Atoi64(TEXT("0")));
	pAvatar_gameStateC->pDefaultVal = pAvatar_gameStateC_defval;
	pAvatarModule->propertys.Add(TEXT("gameStateC"), pAvatar_gameStateC); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_gameStateC->aliasID, pAvatar_gameStateC);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(gameStateC / 11).");

	Property* pAvatar_modelID = new Property();
	pAvatar_modelID->name = TEXT("modelID");
	pAvatar_modelID->properUtype = 13;
	pAvatar_modelID->properFlags = 4;
	pAvatar_modelID->aliasID = 6;
	KBVar* pAvatar_modelID_defval = new KBVar((uint8)FCString::Atoi64(TEXT("0")));
	pAvatar_modelID->pDefaultVal = pAvatar_modelID_defval;
	pAvatarModule->propertys.Add(TEXT("modelID"), pAvatar_modelID); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_modelID->aliasID, pAvatar_modelID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(modelID / 13).");

	Property* pAvatar_moveSpeed = new Property();
	pAvatar_moveSpeed->name = TEXT("moveSpeed");
	pAvatar_moveSpeed->properUtype = 8;
	pAvatar_moveSpeed->properFlags = 4;
	pAvatar_moveSpeed->aliasID = 7;
	KBVar* pAvatar_moveSpeed_defval = new KBVar(FCString::Atof(TEXT("6.5")));
	pAvatar_moveSpeed->pDefaultVal = pAvatar_moveSpeed_defval;
	pAvatarModule->propertys.Add(TEXT("moveSpeed"), pAvatar_moveSpeed); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_moveSpeed->aliasID, pAvatar_moveSpeed);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(moveSpeed / 8).");

	Property* pAvatar_name = new Property();
	pAvatar_name->name = TEXT("name");
	pAvatar_name->properUtype = 3;
	pAvatar_name->properFlags = 4;
	pAvatar_name->aliasID = 8;
	KBVar* pAvatar_name_defval = new KBVar(FString());
	pAvatar_name->pDefaultVal = pAvatar_name_defval;
	pAvatarModule->propertys.Add(TEXT("name"), pAvatar_name); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_name->aliasID, pAvatar_name);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(name / 3).");

	Property* pAvatar_roleType = new Property();
	pAvatar_roleType->name = TEXT("roleType");
	pAvatar_roleType->properUtype = 4;
	pAvatar_roleType->properFlags = 4;
	pAvatar_roleType->aliasID = 9;
	KBVar* pAvatar_roleType_defval = new KBVar((uint8)FCString::Atoi64(TEXT("0")));
	pAvatar_roleType->pDefaultVal = pAvatar_roleType_defval;
	pAvatarModule->propertys.Add(TEXT("roleType"), pAvatar_roleType); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_roleType->aliasID, pAvatar_roleType);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(roleType / 4).");

	Property* pAvatar_state = new Property();
	pAvatar_state->name = TEXT("state");
	pAvatar_state->properUtype = 9;
	pAvatar_state->properFlags = 4;
	pAvatar_state->aliasID = 10;
	KBVar* pAvatar_state_defval = new KBVar((int8)FCString::Atoi64(TEXT("0")));
	pAvatar_state->pDefaultVal = pAvatar_state_defval;
	pAvatarModule->propertys.Add(TEXT("state"), pAvatar_state); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_state->aliasID, pAvatar_state);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(state / 9).");

	Property* pAvatar_teamID = new Property();
	pAvatar_teamID->name = TEXT("teamID");
	pAvatar_teamID->properUtype = 6;
	pAvatar_teamID->properFlags = 4;
	pAvatar_teamID->aliasID = 11;
	KBVar* pAvatar_teamID_defval = new KBVar((uint8)FCString::Atoi64(TEXT("0")));
	pAvatar_teamID->pDefaultVal = pAvatar_teamID_defval;
	pAvatarModule->propertys.Add(TEXT("teamID"), pAvatar_teamID); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_teamID->aliasID, pAvatar_teamID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(teamID / 6).");

	Property* pAvatar_weaponID = new Property();
	pAvatar_weaponID->name = TEXT("weaponID");
	pAvatar_weaponID->properUtype = 5;
	pAvatar_weaponID->properFlags = 4;
	pAvatar_weaponID->aliasID = 12;
	KBVar* pAvatar_weaponID_defval = new KBVar((int32)FCString::Atoi64(TEXT("0")));
	pAvatar_weaponID->pDefaultVal = pAvatar_weaponID_defval;
	pAvatarModule->propertys.Add(TEXT("weaponID"), pAvatar_weaponID); 

	pAvatarModule->usePropertyDescrAlias = true;
	pAvatarModule->idpropertys.Add((uint16)pAvatar_weaponID->aliasID, pAvatar_weaponID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), property(weaponID / 5).");

	TArray<DATATYPE_BASE*> Avatar_onEnding_args;

	Method* pAvatar_onEnding = new Method();
	pAvatar_onEnding->name = TEXT("onEnding");
	pAvatar_onEnding->methodUtype = 14;
	pAvatar_onEnding->aliasID = 1;
	pAvatar_onEnding->args = Avatar_onEnding_args;

	pAvatarModule->methods.Add(TEXT("onEnding"), pAvatar_onEnding); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onEnding->aliasID, pAvatar_onEnding);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onEnding / 14).");

	TArray<DATATYPE_BASE*> Avatar_onPlayerQuitMatch_args;
	Avatar_onPlayerQuitMatch_args.Add(EntityDef::id2datatypes[8]);

	Method* pAvatar_onPlayerQuitMatch = new Method();
	pAvatar_onPlayerQuitMatch->name = TEXT("onPlayerQuitMatch");
	pAvatar_onPlayerQuitMatch->methodUtype = 10;
	pAvatar_onPlayerQuitMatch->aliasID = 2;
	pAvatar_onPlayerQuitMatch->args = Avatar_onPlayerQuitMatch_args;

	pAvatarModule->methods.Add(TEXT("onPlayerQuitMatch"), pAvatar_onPlayerQuitMatch); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onPlayerQuitMatch->aliasID, pAvatar_onPlayerQuitMatch);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onPlayerQuitMatch / 10).");

	TArray<DATATYPE_BASE*> Avatar_onReadyForBattle_args;

	Method* pAvatar_onReadyForBattle = new Method();
	pAvatar_onReadyForBattle->name = TEXT("onReadyForBattle");
	pAvatar_onReadyForBattle->methodUtype = 12;
	pAvatar_onReadyForBattle->aliasID = 3;
	pAvatar_onReadyForBattle->args = Avatar_onReadyForBattle_args;

	pAvatarModule->methods.Add(TEXT("onReadyForBattle"), pAvatar_onReadyForBattle); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onReadyForBattle->aliasID, pAvatar_onReadyForBattle);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onReadyForBattle / 12).");

	TArray<DATATYPE_BASE*> Avatar_onResPlayersInfo_args;
	Avatar_onResPlayersInfo_args.Add(EntityDef::id2datatypes[26]);

	Method* pAvatar_onResPlayersInfo = new Method();
	pAvatar_onResPlayersInfo->name = TEXT("onResPlayersInfo");
	pAvatar_onResPlayersInfo->methodUtype = 9;
	pAvatar_onResPlayersInfo->aliasID = 4;
	pAvatar_onResPlayersInfo->args = Avatar_onResPlayersInfo_args;

	pAvatarModule->methods.Add(TEXT("onResPlayersInfo"), pAvatar_onResPlayersInfo); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onResPlayersInfo->aliasID, pAvatar_onResPlayersInfo);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onResPlayersInfo / 9).");

	TArray<DATATYPE_BASE*> Avatar_onReturnHalls_args;

	Method* pAvatar_onReturnHalls = new Method();
	pAvatar_onReturnHalls->name = TEXT("onReturnHalls");
	pAvatar_onReturnHalls->methodUtype = 13;
	pAvatar_onReturnHalls->aliasID = 5;
	pAvatar_onReturnHalls->args = Avatar_onReturnHalls_args;

	pAvatarModule->methods.Add(TEXT("onReturnHalls"), pAvatar_onReturnHalls); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onReturnHalls->aliasID, pAvatar_onReturnHalls);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onReturnHalls / 13).");

	TArray<DATATYPE_BASE*> Avatar_onStatisticalResult_args;
	Avatar_onStatisticalResult_args.Add(EntityDef::id2datatypes[2]);
	Avatar_onStatisticalResult_args.Add(EntityDef::id2datatypes[13]);

	Method* pAvatar_onStatisticalResult = new Method();
	pAvatar_onStatisticalResult->name = TEXT("onStatisticalResult");
	pAvatar_onStatisticalResult->methodUtype = 15;
	pAvatar_onStatisticalResult->aliasID = 6;
	pAvatar_onStatisticalResult->args = Avatar_onStatisticalResult_args;

	pAvatarModule->methods.Add(TEXT("onStatisticalResult"), pAvatar_onStatisticalResult); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onStatisticalResult->aliasID, pAvatar_onStatisticalResult);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onStatisticalResult / 15).");

	TArray<DATATYPE_BASE*> Avatar_onTeamMateChange_args;

	Method* pAvatar_onTeamMateChange = new Method();
	pAvatar_onTeamMateChange->name = TEXT("onTeamMateChange");
	pAvatar_onTeamMateChange->methodUtype = 11;
	pAvatar_onTeamMateChange->aliasID = 7;
	pAvatar_onTeamMateChange->args = Avatar_onTeamMateChange_args;

	pAvatarModule->methods.Add(TEXT("onTeamMateChange"), pAvatar_onTeamMateChange); 
	pAvatarModule->useMethodDescrAlias = true;
	pAvatarModule->idmethods.Add((uint16)pAvatar_onTeamMateChange->aliasID, pAvatar_onTeamMateChange);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(onTeamMateChange / 11).");

	TArray<DATATYPE_BASE*> Avatar_endOfStatistics_args;

	Method* pAvatar_endOfStatistics = new Method();
	pAvatar_endOfStatistics->name = TEXT("endOfStatistics");
	pAvatar_endOfStatistics->methodUtype = 7;
	pAvatar_endOfStatistics->aliasID = -1;
	pAvatar_endOfStatistics->args = Avatar_endOfStatistics_args;

	pAvatarModule->methods.Add(TEXT("endOfStatistics"), pAvatar_endOfStatistics); 
	pAvatarModule->base_methods.Add(TEXT("endOfStatistics"), pAvatar_endOfStatistics);

	pAvatarModule->idbase_methods.Add(pAvatar_endOfStatistics->methodUtype, pAvatar_endOfStatistics);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(endOfStatistics / 7).");

	TArray<DATATYPE_BASE*> Avatar_enterStartGame_args;

	Method* pAvatar_enterStartGame = new Method();
	pAvatar_enterStartGame->name = TEXT("enterStartGame");
	pAvatar_enterStartGame->methodUtype = 5;
	pAvatar_enterStartGame->aliasID = -1;
	pAvatar_enterStartGame->args = Avatar_enterStartGame_args;

	pAvatarModule->methods.Add(TEXT("enterStartGame"), pAvatar_enterStartGame); 
	pAvatarModule->base_methods.Add(TEXT("enterStartGame"), pAvatar_enterStartGame);

	pAvatarModule->idbase_methods.Add(pAvatar_enterStartGame->methodUtype, pAvatar_enterStartGame);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(enterStartGame / 5).");

	TArray<DATATYPE_BASE*> Avatar_registerHalls_args;

	Method* pAvatar_registerHalls = new Method();
	pAvatar_registerHalls->name = TEXT("registerHalls");
	pAvatar_registerHalls->methodUtype = 4;
	pAvatar_registerHalls->aliasID = -1;
	pAvatar_registerHalls->args = Avatar_registerHalls_args;

	pAvatarModule->methods.Add(TEXT("registerHalls"), pAvatar_registerHalls); 
	pAvatarModule->base_methods.Add(TEXT("registerHalls"), pAvatar_registerHalls);

	pAvatarModule->idbase_methods.Add(pAvatar_registerHalls->methodUtype, pAvatar_registerHalls);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(registerHalls / 4).");

	TArray<DATATYPE_BASE*> Avatar_statisticalResult_args;
	Avatar_statisticalResult_args.Add(EntityDef::id2datatypes[2]);
	Avatar_statisticalResult_args.Add(EntityDef::id2datatypes[13]);

	Method* pAvatar_statisticalResult = new Method();
	pAvatar_statisticalResult->name = TEXT("statisticalResult");
	pAvatar_statisticalResult->methodUtype = 6;
	pAvatar_statisticalResult->aliasID = -1;
	pAvatar_statisticalResult->args = Avatar_statisticalResult_args;

	pAvatarModule->methods.Add(TEXT("statisticalResult"), pAvatar_statisticalResult); 
	pAvatarModule->base_methods.Add(TEXT("statisticalResult"), pAvatar_statisticalResult);

	pAvatarModule->idbase_methods.Add(pAvatar_statisticalResult->methodUtype, pAvatar_statisticalResult);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(statisticalResult / 6).");

	TArray<DATATYPE_BASE*> Avatar_weaponChanged_args;
	Avatar_weaponChanged_args.Add(EntityDef::id2datatypes[8]);

	Method* pAvatar_weaponChanged = new Method();
	pAvatar_weaponChanged->name = TEXT("weaponChanged");
	pAvatar_weaponChanged->methodUtype = 8;
	pAvatar_weaponChanged->aliasID = -1;
	pAvatar_weaponChanged->args = Avatar_weaponChanged_args;

	pAvatarModule->methods.Add(TEXT("weaponChanged"), pAvatar_weaponChanged); 
	pAvatarModule->base_methods.Add(TEXT("weaponChanged"), pAvatar_weaponChanged);

	pAvatarModule->idbase_methods.Add(pAvatar_weaponChanged->methodUtype, pAvatar_weaponChanged);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Avatar), method(weaponChanged / 8).");

	ScriptModule* pFrameSyncReportModule = new ScriptModule("FrameSyncReport", 2);
	EntityDef::moduledefs.Add(TEXT("FrameSyncReport"), pFrameSyncReportModule);
	EntityDef::idmoduledefs.Add(2, pFrameSyncReportModule);

	Property* pFrameSyncReport_position = new Property();
	pFrameSyncReport_position->name = TEXT("position");
	pFrameSyncReport_position->properUtype = 40000;
	pFrameSyncReport_position->properFlags = 4;
	pFrameSyncReport_position->aliasID = 1;
	KBVar* pFrameSyncReport_position_defval = new KBVar(FVector());
	pFrameSyncReport_position->pDefaultVal = pFrameSyncReport_position_defval;
	pFrameSyncReportModule->propertys.Add(TEXT("position"), pFrameSyncReport_position); 

	pFrameSyncReportModule->usePropertyDescrAlias = true;
	pFrameSyncReportModule->idpropertys.Add((uint16)pFrameSyncReport_position->aliasID, pFrameSyncReport_position);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), property(position / 40000).");

	Property* pFrameSyncReport_direction = new Property();
	pFrameSyncReport_direction->name = TEXT("direction");
	pFrameSyncReport_direction->properUtype = 40001;
	pFrameSyncReport_direction->properFlags = 4;
	pFrameSyncReport_direction->aliasID = 2;
	KBVar* pFrameSyncReport_direction_defval = new KBVar(FVector());
	pFrameSyncReport_direction->pDefaultVal = pFrameSyncReport_direction_defval;
	pFrameSyncReportModule->propertys.Add(TEXT("direction"), pFrameSyncReport_direction); 

	pFrameSyncReportModule->usePropertyDescrAlias = true;
	pFrameSyncReportModule->idpropertys.Add((uint16)pFrameSyncReport_direction->aliasID, pFrameSyncReport_direction);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), property(direction / 40001).");

	Property* pFrameSyncReport_spaceID = new Property();
	pFrameSyncReport_spaceID->name = TEXT("spaceID");
	pFrameSyncReport_spaceID->properUtype = 40002;
	pFrameSyncReport_spaceID->properFlags = 16;
	pFrameSyncReport_spaceID->aliasID = 3;
	KBVar* pFrameSyncReport_spaceID_defval = new KBVar((uint32)FCString::Atoi64(TEXT("")));
	pFrameSyncReport_spaceID->pDefaultVal = pFrameSyncReport_spaceID_defval;
	pFrameSyncReportModule->propertys.Add(TEXT("spaceID"), pFrameSyncReport_spaceID); 

	pFrameSyncReportModule->usePropertyDescrAlias = true;
	pFrameSyncReportModule->idpropertys.Add((uint16)pFrameSyncReport_spaceID->aliasID, pFrameSyncReport_spaceID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), property(spaceID / 40002).");

	Property* pFrameSyncReport_seatNo = new Property();
	pFrameSyncReport_seatNo->name = TEXT("seatNo");
	pFrameSyncReport_seatNo->properUtype = 17;
	pFrameSyncReport_seatNo->properFlags = 16;
	pFrameSyncReport_seatNo->aliasID = 4;
	KBVar* pFrameSyncReport_seatNo_defval = new KBVar((int8)FCString::Atoi64(TEXT("0")));
	pFrameSyncReport_seatNo->pDefaultVal = pFrameSyncReport_seatNo_defval;
	pFrameSyncReportModule->propertys.Add(TEXT("seatNo"), pFrameSyncReport_seatNo); 

	pFrameSyncReportModule->usePropertyDescrAlias = true;
	pFrameSyncReportModule->idpropertys.Add((uint16)pFrameSyncReport_seatNo->aliasID, pFrameSyncReport_seatNo);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), property(seatNo / 17).");

	TArray<DATATYPE_BASE*> FrameSyncReport_onFrameMessage_args;
	FrameSyncReport_onFrameMessage_args.Add(EntityDef::id2datatypes[29]);

	Method* pFrameSyncReport_onFrameMessage = new Method();
	pFrameSyncReport_onFrameMessage->name = TEXT("onFrameMessage");
	pFrameSyncReport_onFrameMessage->methodUtype = 18;
	pFrameSyncReport_onFrameMessage->aliasID = 1;
	pFrameSyncReport_onFrameMessage->args = FrameSyncReport_onFrameMessage_args;

	pFrameSyncReportModule->methods.Add(TEXT("onFrameMessage"), pFrameSyncReport_onFrameMessage); 
	pFrameSyncReportModule->useMethodDescrAlias = true;
	pFrameSyncReportModule->idmethods.Add((uint16)pFrameSyncReport_onFrameMessage->aliasID, pFrameSyncReport_onFrameMessage);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), method(onFrameMessage / 18).");

	TArray<DATATYPE_BASE*> FrameSyncReport_reportFrame_args;
	FrameSyncReport_reportFrame_args.Add(EntityDef::id2datatypes[28]);

	Method* pFrameSyncReport_reportFrame = new Method();
	pFrameSyncReport_reportFrame->name = TEXT("reportFrame");
	pFrameSyncReport_reportFrame->methodUtype = 16;
	pFrameSyncReport_reportFrame->aliasID = -1;
	pFrameSyncReport_reportFrame->args = FrameSyncReport_reportFrame_args;

	pFrameSyncReportModule->methods.Add(TEXT("reportFrame"), pFrameSyncReport_reportFrame); 
	pFrameSyncReportModule->cell_methods.Add(TEXT("reportFrame"), pFrameSyncReport_reportFrame);

	pFrameSyncReportModule->idcell_methods.Add(pFrameSyncReport_reportFrame->methodUtype, pFrameSyncReport_reportFrame);

	//DEBUG_MSG("EntityDef::initScriptModules: add(FrameSyncReport), method(reportFrame / 16).");

	ScriptModule* pAccountModule = new ScriptModule("Account", 3);
	EntityDef::moduledefs.Add(TEXT("Account"), pAccountModule);
	EntityDef::idmoduledefs.Add(3, pAccountModule);

	Property* pAccount_position = new Property();
	pAccount_position->name = TEXT("position");
	pAccount_position->properUtype = 40000;
	pAccount_position->properFlags = 4;
	pAccount_position->aliasID = 1;
	KBVar* pAccount_position_defval = new KBVar(FVector());
	pAccount_position->pDefaultVal = pAccount_position_defval;
	pAccountModule->propertys.Add(TEXT("position"), pAccount_position); 

	pAccountModule->usePropertyDescrAlias = true;
	pAccountModule->idpropertys.Add((uint16)pAccount_position->aliasID, pAccount_position);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), property(position / 40000).");

	Property* pAccount_direction = new Property();
	pAccount_direction->name = TEXT("direction");
	pAccount_direction->properUtype = 40001;
	pAccount_direction->properFlags = 4;
	pAccount_direction->aliasID = 2;
	KBVar* pAccount_direction_defval = new KBVar(FVector());
	pAccount_direction->pDefaultVal = pAccount_direction_defval;
	pAccountModule->propertys.Add(TEXT("direction"), pAccount_direction); 

	pAccountModule->usePropertyDescrAlias = true;
	pAccountModule->idpropertys.Add((uint16)pAccount_direction->aliasID, pAccount_direction);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), property(direction / 40001).");

	Property* pAccount_spaceID = new Property();
	pAccount_spaceID->name = TEXT("spaceID");
	pAccount_spaceID->properUtype = 40002;
	pAccount_spaceID->properFlags = 16;
	pAccount_spaceID->aliasID = 3;
	KBVar* pAccount_spaceID_defval = new KBVar((uint32)FCString::Atoi64(TEXT("")));
	pAccount_spaceID->pDefaultVal = pAccount_spaceID_defval;
	pAccountModule->propertys.Add(TEXT("spaceID"), pAccount_spaceID); 

	pAccountModule->usePropertyDescrAlias = true;
	pAccountModule->idpropertys.Add((uint16)pAccount_spaceID->aliasID, pAccount_spaceID);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), property(spaceID / 40002).");

	TArray<DATATYPE_BASE*> Account_onCreateAvatarResult_args;
	Account_onCreateAvatarResult_args.Add(EntityDef::id2datatypes[22]);

	Method* pAccount_onCreateAvatarResult = new Method();
	pAccount_onCreateAvatarResult->name = TEXT("onCreateAvatarResult");
	pAccount_onCreateAvatarResult->methodUtype = 24;
	pAccount_onCreateAvatarResult->aliasID = 1;
	pAccount_onCreateAvatarResult->args = Account_onCreateAvatarResult_args;

	pAccountModule->methods.Add(TEXT("onCreateAvatarResult"), pAccount_onCreateAvatarResult); 
	pAccountModule->useMethodDescrAlias = true;
	pAccountModule->idmethods.Add((uint16)pAccount_onCreateAvatarResult->aliasID, pAccount_onCreateAvatarResult);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), method(onCreateAvatarResult / 24).");

	TArray<DATATYPE_BASE*> Account_onReqAvatarList_args;
	Account_onReqAvatarList_args.Add(EntityDef::id2datatypes[23]);

	Method* pAccount_onReqAvatarList = new Method();
	pAccount_onReqAvatarList->name = TEXT("onReqAvatarList");
	pAccount_onReqAvatarList->methodUtype = 23;
	pAccount_onReqAvatarList->aliasID = 2;
	pAccount_onReqAvatarList->args = Account_onReqAvatarList_args;

	pAccountModule->methods.Add(TEXT("onReqAvatarList"), pAccount_onReqAvatarList); 
	pAccountModule->useMethodDescrAlias = true;
	pAccountModule->idmethods.Add((uint16)pAccount_onReqAvatarList->aliasID, pAccount_onReqAvatarList);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), method(onReqAvatarList / 23).");

	TArray<DATATYPE_BASE*> Account_enterGameRoom_args;
	Account_enterGameRoom_args.Add(EntityDef::id2datatypes[5]);

	Method* pAccount_enterGameRoom = new Method();
	pAccount_enterGameRoom->name = TEXT("enterGameRoom");
	pAccount_enterGameRoom->methodUtype = 21;
	pAccount_enterGameRoom->aliasID = -1;
	pAccount_enterGameRoom->args = Account_enterGameRoom_args;

	pAccountModule->methods.Add(TEXT("enterGameRoom"), pAccount_enterGameRoom); 
	pAccountModule->base_methods.Add(TEXT("enterGameRoom"), pAccount_enterGameRoom);

	pAccountModule->idbase_methods.Add(pAccount_enterGameRoom->methodUtype, pAccount_enterGameRoom);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), method(enterGameRoom / 21).");

	TArray<DATATYPE_BASE*> Account_reqAvatarList_args;

	Method* pAccount_reqAvatarList = new Method();
	pAccount_reqAvatarList->name = TEXT("reqAvatarList");
	pAccount_reqAvatarList->methodUtype = 19;
	pAccount_reqAvatarList->aliasID = -1;
	pAccount_reqAvatarList->args = Account_reqAvatarList_args;

	pAccountModule->methods.Add(TEXT("reqAvatarList"), pAccount_reqAvatarList); 
	pAccountModule->base_methods.Add(TEXT("reqAvatarList"), pAccount_reqAvatarList);

	pAccountModule->idbase_methods.Add(pAccount_reqAvatarList->methodUtype, pAccount_reqAvatarList);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), method(reqAvatarList / 19).");

	TArray<DATATYPE_BASE*> Account_reqCreateAvatar_args;
	Account_reqCreateAvatar_args.Add(EntityDef::id2datatypes[2]);
	Account_reqCreateAvatar_args.Add(EntityDef::id2datatypes[12]);

	Method* pAccount_reqCreateAvatar = new Method();
	pAccount_reqCreateAvatar->name = TEXT("reqCreateAvatar");
	pAccount_reqCreateAvatar->methodUtype = 20;
	pAccount_reqCreateAvatar->aliasID = -1;
	pAccount_reqCreateAvatar->args = Account_reqCreateAvatar_args;

	pAccountModule->methods.Add(TEXT("reqCreateAvatar"), pAccount_reqCreateAvatar); 
	pAccountModule->base_methods.Add(TEXT("reqCreateAvatar"), pAccount_reqCreateAvatar);

	pAccountModule->idbase_methods.Add(pAccount_reqCreateAvatar->methodUtype, pAccount_reqCreateAvatar);

	//DEBUG_MSG("EntityDef::initScriptModules: add(Account), method(reqCreateAvatar / 20).");

}

void EntityDef::initDefTypes()
{
	{
		uint16 utype = 2;
		FString typeName = TEXT("CMD_TYPE");
		FString name = TEXT("UINT8");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 3;
		FString typeName = TEXT("UINT16");
		FString name = TEXT("UINT16");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 5;
		FString typeName = TEXT("SPACE_KEY");
		FString name = TEXT("UINT64");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 4;
		FString typeName = TEXT("FRAMEID");
		FString name = TEXT("UINT32");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 6;
		FString typeName = TEXT("INT8");
		FString name = TEXT("INT8");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 7;
		FString typeName = TEXT("INT16");
		FString name = TEXT("INT16");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 8;
		FString typeName = TEXT("ROOMSTATE");
		FString name = TEXT("INT32");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 9;
		FString typeName = TEXT("INT64");
		FString name = TEXT("INT64");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 1;
		FString typeName = TEXT("STRING");
		FString name = TEXT("STRING");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 12;
		FString typeName = TEXT("NAME");
		FString name = TEXT("UNICODE");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 13;
		FString typeName = TEXT("FLOAT");
		FString name = TEXT("FLOAT");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 14;
		FString typeName = TEXT("DOUBLE");
		FString name = TEXT("DOUBLE");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 10;
		FString typeName = TEXT("PYTHON");
		FString name = TEXT("PYTHON");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 10;
		FString typeName = TEXT("PY_DICT");
		FString name = TEXT("PY_DICT");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 10;
		FString typeName = TEXT("PY_TUPLE");
		FString name = TEXT("PY_TUPLE");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 10;
		FString typeName = TEXT("PY_LIST");
		FString name = TEXT("PY_LIST");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 20;
		FString typeName = TEXT("ENTITYCALL");
		FString name = TEXT("ENTITYCALL");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 11;
		FString typeName = TEXT("BLOB");
		FString name = TEXT("BLOB");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 15;
		FString typeName = TEXT("VECTOR2");
		FString name = TEXT("VECTOR2");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 16;
		FString typeName = TEXT("DIRECTION3D");
		FString name = TEXT("VECTOR3");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 17;
		FString typeName = TEXT("VECTOR4");
		FString name = TEXT("VECTOR4");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 22;
		FString typeName = TEXT("AVATAR_INFOS");
		DATATYPE_AVATAR_INFOS* pDatatype = new DATATYPE_AVATAR_INFOS();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 23;
		FString typeName = TEXT("AVATAR_INFOS_LIST");
		DATATYPE_AVATAR_INFOS_LIST* pDatatype = new DATATYPE_AVATAR_INFOS_LIST();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 25;
		FString typeName = TEXT("MATCHING_INFOS");
		DATATYPE_MATCHING_INFOS* pDatatype = new DATATYPE_MATCHING_INFOS();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 26;
		FString typeName = TEXT("MATCHING_INFOS_LIST");
		DATATYPE_MATCHING_INFOS_LIST* pDatatype = new DATATYPE_MATCHING_INFOS_LIST();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 28;
		FString typeName = TEXT("FS_ENTITY_DATA");
		DATATYPE_FS_ENTITY_DATA* pDatatype = new DATATYPE_FS_ENTITY_DATA();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 29;
		FString typeName = TEXT("FS_FRAME_DATA");
		DATATYPE_FS_FRAME_DATA* pDatatype = new DATATYPE_FS_FRAME_DATA();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 31;
		FString typeName = TEXT("FS_FRAME_LIST");
		DATATYPE_FS_FRAME_LIST* pDatatype = new DATATYPE_FS_FRAME_LIST();
		EntityDef::datatypes.Add(typeName, (DATATYPE_BASE*)pDatatype);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 33;
		FString typeName = TEXT("AnonymousArray_33");
		FString name = TEXT("ENTITY_COMPONENT");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	{
		uint16 utype = 34;
		FString typeName = TEXT("AnonymousArray_34");
		FString name = TEXT("ENTITY_COMPONENT");
		DATATYPE_BASE** fPtr = EntityDef::datatypes.Find(name);
		DATATYPE_BASE* pVal = fPtr != NULL ? *fPtr : NULL;
		EntityDef::datatypes.Add(typeName, pVal);
		EntityDef::id2datatypes.Add(utype, EntityDef::datatypes[typeName]);
		EntityDef::datatype2id.Add(typeName, utype);
	}

	for(auto& Elem : EntityDef::datatypes)
	{
		if(Elem.Value)
		{
			Elem.Value->bind();
		}
	}
}

