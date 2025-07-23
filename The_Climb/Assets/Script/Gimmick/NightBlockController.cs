using UnityEngine;

public class NightBlockController : MonoBehaviour
{
    TimeManager TimeManager;


    [System.Serializable]
    public class DayNightBlock
    {
        public GameObject DayTimeBlock;
        public GameObject NightBlock;
    }
    [SerializeField] private DayNightBlock[] BlockPairs;

    bool LastIsNight;

    void Start()
    {
        TimeManager = FindObjectOfType<TimeManager>();

        if (TimeManager == null)
        {
            Debug.LogError("TimeManager‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
            return;
        }

        LastIsNight = !TimeManager.IsNightProperty;
        UpdateBlocks();
    }

    void Update()
    {
        if (TimeManager == null) return;

        if (TimeManager.IsNightProperty != LastIsNight)
        {
            UpdateBlocks();
            LastIsNight = TimeManager.IsNightProperty;
        }
    }

    void UpdateBlocks()
    {
        foreach (var pair in BlockPairs)
        {
            if (pair.DayTimeBlock != null)
            {
                pair.DayTimeBlock.SetActive(!TimeManager.IsNightProperty); // ’‹‚Ì‚Ý•\Ž¦
            }

            if (pair.NightBlock != null)
            {
                pair.NightBlock.SetActive(TimeManager.IsNightProperty);    // –é‚Ì‚Ý•\Ž¦
            }
        }
    }
}
