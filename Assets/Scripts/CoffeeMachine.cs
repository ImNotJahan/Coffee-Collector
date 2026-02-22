using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class CoffeeMachine : MonoBehaviour
{
    [SerializeField] Image press;
    [SerializeField] Image grounds;
    [SerializeField] Image water;
    [SerializeField] Image coffee;
    [SerializeField] Image pot;

    [SerializeField] TMP_Text infoText;

    [SerializeField] float idealCoffeeStrength = 0.5f;
    [SerializeField] float idealRange = 1f;

    [SerializeField] float groundMultiplier = 1f;
    [SerializeField] float waterMultiplier  = 1f;
    [SerializeField] float pressMultiplier  = 1f;
    [SerializeField] float waitMultiplier  = 1f;

    // i.e. how far up in px the press starts at
    [SerializeField] int pressRange = 250;

    [SerializeField] GameObject qualityScreen;
    [SerializeField] TMP_Text   qualityText;
    [SerializeField] Image      stars;

    [SerializeField] GameObject nextButton;

    float pressPos;

    InputAction action;

    enum Step
    {
        AddGrounds = 0,
        AddWater   = 1,
        Press      = 2,
        Wait       = 3,
        Done       = 4
    }

    Step currentStep = Step.AddGrounds;

    void Start()
    {
        action = InputSystem.actions.FindAction("action");
        
        RefreshMachine();
    }

    void Update()
    {
        if (action.IsPressed())
        {
            Action handler = currentStep switch
            {
                Step.AddGrounds => HandleGrounds,
                Step.AddWater   => HandleWater,
                Step.Press      => HandlePressing,
                _               => HandleWaiting
            };

            handler();
        }
    }

    void HandleGrounds()
    {
        grounds.fillAmount += Time.deltaTime * groundMultiplier;
    }

    void HandleWater()
    {
        water.fillAmount += Time.deltaTime * waterMultiplier;
        coffee.fillAmount = water.fillAmount;
    }

    void HandlePressing()
    {
        pressPos -= Time.deltaTime * pressMultiplier;

        if(pressPos < 0) pressPos = 0;

        UpdatePress();

        if(pressPos == 0)
        {
            NextStep();
        }
    }

    void HandleWaiting()
    {
        coffee.color = new(1, 1, 1, coffee.color.a + Time.deltaTime * waitMultiplier);

        if(coffee.color.a >= 1f)
        {
            NextStep();
        }
    }

    public void NextStep()
    {
        switch (currentStep)
        {
            case Step.AddGrounds:
                currentStep   = Step.AddWater;
                water.enabled = true;
                break;
            
            case Step.AddWater:
                currentStep = Step.Wait;
                nextButton.SetActive(false);
                break;

            case Step.Press:
                currentStep = Step.Done;
                Finish();
                break;
            
            case Step.Wait:
                currentStep   = Step.Press;
                press.enabled = true;
                break;
        }

        UpdateText();
    }

    void Finish()
    {
        BeanEntry entry = BeanHandler.GetSelectedBean();

        // do some calculations for how much money to give...
        int rarityMoneyMult = entry.rarity switch
        {
            Rarity.Off        => 1,
            Rarity.Standard   => 2,
            Rarity.Exchange   => 4,
            Rarity.Premium    => 8,
            Rarity.Speciality => 16,
            _ => throw new Exception("Invalid rarity")
        };

        float coffeeStrength = entry.strength * grounds.fillAmount / (float) Math.Max(water.fillAmount, 0.01);

        float idealDist = Math.Abs(coffeeStrength - idealCoffeeStrength);
        
        float money = (idealRange - idealDist) / idealRange;
        money      *= water.fillAmount * 2;

        if(money < 0.1) money = 0.1f;

        money *= rarityMoneyMult;

        MoneyHandler.ChangeMoney(money);

        // tell how good quality was...
        qualityScreen.SetActive(true);
        qualityText.text = $"+${money:F2}";
        stars.fillAmount = (idealRange - idealDist) / idealRange;
    }

    void UpdateText()
    {
        infoText.text = currentStep switch
        {
            Step.AddGrounds => "Hold space to add grounds",
            Step.AddWater   => "Hold space to add water",
            Step.Press      => "Hold space to press down grounds",
            Step.Wait       => "Hold space to let it brew",
            Step.Done       => "",
            _               => "Invalid step type, no idea how you got here"
        };
    }

    void UpdatePress()
    {
        press.rectTransform.offsetMax = new(0, pressPos);
        press.rectTransform.offsetMin = new(0, pressPos);
    }

    public void RefreshMachine()
    {
        nextButton.SetActive(true);

        currentStep = Step.AddGrounds;

        press.enabled   = false;
        grounds.enabled = true;
        water.enabled   = false;
        coffee.color    = new(255, 255, 255, 0);

        grounds.fillAmount = 0;
        water.fillAmount   = 0;
        coffee.fillAmount  = 0;

        pressPos = pressRange;

        qualityScreen.SetActive(false);

        UpdateText();
        UpdatePress();
    }
}
