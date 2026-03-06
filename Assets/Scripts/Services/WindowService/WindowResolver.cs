using System;
using Windows;

namespace Services.WindowService
{
    /// <summary>
    /// Factory for creating window models.
    /// </summary>
    public class WindowResolver
    {
        private readonly WindowsService _windowsService;

        public WindowResolver(WindowsService windowsService)
        {
            _windowsService = windowsService;
        }

        /// <summary>
        /// Creates a model for PlayConfirmWindow.
        /// </summary>
        public PlayConfirmWindow.Model GetPlayConfirmWindowModel(Action onClick, bool isPlay = true)
        {
            return new PlayConfirmWindow.Model(onClick, _windowsService, isPlay);
        }
    }
}