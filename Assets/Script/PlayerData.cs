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
    public int Level = 1;
    public int ExpToNextLevel; // 🔥 EXP cần để lên cấp

    // Lưu giá trị gốc
    private int originalHP;
    private int originalDamage;
    private float originalSpeedAtk;
    private float originalCritChance;
    private float originalCritDamage;
    private float originalExp;
    private int originalLevel = 1;
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
        Exp = originalExp;
        Level = originalLevel;
        ExpToNextLevel = originalExpToNextLevel;

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
        Exp -= ExpToNextLevel;
        ExpToNextLevel += 5; // 🔥 EXP cần để lên cấp tăng dần
        Debug.Log("Leveled Up! New Level: " + Level);
    }
}
