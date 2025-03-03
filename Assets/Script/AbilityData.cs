using UnityEngine;
public enum AbilityType
{
    IncreaseDamage,    // +30% Damage
    IncreaseSpeedAtk,  // +25% Attack Speed
    IncreaseCrit,      // +10% Crit Chance, +40% Crit Damage
    IncreaseHP,         // +20% Max HP
    FrontArrowPlusOne,  // 🏹 Thay đổi Prefab đạn
    Multishot,  // 🎯 Tăng số mũi tên bắn ra, nhưng giảm Damage & Attack Speed
    DiagonalArrows, // 🏹 Bắn thêm 2 mũi tên góc 45° và -45°
    SideArrowsPlusOne, // 🏹 Bắn thêm 2 mũi tên góc 90° và -90°
    RearArrowPlusOne, // 🏹 Bắn thêm 1 mũi tên góc 180°
    PiercingShot, // 🏹 Bắn xuyên quái, giảm 33% sát thương sau mỗi lần xuyên
    BouncyWall // 🏹 Mũi tên bật lại khi chạm tường, giảm 50% damage
}
[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Create New Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;   // Tên Ability
    public string description;   // Mô tả
    public Sprite icon;          // Ảnh biểu tượng
    public AbilityType abilityType; // Loại Ability
    public int maxAllowedCount = -1; // 🌟 Giới hạn số lần chọn (-1 = không giới hạn)
}

