using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayFunctions {
    public static void AppendArrayToList<T>(T[] array, List<T> list) {
        foreach (T arrayItem in array) {
            list.Add(arrayItem);
        }
    }

    public static void AppendArrayToArray<T>(T[] appendingArray, T[] appendedArray) {
        T[] initalArray = appendedArray;
        appendedArray = new T[appendedArray.Length + initalArray.Length];
        for (int i = 0; i < initalArray.Length; i++) {
            appendedArray[i] = initalArray[i];
        }
        for (int i = 0; i < appendingArray.Length; i++) {
            appendedArray[i + initalArray.Length] = appendingArray[i];
        }
    }
}
