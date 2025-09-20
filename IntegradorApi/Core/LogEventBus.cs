using System;

public static class LogEventBus {
  public static event Action<string>? OnLogReceived;
  public static void Raise(string message) => OnLogReceived?.Invoke(message);
}
