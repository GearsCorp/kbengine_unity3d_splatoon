# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameUtils
from interfaces.EntityCommon import EntityCommon
import Data_avatar_initable
TIMER_TYPE_ADD_TRAP = 1

class Avatar(KBEngine.Entity, EntityCommon):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		EntityCommon.__init__(self)

		# 随机的初始化一个出生位置
		self.position = GameUtils.randomPosition3D(self.modelRadius)

	def isAvatar(self):
		"""
		virtual method.
		"""
		return True
	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------

	def onDestroy(self):
		"""
		KBEngine method.
		entity销毁
		"""
		DEBUG_MSG("Avatar::onDestroy: %i.gameState %i" % (self.id, self.gameStateC))

	def relive(self, exposed, type):
		"""
		defined.
		复活
		"""
		if exposed != self.id:
			return

		DEBUG_MSG("Avatar::relive: %i, type=%i." % (self.id, type))

	def changeGameState(self, gameState):
		self.gameStateC = gameState

	def weaponChanged(self, weaponId):
		self.weaponID = weaponId
		DEBUG_MSG("Avatar_weaponChanged::weaponID: %i" % (self.weaponID))

	def acquireInitData(self, teamIdPosition):
		avatar_datas   = Data_avatar_initable.datas.get(teamIdPosition,{})
		#DEBUG_MSG("acquireInitData:: %s %s " % (type(avatar_datas["position"]), avatar_datas["position"]))
		print(self.direction)
		self.position  = avatar_datas["position"]
		self.direction = avatar_datas["direction"]

	#获取当前的控件对象
	def getCurrSpace(self):
		return self.roomCellEntity;

