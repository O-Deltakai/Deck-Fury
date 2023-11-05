using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameErrorHandler
{

    /// <summary>
    /// Will check if the object is null, and if it is will try to find it using
    /// FindObjectOfType. If it finds it, it will return that object and print a warning, otherwise will return null and print an error that says the
    /// object couldn't be found in the scene. 
    /// 
    /// <para>The purpose of this method is to provide a safety net for critical objects, such that if they aren't set properly
    /// the game will not outright crash unless they are completely missing from the scene.</para>
    /// 
    /// <para>
    /// <b>NOTE:</b> You should only use this method for objects where there should only be one of in the scene, as FindObjectOfType finds the first one available
    /// within the scene.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public static T NullCheck<T>(T obj, string objectName) where T : Object
    {
        if (obj == null)
        {
            Debug.LogWarning($"{objectName} was not set. Attempting to find using FindObjectOfType.");
            obj = Object.FindObjectOfType<T>();

            if (obj == null)
            {
                Debug.LogError($"Catastrophic failure: {objectName} could not be found in the scene!");
            }
            else
            {
                Debug.LogWarning($"{objectName} was found using FindObjectOfType. Please set it properly to avoid performance issues.");
            }
        }

        return obj;
    }

    public delegate bool SafeAction();
/// <summary>
/// A safety net measure which allows you to wrap potentially risky code that may cause a crash in an error catcher such that if the
/// code block fails, the program won't outright crash and will instead essentially skip over the faulty code and into the next line.
/// 
/// <para>This allows us to create common error messages for failed operations and divert operations without crashing the game.</para>
/// 
/// <para><b>NOTE</b>: you typically want to use this for running methods or code written by other people/dynamically generated code that you aren't sure about
/// such that even if the code doesn't work, the program will simply continue running as if the code wasn't there at all. In such cases,
/// you need to make sure that the rest of your code will still work even if the code within the ExecuteSafely block fails, or divert your
/// code operations such that it will, otherwise this safety net won't do anything.
/// </para>
/// 
/// </summary>
/// <example>
/// <code>
/// ErrorHandler.ExecuteSafely(() =>
/// {
///     // Your potentially risky code here.
///     Debug.Log("This is a safe method call.");
///     // Simulating an exception.
///     throw new System.Exception("Oops! Something went wrong.");
///     
///     return true; // if the code runs successfully
/// });
/// 
/// Debug.Log("This line will still execute even though the code above threw an exception");
/// </code>
/// </example>
/// 
/// <param name="action">The action to be executed safely.</param>
    public static bool ExecuteSafely(SafeAction action)
    {
        try
        {
            return action.Invoke();
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"Error occurred: {exception.Message}");
            Debug.LogError(exception.StackTrace);
            return false;
        }
    }


}
