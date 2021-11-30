﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CommonNetCoreFuncs.Tools
{
    public static class ObjectHelpers
    {
        /// <summary>
        /// Copy properties of the same name from one object to another
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(TU).GetProperties().Where(x => x.CanWrite).ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.FirstOrDefault(x => x.Name == sourceProp.Name);
                    if (p != null)
                    {
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
        }

        /// <summary>
        /// Set values in an IEnumerable as an extension of linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="updateMethod"></param>
        /// <returns>IEnumerable with values updated according to updateMethod</returns>
        public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T> updateMethod)
        {
            foreach (T item in items)
            {
                updateMethod(item);
            }
            return items;
        }

        /// <summary>
        /// Removes excess spaces in string properties inside of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void TrimObjectStrings<T>(this T obj)
        {
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    string value = (string)prop.GetValue(obj) ?? string.Empty;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = Regex.Replace(value.Trim(), @"\s+", " "); //Replaces any multiples of spacing with a single space
                        prop.SetValue(obj, value.Trim());
                    }
                }
            }
        }

        /// <summary>
        /// Clone one list into another without a reference linking the two
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Clone<T>(this List<T> list)
        {
            string serialized = JsonConvert.SerializeObject(list);
            return JsonConvert.DeserializeObject<List<T>>(serialized);
        }
    }
}