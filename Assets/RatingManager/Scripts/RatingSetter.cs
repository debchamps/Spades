using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RatingSetter : MonoBehaviour , IPointerClickHandler{
    public int star;

    public void unsetAll() {
        for(int i=1;i<=5;i++) {
                getStarObject(i).GetComponent<RatingSetter>().unset();
        }

    }

    public void set() {

        unsetAll();

        List<GameObject> stars = getSingleStars();
        foreach(GameObject starObj in stars) {
            Debug.Log("In changing colors to yellow");
            starObj.GetComponent<Image>().color =  new Color32(0,240,0,255); 
        }


        GameObject.Find("ratingparent").GetComponent<RatingManager>().setRating(star);
        //GameObject.Find("ScriptEmpty").GetComponent<RatingManager>().submit();

    }

    public void unset() {
        List<GameObject> stars = getSingleStars();
        foreach(GameObject starObj in stars) {
            starObj.GetComponent<Image>().color =  new Color32(166,154,154,255); 
        }

    }



    GameObject getStarObject(int starVal) {
        return GameObject.Find("star" + starVal.ToString());
    }


    List<GameObject> getSingleStars() {
        List<GameObject> singleStars = new List<GameObject>();

        for(int i=1;i<=star;i++) {
            singleStars.Add(getStarObject(star).transform.Find("singlestar" + i.ToString()).gameObject);
        }

        return singleStars;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {

        set();

        // OnClick code goes here ...
    }

    

}