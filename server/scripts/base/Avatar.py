# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameConfigs
import GameConstants
import GameUtils
import random
from MATCHING_INFOS import TMatchingInfos
from MATCHING_INFOS import TMatchingInfosList

TIMER_TYPE_DESTROY = 1
TIMER_TYPE_LEAVEROOM = 2

class Avatar(KBEngine.Proxy):
	def __init__(self):
		KBEngine.Proxy.__init__(self)

		#一系列初始化
		#房间ID默认为0,gameState默认为1(未匹配状态)
		self.roomKey = 0
		self.gameState = GameConstants.GAMESTATE_NO_MATCH
		self.cellData["gameStateC"] = self.gameState
		self.cellData["dbid"] = self.databaseID
		self._destroyTimer = 0
		self.accountEntity = None
		self.exithold = False
		DEBUG_MSG("Avatar_INIT::Halls self.cellData[name]:%s , self.cellData[roleType]: %i , self.cellData[weaponID] : %i , self.cellData[dbid] : %i ." %(self.cellData["name"], self.cellData["roleType"], self.cellData["weaponID"], self.databaseID))

	def createCell(self, space, roomKey, teamId):
		"""
		defined method.
		创建cell实体
		"""
		self.gameState = GameConstants.GAMESTATE_MATCH_NO_ROOM
		self.cellData["gameStateC"] = self.gameState
		self.roomKey = roomKey
		self.cellData["roomCellEntity"] = space
		self.teamId  = teamId
		self.cellData["teamID"] = teamId
		self.createCellEntity(space)

	def onGetCell(self):
		self.gameState = GameConstants.GAMESTATE_PLAYING
		self.cell.changeGameState(GameConstants.GAMESTATE_PLAYING)

	def onTimer(self, id, userArg):
		if(TIMER_TYPE_DESTROY == userArg):
			self.onDestroyTimer()
		# if(TIMER_TYPE_LEAVEROOM == userArg):
		# 	if(self.cell is not None):
		# 		self._leaveroomTimer = 0
		# 		KBEngine.globalData["Halls"].leaveRoom(self, self.roomKey, self.teamId)

	def onDestroyTimer(self):
		DEBUG_MSG("Avatar::onDestroyTimer: %i" % (self.id))
		self.delTimer(self._destroyTimer)
		self._destroyTimer = 0
		self.destroySelf()

	def onClientEnabled(self):
		#如果此刻self._destroyTimer大于0,表示在倒计时销毁中
		self.delTimer(self._destroyTimer)
		self._destroyTimer = 0
		#如果此刻avatar获取控制权，需要根据游戏状态给客户端传输相对应的信息
		self.pushServerSaveData()

		INFO_MSG("Avatar[%i]::onClientEnabled:entities enable. entityCall:%s" % \
			(self.id, self.client))

	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		#延迟销毁,防止客户端断线重连
		self._destroyTimer = self.addTimer(3, 1, TIMER_TYPE_DESTROY)
		DEBUG_MSG("Avatar[%i]::onClientDeath" % (self.id))
		#self.destroySelf()

	def destroySelf(self):
		#当前状态为匹配完成后未分配房间、游戏中，可直接返回
		if(self.gameState >= GameConstants.GAMESTATE_MATCH_NO_ROOM):
			return

		if self.client is not None:
			return

		DEBUG_MSG("Avatar[%i]::self.gameState:[%i] ,cell state:[%i], self state:[%i]" % (self.id, self.gameState, (self.cell is not None),self.isDestroyed) )

		HallEntity = KBEngine.globalData["Halls"]

		 #未匹配状态,可直接退出
		if(GameConstants.GAMESTATE_NO_MATCH == self.gameState):
			pass
		elif(self.gameState == GameConstants.GAMESTATE_MATCHING):
			HallEntity.leaveWaitStartGame(self.id)

		HallEntity.deregisterHalls(self.id)
		#HallEntity.leaveWaitStartGame(self.id)

		if self.cell is not None:
			DEBUG_MSG("Avatar[%i]_destroySelf_cell is exit::" % (self.id))
			self.destroyCellEntity()
			return

		if not self.isDestroyed:
			if self.accountEntity is not None:
				#self.accountEntity.relogin:表示重新登录，所以不需要销毁掉self.accountEntity实体
				if(self.accountEntity.relogin == False):
					DEBUG_MSG("Avatar_accountEntity[%i]::destroy" % (self.accountEntity.id))
					self.accountEntity.activeCharacter = None
					self.accountEntity.destroySelf()
					self.accountEntity = None
				else:
					DEBUG_MSG("Avatar[%i].destroySelf: relogin =%i" % (self.id, self.accountEntity.relogin))
					self.accountEntity.activeCharacter = None
					self.accountEntity.relogin = False
					self.accountEntity = None
			self.destroy()
			DEBUG_MSG("Avatar.destroySelf[%i]::death[%i]" % (self.id, self.isDestroyed))

	def onLoseCell(self):
		"""
		KBEngine method.
		entity的cell部分实体丢失
		"""
		DEBUG_MSG("%s::onLoseCell: %i" % (self.className, self.id))
		#表明游戲結束
		self.gameOver()
		DEBUG_MSG("%s::onLoseCell_gameOver: %i" % (self.className, self.id))
		#返回大厅
		#self.returnHalls()s

	def resAvaterEnterGame(self, avatarsInfosLst):
		if(self.client):
			self.client.onResPlayersInfo(avatarsInfosLst)
		else:
			DEBUG_MSG("Avatar[%i].client(Fun::Avatar_resAvaterEnterGame) is None!" % (self.id))

	def resAvaterQuitGame(self, avatarId):
		if(self.client):
			self.client.onPlayerQuitMatch(avatarId)
		else:
			DEBUG_MSG("Avatar[%i].client(Fun::Avatar_resAvaterQuitGame) is None!" % (self.id))

	def registerHalls(self):
		DEBUG_MSG("Avatar_register::Halls self.cellData[name]:%s , self.cellData[roleType]: %i , self.cellData[weaponID] : %i ." %(self.cellData["name"], self.cellData["roleType"], self.cellData["weaponID"]))
		KBEngine.globalData["Halls"].registerHalls(self, self.cellData["name"], self.cellData["roleType"], self.cellData["weaponID"])

	def deregisterHalls(self):
		KBEngine.globalData["Halls"].deregisterHalls(self.id)

	def enterStartGame(self):
		if(not KBEngine.globalData["Halls"].ifExitHalls(self)):
			self.registerHalls()
		KBEngine.globalData["Halls"].enterStartGame(self)
		self.gameState = GameConstants.GAMESTATE_MATCHING
		self.cellData["gameStateC"] = GameConstants.GAMESTATE_MATCHING

	#统计结果
	def statisticalResult(self, teamId, value):
		DEBUG_MSG("Avatar_statisticalResult::Room_%i" % self.roomKey)
		roomEntity = KBEngine.globalData["Room_%i" % self.roomKey]
		roomEntity.statisticalResult(self.id, teamId, value)

		pass

	def leaveWaitStartGame(self):
		KBEngine.globalData["Halls"].leaveWaitStartGam(eself, self.roomKey)

	def changeGameState(self, gameState):
		self.gameState = gameState

	def getStartGameState(self):
		if(self.gameState == GameConstants.GAMESTATE_NO_MATCH or \
			self.gameState == GameConstants.GAMESTATE_MATCHING):
			return False
		else:
			return True

	def pushServerSaveData(self):
		if(self.gameState >= GameConstants.GAMESTATE_MATCH_NO_ROOM):
			self.pushEnterGameData()
			pass
		else:
			self.pushNoEneterGameData()
			pass

	def pushNoEneterGameData(self):
		#在这里调用要给客户端传输数据的函数
		DEBUG_MSG("Avatar_resNoEneterGameData")
		pass

	def pushEnterGameData(self):
		#在这里调用要给客户端传输数据的函数
		DEBUG_MSG("Avatar_resEnterGameData::")
		pass

	def onDestory(self):
		#self.deregisterHalls()
		DEBUG_MSG("Avatar[%i]_onDestory!" % Avatar.id)
		pass

	def returnHalls(self):
		DEBUG_MSG("Avatar[%i]_returnHalls!" % (self.id))
		KBEngine.globalData["Halls"].returnHalls(self.id)
		self.gameState = GameConstants.GAMESTATE_NO_MATCH

		if(self.client):
			self.client.onReturnHalls()
		else:
			if(0 == self._destroyTimer):
				self.destroy()
			DEBUG_MSG("Avatar[%i].client(Fun::Avatar_returnHalls) is None!" % (self.id))
			return
		DEBUG_MSG("Avatar[%i].client_onReturnHalls!" % (self.id))

	def gameOver(self):
		DEBUG_MSG("Avatar[%i]_gameOver::"% (self.id))
		self.gameState = GameConstants.GAMESTATE_END
		if(self.client):
			self.client.onEnding()
		else:
			DEBUG_MSG("Avatar[%i].client(Fun::Avatar_returnHalls) is None!" % (self.id))
			KBEngine.globalData["Halls"].leaveWaitStartGame(self.id)
		DEBUG_MSG("Avatar[%i].client_onEnding!"% (self.id))

	def endOfStatistics(self):
		DEBUG_MSG("Avatar[%i]_endOfStatistics!" % (self.id))
		self.returnHalls()

	def weaponChanged(self, weaponId):
		DEBUG_MSG("Avatar_weaponChanged::weaponId(%i)" % (weaponId))

		if(self.cell is None):
			self.cellData["weaponID"] = weaponId
		else:
			self.cell.weaponChanged(weaponId)
		if(self.accountEntity):
			self.accountEntity.accountCharacter(weaponId)
		KBEngine.globalData["Halls"].weaponIdChanged(self.id, weaponId)


