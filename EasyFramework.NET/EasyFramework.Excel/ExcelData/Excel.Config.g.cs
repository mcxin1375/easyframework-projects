
namespace Game
{
    public partial class Character
    {
        /// <summary>
        /// id
        /// </summary>
        public readonly int Id; // id
        /// <summary>
        /// 角色id
        /// </summary>
        public readonly int CharacterId; // 角色id
        /// <summary>
        /// 名称
        /// </summary>
        public readonly int Name; // 名称
        /// <summary>
        /// 品质
        /// </summary>
        public readonly Quality Quality; // 品质
        /// <summary>
        /// 大类型
        /// </summary>
        public readonly CharacterType Type; // 大类型
        /// <summary>
        /// 类型
        /// </summary>
        public readonly CharacterSubType SubType; // 类型
        /// <summary>
        /// 阵营
        /// </summary>
        public readonly int Camp; // 阵营
        /// <summary>
        /// 等级
        /// </summary>
        public readonly int Level; // 等级
        /// <summary>
        /// 升级类型
        /// </summary>
        public readonly LevelUpType LvUpType; // 升级类型
        /// <summary>
        /// 升级参数
        /// </summary>
        public readonly int LvUpData; // 升级参数
        /// <summary>
        /// 攻击力
        /// </summary>
        public readonly int Attack; // 攻击力
        /// <summary>
        /// 最大生命
        /// </summary>
        public readonly int HpMax; // 最大生命
        /// <summary>
        /// 攻速
        /// </summary>
        public readonly int AttackSpeed; // 攻速
        /// <summary>
        /// 移速
        /// </summary>
        public readonly int MoveSpeed; // 移速
        /// <summary>
        /// 命中率
        /// </summary>
        public readonly int Hit; // 命中率
        /// <summary>
        /// 闪避率
        /// </summary>
        public readonly int Dodge; // 闪避率
        /// <summary>
        /// 暴击率
        /// </summary>
        public readonly int Critical; // 暴击率
        /// <summary>
        /// 暴伤
        /// </summary>
        public readonly int CriticalHurt; // 暴伤
        /// <summary>
        /// 风属性攻击
        /// </summary>
        public readonly int Wind; // 风属性攻击
        /// <summary>
        /// 火属性攻击
        /// </summary>
        public readonly int Fire; // 火属性攻击
        /// <summary>
        /// 电属性攻击
        /// </summary>
        public readonly int Thunder; // 电属性攻击
        /// <summary>
        /// 水属性攻击
        /// </summary>
        public readonly int Water; // 水属性攻击
        /// <summary>
        /// 毒属性攻击
        /// </summary>
        public readonly int Poison; // 毒属性攻击
        /// <summary>
        /// 碰撞伤害
        /// </summary>
        public readonly int CollisionDamage; // 碰撞伤害
        /// <summary>
        /// 射程
        /// </summary>
        public readonly int AttackRange; // 射程
        /// <summary>
        /// 视野
        /// </summary>
        public readonly int View; // 视野
        /// <summary>
        /// 是否穿墙
        /// </summary>
        public readonly int ThroughWall; // 是否穿墙
        /// <summary>
        /// 是否穿水面
        /// </summary>
        public readonly int ThroughWater; // 是否穿水面
        /// <summary>
        /// 交互范围
        /// </summary>
        public readonly int InteractionView; // 交互范围
        /// <summary>
        /// AI类型
        /// </summary>
        public readonly int Ai; // AI类型
        /// <summary>
        /// 普攻
        /// </summary>
        public readonly int NormalSkill; // 普攻
        /// <summary>
        /// 技能组1
        /// </summary>
        public readonly string Skill1; // 技能组1
        /// <summary>
        /// 技能组2
        /// </summary>
        public readonly string Skill2; // 技能组2
        /// <summary>
        /// 技能组3
        /// </summary>
        public readonly string Skill3; // 技能组3
        /// <summary>
        /// 技能组4
        /// </summary>
        public readonly string Skill4; // 技能组4
        /// <summary>
        /// 被动技能
        /// </summary>
        public readonly string PassiveSkill; // 被动技能
        /// <summary>
        /// 功能性掉落
        /// </summary>
        public readonly int FunctionalDrop; // 功能性掉落
        /// <summary>
        /// 受击掉落
        /// </summary>
        public readonly string HurtDropGroupId; // 受击掉落
        /// <summary>
        /// 死亡掉落
        /// </summary>
        public readonly string DeathDropGroupId; // 死亡掉落
        /// <summary>
        /// 模型
        /// </summary>
        public readonly int Model; // 模型
        /// <summary>
        /// 头像
        /// </summary>
        public readonly string HeadIcon; // 头像
        /// <summary>
        /// 入场预警
        /// </summary>
        public readonly int EnterWarning; // 入场预警
        /// <summary>
        /// 交互范围类型
        /// </summary>
        public readonly InteractionType InteractionType; // 交互范围类型
        /// <summary>
        /// 角色出生台词
        /// </summary>
        public readonly int BirthLine; // 角色出生台词
        /// <summary>
        /// 角色死亡台词
        /// </summary>
        public readonly int DeathLine; // 角色死亡台词
        /// <summary>
        /// 冲刺预警
        /// </summary>
        public readonly int SprintWarning; // 冲刺预警
        /// <summary>
        /// 距离预警
        /// </summary>
        public readonly int FarWarning; // 距离预警
        /// <summary>
        /// 出生暂停
        /// </summary>
        public readonly bool BornStop; // 出生暂停
        /// <summary>
        /// 出生清怪
        /// </summary>
        public readonly bool BornClear; // 出生清怪
        /// <summary>
        /// 小地图显示
        /// </summary>
        public readonly bool ShowMap; // 小地图显示
    }

}
