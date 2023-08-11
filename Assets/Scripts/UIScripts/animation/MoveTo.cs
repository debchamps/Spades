using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    // Start is called before the first frame update


    public void moveTo(Vector3 location, float seconds) {
          StartCoroutine (MoveOverSeconds (gameObject, location, seconds));
    }

    public void moveTo(GameObject objectToMove, Vector3 location, float seconds) {
          StartCoroutine (MoveOverSeconds (objectToMove, location, seconds));
    }

    public static IEnumerator MoveOverSeconds (GameObject objectToMove, Vector3 end, float seconds)
    {
     float elapsedTime = 0;
     Vector3 startingPos = objectToMove.transform.position;
     while (elapsedTime < seconds)
     {
             objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
             elapsedTime += Time.deltaTime;
             yield return new WaitForEndOfFrame();
         }
         objectToMove.transform.position = end;
     }


public static IEnumerator moveToPosition(Transform transform, Vector3 position, float timeToMove)
   {
      var currentPos = transform.position;
      var t = 0f;
       while(t < 1)
       {
             t += Time.deltaTime / timeToMove;
             transform.position = Vector3.Lerp(currentPos, position, t);
             yield return null;
      }
    }

}
