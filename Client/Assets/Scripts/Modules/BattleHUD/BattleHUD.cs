using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using ILogger = Core.Infrastructure.ILogger;

namespace Core.Framework
{
    public partial class BattleHUD : BaseModule<BattleHUDView, BattleHUDModel>
    {
        private readonly ILogger _logger;
        private readonly IBundleLoader _bundleLoader;
        private readonly SignalBus _signalBus;

        public BattleHUD(
            ILogger logger,
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader,
            SignalBus signalBus)
        {
            _logger = logger;
            _bundleLoader = bundleLoader;
            _signalBus = signalBus;
        }

        protected override void OnDisposed()
        {
            
        }

        protected override void OnViewReady()
        {
            
        }
    }
}
