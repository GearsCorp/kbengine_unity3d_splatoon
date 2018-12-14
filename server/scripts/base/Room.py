# -*- coding: utf-8 -*-
import KBEngine
import random
import copy
import math
from KBEDebug import *
import GameConfigs
import GameConstants

TIMER_TYPE_STATISTICAL = 1
class Room(KBEngine.Entity):
	"""
	一个可操控cellapp上真正space的实体
	注意：它是一个实体，并不是真正的space，真正的space存在于cellapp的内存中，通过这个实体与之关联并操控space。
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)

		self.cellData["roomKeyC"] = self.roomKey

		# 请求在cellapp上创建cell空间
		self.createCellEntityInNewSpace(None)

		# 让baseapp和cellapp都能够方便的访问到这个房间的entityCall
		KBEngine.globalData["Room_%i" % self.roomKey] = self
		#编排位置
		self.redPosition = 0
		self.bluePosition = 0
		self.bRedTeam = True

		self.avatars = {}

		#{TeamId:[values]}
		self.results  = {}

		self.statisticalTimer = 0

		self.statisticalFlag  = False


	def onGetCell(self):
		KBEngine.globalData["Halls"].onRoomGetCell(self, self.roomKey)

	def onLoseCell(self):
		KBEngine.globalData["Halls"].onRoomStateChanged(self.roomKey, GameConstants.GAMESTATE_END)
		pass


	def enterRoom(self, entityCall, teamId):

		entityCall.createCell(self.cell, self.roomKey, teamId)
		self.cell.enterRoom(entityCall, teamId)
		self.avatars[entityCall.id] = entityCall
		print("enterRoom_enterRoom")
		pass

	def leaveRoom(self, entityId, teamId):
		pass

	def onDestory(self):
		DEBUG_MSG("Room_%i is Destroy!" % self.roomKey)
		pass

	def onTimer(self, id, userArg):
		if(id == TIMER_TYPE_STATISTICAL):
			statisticalFlag = True
			self.delTimer(self.statisticalTimer)
			self.statisticalTimer = 0
			self.anayizeResults()
			#统计结束后，可以销毁掉room实体
			KBEngine.globalData["Halls"].removeRoom(self.roomKey)
			self.avatars = {}
			self.destroy()

	#统计结果
	def statisticalResult(self, entityId, teamId, value):
		if(not self.statisticalFlag):
			if(self.statisticalTimer > 0):
				self.delTimer(self.statisticalTimer)
			self.statisticalTimer = self.addTimer(3, 1, TIMER_TYPE_STATISTICAL)
		else:
			DEBUG_MSG("Room_statisticalResult:: statisticalFlag(%i)" % (self.statisticalFlag))
			return

		KBEngine.globalData["Halls"].onRoomStateChanged(self.roomKey, GameConstants.GAMESTATE_STATISTICAL)

		if(self.avatars.__contains__(entityId)):
			if(not self.results.__contains__(teamId)):
				self.results[teamId] = []
			self.results[teamId].append(value)
		DEBUG_MSG("Room_statisticalResult::results(%i)" % len(self.results))

	def anayizeResults(self):
		if(len(self.results) == 0):
			DEBUG_MSG("Room_anayizeResults:: results is null!")

		DEBUG_MSG("Room_anayizeResults_Begine")
		redTeam  = 0
		blueTeam = 0
		values = []
		totalValue = 0
		teamId = 0

		if(0 == len(self.results)):
			self.transmitResults(GameConfigs.RED_TEAM_ID, 50)
			return


		if(self.results.__contains__(GameConfigs.RED_TEAM_ID)):
			redTeam = len(self.results[GameConfigs.RED_TEAM_ID])
			pass
		if(self.results.__contains__(GameConfigs.BLUE_TEAM_ID)):
			blueTeam = len(self.results[GameConfigs.BLUE_TEAM_ID])
			pass

		if(redTeam >= blueTeam):
			values = self.results[GameConfigs.RED_TEAM_ID]
			teamId = GameConfigs.RED_TEAM_ID
		else:
			values = self.results[GameConfigs.BLUE_TEAM_ID]
			teamId = GameConfigs.BLUE_TEAM_ID

		for tmpValue in values:
			totalValue += tmpValue
		endValue = totalValue / len(self.results[teamId])
		endValue = round(endValue, 1)
		self.transmitResults(teamId, endValue)


	def transmitResults(self, teamId, endValue):
			#向客戶端传输结算数据
		for avatar in self.avatars.values():
			if(avatar.client):
				avatar.client.onStatisticalResult(teamId, endValue)
			else:
				DEBUG_MSG("Avatar[%i].client(Fun::room_anayizeResults) is None!" % (avatar.id))
		DEBUG_MSG("Room_anayizeResults_End::teamId[%i]_endValue[%f]"%(teamId, endValue))

