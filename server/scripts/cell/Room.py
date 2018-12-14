# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameConfigs
import random
import GameUtils

TIMER_TYPE_DESTROY = 1
TIMER_TYPE_READY = 2
TIMER_TYPE_START = 3
TIMER_TYPE_MINTIME = 4

TIMER_TYPE_ROOM_TICK = 5 #房间帧同步的计算


class Room(KBEngine.Entity):
	"""
	游戏场景
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)

		# 把自己移动到一个不可能触碰陷阱的地方
		self.position = (999999.0, 0.0, 0.0)

		# 这个房间中所有的玩家
		self.redAvatars = []
		self.blueAvatars = []

		# 设置房间必要的数据，客户端可以获取之后做一些显示和限制
		KBEngine.setSpaceData(self.spaceID, "GAME_MAP_SIZE",  str(GameConfigs.GAME_MAP_SIZE))
		KBEngine.setSpaceData(self.spaceID, "ROOM_MAX_PLAYER",  str(GameConfigs.ROOM_MAX_PLAYER))
		KBEngine.setSpaceData(self.spaceID, "GAME_ROUND_TIME",  str(GameConfigs.GAME_ROUND_TIME))

		self._startBattleTime = 0
		self._destroyTimer = 0

	def onTimer(self, id, userArg):
		if(TIMER_TYPE_DESTROY == userArg):
			#调用帧同步组件，结束幀同步
			self.componentFrameSync.stop()
			#释放掉创建的房间资源
			self.onDestroyTimer()

		if(TIMER_TYPE_MINTIME == userArg):
			KBEngine.globalData["Halls"].onRoomMinTimeFlagChanged(self.roomKeyC, True)

		if(TIMER_TYPE_READY == userArg):

			# 开始记录一局游戏时间， 时间结束后将玩家踢出空间同时销毁自己和空间
			self._destroyTimer = self.addTimer(GameConfigs.GAME_ROUND_TIME, 0, TIMER_TYPE_DESTROY)
			#同开始在记录玩家加入游戏的最小时间值，到达该最小时间阈值不再加入
			self.addTimer(GameConfigs.GAME_ROUND_TIME - GameConfigs.GAME_MIN_REMAIN_TIME, 0, TIMER_TYPE_MINTIME)
			self._startBattleTime = 0

			#调用帧同步组件，开启幀同步
			self.componentFrameSync.start()

			DEBUG_MSG("room_avatar_onReadyForBattle")


	def onDestroyTimer(self):
		# for avatar in self.redAvatars:
		# 	KBEngine.globalData["Halls"].deregisterHalls(avatar)
		# 	KBEngine.globalData["Halls"].leaveWaitStartGame(avatar)


		# for avatar in self.blueAvatars:
		# 	KBEngine.globalData["Halls"].deregisterHalls(avatar)
		# 	KBEngine.globalData["Halls"].leaveWaitStartGame(avatar)
		# KBEngine.globalData["Halls"].removeRoom(self.roomKeyC)
		self.redAvatars = []
		self.blueAvatars = []
		self.destroySpace()

	def onDestory(self):
		DEBUG_MSG("Room::onDestroy: %i" % (self.id))
		#del KBEngine.globalData["Room_%i" % self.roomKeyC]

	def enterRoom(self, entityCall, teamId):
		teamLen = 0
		if(teamId == GameConfigs.RED_TEAM_ID):
			self.redAvatars.append(entityCall)
			teamLen = len(self.redAvatars)
		else:
			self.blueAvatars.append(entityCall)
			teamLen = len(self.blueAvatars)
		strPositionId = str(teamId) + "_" + str(teamLen)
		DEBUG_MSG("Room_enterRoom_strPositionId:: %s %i" % (strPositionId, entityCall.id))
		#print(entityCall)
		KBEngine.entities.get(entityCall.id).acquireInitData(strPositionId)

		#判斷戰斗模式
		self.ifEnterCombatMode(entityCall)



	def startBattle(self, entityCall):
		DEBUG_MSG("Room_startBattle::entityCall[%i]" %(entityCall.id))

		if(self._startBattleTime == 0 and self._destroyTimer == 0):
			self._startBattleTime = self.addTimer(GameConfigs.GAME_READY_TIME, 0, TIMER_TYPE_READY)
			DEBUG_MSG("Room_startBattle::_startBattleTime(%i)" %(self._startBattleTime))
			for avatar in self.redAvatars:
				if(avatar.client):
					avatar.client.onReadyForBattle()
				else:
					DEBUG_MSG("Avatar[%i].client(Fun::room_cell_onTimer_TIMER_TYPE_READY) is None!" % (avatar.id))
			for avatar in self.blueAvatars:
				if(avatar.client):
					avatar.client.onReadyForBattle()
				else:
					DEBUG_MSG("Avatar[%i].client(Fun::room_cell_onTimer_TIMER_TYPE_READY) is None!" % (avatar.id))

		elif((self._startBattleTime == 0 and self._destroyTimer > 0) or (self._startBattleTime != 0) ):
			if(entityCall.client):
				entityCall.client.onReadyForBattle()
			else:
				DEBUG_MSG("Avatar[%i].client(Fun::room_cell_startBattle) is None!" % (entityCall.id))
				return
			DEBUG_MSG("Room_startBattle::_destroyTimer(%i)" %(self._destroyTimer))


	def ifEnterCombatMode(self, entityCall):
		DEBUG_MSG("Room_ifEnterCombatMode::self.redAvatare_len:%i,self.blueAvatars_len:%i" %(len(self.redAvatars),len(self.blueAvatars)))
		# if(len(self.redAvatars) < 1):
		# 	return
		# if(len(self.blueAvatars) < 1):
		# 	return
		self.startBattle(entityCall)


