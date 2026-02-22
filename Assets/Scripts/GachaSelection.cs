using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GachaSelection : MonoBehaviour
{
    [SerializeField] GachaPull gachaPull;

    [SerializeField] Button CheapBeansButton;
    [SerializeField] Button QualityBeansButton;
    [SerializeField] Button ExoticBeansButton;

    void OnEnable()
    {
        CheapBeansButton.interactable   = MoneyHandler.GetMoney() >= 1;
        QualityBeansButton.interactable = MoneyHandler.GetMoney() >= 10;
        ExoticBeansButton.interactable  = MoneyHandler.GetMoney() >= 25;
    }

    public void CheapBeans()
    {
        MoneyHandler.ChangeMoney(-1);

        GotoPull(BeanLevel.Cheap);
    }

    public void QualityBeans()
    {
        MoneyHandler.ChangeMoney(-10);

        GotoPull(BeanLevel.Quality);   
    }

    public void ExoticBeans()
    {
        MoneyHandler.ChangeMoney(-25);
        
        GotoPull(BeanLevel.Exotic);
    }

    void GotoPull(BeanLevel level)
    {
        gameObject.SetActive(false);
        gachaPull.gameObject.SetActive(true);

        gachaPull.Refresh(level);
    }
}

public enum BeanLevel
{
    Cheap, Quality, Exotic
}