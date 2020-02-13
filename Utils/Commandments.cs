using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public static class Commandments
    {
        [System.Serializable]
        public enum Shrines
        {
            FIRE_STAGE_1,
            FIRE_STAGE_2,
            FIRE_STAGE_3,
            NEXUS,
        }

        [System.Serializable]
        public enum LimitsType
        {
            CameraLimitUp,
            CameraLimitDown,
            CameraLimitLeft,
            CameraLimitRight,
            CameraLimitVertical,
            CameraLimitHorizontal,
            CameraLimitDiagonal
        }

        [System.Serializable]
        public enum Layers
        {
            Floor,
            CameraLimits
        }

        [System.Serializable]
        public enum Tags
        {
            Floor,
            Enemy
        }

        public enum ZodiacAnimal
        {
            RAT,
            OX,
            TIGER,
            RABBIT,
            DRAGON,
            SNAKE,
            HORSE,
            GOAT,
            MONKEY,
            ROOSTER,
            DOG,
            BOAR,
            CAT,
        }

        public enum Element
        {
            EARTH,
            METAL,
            ICE,
            LIFE,
            WATER,
            DEATH,
            WIND,
            LIGHT,
            LIGHTNING,
            FIRE,
            DARK,
            WOOD,
            NEUTRAL
        }

        public enum Weapon
        {
            HAMMER,
            AXE,
            SHIELD,
            STAFF,
            TRIDENT,
            SCYTHE,
            MACE,
            BOW,
            SPEAR,
            SWORD,
            SHURIKEN,
            FISTS
        }

                                    /*                                                                  Receive More Damage
                                                    Terra	Metal	Ice	Life	Agua	Death	Ar	Luz	Lightning	Fogo	Trevas	Wood
                                    Terra
                                    Metal
                                    Ice
Deal                           Life
Damage                    Agua	
                                    Death	
                                    Ar	
                                    Luz	
                                    Lightning	
                                    Fogo	
                                    Trevas	
                                    Wood
        
             */

        public static float[,] damageTable = new float[,]  {{1,    0.5f,    1,    1,    1,    1,    0.25f,    1,    2,    2,    1,    1},
                                                                                                {1.5f,    1,    1,    1,    0.5f,    1,    1,    1,    0.5f,    1.5f,    1,    1},
                                                                                                {1,    2,    1,    1,     0.5f,    1,    1,    1,    1,    0.25f,    1,    1.5f},
                                                                                                {1,    0.25f,    1,    1,     1,    2,    1,    0.5f,    1,    1,    1.5f,    0.5f},
                                                                                                {1,    1.5f,    0.5f,    0.5f,     1,    1,    0.5f,    1,    1,    2,    1,    1},
                                                                                                {1,    1,    1,    2,     1.5f,    1,    1,    1.5f,    0.5f,    0.5f,    0.5f,    1.5f},
                                                                                                {2,    1,    1,    0.5f,     1,    1,    1,    1,    1.5f,    1,    0.5f,    0.25f},
                                                                                                {1,    0.5f,    1,    0.5f,     0.5f,    1.5f,    1,    1,    1,    1,    2,    1},
                                                                                                {0.25f,    0.5f,    1,    1,     2,    1.5f,    0.5f,    0.25f,    1,    1,    1.5f,    1.5f},
                                                                                                {1,    1,    1.5f,    1,     0.25f,    0.5f,    1,    1,    1,    1,    1.5f,    2},
                                                                                                {1,    1.5f,    1,    1.5f,     1.5f,    0.5f,    1,    2,    0.5f,    0.5f,    2,    1},
                                                                                                {1.5f,    1,    1,    1,     1.5f,    1,    2,    1,    0.5f,    0.5f,    1,    1}
                                                                                            };

        public enum Modifiers
        {
            Invulnerability,
            InfiniteMP,
            RemoveIgnoreColisions,
            Regen,
            IncreaseDamage,
            GreaterIncreaseDamage,
            Thorns,
            Vampire,
            DamageConverter,
            IncreaseAttackSpeed,
            IncreaseCriticalChance,
            ReduceDamage,
            GreaterReduceDamage,
            ReduceFallingDamage,
            IncreaseRolling,
            IncreaseJumpHeight,
            IncreaseMoveSpeed,
            IncreaseDropRate,
            IncreaseScriptDropRate,
            IncreaseHealingRate,
            IncreaseUltimateRate,
            ManaShield,
            WallJump,
            Poison = 1000,
            Paralysis
        }

        public static bool isBuff(this Modifiers id)
        {
            return id < Modifiers.Poison;
        }
        public static float GetValue(this Modifiers id)
        {
            if (UIManager.GetInstance().player.CheckModifier(id) == false)
                return 1;

            float baseValue = 0;

            switch (id)
            {
                case Modifiers.Poison:
                    baseValue = -4 * Time.fixedDeltaTime;
                    break;
                case Modifiers.Regen:
                    baseValue = 1 * Time.fixedDeltaTime;
                    break;
                case Modifiers.IncreaseDamage:
                    baseValue = 1.5f;
                    break;
                case Modifiers.GreaterIncreaseDamage:
                    baseValue = 2f;
                    break;
                case Modifiers.Thorns:
                    baseValue = 0.5f;
                    break;
                case Modifiers.Vampire:
                    baseValue = 0.05f;
                    break;
                case Modifiers.DamageConverter:
                    baseValue = 0.02f;
                    break;
                case Modifiers.IncreaseAttackSpeed:
                    baseValue = 1.5f;
                    break;
                case Modifiers.IncreaseCriticalChance:
                    baseValue = 2f;
                    break;
                case Modifiers.ReduceDamage:
                    baseValue = 0.8f;
                    break;
                case Modifiers.GreaterReduceDamage:
                    baseValue = 0.6f;
                    break;
                case Modifiers.ReduceFallingDamage:
                    baseValue = 0f;
                    break;
                case Modifiers.IncreaseRolling:
                    baseValue = 1.5f;
                    break;
                case Modifiers.IncreaseJumpHeight:
                    baseValue = 1.3f;
                    break;
                case Modifiers.IncreaseMoveSpeed:
                    baseValue = 1.3f;
                    break;
                case Modifiers.IncreaseDropRate:
                    baseValue = 2f;
                    break;
                case Modifiers.IncreaseScriptDropRate:
                    baseValue = 2f;
                    break;
                case Modifiers.IncreaseHealingRate:
                    baseValue = 1.5f;
                    break;
                case Modifiers.IncreaseUltimateRate:
                    baseValue = 1.3f;
                    break;
                case Modifiers.ManaShield:
                    baseValue = 0.6f;
                    break;
            }
            return baseValue;
        }

        public static Weapon toWeapon(this Element currentElement)
        {
            return (Weapon)currentElement;
        }

        public static Weapon toWeapon(this ZodiacAnimal animal)
        {
            return (Weapon)animal;
        }

        public static ZodiacAnimal toAnimal(this Element currentElement)
        {
            return (ZodiacAnimal)currentElement;
        }

        public static ZodiacAnimal toAnimal(this Weapon weapon)
        {
            return (ZodiacAnimal)weapon;
        }

        public static Element toElement(this Weapon weapon)
        {
            return (Element)weapon;
        }

        public static Element toElement(this ZodiacAnimal animal)
        {
            return (Element)animal;
        }

        public static bool compare(ZodiacAnimal animal, Element currentElement)
        {
            return (int)animal == (int)currentElement;
        }

        public static bool compare(ZodiacAnimal animal, Weapon weapon)
        {
            return (int)animal == (int)weapon;
        }

        public static bool compare(Weapon weapon, Element currentElement)
        {
            return (int)weapon == (int)currentElement;
        }

        public static int getNumberofAnimals()
        {
            var myEnumMemberCount = System.Enum.GetNames(typeof(Weapon)).Length;
            return myEnumMemberCount;
        }

        public static int toInt(this Weapon weapon)
        {
            return (int)weapon;
        }

        public static int toInt(this ZodiacAnimal animal)
        {
            return (int)animal;
        }

        public static int toInt(this Element currentElement)
        {
            return (int)currentElement;
        }

    }
}