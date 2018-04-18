using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageEffects : MonoBehaviour
{

    //private CanvasGroup mainCanvasGroup;
    private Image mainImage;
    [SerializeField] private Image redMask;

    public static ImageEffects PUBLIC;
    private void Awake()
    {
        PUBLIC = this;
        //mainCanvasGroup = GetComponent<CanvasGroup>();
        mainImage = GetComponent<Image>();
    }

    public enum EffectType
    {
        Brighten,
        Darken,
        Red_Flash,
        Null,
    }

    public void PlayEffect(EffectType type, float duration)
    {
        if (type == EffectType.Null) return;
        switch (type)
        {
            case EffectType.Brighten:
                {
                    mainImage.color = Color.black;
                    mainImage.DOFade(0, duration).SetEase(Ease.OutQuad);
                    break;
                }
            case EffectType.Darken:
                {
                    mainImage.color = new Color(0, 0, 0, 0);
                    mainImage.DOFade(1, duration).SetEase(Ease.Linear);
                    break;
                }
            case EffectType.Red_Flash:
                {
                    if (redMask)
                    {
                        redMask.color = Color.white;
                        redMask.DOFade(0, duration).SetEase(Ease.OutQuint);
                    }
                    break;
                }
        }
    }

    public void SetMainAlpha(float alpha)
    {
        Color c = mainImage.color;
        c.a = alpha;
        mainImage.color = c;
    }



}
