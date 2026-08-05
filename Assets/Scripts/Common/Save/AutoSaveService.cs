using System;
using UnityEngine;
using Zenject;

namespace Common.Save
{
    public class AutoSaveService : IInitializable, IDisposable
    {
        private readonly SaveService _saveService;

        public AutoSaveService(SaveService saveService)
        {
            _saveService = saveService;
        }

        public void Initialize()
        {
            Application.focusChanged += OnFocusChanged;
            Application.quitting += OnQuitting;
        }

        private void OnFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                return;
            }

            Save();
        }

        private void OnQuitting()
        {
            Save();
        }

        private void Save()
        {
            _saveService.SaveAll();
        }

        public void Dispose()
        {
            Application.focusChanged -= OnFocusChanged;
            Application.quitting -= OnQuitting;
        }
    }
}
