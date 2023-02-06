namespace System.Collections.Generic;

/// <summary>
/// Extensiones de colecciones
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Elimina los duplicados de la lista
    /// </summary>
    /// <param name="inputList"></param>
    /// <returns></returns>
    public static List<T> RemoveDuplicates<T>(this List<T> inputList)
    {
        Dictionary<T, int> uniqueStore = new();
        List<T> finalList = new();

        if (inputList != null)
        {
            foreach (T currValue in inputList)
            {
                if (currValue != null && !uniqueStore.ContainsKey(currValue))
                {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
        }

        return finalList;
    }

    /// <summary>
    /// Obtiene los diferentes valores a través de una función selectora
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static List<TResult> SelectDistinctValues<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source?.Select(selector)?.Distinct()?.ToList();
    }

    /// <summary>
    /// Obtiene los diferentes valores a través de una función selectora
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<TSource> SelectDistinctValues<TSource>(this IEnumerable<TSource> source)
    {
        return source?.Distinct()?.ToList();
    }

    #region IList optimized
    /// <summary>
    /// Implementa la función Find para los IList.
    /// Si la lista es de tipo List, usa el método Find correspondiente, si no, se hace una búsqueda secuencial a través del predicado indicado
    /// </summary>
    /// <typeparam name="TObject">Tipo de objeto listado</typeparam>
    /// <param name="list">Lista de objetos TObject, implementando IList</param>
    /// <param name="function">Predicado de comparación de elementos</param>
    /// <returns>El primer objeto que cumple el predicado. Si no hay ninguno, el objeto por defecto</returns>
    public static TObject Find<TObject>(this IList<TObject> list, Predicate<TObject> function)
    {
        if (list == null) return default;

        TObject returnValue = default;

        if (list is List<TObject> listObject)
        {
            returnValue = listObject.Find(function);
            return returnValue;
        }

        foreach (TObject currentItem in list)
        {
            if (function.Invoke(currentItem))
            {
                returnValue = currentItem;
                break;
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Implementa la función FindAll para los IList.
    /// Si la lista es de tipo List, usa el método FindAll correspondiente, si no, se hace una búsqueda secuencial a través del predicado indicado
    /// </summary>
    /// <typeparam name="TObject">Tipo de objeto listado</typeparam>
    /// <param name="list">Lista de objetos TObject, implementando IList</param>
    /// <param name="function">Predicado de comparación de elementos</param>
    /// <returns>El listado de objetos que cumplen el predicado. Si no hay ninguno, el objeto por defecto</returns>
    public static IList<TObject> FindAll<TObject>(this IList<TObject> list, Predicate<TObject> function)
    {
        List<TObject> returnValue = new();

        if (list == null) return returnValue;

        if (list is List<TObject> listObject)
        {
            returnValue = listObject.FindAll(function);
            return returnValue;
        }

        foreach (TObject currentItem in list)
        {
            if (function.Invoke(currentItem) && !returnValue.Contains(currentItem))
            {
                returnValue.Add(currentItem);
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Devuelve el primer elemento de una lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Si el argumento es null</exception>
    /// <exception cref="InvalidOperationException">Si la secuencia no contiene elementos</exception>
    public static T First<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (list.Count <= 0) throw new InvalidOperationException();
        return list[0];
    }

    /// <summary>
    /// Devuelve el primer elemento de una lista o su valor por defecto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="defaultValue">Valor por defecto</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Si el argumento es null</exception>
    public static T FirstOrDefault<T>(this IList<T> list, T defaultValue = default)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (list.Count <= 0) return defaultValue;
        return list[0];
    }

    /// <summary>
    /// Devuelve el último elemento de una lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Si el argumento es null</exception>
    /// <exception cref="InvalidOperationException">Si la secuencia no contiene elementos</exception>
    public static T Last<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (list.Count <= 0) throw new InvalidOperationException();
        return list[list.Count - 1];
    }

    /// <summary>
    /// Devuelve el último elemento de una lista o su valor por defecto
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="defaultValue">Valor por defecto</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Si el argumento es null</exception>
    public static T LastOrDefault<T>(this IList<T> list, T defaultValue = default)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (list.Count <= 0) return defaultValue;
        return list[list.Count - 1];
    }

    /// <summary>
    /// Reemplaza Where por FindAll cuando se realiza una búsqueda en una lista
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IList<T> Where<T>(this IList<T> list, Predicate<T> predicate)
    {
        return list.FindAll<T>(predicate);
    }
    #endregion

    /// <summary>
    /// Añadir o/y cambiar valor de un Dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Y"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<T, Y> AddOrUpdate<T, Y>(this Dictionary<T, Y> dic, T key, Y value)
    {
        if (dic.ContainsKey(key))
            dic[key] = value;
        else
            dic.Add(key, value);
        return dic;
    }

    /// <summary>
    /// Method AddRange for HashSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static HashSet<T> AddRange<T>(this HashSet<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }

        return collection;
    }

    /// <summary>
    /// Permite sustituir una lista interminable de OR's por el método IN de forma similar a, por ejemplo, SQL
    /// if( reallyLongStringVariableName == "string1" || 
    ///     reallyLongStringVariableName == "string2" || 
    ///     reallyLongStringVariableName == "string3")
    ///     se puede sustituir por:
    /// if(reallyLongStringVariableName.In("string1","string2","string3"))
    /// </summary>
    /// <typeparam name="T">Tipo de la variable que se va a usar en la comparación</typeparam>
    /// <param name="source">variable "fuente"</param>
    /// <param name="compareValues">Listado de valores a comparar con la fuente</param>
    /// <returns></returns>
    public static bool In<T>(this T source, params T[] compareValues)
    {
        if (null == source) return false;

        // pero como no es así...
        List<T> list = new(compareValues);
        return list.Contains(source);
    }

    /// <summary>
    /// Permite sustituir una lista interminable de OR's por el método IN de forma similar a, por ejemplo, SQL
    /// if( reallyLongStringVariableName == "string1" || 
    ///     reallyLongStringVariableName == "string2" || 
    ///     reallyLongStringVariableName == "string3")
    ///     se puede sustituir por:
    /// if(reallyLongStringVariableName.In("string1","string2","string3"))
    /// </summary>
    /// <typeparam name="T">Tipo de la variable que se va a usar en la comparación</typeparam>
    /// <param name="source">variable "fuente"</param>
    /// <param name="compareValues">Listado de valores a comparar con la fuente</param>
    /// <returns></returns>
    public static bool In<T>(this T source, IList<T> compareValues)
    {
        if (null == source) return false;

        return compareValues.Contains(source);
    }

    /// <summary>
    /// Checks if a variable belongs to an interval, endpoints included.
    /// I.e. actual IN [lower, upper] so 5 IN [5, 5] is true
    /// </summary>
    /// <typeparam name="T">A comparable type</typeparam>
    /// <param name="actual">Instance of comparable type</param>
    /// <param name="lower">Lower value to compare if actual value is greater or equal</param>
    /// <param name="upper">Upper value to compare if actual value is lesser or equal</param>
    /// <returns>True if the value actual is BETWEEN lower AND upper both sides included</returns>
    public static bool IsBetween<T>(this T actual, T lower, T upper) where T : IComparable<T>
    {
        return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
    }
}
