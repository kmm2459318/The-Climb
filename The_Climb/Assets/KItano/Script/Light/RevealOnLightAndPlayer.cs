//using UnityEngine;
//using System.Collections;

//public class RevealOnLightAndPlayer : MonoBehaviour
//{
//    [Header("ライト判定")]
//    [SerializeField] private Color purpleColor = new Color(0.5f, 0f, 1f);
//    [SerializeField] private float colorThreshold = 0.2f;

//    [Header("時間")]
//    [SerializeField] private float activationTime = 3f;
//    [SerializeField] private float stayVisibleTime = 2f;
//    [SerializeField] private float fadeDuration = 1f;

//    [Header("物理")]
//    [SerializeField] private Collider solidCollider;

//    private Renderer rend;

//    private bool playerInside;
//    private bool litByPurple;
//    private bool activated;

//    private float timer;

//    // ----------------------------------------------------
//    private void OnEnable()
//    {
//        Debug.Log("Reveal：Enable");

//        LightController.OnLightEnter += OnLightEnter;
//        LightController.OnLightExit += OnLightExit;
//    }

//    private void OnDisable()
//    {
//        Debug.Log("Reveal：Disable");

//        LightController.OnLightEnter -= OnLightEnter;
//        LightController.OnLightExit -= OnLightExit;
//    }

//    private void Start()
//    {
//        rend = GetComponent<Renderer>();

//        SetVisible(false);
//        SetCollider(false);

//        Debug.Log("Reveal：Initialized");
//    }

//    // ----------------------------------------------------
//    private void Update()
//    {
//        if (activated) return;

//        if (litByPurple && playerInside)
//        {
//            timer += Time.deltaTime;

//            if (timer >= activationTime)
//            {
//                StartCoroutine(ActivationRoutine());
//            }
//        }
//        else
//        {
//            timer = 0f;
//        }
//    }

//    // ----------------------------------------------------
//    private void OnLightEnter(GameObject hitObj, Color color)
//    {
//        if (hitObj != gameObject) return;

//        litByPurple = IsPurple(color);
//        timer = 0f;

//        Debug.Log($"Reveal：LightEnter 紫={litByPurple}");
//    }

//    private void OnLightExit(GameObject hitObj, Color color)
//    {
//        if (hitObj != gameObject) return;

//        litByPurple = false;
//        timer = 0f;

//        if (!activated)
//            SetVisible(false);

//        Debug.Log("Reveal：LightExit");
//    }

//    // ----------------------------------------------------
//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        playerInside = true;
//        Debug.Log("Reveal：PlayerEnter");
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (!other.CompareTag("Player")) return;

//        playerInside = false;
//        Debug.Log("Reveal：PlayerExit");
//    }

//    // ----------------------------------------------------
//    private IEnumerator ActivationRoutine()
//    {
//        activated = true;
//        timer = 0f;

//        Debug.Log("Reveal：Activate");

//        SetCollider(true);
//        yield return Fade(Color.cyan, 1f);

//        yield return new WaitForSeconds(stayVisibleTime);

//        yield return Fade(Color.white, 0f);
//        SetCollider(false);

//        activated = false;

//        Debug.Log("Reveal：Deactivate");
//    }

//    // ----------------------------------------------------
//    private IEnumerator Fade(Color color, float alpha)
//    {
//        Color start = rend.material.color;
//        Color end = color;
//        end.a = alpha;

//        float t = 0f;
//        while (t < fadeDuration)
//        {
//            t += Time.deltaTime;
//            rend.material.color = Color.Lerp(start, end, t / fadeDuration);
//            yield return null;
//        }
//    }

//    // ----------------------------------------------------
//    private void SetVisible(bool visible)
//    {
//        Color c = rend.material.color;
//        c.a = visible ? 1f : 0f;
//        rend.material.color = c;
//    }

//    private void SetCollider(bool enable)
//    {
//        if (solidCollider)
//            solidCollider.enabled = enable;
//    }

//    private bool IsPurple(Color c)
//    {
//        return Vector3.Distance(
//            new Vector3(c.r, c.g, c.b),
//            new Vector3(purpleColor.r, purpleColor.g, purpleColor.b)
//        ) < colorThreshold;
//    }
//}
