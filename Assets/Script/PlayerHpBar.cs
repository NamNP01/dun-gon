using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    public static PlayerHpBar Instance // singlton     
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PlayerHpBar>();
                if (instance == null)
                {
                    var instanceContainer = new GameObject("PlayerHpBar");
                    instance = instanceContainer.AddComponent<PlayerHpBar>();
                }
            }
            return instance;
        }
    }
    private static PlayerHpBar instance;
    public PlayerData playerData;

    public Transform player;
    public Slider hpBar;
    //public float maxHp;
    public float currentHp;

    public GameObject HpLineFolder;

    public TextMeshProUGUI playerHpText;
    float unitHp = 200f;

    public Vector3 hpBarOffset = new Vector3(0, 2f, 0);  // Điều chỉnh vị trí theo ý muốn


    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }
        UpdateHpText();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + hpBarOffset;
        hpBar.value = currentHp / playerData.HP;
    }

    public void GetHpBoost(int hpAmount)
    {
        playerData.HP += hpAmount;
        currentHp += hpAmount;
        UpdateHpText();

        float scaleX = (1000f / unitHp) / (playerData.HP / unitHp);
        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);

        foreach (Transform child in HpLineFolder.transform)
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
        }

        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
    }
    public void UpdateHpText()
    {
        if (playerHpText != null)
        {
            playerHpText.text = currentHp.ToString(); // Chỉ hiển thị số máu
        }
    }
}