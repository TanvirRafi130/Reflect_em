using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager Instance => instance;


    [Header("Shop Panel")]
    public GameObject shopPanel;
    public List<GameObject> otherhPanels;
    public Button shopCloseButton;
    [Header("Health")]
    public int healthPrice = 20;
    public float healthIncreaseBy = 100;
    public int healthPriceIncrease = 30;
    public TextMeshProUGUI priceTextHealth;
    public Button healthBuyButton;
    [Header("Full Health")]
    public int fullHealthPrice = 500;
    public int fullHealthPriceIncrease = 2000;
    public TextMeshProUGUI priceTextFullealth;
    public Button fullHealthBuyButton;

    [Header("Shield")]
    public int shieldRechargePrice = 20;
    public float shieldRechareSpeedIncreaseBy = 0.2f;
    public int shieldRechargePriceIncrease = 30;
    public TextMeshProUGUI priceTextShield;
    public Button shieldBuyButton;
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthBuyButton.onClick.AddListener(() => HealthBuy());
        fullHealthBuyButton.onClick.AddListener(() => FullHealthBuy());
        shieldBuyButton.onClick.AddListener(() => ShieldBuy());
        shopCloseButton.onClick.AddListener(() => ShopClose());

        shopPanel.gameObject.transform.localScale = Vector3.zero;
        foreach (var item in otherhPanels)
        {
            item.GetComponent<CanvasGroup>().alpha = 0;

        }
        SetPriceTexts();
    }

    void SetPriceTexts()
    {
        priceTextHealth.text = healthPrice.ToString();
        priceTextShield.text = shieldRechargePrice.ToString();
        priceTextFullealth.text = fullHealthPrice.ToString();
    }

    void FullHealthBuy()
    {
        GameManager.Instance.RemoveCurrency(healthPrice);
        Player.Instance.GivePlayerMaxHealth();
        fullHealthPrice += fullHealthPriceIncrease;
        SetPriceTexts();
        CheckAllButon();

    }
    void HealthBuy()
    {
        GameManager.Instance.RemoveCurrency(healthPrice);
        Player.Instance.IncreasePlayerHealthWithCurrency(healthIncreaseBy);
        healthPrice += healthPriceIncrease;
        SetPriceTexts();
        CheckAllButon();

    }
    void ShieldBuy()
    {
        GameManager.Instance.RemoveCurrency(shieldRechargePrice);
        Player.Instance.IncreasePlayerShieldSpeedWithCurrency(shieldRechareSpeedIncreaseBy);
        shieldRechargePrice += shieldRechargePriceIncrease;
        SetPriceTexts();
        CheckAllButon();
    }

    void CheckBuyingAbility(Button button, int price)
    {
        if (price <= GameManager.Instance.currencyAmount)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    void ShopClose()
    {
        shopPanel.gameObject.transform.DOScale(0f, 0.2f).SetEase(Ease.InBounce)
        .OnStart(() =>
        {
            foreach (var item in otherhPanels)
            {
                item.GetComponent<CanvasGroup>().DOFade(0, 0.1f).SetEase(Ease.Flash);

            }
        }).OnComplete(() =>
        {
            GameManager.Instance.isShopOn = false;
        })
        ;

    }
    void CheckAllButon()
    {

        CheckBuyingAbility(healthBuyButton, healthPrice);
        CheckBuyingAbility(shieldBuyButton, shieldRechargePrice);
        CheckBuyingAbility(fullHealthBuyButton, fullHealthPrice);

    }
    public void OpenShop()
    {
        CheckAllButon();
        shopPanel.gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            foreach (var item in otherhPanels)
            {
                item.GetComponent<CanvasGroup>().DOFade(1, 0.1f).SetEase(Ease.Flash);

            }
        });


    }


}
