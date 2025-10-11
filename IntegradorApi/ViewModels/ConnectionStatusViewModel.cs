using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;

namespace IntegradorApi.ViewModels;

public class ConnectionStatusViewModel : INotifyPropertyChanged {
  public Connection Connection { get; }
  public string TechnologyGlyph { get; }
  public string DirectionGlyph { get; }

  private SolidColorBrush _statusBrush = new(Colors.Gray);
  public SolidColorBrush StatusBrush {
    get => _statusBrush;
    set {
      _statusBrush = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusBrush)));
    }
  }

  public ConnectionStatusViewModel(Connection connection) {
    Connection = connection;

    switch (connection.TypeConnection) {
      case ConnectionType.APIREST:
        TechnologyGlyph = "\uE753";
        break;
      case ConnectionType.MYSQL:
      case ConnectionType.POSTGRESSQL:
        TechnologyGlyph = "\uE756";
        break;
      default:
        TechnologyGlyph = "\uE895";
        break;
    }

    switch (connection.TypeDataSource) {
      case DataSourceType.ORIGIN:
        DirectionGlyph = "\uE896";
        break;
      case DataSourceType.DESTINATION:
        DirectionGlyph = "\uE898";
        break;
      default:
        DirectionGlyph = "";
        break;
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;
}
