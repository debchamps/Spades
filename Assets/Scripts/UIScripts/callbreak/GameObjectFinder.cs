using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cached GameObject lookup utility.
/// Eliminates repeated GameObject.Find() and GetComponent() calls.
/// Call InvalidateCardCache() after each new deal so the card→id map is rebuilt.
/// </summary>
public class GameObjectFinder : MonoBehaviour
{
    private static List<GameObject> _cachedCardObjects;
    private static Dictionary<string, GameObject> _cardById = new Dictionary<string, GameObject>();
    private static Dictionary<PlayerPosition, GameObject> _scoreCache = new Dictionary<PlayerPosition, GameObject>();
    private static List<GameObject> _avatarCache;

    public static List<GameObject> getCardObjects()
    {
        if (_cachedCardObjects == null)
        {
            _cachedCardObjects = new List<GameObject>(13);
            for (int i = 1; i <= 13; i++)
            {
                GameObject go = GameObject.Find("testCard" + i.ToString());
                if (go != null) _cachedCardObjects.Add(go);
            }
        }
        return _cachedCardObjects;
    }

    public static GameObject findCardGameObject(Card card)
    {
        if (card == null) return null;
        string targetId = card.getCardId();

        if (_cardById.TryGetValue(targetId, out GameObject cached) && cached != null)
            return cached;

        _cardById.Clear();
        foreach (GameObject go in getCardObjects())
        {
            if (go == null) continue;
            CardScript cs = go.GetComponent<CardScript>();
            if (cs != null && cs.card != null)
                _cardById[cs.card.getCardId()] = go;
        }

        return _cardById.TryGetValue(targetId, out GameObject result) ? result : null;
    }

    public static void InvalidateCardCache()
    {
        _cachedCardObjects = null;
        _cardById.Clear();
    }

    public static List<GameObject> getBiddingButtons()
    {
        List<GameObject> gameObjects = new List<GameObject>(14);
        for (int i = 1; i <= 14; i++)
            gameObjects.Add(GameObject.Find("spadeBidButton" + i.ToString()));
        return gameObjects;
    }

    public static GameObject getPlayerScoreObject(PlayerPosition pos)
    {
        if (!_scoreCache.TryGetValue(pos, out GameObject go) || go == null)
        {
            go = GameObject.Find("player" + PlayerPositionHelper.getName(pos).ToLower() + "score");
            if (go != null) _scoreCache[pos] = go;
        }
        return go;
    }

    public static void InvalidateScoreCache() => _scoreCache.Clear();

    public static List<GameObject> playerAvatar()
    {
        if (_avatarCache == null)
        {
            _avatarCache = new List<GameObject>(PlayerPositionHelper.PLAYER_POSITIONS.Length);
            foreach (PlayerPosition pos in PlayerPositionHelper.PLAYER_POSITIONS)
                _avatarCache.Add(GameObject.Find("player" + PlayerPositionHelper.getName(pos).ToLower() + "avatar"));
        }
        return _avatarCache;
    }
}
