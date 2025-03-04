using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AngleAbilityManager : MonoBehaviour
{
    public GameObject abilityPanel;
    public Button[] abilityButtons;
    public List<AbilityData> allAngleAbilities;
    public PlayerData playerData;

    public static AngleAbilityManager Instance { get; private set; }

    private List<AbilityData> selectedAbilities = new List<AbilityData>();
    private Dictionary<AbilityType, int> abilityCounts = new Dictionary<AbilityType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        abilityPanel.SetActive(false);
    }

    public void ShowAngleAbilitySelection(Angel angel)
    {
        Debug.Log($"👼 Mở menu chọn kỹ năng từ {angel.angelType}");
        abilityPanel.SetActive(true);
        selectedAbilities.Clear();

        // Lấy danh sách tất cả kỹ năng có sẵn
        List<AbilityData> healAbilities = allAngleAbilities.FindAll(ability =>
            ability.abilityType == AbilityType.Heal);

        List<AbilityData> critAbilities = allAngleAbilities.FindAll(ability =>
            ability.abilityType == AbilityType.CritMasterMinor);

        List<AbilityData> damageAbilities = allAngleAbilities.FindAll(ability =>
            ability.abilityType == AbilityType.AttackBoostMinor);

        List<AbilityData> speedAbilities = allAngleAbilities.FindAll(ability =>
            ability.abilityType == AbilityType.AttackSpeedBoostMinor);

        // Luôn có Heal
        if (healAbilities.Count > 0)
        {
            selectedAbilities.Add(healAbilities[Random.Range(0, healAbilities.Count)]);
        }

        // Chọn ability thứ 2 theo loại Angel
        switch (angel.angelType)
        {
            case Angel.AngelType.CritAngel:
                if (critAbilities.Count > 0)
                    selectedAbilities.Add(critAbilities[Random.Range(0, critAbilities.Count)]);
                break;

            case Angel.AngelType.DamageAngel:
                if (damageAbilities.Count > 0)
                    selectedAbilities.Add(damageAbilities[Random.Range(0, damageAbilities.Count)]);
                break;

            case Angel.AngelType.SpeedAngel:
                if (speedAbilities.Count > 0)
                    selectedAbilities.Add(speedAbilities[Random.Range(0, speedAbilities.Count)]);
                break;
        }

        // Hiển thị các ability lên UI
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < selectedAbilities.Count)
            {
                AbilityData ability = selectedAbilities[i];
                abilityButtons[i].gameObject.SetActive(true);
                abilityButtons[i].GetComponent<Image>().sprite = ability.icon;
                abilityButtons[i].onClick.RemoveAllListeners();
                abilityButtons[i].onClick.AddListener(() => SelectAngleAbility(ability));
            }
            else
            {
                abilityButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectAngleAbility(AbilityData ability)
    {
        Debug.Log("Chọn Ability góc bắn: " + ability.abilityName);

        if (!abilityCounts.ContainsKey(ability.abilityType))
        {
            abilityCounts[ability.abilityType] = 0;
        }
        abilityCounts[ability.abilityType]++;

        Debug.Log($"🔢 {ability.abilityName} đã chọn {abilityCounts[ability.abilityType]}/{ability.maxAllowedCount}");

        // Áp dụng Ability vào Player
        playerData.ApplyAbilityEffect(ability);

        // Đóng UI
        abilityPanel.SetActive(false);
    }
}
