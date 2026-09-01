
namespace Game
{
    public enum Quality
    {
        /// <summary>
        /// 白
        /// </summary>
        White = 0, // 白
        /// <summary>
        /// 绿
        /// </summary>
        Green = 1, // 绿
        /// <summary>
        /// 蓝
        /// </summary>
        Blue = 2, // 蓝
        /// <summary>
        /// 紫
        /// </summary>
        Purple = 3, // 紫
        /// <summary>
        /// 橙
        /// </summary>
        Orange = 4, // 橙
        /// <summary>
        /// 红
        /// </summary>
        Red = 5, // 红
    }
    public enum AudioType
    {
        /// <summary>
        /// 不播放
        /// </summary>
        Normal = 0, // 不播放
        /// <summary>
        /// 背景音乐
        /// </summary>
        BGM = 101, // 背景音乐
        /// <summary>
        /// UI交互音效
        /// </summary>
        UI = 201, // UI交互音效
        /// <summary>
        /// 角色音效
        /// </summary>
        Role = 210, // 角色音效
        /// <summary>
        /// 角色出生
        /// </summary>
        RoleSpawn = 211, // 角色出生
        /// <summary>
        /// 角色死亡
        /// </summary>
        RoleDie = 212, // 角色死亡
        /// <summary>
        /// 角色移动
        /// </summary>
        RoleMove = 213, // 角色移动
    }
    public enum CharacterType
    {
        /// <summary>
        /// 英雄
        /// </summary>
        Hero = 4, // 英雄
        /// <summary>
        /// 怪物
        /// </summary>
        Monster = 1, // 怪物
        /// <summary>
        /// 交互物
        /// </summary>
        Interactable = 2, // 交互物
        /// <summary>
        /// 建筑
        /// </summary>
        Building = 3, // 建筑
        /// <summary>
        /// 宝宝
        /// </summary>
        Pet = 5, // 宝宝
    }
    public enum CharacterSubType
    {
        /// <summary>
        /// 小怪
        /// </summary>
        Monster = 101, // 小怪
        /// <summary>
        /// 精英怪
        /// </summary>
        EliteMonster = 102, // 精英怪
        /// <summary>
        /// Boss
        /// </summary>
        Boss = 103, // Boss
        /// <summary>
        /// 转盘
        /// </summary>
        ZhuanPan = 201, // 转盘
        /// <summary>
        /// 女武神
        /// </summary>
        Valkyrie = 202, // 女武神
        /// <summary>
        /// 天使
        /// </summary>
        Angel = 203, // 天使
        /// <summary>
        /// 恶魔
        /// </summary>
        Demon = 204, // 恶魔
        /// <summary>
        /// 商人
        /// </summary>
        Merchant = 205, // 商人
        /// <summary>
        /// 药物
        /// </summary>
        Drug = 206, // 药物
        /// <summary>
        /// 宠物蛋
        /// </summary>
        PetEgg = 207, // 宠物蛋
        /// <summary>
        /// 地块
        /// </summary>
        Tile = 208, // 地块
        /// <summary>
        /// 建筑
        /// </summary>
        Build = 301, // 建筑
        /// <summary>
        /// 传送点
        /// </summary>
        Teleport = 302, // 传送点
        /// <summary>
        /// 出生点
        /// </summary>
        BornPoint = 303, // 出生点
        /// <summary>
        /// 家园建筑
        /// </summary>
        HomeBuilding = 304, // 家园建筑
        /// <summary>
        /// 植物
        /// </summary>
        Plant = 305, // 植物
        /// <summary>
        /// 水面
        /// </summary>
        Water = 306, // 水面
        /// <summary>
        /// 风宝宝
        /// </summary>
        WindPet = 501, // 风宝宝
        /// <summary>
        /// 火宝宝
        /// </summary>
        FirePet = 502, // 火宝宝
        /// <summary>
        /// 电宝宝
        /// </summary>
        ThunderPet = 503, // 电宝宝
        /// <summary>
        /// 水宝宝
        /// </summary>
        WaterPet = 504, // 水宝宝
        /// <summary>
        /// 毒宝宝
        /// </summary>
        PoisonPet = 505, // 毒宝宝
    }
    public enum InteractionType
    {
        /// <summary>
        /// 碰撞
        /// </summary>
        Collision = 0, // 碰撞
        /// <summary>
        /// 范围内
        /// </summary>
        InRange = 1, // 范围内
    }
    public enum LevelUpType
    {
        /// <summary>
        /// 不可升级
        /// </summary>
        None = 0, // 不可升级
        /// <summary>
        /// 英雄经验值
        /// </summary>
        Exp = 1, // 英雄经验值
        /// <summary>
        /// 战斗时长累计
        /// </summary>
        BattleTime = 2, // 战斗时长累计
        /// <summary>
        /// 英雄连击次数累计
        /// </summary>
        TotalComboCount = 3, // 英雄连击次数累计
        /// <summary>
        /// 英雄攻击次数累计
        /// </summary>
        TotalAttackCount = 4, // 英雄攻击次数累计
        /// <summary>
        /// 英雄移动时长累计
        /// </summary>
        TotalMoveDuration = 5, // 英雄移动时长累计
        /// <summary>
        /// 英雄静止时长累计
        /// </summary>
        TotalStandDuration = 6, // 英雄静止时长累计
    }

}
