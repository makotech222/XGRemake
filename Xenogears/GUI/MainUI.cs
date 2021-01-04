using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering.Sprites;
using Stride.UI.Controls;
using Xenogears.Utilities;

namespace Xenogears.GUI
{
    public class MainUI : BaseUltralightUI
    {
        #region Fields

        protected MainVM _mainVM;
        #endregion Fields

        #region Methods

        public override void Start()
        {

            this.AssetDirectory = @"";
            this.HtmlFile = "MainGUI.html";
            base.Start();
            _mainVM = new MainVM();
        }

        public override void Update()
        {
            if (_renderer == null)
                return;
            if (_mainVM.DoUpdate)
            {
                var json = JsonConvert.SerializeObject(_mainVM, _serializationSettings);
                string exception = "";
                _view.EvaluateScript($"updateVM({json})", out exception);
                if (exception.NullIfEmpty() != null)
                    throw new Exception(exception);
                _mainVM.DoUpdate = false;
            }

            base.Update();
        }

        #endregion Methods

        #region Classes

        public class BaseVM : INotifyPropertyChanged
        {
            protected bool _processingTurn;

            public bool ProcessingTurn
            {
                get
                {
                    return _processingTurn;
                }
                set
                {
                    if (_processingTurn != value)
                    {
                        _processingTurn = value;
                        OnPropertyChanged();
                    }
                }
            }

            public bool DoUpdate { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                DoUpdate = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class MainVM
        {
            private List<BaseVM> _vms = new List<BaseVM>();

            public bool DoUpdate
            {
                get { return _vms.Any(x => x.DoUpdate); }
                set { _vms.ForEach(x => x.DoUpdate = value); }
            }

            public MainVM()
            {
                _vms = new List<BaseVM>()
                {
                };
            }
        }

       
        #endregion Classes
    }
}