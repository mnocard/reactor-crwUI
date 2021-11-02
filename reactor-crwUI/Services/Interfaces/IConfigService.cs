using reactor_crwUI.Model;

namespace reactor_crwUI.Services.Interfaces
{
    interface IConfigService
    {
        bool SaveConfig(Config config);
        Config LoadConfig();
    }
}
