using ReactiveShowcase.Installers;
using IPA;
using IPA.Logging;
using SiraUtil.Attributes;
using SiraUtil.Zenject;

namespace ReactiveShowcase
{
    [Plugin(RuntimeOptions.DynamicInit)]
    [NoEnableDisable]
    [Slog]
    // ReSharper disable once UnusedType.Global
    public class Plugin
    {
        [Init]
        public Plugin(Logger logger, Zenjector zenjector)
        {
            zenjector.UseLogger(logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
        }
    }
}
