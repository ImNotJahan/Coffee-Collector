using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class BeanHandler : MonoBehaviour
{
    static BeanHandler instance;

    [SerializeField] BeanEntry[] entries;

    BeanEntry selectedBean;

    readonly List<BeanEntry> ownedBeans = new();

    void Awake()
    {
        instance = this;

        // give player off beans to start out
        List<BeanEntry> candidates = new();

        foreach(BeanEntry entry in instance.entries)
        {
            if(entry.rarity == Rarity.Off) candidates.Add(entry);
        }
        
        instance.ownedBeans.Add(RNGHelper.RandomElementByWeight(candidates, (x) => (int) x.rarity));

        selectedBean = ownedBeans[0];
    }

    public static BeanEntry PullBean(BeanLevel level)
    {
        List<BeanEntry> candidates = new();

        foreach(BeanEntry entry in instance.entries)
        {
            switch (level)
            {
                case BeanLevel.Cheap:
                    if(entry.rarity == Rarity.Off || entry.rarity == Rarity.Standard || entry.rarity == Rarity.Exchange) candidates.Add(entry);
                    break;
                
                case BeanLevel.Quality:
                    if(entry.rarity == Rarity.Standard || entry.rarity == Rarity.Exchange || entry.rarity == Rarity.Premium) candidates.Add(entry);
                    break;
                
                case BeanLevel.Exotic:
                    if(entry.rarity == Rarity.Exchange || entry.rarity == Rarity.Premium || entry.rarity == Rarity.Speciality) candidates.Add(entry);
                    break;
            }
        }
        
        BeanEntry selected = RNGHelper.RandomElementByWeight(candidates, (x) => (int) x.rarity);

        if(!instance.ownedBeans.Contains(selected)) instance.ownedBeans.Add(selected);

        return selected;
    }

    public static BeanEntry GetSelectedBean() => instance.selectedBean;

    public static List<BeanEntry> GetOwnedBeans() => instance.ownedBeans;

    public static void SetSelectedBean(BeanEntry entry) => instance.selectedBean = entry;

    public static int GetTotalNumBeans() => instance.entries.Length;
}

public static class RNGHelper
{
    // https://stackoverflow.com/a/11930875
    public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector) {
        float totalWeight = sequence.Sum(weightSelector);
        // The weight we are after...
        float itemWeightIndex =  (float) new System.Random().NextDouble() * totalWeight;
        float currentWeightIndex = 0;

        foreach(var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) }) {
            currentWeightIndex += item.Weight;
            
            // If we've hit or passed the weight we are after for this item then it's the one we want....
            if(currentWeightIndex >= itemWeightIndex)
                return item.Value;
            
        }
        
        return default;
    }
}

public enum Rarity
{
    Off        = 9,
    Standard   = 10,
    Exchange   = 5,
    Premium    = 3,
    Speciality = 1
}

[Serializable]
public struct BeanEntry
{
    public Sprite beanImage;
    public Rarity rarity;
    public string beansName;
    public float  strength;
    public string description;
}