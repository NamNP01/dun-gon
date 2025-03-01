using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class AbilityManager : MonoBehaviour
{
    public GameObject abilityPanel;
    public Button[] abilityButtons;
    public List<AbilityData> allAbilities;
    public PlayerData playerData;

    public static AbilityManager Instance { get; private set; }

    private List<AbilityData> selectedAbilities = new List<AbilityData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
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

    public void ShowAbilitySelection()
    {
        // Reset lại alpha của tất cả button
        foreach (var btn in abilityButtons)
        {
            Image buttonImage = btn.GetComponent<Image>();
            if (buttonImage != null)
            {
                Color tempColor = buttonImage.color;
                tempColor.a = 1f; // Đặt lại alpha về 1 để hiện lại nút
                buttonImage.color = tempColor;
            }
        }
        Debug.Log("Mở menu chọn kỹ năng!");
        abilityPanel.SetActive(true);
        selectedAbilities.Clear();



        List<AbilityData> tempList = new List<AbilityData>(allAbilities);
        for (int i = 0; i < 3 && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedAbilities.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        Sequence sequence = DOTween.Sequence(); // 🔥 Tạo một chuỗi hiệu ứng

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            if (i < selectedAbilities.Count)
            {
                AbilityData ability = selectedAbilities[i];
                abilityButtons[i].gameObject.SetActive(true);
                abilityButtons[i].GetComponent<Image>().sprite = ability.icon;
                abilityButtons[i].onClick.RemoveAllListeners();
                abilityButtons[i].onClick.AddListener(() => SelectAbility(ability));

                //eff scale
                // 🔥 Reset Scale trước khi bắt đầu hiệu ứng
                abilityButtons[i].transform.localScale = Vector3.one * 2.4f;

                abilityButtons[i].transform.localScale = Vector3.zero; // Bắt đầu từ 0 để phóng to
                abilityButtons[i].transform.DOScale(Vector3.one * 2.4f, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(0.1f * i);
            }
            else
            {
                abilityButtons[i].gameObject.SetActive(false);
            }
        }
        // 🌟 Sau khi hiệu ứng xong, mới dừng thời gian
        sequence.AppendInterval(0.5f); // Delay một chút (~0.4s)
        sequence.AppendCallback(() => Time.timeScale = 0);
    }

    public void SelectAbility(AbilityData ability)
    {
        Debug.Log("Chọn Ability: " + ability.abilityName);

        // 🔥 Tìm button đã chọn
        Button selectedButton = null;
        foreach (var btn in abilityButtons)
        {
            Image buttonImage = btn.GetComponent<Image>();
            if (buttonImage.sprite == ability.icon)
            {
                selectedButton = btn;
                break;
            }
        }

        if (selectedButton != null)
        {
            Image buttonImage = selectedButton.GetComponent<Image>();

            if (buttonImage != null)
            {
                Sequence selectSequence = DOTween.Sequence();
                //selectSequence.Append(selectedButton.transform.DOScale(4f, 0.15f) // 🔥 Phóng to lên 1.2 lần
                //    .SetEase(Ease.OutBack)
                //    .SetUpdate(true));
                //selectSequence.Append(selectedButton.transform.DOScale(2.4f, 0.15f) // 🔥 Thu nhỏ về lại 1
                //    .SetEase(Ease.InOutBack)
                //    .SetUpdate(true));
                selectSequence.Append(selectedButton.transform.DOScale(0f, 0.3f) // 🔥 Biến mất
                    .SetEase(Ease.InBack)
                    .SetUpdate(true));

                buttonImage.DOFade(0, 0.3f).SetUpdate(true); // 🔥 Mờ dần ngay cả khi pause game
            }
        }

        // 🌟 Delay một chút rồi tắt panel
        DOVirtual.DelayedCall(0.65f, () =>
        {
            playerData.ApplyAbilityEffect(ability);
            Time.timeScale = 1;
            abilityPanel.SetActive(false);
        }).SetUpdate(true);
    }


}

