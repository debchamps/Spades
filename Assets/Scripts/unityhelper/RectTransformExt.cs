using UnityEngine;
 using System.Collections;
 
 static public class RectTransformExt {
     /// <summary>
     /// Converts RectTransform.rect's local coordinates to world space
     /// Usage example RectTransformExt.GetWorldRect(myRect, Vector2.one);
     /// </summary>
     /// <returns>The world rect.</returns>
     /// <param name="rt">RectangleTransform we want to convert to world coordinates.</param>
     /// <param name="scale">Optional scale pulled from the CanvasScaler. Default to using Vector2.one.</param>
     static public Rect GetWorldRect (RectTransform rt, Vector2 scale) {
         // Convert the rectangle to world corners and grab the top left
         Vector3[] corners = new Vector3[4];
         rt.GetWorldCorners(corners);
         Vector3 topLeft = corners[0];
 
         // Rescale the size appropriately based on the current Canvas scale
         Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
 
         return new Rect(topLeft, scaledSize);
     }

     public static Rect GetWorldRect(this RectTransform rectTransform)
     {
         Vector3[] corners = new Vector3[4];
         rectTransform.GetWorldCorners(corners);
         // Get the bottom left corner.
         Vector3 position = corners[0];
         
         Vector2 size = new Vector2(
             rectTransform.lossyScale.x * rectTransform.rect.size.x,
             rectTransform.lossyScale.y * rectTransform.rect.size.y);
 
         return new Rect(position, size);
     }
 }
 