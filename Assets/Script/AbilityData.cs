using UnityEngine;
public enum AbilityType
{
    IncreaseDamage,    // +30% Damage
    IncreaseSpeedAtk,  // +25% Attack Speed
    IncreaseCrit,      // +10% Crit Chance, +40% Crit Damage
    IncreaseHP         // +20% Max HP
}
[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Create New Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;   // Tên Ability
    public string description;   // Mô tả
    public Sprite icon;          // Ảnh biểu tượng
    public AbilityType abilityType; // Loại Ability
}

