using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class CoffeeSelection : MonoBehaviour
{
    [SerializeField] TMP_Text numCompletedText;
    
    [SerializeField] Image    beansImage;
    [SerializeField] TMP_Text beansDesc;
    [SerializeField] Image    stars;

    List<BeanEntry> ownedBeans;
    int             selectedIndex;

    void OnEnable()
    {
        ownedBeans    = BeanHandler.GetOwnedBeans();
        selectedIndex = ownedBeans.IndexOf(BeanHandler.GetSelectedBean());

        numCompletedText.text = $"{ownedBeans.Count}/{BeanHandler.GetTotalNumBeans()} beans collected";

        UpdateBean();
    }
    
    public void LeftPressed()
    {
        selectedIndex--;

        if(selectedIndex < 0) selectedIndex = ownedBeans.Count - 1;

        UpdateBean();
    }

    public void RightPressed()
    {
        selectedIndex++;

        if(selectedIndex >= ownedBeans.Count) selectedIndex = 0;

        UpdateBean();
    }

    public void Select()
    {
        BeanHandler.SetSelectedBean(ownedBeans[selectedIndex]);
    }

    private void UpdateBean()
    {
        BeanEntry bean = ownedBeans[selectedIndex];

        int starCount = bean.rarity switch
        {
            Rarity.Off        => 1,
            Rarity.Standard   => 2,
            Rarity.Exchange   => 3,
            Rarity.Premium    => 4,
            Rarity.Speciality => 5,
            _ => throw new System.Exception("Invalid rarity")
        };

        beansImage.sprite = bean.beanImage;
        stars.fillAmount  = starCount / 5f;
        beansDesc.text    = $"{bean.beansName}\n{bean.strength}/5 strength\n<size=50%>{bean.description}</size>";
    }
}
