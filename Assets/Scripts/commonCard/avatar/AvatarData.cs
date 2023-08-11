using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarData : MonoBehaviour
{

    public PlayerPosition selected;

    public bool isSelected = false;

    // Start is called before the first frame update
    
    public void set(PlayerPosition pos) {
        isSelected = true;
        this.selected = pos;
    }


    public void unset() {
        isSelected = false;
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
