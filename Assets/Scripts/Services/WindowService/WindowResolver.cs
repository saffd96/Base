using System;
using Windows;

namespace Services
{
    public class WindowResolver
    {
        private readonly WindowsService _windowsService;

        public WindowResolver(WindowsService windowsService)
        {
            _windowsService = windowsService;
        }

        public PlayConfirmWindow.Model GetPlayConfirmWindowModel(Action onClick, bool isPlay = true)
        {
            return new(onClick, _windowsService, isPlay);
        }
    }
}