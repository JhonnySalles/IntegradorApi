using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IntegradorApi.Services;

public class SettingsService {
    private readonly string _settingsFilePath;

    public SettingsService() {
        _settingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
    }

    public bool GetSincronizacaoStatus() {
        return AppConfig.Configuration.GetValue<bool>("AppSettings:Sincronizar");
    }

    public void SetSincronizacaoStatus(bool novoStatus) {
        var json = File.ReadAllText(_settingsFilePath);
        var jsonObj = JObject.Parse(json);

        if (jsonObj["AppSettings"] == null)
            jsonObj["AppSettings"] = new JObject();

        jsonObj["AppSettings"]["Sincronizar"] = novoStatus;

        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(_settingsFilePath, output);

        (AppConfig.Configuration as IConfigurationRoot).Reload();
    }
}
