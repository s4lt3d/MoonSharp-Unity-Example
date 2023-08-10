# Unity with MoonSharp: A Lua Scripting Example

This example demonstrates how to integrate Lua scripting into a Unity project using the MoonSharp interpreter. The provided [`LuaTest.cs`](https://github.com/s4lt3d/MoonSharp-Unity-Example/blob/main/Assets/LuaTest.cs) MonoBehaviour showcases the basics of how to call Lua functions from C#, expose C# methods to Lua, and access Lua variables from C#.

## Prerequisites

- Unity 2022.3
- MoonSharp library (which serves as the Lua script engine)

## Overview

### Calling Lua Functions from C#

The example contains an `update` function written in Lua, which is called during Unity's `Update` method:

```lua
function update()
    local currentTime = time()  -- Get time from C# script
    print('Time since startup: ' .. currentTime)
end
```
In C#, the Lua update function is fetched as a DynValue and then invoked during Unity's Update:

```
luaUpdateFunction = luaScript.Globals.Get("update");
...
if (luaUpdateFunction.Type == DataType.Function)
{
    luaScript.Call(luaUpdateFunction);
}
```
### Exposing C# Functions to Lua
C# functions can be registered with the Lua environment to make them callable from Lua scripts. In the provided example:

LuaPrint allows Lua to log messages to the Unity console.
CalculateMagnitude computes the magnitude given two float arguments.
GetTime fetches the time since the application started.
```csharp

luaScript.Globals["print"] = (Action<string>)LuaPrint;
luaScript.Globals["magnitude"] = (Func<float, float, float>)CalculateMagnitude;
luaScript.Globals["time"] = (Func<float>)GetTime;
```
Accessing Lua Variables from C#
Lua variables can be accessed from C# after they've been defined in the Lua environment. For instance, after defining x in Lua, it's fetched in C# as follows:

```csharp
DynValue xValue = luaScript.Globals.Get("x");
Debug.Log("Getting variable x from state: " + xValue.Number);
```
## How to Run the Example
Ensure you have Unity 2022.3 installed.
Import the MoonSharp library into your Unity project.
Create a new MonoBehaviour script named LuaTest and paste the provided code into it.
Attach the LuaTest script to a GameObject in your Unity scene.
Run the scene. Observe the Unity console for messages logged from both C# and Lua.
Important Notes
Always wrap your DoString calls with a try-catch to handle any Lua runtime errors gracefully.
Ensure that the C# methods you expose to Lua have the correct signature to match the expected Lua function calls.
