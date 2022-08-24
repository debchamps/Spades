using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UnityHelper {


    public static Rect Get_Rect(GameObject gameObject)
    {   
	if (gameObject != null)
	{
		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

		if (rectTransform != null)
		{
			return rectTransform.rect;
		}
	}
	else
	{
		Debug.Log("Game object is null.");
	}

	return new Rect();
}


public static float Get_Width(Component component)
{
	if (component != null)
	{
		return Get_Width(component.gameObject);
	}

	return 0;
}
public static float Get_Width(GameObject gameObject)
{
	if (gameObject != null)
	{
		var rect = Get_Rect(gameObject);
		if (rect != null)
		{
			return rect.width;
		}
	}

	return 0;
}


public static float Get_Height(Component component)
{
	if (component != null)
	{
		return Get_Height(component.gameObject);
	}

	return 0;
}
public static float Get_Height(GameObject gameObject)
{
	if (gameObject != null)
	{
		var rect = Get_Rect(gameObject);
		if (rect != null)
		{
			return rect.height;
		}
	}

	return 0;
}
}