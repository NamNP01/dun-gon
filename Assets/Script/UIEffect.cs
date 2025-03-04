using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEditor.MaterialProperty;

public class UIEffect : MonoBehaviour
{
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        StartGlowEffect();
    }

    void StartGlowEffect()
    {
        image.DOFade(0.5f, 0.6f).SetLoops(-1, LoopType.Yoyo); // Làm icon sáng tối liên tục
    }
}
