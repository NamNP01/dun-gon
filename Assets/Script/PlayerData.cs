using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int HP;
    public int Damage;
    public float SpeedAtk;
    public float critChance;
    public float critDamage;
    public float Exp;
    public int Level;
    public int ExpToNextLevel; // 🔥 EXP cần để lên cấp
    public bool hasDiagonalArrows = false;
    public bool hasSideArrows = false;
    public bool hasRearArrow = false;
    public bool hasMultishot = false;
    public bool hasPiercingShot = false;
    public bool hasRicochet = false; // 🌟 Kích hoạt kỹ năng Ricochet
    public bool hasBouncyWall = false;






    // Lưu giá trị gốc
    private int originalHP;
    private int originalDamage;
    private float originalSpeedAtk;
    private float originalCritChance;
    private float originalCritDamage;
    private float originalExp;
    private int originalLevel;
    private int originalExpToNextLevel;

    private void OnEnable()
    {
        // Lưu giá trị gốc khi ScriptableObject được kích hoạt
        originalHP = HP;
        originalDamage = Damage;
        originalSpeedAtk = SpeedAtk;
        originalCritChance = critChance;
        originalCritDamage = critDamage;
        originalExp = Exp;
        originalLevel = Level;
        originalExpToNextLevel = ExpToNextLevel;

    }

    public void ResetStats()
    {
        // Khôi phục lại giá trị gốc
        HP = originalHP;
        Damage = originalDamage;
        SpeedAtk = originalSpeedAtk;
        critChance = originalCritChance;
        critDamage = originalCritDamage;
        Level = 1;
        Exp = 0;
        ExpToNextLevel = 5;
        hasDiagonalArrows = false;
        hasSideArrows = false;
        hasRearArrow = false;
        hasMultishot = false;
        hasPiercingShot = false;
        hasRicochet = false;
        hasBouncyWall = false;
    }


    public void GainExp(int amount)
    {
        Exp += amount;
        Debug.Log("Gained " + amount + " EXP. Total: " + Exp);

        if (Exp >= ExpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Level++;
        Debug.Log("Leveled Up! New Level: " + Level);

        Exp -= ExpToNextLevel;
        ExpToNextLevel += 5; // 🔥 EXP cần để lên cấp tăng dần
        Debug.Log("Leveled Up! New Level: " + Level);


        if (AbilityManager.Instance != null)
        {
            AbilityManager.Instance.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("⚠ AbilityManager chưa được khởi tạo!");
        }
    }
    public void ApplyAbilityEffect(AbilityData ability)
    {
        switch (ability.abilityType)
        {
            case AbilityType.IncreaseDamage:
                int damageIncrease = Mathf.RoundToInt(originalDamage * 0.3f);
                Damage += damageIncrease;
                Debug.Log($"🔥 Tăng {damageIncrease} Damage! Tổng Damage: {Damage}");
                break;

            case AbilityType.IncreaseSpeedAtk:
                float speedIncrease = originalSpeedAtk * 0.25f;
                SpeedAtk += speedIncrease;
                Debug.Log($"⚡ Tăng {speedIncrease:F2} Speed Attack! Tổng Speed: {SpeedAtk}");
                break;

            case AbilityType.IncreaseCrit:
                critChance += 0.1f;
                critDamage += 0.4f;
                Debug.Log($"🎯 Tăng 10% Crit Chance & 40% Crit Damage! Crit Chance: {critChance}%, Crit Damage: {critDamage}%");
                break;

            case AbilityType.IncreaseHP:
                int hpIncrease = Mathf.RoundToInt(originalHP * 0.2f);
                //HP += hpIncrease;
                if (PlayerHpBar.Instance != null)
                {
                    PlayerHpBar.Instance.GetHpBoost(hpIncrease);
                }

                Debug.Log($"❤️ Tăng {hpIncrease} HP! Tổng HP: {HP}");
                break;

            case AbilityType.FrontArrowPlusOne:
                PlayerTargeting.Instance.ChangeBoltPrefab();
                Debug.Log("🏹 Đạn đã thay đổi sang loại mới!");
                break;

            case AbilityType.Multishot:

                if (!hasMultishot)
                {
                    hasMultishot = true;
                    Debug.Log("🏹 Kỹ năng Multishot được kích hoạt!");
                }

                // ⚠ Giảm Damage & Attack Speed dựa trên giá trị gốc
                int damageReduction = Mathf.RoundToInt(originalDamage * 0.1f);
                float speedReduction = originalSpeedAtk * 0.15f;

                Damage -= damageReduction;
                SpeedAtk -= speedReduction;

                Debug.Log($"⚠ Giảm {damageReduction} Damage & {speedReduction:F2} Attack Speed! Tổng Damage: {Damage}, Speed: {SpeedAtk}");
                break;

            case AbilityType.DiagonalArrows:
                if (!hasDiagonalArrows)
                {
                    // Lần đầu tiên nâng cấp, kích hoạt bắn mũi tên phụ
                    hasDiagonalArrows = true;
                    Debug.Log("🏹 Đã kích hoạt Diagonal Arrows!");
                }
                else
                {
                    // Những lần sau, thay đổi prefab của mũi tên phụ
                    PlayerTargeting.Instance.ChangeDiagonalArrowPrefab();
                    Debug.Log("🔄 Đã thay đổi đạn phụ!");
                }
                break;

            case AbilityType.SideArrowsPlusOne:
                if (!hasSideArrows)
                {
                    hasSideArrows = true;
                    Debug.Log("🏹 Đã kích hoạt Side Arrows +1!");
                }
                else
                {
                    PlayerTargeting.Instance.ChangeSideArrowPrefab();
                    Debug.Log("🔄 Đã thay đổi đạn Side Arrows!");
                }
                break;

            case AbilityType.RearArrowPlusOne:
                if (!hasRearArrow)
                {
                    hasRearArrow = true;
                    Debug.Log("🏹 Đã kích hoạt Rear Arrow +1!");
                }
                else
                {
                    PlayerTargeting.Instance.ChangeRearArrowPrefab();
                    Debug.Log("🔄 Đã thay đổi đạn Rear Arrow!");
                }
                break;

            case AbilityType.PiercingShot:
                if (!hasPiercingShot)
                {
                    hasPiercingShot = true;
                    Debug.Log("🏹 Kỹ năng Piercing Shot được kích hoạt!");
                }
                break;

            case AbilityType.BouncyWall:
                hasBouncyWall = true;
                Debug.Log("🏹 Kỹ năng Bouncy Wall được kích hoạt!");
                break;

        }
    }
}
    