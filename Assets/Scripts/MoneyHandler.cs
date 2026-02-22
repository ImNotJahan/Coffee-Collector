using TMPro;
using UnityEngine;

public class MoneyHandler : MonoBehaviour
{
    static MoneyHandler instance;

    [SerializeField] TMP_Text moneyText;

    float money;

    void Awake()
    {
        instance = this;

        money = 0;

        RefreshText();
    }

    void RefreshText()
    {
        moneyText.text = $"${money:F2}";
    }

    public static void ChangeMoney(float byAmount)
    {
        instance.money += byAmount;

        instance.RefreshText();
    }

    public static float GetMoney() => instance.money;
}