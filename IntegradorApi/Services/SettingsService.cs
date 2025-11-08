using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace IntegradorApi.Services;

public class SettingsService {
  private readonly string _settingsFilePath;

  public SettingsService() {
    _settingsFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
  }

  public bool GetSincronizacaoStatus() {
    return AppConfig.Configuration.GetValue<bool>("AppSettings:Sincronizar");
  }

  public void SaveConnectionSettings(string desc, string addr, string port, string user, string pass) {
    var json = File.ReadAllText(_settingsFilePath);
    var jsonObj = JObject.Parse(json);

    var connectionSettings = jsonObj["ConnectionStrings"]?["LocalDatabase"];
    if (connectionSettings != null) {
      connectionSettings["Description"] = desc;
      connectionSettings["Address"] = addr;
      connectionSettings["Port"] = port;
      connectionSettings["User"] = user;
      connectionSettings["Password"] = pass;
    }

    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
    File.WriteAllText(_settingsFilePath, output);

    (AppConfig.Configuration as IConfigurationRoot)?.Reload();
  }

  public void SetSincronizacaoStatus(bool novoStatus) {
    var json = File.ReadAllText(_settingsFilePath);
    var jsonObj = JObject.Parse(json);

    if (jsonObj["AppSettings"] == null)
      jsonObj["AppSettings"] = new JObject();

    jsonObj["AppSettings"]!["Sincronizar"] = novoStatus;

    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
    File.WriteAllText(_settingsFilePath, output);

    (AppConfig.Configuration as IConfigurationRoot)?.Reload();
  }
}
