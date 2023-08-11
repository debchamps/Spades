 using UnityEngine;
 using UnityEditor;
 
 public static class DebugMenu
 {
     [MenuItem("Debug/Print Global Position")]
     public static void PrintGlobalPosition()
     {
          Camera camera = GameObject.Find ("CameraMain").GetComponent<Camera>();

         if (Selection.activeGameObject != null)
         {
             Debug.Log(Selection.activeGameObject.name + " is at " + Selection.activeGameObject.transform.position+ " and " + Selection.activeGameObject.name + " world is at " + camera.ScreenToWorldPoint(Selection.activeGameObject.transform.position));

             
         }
     }
 }
