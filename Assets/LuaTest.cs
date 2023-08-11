using UnityEngine;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

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

        // Marshaling examples
        List<int> numbers = new List<int> { 3, 4, 1, 2, 5 };
        luaScript.Globals["numbers"] = numbers;

        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "first_name", "John" },
            { "last_name", "Doe" }
        };
        luaScript.Globals["dict"] = dict;

        Dictionary<string, object> binaryTree = MakeSimpleTree();
        luaScript.Globals["binaryTree"] = binaryTree;

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

            -- Print List
            for i, num in ipairs(numbers) do print('list- i: ' .. i .. ' value: ' .. num) end

            -- Print Dictionary
            for key, value in pairs(dict) do print('dict- ' .. key .. ': ' .. value) end

            -- Print Binary Tree
            function printTree(tree)
                if tree == nil then return end
                print('binary tree traverse: ' .. tree.value)
                printTree(tree.left)
                printTree(tree.right)
            end

            printTree(binaryTree)
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

    private static Dictionary<string, object> MakeSimpleTree()
    {
        return new Dictionary<string, object>
        {
            { "value", 10 },
            { "left", new Dictionary<string, object>
                {
                    { "value", 5 },
                    { "left", null },
                    { "right", null }
                }
            },
            { "right", new Dictionary<string, object>
                {
                    { "value", 20 },
                    { "left", null },
                    { "right", null }
                }
            }
        };
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

    static public void LuaPrint(string message)
    {
        Debug.Log(message);
    }

    static public float CalculateMagnitude(float x, float y)
    {
        return (float)Math.Sqrt(x * x + y * y);
    }

    static public float GetTime()
    {
        return (float)Time.realtimeSinceStartup;
    }
}
