# -*- coding: utf-8 -*-
import KBEngine
import Functor
from KBEDebug import *
import traceback
import GameConfigs
import GameConstants

from MATCHING_INFOS import TMatchingInfos
from MATCHING_INFOS import TMatchingInfosList

FIND_ROOM_NOT_FOUND = 0
FIND_ROOM_CREATING = 1

class Halls(KBEngine.Entity):
	"""
	这是一个脚本层封装的房间管理器
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)

		# 向全局共享数据中注册这个管理器的entityCall以便在所有逻辑进程中可以方便的访问
		KBEngine.globalData["Halls"] = self

		# 所有房间，是个字典结构，包含 {"roomEntityCall", "playerCount", "enterRoomReqs",."roomKey", "roomState"}
		# enterRoomReqs, 在房间未创建完成前， 请求进入房间和登陆到房间的请求记录在此，等房间建立完毕将他们扔到space中
		self.rooms = {}

		#進入大廳中所有的avatars
		self.avatars = {}

		#玩家屬性
		self.position = 0;

		#確定開始游戲的avatars
		self.enterGameAvatars = {}
		self.awaitRoomAvatars = {}
		self.bRedTeam = True
		self.lastNewRoomKey = 0


	# def enterRoom(self, entityCall, position, direction, roomKey):
	# 	roomData = self.findRoom(roomKey)

	# 	roomData["playerCount"] += 1

	# 	roomEntityCall = roomData["roomEntityCall"]

	# 	if((roomEntityCall is None) or (roomEntityCall.cell is None)):
	# 		roomData["enterRoomReqs"].append(entityCall)
	# 	else:
	# 		roomEntityCall.enterRoom(entityCall, position, direction)


	#注冊进入大廳
	def registerHalls(self, entityCall, name, roleType, weaponId):
		if(not self.avatars.__contains__(entityCall.id)):
			self.avatars[entityCall.id] = {}
		avatarInfos = self.avatars[entityCall.id]
		avatarInfos["entityCall"] = entityCall
		avatarInfos["name"] = name
		avatarInfos["roleType"] = roleType
		avatarInfos["weaponId"] = weaponId
		DEBUG_MSG("registerHalls_success_avatar[%i]" % entityCall.id)

	#反注册退出大廳
	def deregisterHalls(self, entityId):
		DEBUG_MSG("Halls_deregisterHalls_entityId(%i)" % (entityId))
		if(self.avatars.__contains__(entityId)):
			del self.avatars[entityId]
		else:
			DEBUG_MSG("Halls_deregisterHalls_entityId(%i)  is no exit(avatars)" % (entityId))

	def ifExitHalls(self, entityCall):
		if(self.avatars.__contains__(entityCall.id)):
			return True
		else:
			return False

	def enterStartGame(self, entityCall):
		if self.ifExitHalls(entityCall):

			#处理等到进入游戏的人员数量
			self.dealRoomAvatars(entityCall.id)

			avatarInfosHalls = self.avatars[entityCall.id]

			if(not self.enterGameAvatars.__contains__(entityCall.id)):
				self.enterGameAvatars[entityCall.id] = {}

			avatarInfos = self.enterGameAvatars[entityCall.id]
			avatarInfos["entityCall"] = entityCall
			avatarInfos["name"] = avatarInfosHalls["name"]
			avatarInfos["roleType"] = avatarInfosHalls["roleType"]
			avatarInfos["weaponId"] = avatarInfosHalls["weaponId"]

			avatarInfos["teamId"] = self.acquireTeamId(entityCall.id)
			# if(self.bRedTeam):
			# 	avatarInfos["teamId"] = GameConfigs.RED_TEAM_ID
			# else:
			# 	avatarInfos["teamId"] = GameConfigs.BLUE_TEAM_ID
			# self.bRedTeam = not self.bRedTeam
			avatarInfos["roomPosition"] = len(self.enterGameAvatars)
			DEBUG_MSG("Halls_enterStartGame::enterGameAvatars[%s]" % str(self.enterGameAvatars))
			self.newAvatarEntetGame(entityCall)


			#当人数到达最小人数伐值时，即可进入room
			if(len(self.awaitRoomAvatars["entityId"]) >= GameConfigs.ROOM_MIN_PLAYER):
				DEBUG_MSG("halls_enterRoom_len(self.awaitRoomAvatars)[%i],[%s]" % (len(self.awaitRoomAvatars["entityId"]),str(self.awaitRoomAvatars)))

				if(len(self.awaitRoomAvatars["entityId"]) == GameConfigs.ROOM_MIN_PLAYER):

					for avatarId in self.awaitRoomAvatars["entityId"]:
						self.enterRoom(self.enterGameAvatars[avatarId]["entityCall"], 0, self.enterGameAvatars[avatarId]["teamId"])
				else:
					self.enterRoom(entityCall, 0, avatarInfos["teamId"])
			# if(len(self.awaitRoomAvatars) == GameConfigs.ROOM_MAX_PLAYER):
			# 	self.awaitRoomAvatars = []

		else:
			DEBUG_MSG("Halls_enterStartGame: avatar[%i] is no regist halls!" % (entityCall.id))

	def leaveWaitStartGame(self, entityId):
		DEBUG_MSG("Halls_leaveWaitStartGame_entityId(%i) ,enterGameAvatars::%s" % (entityId, str(self.enterGameAvatars)))

		if(self.enterGameAvatars.__contains__(entityId)):
			del self.enterGameAvatars[entityId]
		else:
			DEBUG_MSG("Halls_leaveWaitStartGame_entityId(%i)  is no exit(enterGameAvatars)" % (entityId))

		if(self.awaitRoomAvatars.__contains__("entityId")):
			entityIdLst = self.awaitRoomAvatars["entityId"]
			if(entityIdLst.__contains__(entityId)):
				entityIdLst.remove(entityId)
				#告知别的玩家,变化为entityId的玩家退出比配
				DEBUG_MSG("Halls_enterGame::%s")
				for avatarId in entityIdLst:
					self.enterGameAvatars[avatarId]["entityCall"].resAvaterQuitGame(entityId)
					DEBUG_MSG("Halls_leaveWaitStartGame_resAvaterQuitGame_avatarId(%i)!" % (avatarId))

				if(self.awaitRoomAvatars["redTeamId"].__contains__(entityId)):
					self.awaitRoomAvatars["redTeamId"].remove(entityId)
				if(self.awaitRoomAvatars["blueTeamId"].__contains__(entityId)):
					self.awaitRoomAvatars["blueTeamId"].remove(entityId)

		else:
			DEBUG_MSG("Halls_awaitRoomAvatars_entityId(%i)  is no exit(awaitRoomAvatars)" % (entityId))


	def dealRoomAvatars(self, entityId):
		#处理等到进入游戏的人员数量
		if(self.rooms.__contains__(self.lastNewRoomKey)):
			if((self.rooms[self.lastNewRoomKey].__contains__("playerCount") and\
				self.rooms[self.lastNewRoomKey] == GameConfigs.ROOM_MAX_PLAYER)):
				self.awaitRoomAvatars = {}

			if((self.rooms[self.lastNewRoomKey].__contains__("arriveMinTimeFlag") and\
				self.rooms[self.lastNewRoomKey]["arriveMinTimeFlag"] == True)):
				self.awaitRoomAvatars = {}

		if(not self.awaitRoomAvatars.__contains__("entityId")):
			self.awaitRoomAvatars["entityId"] = []
		if(not self.awaitRoomAvatars.__contains__("redTeamId")):
			self.awaitRoomAvatars["redTeamId"] = []
		if(not self.awaitRoomAvatars.__contains__("blueTeamId")):
			self.awaitRoomAvatars["blueTeamId"] = []

		self.awaitRoomAvatars["entityId"].append(entityId)

	def acquireTeamId(self, entityId):
		if(len(self.awaitRoomAvatars["redTeamId"]) <= len(self.awaitRoomAvatars["blueTeamId"])):
			self.awaitRoomAvatars["redTeamId"].append(entityId)
			return GameConfigs.RED_TEAM_ID
		else:
			self.awaitRoomAvatars["blueTeamId"].append(entityId)
			return  GameConfigs.BLUE_TEAM_ID

	def newAvatarEntetGame(self, entityCall):
		DEBUG_MSG("Halls_newAvatarEntetGame: avatar[%i] enterGame!" % (entityCall.id))

		matchinginfoLst = TMatchingInfosList()
		for avatarId in self.awaitRoomAvatars["entityId"]:
			avatarInfos = self.enterGameAvatars[avatarId]
			matchinginfo = self.packAvatarInfos(avatarInfos)
			matchinginfoLst[avatarInfos["entityCall"].id] = matchinginfo
			DEBUG_MSG("Halls_newAvatarEntetGame: self.enterGameAvatars.len{%i}, weaponId: %i" % (len(self.enterGameAvatars), matchinginfo.asDict()["weaponId"]) )
			if(avatarInfos["entityCall"].id != entityCall.id):
				otherAvatarInfoLst = TMatchingInfosList()
				matchinginfo = self.packAvatarInfos(self.enterGameAvatars[entityCall.id])
				otherAvatarInfoLst[avatarInfos["entityCall"].id] = matchinginfo
				avatarInfos["entityCall"].resAvaterEnterGame(otherAvatarInfoLst)
				DEBUG_MSG("Halls_newAvatarEntetGame: other.resAvaterEnterGame.len{%i}" % (len(otherAvatarInfoLst)) )

		entityCall.resAvaterEnterGame(matchinginfoLst);
		DEBUG_MSG("Halls_newAvatarEntetGame: own.resAvaterEnterGame.len{%i}" % (len(matchinginfoLst)) )

	def packAvatarInfos(self, avatarInfos):
		matchinginfo = TMatchingInfos()
		matchinginfo.extend([avatarInfos["entityCall"].id, avatarInfos["name"], avatarInfos["roleType"], avatarInfos["weaponId"],\
			0, avatarInfos["roomPosition"], avatarInfos["teamId"]])
		return matchinginfo

	def enterRoom(self, entityCall, roomKey, teamId):
		roomData = self.findRoom(roomKey)
		if(not roomData.__contains__("roomState")):
			roomData["roomState"] = GameConstants.GAMESTATE_MATCH_NO_ROOM
		if(not roomData.__contains__("arriveMinTimeFlag")):
			roomData["arriveMinTimeFlag"] = False
		roomData["playerCount"] += 1

		roomEntityCall = roomData["roomEntityCall"]

		if((roomEntityCall is None) or (roomEntityCall.cell is None)):
			roomData["enterRoomReqs"].append(entityCall)
			DEBUG_MSG("Halls_enterRoom_(roomEntityCall is None) or (roomEntityCall.cell is None)")
		else:
			roomEntityCall.enterRoom(entityCall, teamId)

	def leaveRoom(self, avatarID, roomKey, teamId):
		return
		if roomKey in self.rooms.keys():
			roomData = self.rooms.get(roomKey)
			if type(roomData) is dict:
				roomEntityCall = roomData["roomEntityCall"]
				if roomEntityCall:
					roomEntityCall.leaveRoom(avatarID, teamId)

	def findRoom(self, roomKey):
		roomData = self.rooms.get(roomKey)

		if roomData is None:
			roomData = self.rooms.get(self.lastNewRoomKey)
			if((roomData is not None) and \
				(roomData["playerCount"] < GameConfigs.ROOM_MAX_PLAYER) and\
				(roomData["roomState"] < GameConstants.GAMESTATE_END) and\
				(not roomData["arriveMinTimeFlag"])):
				return roomData

			self.lastNewRoomKey = KBEngine.genUUID64()

			params = {
				"roomKey" : self.lastNewRoomKey, # base, BASE_AND_CLIENT
			}
			KBEngine.createEntityAnywhere("Room", params, Functor.Functor(self.onRoomCreatedCB, self.lastNewRoomKey))

			roomData = {"roomEntityCall":None, "playerCount":0, "enterRoomReqs":[], "roomKey": self.lastNewRoomKey}
			self.rooms[self.lastNewRoomKey] = roomData


			return roomData
		else:
			return roomData

	def removeRoom(self, roomKey):
		DEBUG_MSG("Halls::onRoomLoseCell:_Begin_Space%i." %(roomKey))
		del self.rooms[roomKey]
		DEBUG_MSG("Halls::onRoomLoseCell:_End_Space %i." %(roomKey))
		if(self.lastNewRoomKey == roomKey):
			if(len(self.rooms) == 0):
				lastNewRoomKey = 0
			else:
				lastNewRoomKey = self.rooms.keys()[0]
		#self.bRedTeam = True

	def onRoomCreatedCB(self, keyRoom, entityCall):
		"""
		一个space创建好后的回调
		"""
		DEBUG_MSG("Room[%i] is created!"%(keyRoom))

	def onRoomGetCell(self, roomEntityCall, roomKey):

		if self.rooms is None:
			DEBUG_MSG("Halls[%i]_onRoomGetCell no rooms data!"%(self.id));
			return
		self.rooms[roomKey]["roomEntityCall"] = roomEntityCall

		roomData =  self.rooms.get(roomKey)
		if roomData is None:
			DEBUG_MSG("Halls[%i]_onRoomGetCell no roomData[%i] data!"%(self.id, roomKey))
			return

		DEBUG_MSG("Halls_onRoomGerCell::roomData[%s]" % str(roomData))
		for entity in roomData["enterRoomReqs"]:
			DEBUG_MSG("Halls_onRoomGerCell::self.enterGameAvatars.count(%i)" %(len(self.enterGameAvatars)))
			DEBUG_MSG("Halls_onRoomGerCell::entity[%s]" % type(entity))
			avatarInfos = self.enterGameAvatars[entity.id]
			DEBUG_MSG("Halls_onRoomGerCell::roomData[roomEntityCall][%i]"%(roomEntityCall.id))
			roomEntityCall.enterRoom(entity, avatarInfos["teamId"])

		self.rooms[roomKey]["enterRoomReqs"] = []
		self.rooms[roomKey]["roomState"] = GameConstants.GAMESTATE_PLAYING

	def onRoomLoseCell(self, roomKey):
		self.removeRoom(roomKey)

	def onRoomStateChanged(self, roomKey, state):
		if(self.rooms.__contains__(roomKey)):
			self.rooms[roomKey]["roomState"] = state
			DEBUG_MSG("Halls_onRoomStateChanged::[%i]!" % state)
		else:
			DEBUG_MSG("Halls_onRoomStateChanged:: no find keyRoom[%i]" % roomKey)

	def onRoomMinTimeFlagChanged(self, roomKey, bArriveMinTime):
		if(self.rooms.__contains__(roomKey)):
			self.rooms[roomKey]["arriveMinTimeFlag"] = bArriveMinTime
			DEBUG_MSG("Halls_onRoomMinTimeFlagChanged::[%i]!" % bArriveMinTime)
		else:
			DEBUG_MSG("Halls_onRoomMinTimeFlagChanged:: no find keyRoom[%i]" % roomKey)

	def returnHalls(self, entityId):
		self.leaveWaitStartGame(entityId)

	def weaponIdChanged(self, entityId, weaponId):
		if(self.enterGameAvatars.__contains__(entityId)):
			infos = self.enterGameAvatars[entityId]
			infos["weaponId"] = weaponId

			matchinginfoLst = TMatchingInfosList()
			matchinginfo = self.packAvatarInfos(infos)
			matchinginfoLst[entityId] = matchinginfo

			for avatar in self.awaitRoomAvatars["entityId"]:
				infos["entityCall"].resAvaterEnterGame(matchinginfoLst);
				DEBUG_MSG("Halls_weaponIdChanged::resAvaterEnterGame_avatar(%i)" %(avatar))










