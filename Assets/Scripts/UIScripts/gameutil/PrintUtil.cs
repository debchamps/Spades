using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PrintUtil {

    public static string print<X,Y>(Dictionary<X, Y> dict ) {
        string print = "{";
        foreach(X keyX in dict.Keys) {
            print = print + keyX + ":" + dict[keyX] + ",  "; 
        }

        print = print + "}\n";

        return print;
    }
}