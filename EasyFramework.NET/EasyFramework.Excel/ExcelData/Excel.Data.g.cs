
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Game
{
    public class ExcelData
    {
        public readonly int SvnRevision;

        public readonly Character[] CharacterItems;

        private readonly Dictionary<int, Character> _CharacterDict;

        private ExcelData(BinaryReader reader)
        {
            SvnRevision = reader.ReadInt32();
            CharacterItems = Character.Load(reader);

            _CharacterDict = CharacterItems.ToDictionary(item => item.Id, item => item);

        }
        public Character GetCharacter(int key) => _CharacterDict.ContainsKey(key) ? _CharacterDict[key] : null;

        public static ExcelData Load(byte[] binary)
        {
            using MemoryStream memoryStream = new MemoryStream(binary);
            using BinaryReader reader = new BinaryReader(memoryStream);
            return new ExcelData(reader);
        }
        public static ExcelData Load(string file)
        {
            using FileStream fileStream = new FileStream(file, FileMode.Open);
            using BinaryReader reader = new BinaryReader(fileStream);
            return new ExcelData(reader);
        }
    }

    public partial class Character
    {
        internal static Character[] Load(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            var arr = new Character[count];
            for (int i = 0; i < count; i++) arr[i] = new Character(reader);
            return arr;
        }
        private Character(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            CharacterId = reader.ReadInt32();
            Name = reader.ReadInt32();
            Quality = (Quality)reader.ReadInt32();
            Type = (CharacterType)reader.ReadInt32();
            SubType = (CharacterSubType)reader.ReadInt32();
            Camp = reader.ReadInt32();
            Level = reader.ReadInt32();
            LvUpType = (LevelUpType)reader.ReadInt32();
            LvUpData = reader.ReadInt32();
            Attack = reader.ReadInt32();
            HpMax = reader.ReadInt32();
            AttackSpeed = reader.ReadInt32();
            MoveSpeed = reader.ReadInt32();
            Hit = reader.ReadInt32();
            Dodge = reader.ReadInt32();
            Critical = reader.ReadInt32();
            CriticalHurt = reader.ReadInt32();
            Wind = reader.ReadInt32();
            Fire = reader.ReadInt32();
            Thunder = reader.ReadInt32();
            Water = reader.ReadInt32();
            Poison = reader.ReadInt32();
            CollisionDamage = reader.ReadInt32();
            AttackRange = reader.ReadInt32();
            View = reader.ReadInt32();
            ThroughWall = reader.ReadInt32();
            ThroughWater = reader.ReadInt32();
            InteractionView = reader.ReadInt32();
            Ai = reader.ReadInt32();
            NormalSkill = reader.ReadInt32();
            Skill1 = reader.ReadString();
            Skill2 = reader.ReadString();
            Skill3 = reader.ReadString();
            Skill4 = reader.ReadString();
            PassiveSkill = reader.ReadString();
            FunctionalDrop = reader.ReadInt32();
            HurtDropGroupId = reader.ReadString();
            DeathDropGroupId = reader.ReadString();
            Model = reader.ReadInt32();
            HeadIcon = reader.ReadString();
            EnterWarning = reader.ReadInt32();
            InteractionType = (InteractionType)reader.ReadInt32();
            BirthLine = reader.ReadInt32();
            DeathLine = reader.ReadInt32();
            SprintWarning = reader.ReadInt32();
            FarWarning = reader.ReadInt32();
            BornStop = reader.ReadBoolean();
            BornClear = reader.ReadBoolean();
            ShowMap = reader.ReadBoolean();
        }
    }

}
