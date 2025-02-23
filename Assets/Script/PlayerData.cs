using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int HP;
    public int Damage;
    public float SpeedAtk;
    public float critChance;
    public float critDamage;


    // Thêm các trường dữ liệu khác của người chơi tại đây

    
}