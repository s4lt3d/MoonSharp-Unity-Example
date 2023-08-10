using UnityEngine;
using MoonSharp.Interpreter;
using System;

public class LuaTest : MonoBehaviour
{
    Script luaScript = new Script();
    DynValue luaUpdateFunction;

    void Start()
    {
        // Register functions. 
        luaScript.Globals["print"] = (Action<string>)LuaPrint;
        luaScript.Globals["magnitude"] = (Func<float, float, float>)CalculateMagnitude;
        luaScript.Globals["time"] = (Func<float>)GetTime;

        string lua_code = @"
            -- Called from C# script 
            function update()
                local currentTime = time()  -- Get time from C# script
                print('Time since startup: ' .. currentTime)
            end

            x = 3
            y = 4
            z = magnitude(x, y)
            print('z = ' .. z)
        ";

        try
        {
            luaScript.DoString(lua_code);
        }
        catch (ScriptRuntimeException e)
        {
            Debug.LogError("Lua error: " + e.DecoratedMessage);
        }

        // Register callable functions. 
        luaUpdateFunction = luaScript.Globals.Get("update");


        DynValue xValue = luaScript.Globals.Get("x");
        Debug.Log("Getting variable x from state: " + xValue.Number);
        CallLuaUpdate();
    }


    void Update()
    {
        CallLuaUpdate();
    }

    private void CallLuaUpdate()
    {
        // Safely call the Lua update function if it exists
        if (luaUpdateFunction.Type == DataType.Function)
        {
            luaScript.Call(luaUpdateFunction);
        }
    }

    // MoonSharp will call into these functions from script

    // lua script: print( string )
    static public void LuaPrint(string message)
    {
        Debug.Log(message);
    }

    // lua script: float magnitude( float float )
    static public float CalculateMagnitude(float x, float y)
    {
        return (float)Math.Sqrt(x * x + y * y);
    }

    // lua script: float time() 
    static public float GetTime()
    {
        return (float)Time.realtimeSinceStartup;
    }
}
