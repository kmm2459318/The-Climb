using System.Collections;
#if UNITY_EDITOR
using UnityEditor.ShortcutManagement;
#endif
using UnityEngine;
using UnityEngine.Playables;
using Zenject.SpaceFighter;

public class StalkerHome : MonoBehaviour
{
    [SerializeField] GameObject stalkerPrefab;
    BuddyController buddy;

    public bool spawned = false;      //スポーン済か判定
    private bool showing = false;     //画面内にあるか判定
    private bool called = false;      //ストーカーハンド生成関数呼び出したか判定
    private GameObject myStalker;     //生み出す敵


    void Start()
    {
        if (GameObject.FindWithTag("Buddy") != null)
        {
            buddy = GameObject.FindWithTag("Buddy").GetComponent<BuddyController>();
        }
    }

    void Update()
    {
        //ストーカーハンド、スポーン
        if (!called && showing)
        {
            StartCoroutine(StalkerSpawn());
        }

        if (myStalker == null && !buddy.beingKidnapped && spawned)
        {
            spawned = false;
            called = false;
        }
    }

    private IEnumerator StalkerSpawn()
    {
        called = true;

        yield return new WaitForSeconds(1.5f);

        myStalker = Instantiate(stalkerPrefab, transform);
        spawned = true;
        //myStalker.transform.SetParent(transform);
    }

    private void OnBecameVisible()
    {
        showing = true;
    }

    private void OnBecameInvisible()
    {
        showing = false;
    }
}
