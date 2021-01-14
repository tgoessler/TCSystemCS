# C# Utilities

## TCSystem.Logging

  Wraps Serilog by an abstract Logger class where debug logging is not compiled into your code when using the provided Logger class.
  
  It also integrates direct logging to Microsoft App Center which can be configured at runtime.
  
  The idea is to add a Log.cs in each folder/namespace.
  
  This helps that each log can be filtered by its namespace.
  ### Init Logging
  ```CS
  Factory.InitLogging(Factory.LoggingOptions.Console | Factory.LoggingOptions.File,
                "Output.txt", 1, 10 * 1024);
  ```
  ### Log.cs example
  ```CS
  namespace Hello.World
  {
      internal static class Log
      {
          public static Logger Instance { get; } = Factory.GetLogger(typeof(Log));
      }
  }
  ```
  ### Usage in your code
  ```CS
  Log.Instance.Info("Hello World");
  ```
