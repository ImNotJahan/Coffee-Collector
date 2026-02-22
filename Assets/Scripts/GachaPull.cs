using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GachaPull : MonoBehaviour
{
    [SerializeField] RectTransform  boxClosed;
    [SerializeField] GameObject boxOpen;

    [SerializeField] Image beans;

    [SerializeField] Image      stars;
    [SerializeField] GameObject statsScreen;
    [SerializeField] TMP_Text   statsText;

    BeanEntry pulledBean;

    enum State { Opening, Open, Stats }
    State state;

    int clicksUntilOpen;

    float beansUpProgress = 0f;

    [SerializeField] int   beansMax    = 100;
    [SerializeField] int   beansMin    = -10;
    [SerializeField] float beansUpMult = 0.5f;

    BeanLevel level;

    void Start()
    {
        Refresh(BeanLevel.Cheap);
    }

    public void Refresh(BeanLevel level)
    {
        state = State.Opening;

        boxClosed.gameObject.SetActive(true);
        boxOpen.SetActive(false);

        boxClosed.offsetMax = new();
        boxClosed.offsetMin = new();

        clicksUntilOpen = Random.Range(7, 15);

        this.level = level;

        statsScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Opening)
        {
            if(!Mouse.current.leftButton.wasPressedThisFrame) return;

            clicksUntilOpen--;

            static float rng() => Random.Range(0, 30);

            boxClosed.offsetMax = new(rng(), rng());
            boxClosed.offsetMin = new(rng(), rng());

            if(clicksUntilOpen == 0) PullBeans();
        }
        else if(state == State.Open)
        {
            beansUpProgress += Time.deltaTime * beansUpMult;

            if(beansUpProgress > 1) beansUpProgress = 1;

            beans.rectTransform.anchoredPosition = new(0, beansMin + (beansMax - beansMin) * beansUpProgress);

            if(beansUpProgress == 1) ShowStatsMenu();
        }
    }

    void ShowStatsMenu()
    {
        state = State.Stats;

        statsScreen.SetActive(true);

        int starCount = pulledBean.rarity switch
        {
            Rarity.Off        => 1,
            Rarity.Standard   => 2,
            Rarity.Exchange   => 3,
            Rarity.Premium    => 4,
            Rarity.Speciality => 5,
            _ => throw new System.Exception("Invalid rarity")
        };

        stars.fillAmount = starCount / 5f;

        statsText.text = pulledBean.beansName;
    }

    void PullBeans()
    {
        boxClosed.gameObject.SetActive(false);
        boxOpen.SetActive(true);

        state = State.Open;

        // pick beans & set image
        pulledBean   = BeanHandler.PullBean(level);
        beans.sprite = pulledBean.beanImage;

        // draw upwards ani
        beansUpProgress = 0f;

        beans.rectTransform.anchoredPosition = new(0, beansMin);
    }
}
