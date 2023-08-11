using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarCartoonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set() {

    }

    public void unset() {

    }

    public void open() {
        this.gameObject.transform.parent.GetComponent<PlayerProfileScript>().setAvatar(this.gameObject);
    }


}
