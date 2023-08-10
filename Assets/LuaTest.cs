using UnityEngine;
using MoonSharp.Interpreter;
using System;

public class LuaTest : MonoBehaviour
{
    void Start()
    {
        Script state = new Script();

        // Register functions. In MoonSharp, you can simply expose C# methods to Lua by adding them as globals.
        state.Globals["print"] = (Action<string>)LuaPrint;
        state.Globals["magnitude"] = (Func<float, float, float>)CalculateMagnitude;

        string lua_code = @"
                x = 3
                y = 4
                z = magnitude(x, y)
                print('z = ' .. z)
            ";

        Debug.Log("Running Lua Script");

        state.DoString(lua_code);

        // Get variables from the script. In MoonSharp, it's a bit different.
        DynValue xValue = state.Globals.Get("x");
        Debug.Log("Getting variable x from state: " + xValue.Number);
    }

    // MoonSharp directly supports standard C# function signatures, so we don't need "params object[]".
    static public void LuaPrint(string message)
    {
        Debug.Log(message);
    }

    static public float CalculateMagnitude(float x, float y)
    {
        return (float)Math.Sqrt(x * x + y * y);
    }
}
