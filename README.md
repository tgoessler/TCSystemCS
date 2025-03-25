# C# Utilities

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ThE-TiGeR_TCSystemCS&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=ThE-TiGeR_TCSystemCS)

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

## TCSystem.Gps
  Allows to read GPS data from a google location history file (records.json).

## TCSystem.MetaData

  Provide classes for managing meta date from photos.

  It supports date/time, location and face information.

## TCSystem.MetaDataDB

  Sqlite database read/write/query for phot meta data.

## TCSystem.Thread

  Provides extensions/asybc helper functions for multi threading.
  Implements a worker thread which queues commands to execute and executes them in the given order.

## TCSystem.Util

  Provides helpfull extensions for containers / enumerables