using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int HP;
    public int Damage;
    public float SpeedAtk;
    public float critChance;
    public float critDamage;

    // Lưu giá trị gốc
    private int originalHP;
    private int originalDamage;
    private float originalSpeedAtk;
    private float originalCritChance;
    private float originalCritDamage;

    private void OnEnable()
    {
        // Lưu giá trị gốc khi ScriptableObject được kích hoạt
        originalHP = HP;
        originalDamage = Damage;
        originalSpeedAtk = SpeedAtk;
        originalCritChance = critChance;
        originalCritDamage = critDamage;
    }

    public void ResetStats()
    {
        // Khôi phục lại giá trị gốc
        HP = originalHP;
        Damage = originalDamage;
        SpeedAtk = originalSpeedAtk;
        critChance = originalCritChance;
        critDamage = originalCritDamage;
    }
}
