using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PrefabScript : MonoBehaviour
{

    public UnityEvent m_MyEvent = new UnityEvent();
    public int gameId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void test() {
        Debug.Log("Test Prefab success");
            //GamePlay.testPrefab();

    }
}
