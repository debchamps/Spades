using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Legacy coroutine-based movement helper.
/// WaitForEndOfFrame is cached (one allocation for the app lifetime).
/// SmoothStep replaces linear lerp for natural ease-in/ease-out motion.
/// </summary>
public class MoveTo : MonoBehaviour
{
    private static readonly WaitForEndOfFrame _waitEndOfFrame = new WaitForEndOfFrame();

    public void moveTo(Vector3 location, float seconds)
    {
        StartCoroutine(MoveOverSeconds(gameObject, location, seconds));
    }

    public void moveTo(GameObject objectToMove, Vector3 location, float seconds)
    {
        StartCoroutine(MoveOverSeconds(objectToMove, location, seconds));
    }

    public static IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float   elapsedTime = 0f;
        Vector3 startingPos = objectToMove.transform.position;

        while (elapsedTime < seconds)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / seconds);
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, t);
            elapsedTime += Time.deltaTime;
            yield return _waitEndOfFrame;
        }
        objectToMove.transform.position = end;
    }

    public static IEnumerator moveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        Vector3 currentPos = transform.position;
        float   t          = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = position;
    }
}
