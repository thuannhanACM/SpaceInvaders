using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Framework
{
    public class BattleHUDModel : BaseModuleContextModel
    {
        public override string ViewId => "Assets/Bundles/Views/BattleHUD/BattleHUD.prefab";

        public override ModuleName ModuleName => ModuleName.BattleHUD;
        public int Score;
    }
}
