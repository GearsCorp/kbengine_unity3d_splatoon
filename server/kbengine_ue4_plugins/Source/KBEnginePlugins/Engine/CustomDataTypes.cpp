#include "CustomDataTypes.h"
#include "EntityDef.h"
#include "KBDebug.h"
#include "DataTypes.h"
#include "Runtime/Core/Public/Misc/Variant.h"

void DATATYPE_AVATAR_INFOS::createFromStreamEx(MemoryStream& stream, AVATAR_INFOS& datas)
{
	datas.dbid = stream.readUint64();
	datas.name = stream.readUnicode();
	datas.roleType = stream.readUint8();
	datas.weaponId = stream.readInt32();
}

void DATATYPE_AVATAR_INFOS::addToStreamEx(Bundle& stream, const AVATAR_INFOS& v)
{
	stream.writeUint64(v.dbid);
	stream.writeUnicode(v.name);
	stream.writeUint8(v.roleType);
	stream.writeInt32(v.weaponId);
}

void DATATYPE_AVATAR_INFOS_LIST::createFromStreamEx(MemoryStream& stream, AVATAR_INFOS_LIST& datas)
{
	values_DataType.createFromStreamEx(stream, datas.values);
}

void DATATYPE_AVATAR_INFOS_LIST::addToStreamEx(Bundle& stream, const AVATAR_INFOS_LIST& v)
{
	values_DataType.addToStreamEx(stream, v.values);
}

void DATATYPE_MATCHING_INFOS::createFromStreamEx(MemoryStream& stream, MATCHING_INFOS& datas)
{
	datas.id = stream.readInt32();
	datas.name = stream.readUnicode();
	datas.roleType = stream.readUint8();
	datas.weaponId = stream.readInt32();
	datas.roomId = stream.readUint8();
	datas.roomPosition = stream.readUint8();
	datas.teamId = stream.readUint8();
}

void DATATYPE_MATCHING_INFOS::addToStreamEx(Bundle& stream, const MATCHING_INFOS& v)
{
	stream.writeInt32(v.id);
	stream.writeUnicode(v.name);
	stream.writeUint8(v.roleType);
	stream.writeInt32(v.weaponId);
	stream.writeUint8(v.roomId);
	stream.writeUint8(v.roomPosition);
	stream.writeUint8(v.teamId);
}

void DATATYPE_MATCHING_INFOS_LIST::createFromStreamEx(MemoryStream& stream, MATCHING_INFOS_LIST& datas)
{
	values_DataType.createFromStreamEx(stream, datas.values);
}

void DATATYPE_MATCHING_INFOS_LIST::addToStreamEx(Bundle& stream, const MATCHING_INFOS_LIST& v)
{
	values_DataType.addToStreamEx(stream, v.values);
}

void DATATYPE_FS_ENTITY_DATA::createFromStreamEx(MemoryStream& stream, FS_ENTITY_DATA& datas)
{
	datas.entityid = stream.readInt32();
	datas.cmd_type = stream.readUint8();
	datas.datas = stream.readBlob();
}

void DATATYPE_FS_ENTITY_DATA::addToStreamEx(Bundle& stream, const FS_ENTITY_DATA& v)
{
	stream.writeInt32(v.entityid);
	stream.writeUint8(v.cmd_type);
	stream.writeBlob(v.datas);
}

void DATATYPE_FS_FRAME_DATA::createFromStreamEx(MemoryStream& stream, FS_FRAME_DATA& datas)
{
	datas.frameid = stream.readUint32();
	operation_DataType.createFromStreamEx(stream, datas.operation);
}

void DATATYPE_FS_FRAME_DATA::addToStreamEx(Bundle& stream, const FS_FRAME_DATA& v)
{
	stream.writeUint32(v.frameid);
	operation_DataType.addToStreamEx(stream, v.operation);
}

void DATATYPE_FS_FRAME_LIST::createFromStreamEx(MemoryStream& stream, FS_FRAME_LIST& datas)
{
	values_DataType.createFromStreamEx(stream, datas.values);
}

void DATATYPE_FS_FRAME_LIST::addToStreamEx(Bundle& stream, const FS_FRAME_LIST& v)
{
	values_DataType.addToStreamEx(stream, v.values);
}

