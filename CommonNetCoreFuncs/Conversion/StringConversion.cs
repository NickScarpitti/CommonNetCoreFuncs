﻿using System.Security.Cryptography;
using System.Text;
using CommonNetCoreFuncs.Tools;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommonNetCoreFuncs.Conversion;

public enum EYesNo
{
    Yes,
    No
}

/// <summary>
/// Methods for converting various variable types to string and vice versa
/// </summary>
public static class StringConversion
{
    /// <summary>
    /// Converts Nullable DateTime to string using the passed in formatting
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format">Date time format</param>
    /// <returns>Formatted string representation of the passed in nullable DateTime</returns>
    public static string? ToNString(this DateTime? value, string? format = null)
    {
        string? output = null;
        if (value != null)
        {
            DateTime dtActual = (DateTime)value;
            output = dtActual.ToString(format);
        }
        return output;
    }

    /// <summary>
    /// Converts Nullable DateTime to string using the passed in formatting
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format">Timespan format</param>
    /// <returns>Formatted string representation of the passed in nullable Timespan</returns>
    public static string? ToNString(this TimeSpan? value, string? format = null)
    {
        string? output = null;
        if (value != null)
        {
            TimeSpan tsActual = (TimeSpan)value;
            output = tsActual.ToString(format);
        }
        return output;
    }

    /// <summary>
    /// Converts nullable int to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String representation of the passed in nullable int</returns>
    public static string? ToNString(this int? value)
    {
        string? output = null;
        if (value != null)
        {
            output = value.ToString();
        }
        return output;
    }

    /// <summary>
    /// Converts nullable long to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String representation of the passed in nullable long</returns>
    public static string? ToNString(this long? value)
    {
        string? output = null;
        if (value != null)
        {
            output = value.ToString();
        }
        return output;
    }

    /// <summary>
    /// Converts nullable double to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String representation of the passed in nullable double</returns>
    public static string? ToNString(this double? value)
    {
        string? output = null;
        if (value != null)
        {
            output = value.ToString();
        }
        return output;
    }

    /// <summary>
    /// Converts nullable decimal to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String representation of the passed in nullable decimal</returns>
    public static string? ToNString(this decimal? value)
    {
        string? output = null;
        if (value != null)
        {
            output = value.ToString();
        }
        return output;
    }

    /// <summary>
    /// Converts nullable object to string
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String representation of the passed in nullable object</returns>
    public static string? ToNString(this object? value)
    {
        string? output = null;
        if (value != null)
        {
            output = value.ToString();
        }
        return output;
    }

    /// <summary>
    /// Converts value to select list item
    /// </summary>
    /// <param name="value"></param>
    /// <returns>SelectListItem with text and value properties set to the passed in value</returns>
    public static SelectListItem? ToSelectListItem(this string? value, bool selected)
    {
        return value != null ? new SelectListItem { Value= value , Text = value, Selected = selected} : null;
    }

    /// <summary>
    /// Converts value to select list item
    /// </summary>
    /// <param name="value"></param>
    /// <returns>SelectListItem with text and value properties set to the passed in value</returns>
    public static SelectListItem? ToSelectListItem(this string? value)
    {
        return value != null ? new SelectListItem { Value = value, Text = value } : null;
    }

    /// <summary>
    /// Converts value to select list item
    /// </summary>
    /// <param name="value"></param>
    /// <param name="text"></param>
    /// <returns>SelectListItem with text and value properties set to the passed in text and value. Will use value for text if text is null</returns>
    public static SelectListItem? ToSelectListItem(this string? value, string? text, bool selected)
    {
        return value != null && text != null ? new SelectListItem { Value = value, Text = text, Selected = selected } : null;
    }

    /// <summary>
    /// Converts value to select list item
    /// </summary>
    /// <param name="value"></param>
    /// <param name="text"></param>
    /// <returns>SelectListItem with text and value properties set to the passed in text and value. Will use value for text if text is null</returns>
    public static SelectListItem? ToSelectListItem(this string? value, string? text)
    {
        return value != null && text != null ? new SelectListItem { Value = value, Text = text } : null;
    }

    /// <summary>
    /// Converts list of string representations of integers into list of integers
    /// </summary>
    /// <param name="values"></param>
    /// <returns>List of integers where the strings could be parsed to integers and not null</returns>
    public static IEnumerable<int> ToListInt(this IEnumerable<string> values)
    {
        return values.Select(x => { return int.TryParse(x, out int i) ? i : (int?)null; }).Where(i => i.HasValue).Select(i => i!.Value);
    }

    /// <summary>
    /// Converts list of string representations of integers into list of integers
    /// </summary>
    /// <param name="values"></param>
    /// <returns>List of integers where the strings could be parsed to integers and not null</returns>
    public static List<int> ToListInt(this IList<string> values)
    {
        return values.Select(x => { return int.TryParse(x, out int i) ? i : (int?)null; }).Where(i => i.HasValue).Select(i => i!.Value).ToList();
    }

    /// <summary>
    /// Used to reduce boilerplate code for parsing strings into nullable integers
    /// </summary>
    /// <param name="value">String value to be converted to nullable int</param>
    /// <returns>Nullable int parsed from a string</returns>
    public static int? ToNInt(this string? value)
    {
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int i))
        {
            return i;
        }
        return null;
    }

    /// <summary>
    /// Used to reduce boilerplate code for parsing strings into nullable doubles
    /// </summary>
    /// <param name="value">String value to be converted to nullable double</param>
    /// <returns>Nullable double parsed from a string</returns>
    public static double? ToNDouble(this string? value)
    {
        if (!string.IsNullOrEmpty(value) && double.TryParse(value, out double i))
        {
            return i;
        }
        return null;
    }

    /// <summary>
    /// Used to reduce boilerplate code for parsing strings into nullable decimals
    /// </summary>
    /// <param name="value">String value to be converted to nullable decimal</param>
    /// <returns>Nullable decimal parsed from a string</returns>
    public static decimal? ToNDecimal (this string? value)
    {
        if (!string.IsNullOrEmpty(value) && decimal.TryParse(value, out decimal i))
        {
            return i;
        }
        return null;
    }

    /// <summary>
    /// Used to reduce boilerplate code for parsing strings into nullable DateTimes
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Nullable DateTime parsed from a string</returns>
    public static DateTime? ToNDateTime(this string? value)
    {
        DateTime? dtn = null;
        if (DateTime.TryParse(value, out DateTime dt))
        {
            dtn = dt;
        }
        else if (double.TryParse(value, out double dbl))
        {
            dtn = DateTime.FromOADate(dbl);
        }
        return dtn;
    }

    /// <summary>
    /// Convert string "Yes"/"No" value into bool
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Bool representation of string value passed in</returns>
    public static bool YesNoToBool(this string? value)
    {
        return value.StrEq(EYesNo.Yes.ToString());
    }

    /// <summary>
    /// Convert string "Y"/"N" value into bool
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Bool representation of string value passed in</returns>
    public static bool YNToBool(this string? value)
    {
        return value.StrEq("Y");
    }

    /// <summary>
    /// Cleans potential parsing issues out of a query parameter
    /// </summary>
    /// <param name="value"></param>
    /// <returns>String equivalent of value passed in replacing standalone text "null" with null value or removing any new line characters and extra spaces</returns>
    public static string? CleanQueryParam(this string? value)
    {
        return value.MakeNullNull()?.Replace("\n", "").Trim();
    }

    /// <summary>
    /// Cleans potential parsing issues out of a list of query parameters
    /// </summary>
    /// <param name="values"></param>
    /// <returns>List of string equivalents of the values passed in replacing standalone text "null" with null value or removing any new line characters and extra spaces</returns>
    public static IEnumerable<string>? CleanQueryParam(this IEnumerable<string>? values)
    {
        if (values == null)
        {
            return null;
        }

        List<string?> cleanValues = new();
        if (values.Any())
        {
            foreach (string? value in values)
            {
                cleanValues.Add(value.MakeNullNull()?.Replace("\n", "").Trim());
            }
        }

        return (cleanValues ?? new()).Where(x => x != null)!;
    }

    /// <summary>
    /// Cleans potential parsing issues out of a list of query parameters
    /// </summary>
    /// <param name="values"></param>
    /// <returns>List of string equivalents of the values passed in replacing standalone text "null" with null value or removing any new line characters and extra spaces</returns>
    public static List<string>? CleanQueryParam(this IList<string>? values)
    {
        if (values == null)
        {
            return null;
        }

        List<string?> cleanValues = new();
        if (values.Any())
        {
            foreach (string? value in values)
            {
                cleanValues.Add(value.MakeNullNull()?.Replace("\n", "").Trim());
            }
        }

        return (cleanValues ?? new()).Where(x => x != null).ToList()!;
    }

    /// <summary>
    /// Converts list of query parameters into a query parameter string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters">List of a type that can be converted to string</param>
    /// <param name="queryParameterName">The name to be used in front of the equals sign for the query parameter string</param>
    /// <returns>String representation of the list passed in as query parameters with the name passed in as queryParameterName</returns>
    public static string ListToQueryParameters<T>(this IEnumerable<T>? parameters, string? queryParameterName)
    {
        string queryString = string.Empty;
        bool firstItem = true;
        if (parameters != null && parameters.Any() && !string.IsNullOrWhiteSpace(queryParameterName))
        {
            foreach (T parameter in parameters)
            {
                if (!firstItem)
                {
                    queryString += $"&{queryParameterName}={parameter}";
                }
                else
                {
                    queryString = $"{queryParameterName}={parameter}";
                    firstItem = false;
                }
            }
        }
        return queryString;
    }

    /// <summary>
    /// Get file name safe date in the chosen format
    /// </summary>
    /// <param name="dateFormat"></param>
    /// <returns>File name safe formatted date</returns>
    public static string GetSafeDate(string dateFormat)
    {
        return DateTime.Today.ToString(dateFormat).Replace("/", "-");
    }

    /// <summary>
    /// Adds number in () at the end of a file name if it would create a duplicate in the savePath
    /// </summary>
    /// <param name="savePath">Path to get unique name for</param>
    /// <param name="fileName">File name to make unique</param>
    /// <param name="extension">File extension</param>
    /// <returns>Unique file name string</returns>
    public static string MakeExportNameUnique(string savePath, string fileName, string extension)
    {
        int i = 0;
        string outputName = fileName;
        while (File.Exists(Path.Combine(savePath, outputName)))
        {
            outputName = $"{fileName.Left(fileName.Length - extension.Length)} ({i}).{extension}";
            i++;
        }
        return outputName;
    }

    public enum EHashAlgorithm
    {
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        MD5,
        RSA
    }

    /// <summary>
    /// Takes in a string and returns the hashed value of it using the passed in hashing algorithm
    /// </summary>
    /// <param name="originalString">String to be hashed</param>
    /// <param name="algorithm">Which algorithm to use for the hash operation</param>
    /// <returns>Hash string</returns>
    public static string GetHash(this string originalString, EHashAlgorithm algorithm)
    {
        byte[] bytes;

        switch (algorithm)
        {
            case EHashAlgorithm.SHA1:
                using (SHA1 hasher = SHA1.Create())
                {
                    bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                }
                break;
            case EHashAlgorithm.SHA256:
                using (SHA256 hasher = SHA256.Create())
                {
                    bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                }
                break;
            case EHashAlgorithm.SHA384:
                using (SHA384 hasher = SHA384.Create())
                {
                    bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                }
                break;
            case EHashAlgorithm.MD5:
                using (MD5 hasher = MD5.Create())
                {
                    bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                }
                break;
            case EHashAlgorithm.SHA512: default:
                using (SHA512 hasher = SHA512.Create())
                {
                    bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                }
                break;
        }

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
}
