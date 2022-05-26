using System;
using System.Collections.Generic;
using System.Linq;

public static class Extend {
    public static IEnumerable<T> SliceThirdDim<T>(this T[, , ] array, int x, int y) {
        //i.e. dims: row = 0, column = 1
        T[] arr = new T[array.GetUpperBound(2)];
        for (var i = array.GetLowerBound(2); i <= array.GetUpperBound(2); i++) {
            yield return array[x, y, i];
        }
    }
}