import KBEngine
from AVATAR_INFOS import TAvatarInfos
from AVATAR_INFOS import TAvatarInfosList
from KBEDebug import *
import GameConfigs

TIMER_TYPE_DESTROY = 1
class Account(KBEngine.Proxy):
	def __init__(self):
		KBEngine.Proxy.__init__(self)
		self.activeCharacter = None
		self.relogin = False
		self._destroyTimer = 0

	def reqAvatarList(self):
		DEBUG_MSG("Account[%i].reqAvatarRoteList: size=%i." % (self.id, len(self.characters)))
		if(self.client):
			self.client.onReqAvatarList(self.characters)
		else:
			DEBUG_MSG("Account[%i].client(Fun::Account_reqAvatarList) is None!" % (self.id))

	def reqCreateAvatar(self, roleType, name):
		props = {
			"name"				: name,
			"roleType"			: roleType,
			"weaponID"			: GameConfigs.WEAPON_A_ID,
			"direction"			: (0, 0, 1),
			"position"			: (0, 0, 0),
		}

		INFO_MSG('Account::_onAvatarSaved:(%i) create avatar state: %s, %i' % (self.id, name, roleType))
		avatar = KBEngine.createEntityLocally('Avatar', props);
		if avatar:
			avatar.writeToDB(self._onAvatarSaved)
		else:
			DEBUG_MSG("avatar is null")

	def _onAvatarSaved(self, success, avatar):
		"""
		新建角色写入数据库回调
		"""
		INFO_MSG('Account::_onAvatarSaved:(%i) create avatar state: %i, %s, %i' % (self.id, success, avatar.cellData["name"], avatar.databaseID))

		# 如果此时账号已经销毁， 角色已经无法被记录则我们清除这个角色
		if self.isDestroyed:
			if avatar:
				avatar.destroy(True)
			return

		avatarinfo = TAvatarInfos()
		avatarinfo.extend([0, "", 0, 0])
		INFO_MSG('_onAvatarSaved::success_(%i)' % success)
		if success:
			info = TAvatarInfos()
			info.extend([avatar.databaseID, avatar.cellData["name"], avatar.cellData["roleType"], avatar.cellData["weaponID"]])
			self.characters[avatar.databaseID] = info
			avatarinfo[0] = avatar.databaseID
			avatarinfo[1] = avatar.cellData["name"]
			avatarinfo[2] = avatar.cellData["roleType"]
			avatarinfo[3] = avatar.cellData["weaponID"]
			INFO_MSG('create avatar state:%i, %s, %i' % (success, avatar.cellData["name"], avatar.cellData["weaponID"]))
			self.writeToDB()
		else:
			avatarinfo[1] = "创建失败了"

		avatar.destroy()

		if self.client:
			INFO_MSG('Account::onCreateAvatarResult: create avatar state: %i' % (self.id))
			self.client.onCreateAvatarResult(avatarinfo)
		else:
			DEBUG_MSG("Account[%i].client(Fun::Account_onCreateAvatarResult) is None!" % (self.id))


	def onClientEnabled(self):

		#获取控制器,将销毁控制器去除
		self._destroyTimer = 0
		INFO_MSG("Account[%i]::onClientEnabled:entities enable. entityCall:%s, clientType(%i), clientDatas=(%s), accountName=%s" % \
			(self.id, self.client, self.getClientType(), self.getClientDatas(), self.__ACCOUNT_NAME__))
		# 如果一个在线的账号被一个客户端登陆并且onLogOnAttempt返回允许
		# 那么会踢掉之前的客户端连接
		# 那么此时self.activeAvatar可能不为None， 常规的流程是销毁这个角色等新客户端上来重新选择角色进入
		if self.activeCharacter is not None and (not self.relogin):
			self.giveClientTo(self.activeCharacter)

	def onClientDeath(self):
		DEBUG_MSG("Account[%i]::onClientDeath_onClientDeath" % (self.id))
		self.destroySelf()

	def onLogOnAttempt(self, ip, port, password):
		"""
		KBEngine method.
		客户端登陆失败时会回调到这里,一般为异地登录处理
		"""
		INFO_MSG("Account[%i]::onLogOnAttempt: ip=%s, port=%i, selfclient=%s" % (self.id, ip, port, self.client))

		# if((self.client is None) and )

		# if self.activeCharacter is not None:
		# 	if self.activeCharacter.client is not None:
		# 		self.activeCharacter.giveClientTo(self)
			# #分為開始游戲和未開始游戲
			# #未開始游戲:則銷毀之前的avatar
			# #開始游戲:則保留之前的avatar,不進行處理
			# if(not self.activeCharacter.getStartGameState):
			# 	self.relogin = True
			# 	self.activeCharacter.destroySelf()
			# 	self.activeCharacter = None
		if(self.client is not None):
			DEBUG_MSG("Account[%i]_client is exit!!" % (self.id))
		if(self.activeCharacter is not None and self.activeCharacter.client is not None):
			DEBUG_MSG("Account[%i]_activeCharacter_client is exit!!" % (self.id))

		#如果有客户端连接在,表示异地登录
		if(self.client is not None or \
			(self.activeCharacter is not None and \
			 self.activeCharacter.client is not None)):
			INFO_MSG("Account[%i]::onLogOnAttempt——dealNonLocalLogin: " % (self.id))
			self.dealNonLocalLogin()
		#表示客户端断线重连
		else:
			INFO_MSG("Account[%i]::onLogOnAttempt——dealDisconnect: " % (self.id))
			self.dealDisconnect()
			pass


		# if(self.activeCharacter is not None and \
		# 	self.activeCharacter)
		INFO_MSG("Account[%i]::onLogOnAttempt——KBEngine.LOG_ON_ACCEPT successed!")
		return KBEngine.LOG_ON_ACCEPT

	def enterGameRoom(self, dbid):
		if self.activeCharacter is None:
			if dbid in self.characters:
				KBEngine.createEntityFromDBID("Avatar", dbid, self.__onAvatarCreated)
			else:
				ERROR_MSG("Account[%i]::enterGameRoom: not found dbid(%i)" % (self.id, dbid))
		else:
			ERROR_MSG("Account[%i]::enterGameRoom: activeCharacter[%i] is exit" % (self.id, self.activeCharacter.id))

	def __onAvatarCreated(self, baseRef, databaseID, wasActive):
		if wasActive:
			ERROR_MSG("Account::__onAvatarCreated:(%i): this character is in world now!" % (self.id))
			return
		if baseRef is None:
			ERROR_MSG("Account::__onAvatarCreated:(%i): this character create fail!" % (self.id))
			return

		avatar = KBEngine.entities.get(baseRef.id)
		if avatar is None:
			ERROR_MSG("Account::__onAvatarCreated:(%i): this character no exit!" % (self.id))
			return
		if self.isDestroyed:
			ERROR_MSG("Account::__onAvatarCreated:(%i): i dead, will the destroy of Avatar!" % (self.id))
			avatar.destroy()
			return
		info = self.characters[databaseID]
		#__CLY__Start__
			#预留:这里是对avatar的一些基本属性的配置

		#__CLY__End__
		self.activeCharacter = avatar
		avatar.accountEntity = self

		#将avatar注册到大厅
		self.activeCharacter.registerHalls()
		self.giveClientTo(self.activeCharacter)

	def destroySelf(self):
		if self.activeCharacter:
			DEBUG_MSG("self.activeCharacter[%i]" % (self.activeCharacter.id))

		self._destroyTimer = self.addTimer(3, 1, TIMER_TYPE_DESTROY)
		DEBUG_MSG("Account.destroySelf[%i]::death[%i]" % (self.id, self.isDestroyed))

	def onDestroyTimer(self):
		self.delTimer(self._destroyTimer)
		self._destroyTimer = 0
		if((self.activeCharacter is not None) and
			(not self.activeCharacter.isDestroyed)):
			self.activeCharacter.destroy()
			self.activeCharacter.accountEntity = None
			self.activeCharacter = None
			DEBUG_MSG("onDestroyTimer::Account_activeCharacter__")
		self.destroy()
		DEBUG_MSG("Account.onDestoryTimer[%i]::death[%i]" % (self.id, self.isDestroyed))

	def onTimer(self, id, userArg):
		if(TIMER_TYPE_DESTROY == userArg):
			self.onDestroyTimer()

	#处理异地登录
	def dealNonLocalLogin(self):
		if(self.activeCharacter is not None and self.activeCharacter.client is not None):

			self.relogin = True
			DEBUG_MSG("Account_dealNonLocalLogin_giveClientToBegin::[%i]" %(self.activeCharacter.getStartGameState()))
			self.activeCharacter.giveClientTo(self)
			DEBUG_MSG("Account_dealNonLocalLogin_giveClientToEnd::[%i]" %(self.activeCharacter.getStartGameState()))
			#分為開始游戲和未開始游戲
			#未開始游戲:則銷毀之前的avatar
			#開始游戲:則保留之前的avatar,不進行處理
			if(not self.activeCharacter.getStartGameState()):
				DEBUG_MSG("Account_dealNonLocalLogin_activeCharacter_destroySelf")
				self.activeCharacter.destroySelf()
			self.relogin = False
		else:
			DEBUG_MSG("Account_dealNonLocalLogin_activeCharacter is None")

	#处理断线重登==>情况：断线情况包括主动关闭客户端、客户端断线未在规定时间内登录
	def dealDisconnect(self):

		pass

	def accountCharacter(self, weaponId):
		infos = self.characters[self.databaseID].asDict()
		DEBUG_MSG("weaponChanged__infos::%s !" % (infos))
		infos["weaponId"] = weaponId

		avatarinfo = TAvatarInfos()
		self.characters[self.databaseID] = avatarinfo.createFromDict(infos)
		DEBUG_MSG("weaponChanged__infos::%s !" % (self.characters))
		pass