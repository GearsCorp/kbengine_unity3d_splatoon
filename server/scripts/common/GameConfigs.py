# -*- coding: utf-8 -*-

"""
"""

# ------------------------------------------------------------------------------
# entity state
# ------------------------------------------------------------------------------
ENTITY_STATE_UNKNOW										= -1
ENTITY_STATE_SAFE										= 0
ENTITY_STATE_FREE										= 1
ENTITY_STATE_MAX    									= 4

#  红队ID
RED_TEAM_ID = 1

#  蓝队ID
BLUE_TEAM_ID = 2

#  一个房间最大人数
ROOM_MAX_PLAYER = 8

#  一个房间最少人数
ROOM_MIN_PLAYER = 1

# 限制玩家最大分割数量
PLAYER_LIMIT_SPLIT = 16

#  一局游戏时间（秒）
GAME_ROUND_TIME = 60 * 20 #60 * 12

#一局游戏最小剩余时间
GAME_MIN_REMAIN_TIME = 5

GAME_READY_TIME = 3 #60 * 12


# Bots机器人AI频率（秒）
BOTS_UPDATE_TIME = 0.3

# 地图大小(米)
GAME_MAP_SIZE = 200


#武器A
WEAPON_A_ID = 1000

#武器B
WEAPON_B_ID = 1001
