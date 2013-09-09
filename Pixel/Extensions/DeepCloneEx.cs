using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pixel.Extensions {
  // http://stackoverflow.com/questions/8025890/is-there-a-much-better-way-to-create-deep-and-shallow-clones-in-c/8026574#8026574
  public static class DeepCloneEx {
    public static T DeepClone<T>(this T original) {
      return original.DeepClone(new Dictionary<object, object>());
    }

    private static T DeepClone<T>(this T original, IDictionary<object, object> copies) {
      T result;
      var t = original.GetType();

      object tmpResult;
      if (copies.TryGetValue(original, out tmpResult)) {
        return (T)tmpResult;
      }

      if (!t.IsArray) {
        result = (T)Activator.CreateInstance(t);
        copies.Add(original, result);

        foreach (
          var field in
            t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy |
                        BindingFlags.Instance)) {
          var fieldValue = field.GetValue(original);
          var fieldType = field.FieldType;

          if (!fieldType.IsValueType && fieldType != typeof(string)) {
            fieldValue = fieldValue.DeepClone(copies);
          }

          field.SetValue(result, fieldValue);
        }
      } else {
        var originalArray = (Array)(object)original;
        var resultArray = (Array)originalArray.Clone();

        if (!t.GetElementType().IsValueType) {
          var lengths = new int[t.GetArrayRank()];
          var indicies = new int[lengths.Length];

          for (var i = 0; i < lengths.Length; i++) {
            lengths[i] = resultArray.GetLength(i);
          }

          var p = lengths.Length - 1;
          while (Increment(indicies, lengths, p)) {
            var value = resultArray.GetValue(indicies);
            if (value != null) {
              resultArray.SetValue(value.DeepClone(copies), indicies);
            }
          }
        }
        result = (T)(object)resultArray;
      }
      return result;
    }

    private static bool Increment(IList<int> indicies, IList<int> lengths, int p) {
      if (p <= -1) return false;
      indicies[p]++;
      if (indicies[p] < lengths[p]) return true;
      if (!Increment(indicies, lengths, p - 1)) return false;
      indicies[p] = 0;
      return true;
    }
  }
}