using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System;
using System.IO;

namespace IntegradorApi.Services;

public class UISink : ILogEventSink {
  private readonly IFormatProvider? _formatProvider;
  private readonly MessageTemplateTextFormatter? _formatter;
  public UISink(IFormatProvider? formatProvider) => _formatProvider = formatProvider;

  public UISink() {
    _formatter = new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}");
  }

  public void Emit(LogEvent logEvent) {
    using var writer = new StringWriter();
    _formatter!.Format(logEvent, writer);
    var message = writer.ToString();
    LogEventBus.Raise(message);
  }
}
