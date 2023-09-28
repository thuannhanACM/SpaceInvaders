
using Core.Framework.Utilities;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.U2D;

namespace Core.Framework
{
    public class ModuleAtlasCtrl: IDisposable
    {
        private readonly IBundleLoader _bundleLoader;
        private AtlasInfo _atlasInfo;
        private ModuleName _moduleName;
        private const string AtlasBasePath = "Assets/Bundles/Views/{0}/{1}.spriteatlasv2";

        public ModuleAtlasCtrl(
            IBundleLoader bundleLoader,
            ModuleName moduleName)
        {
            _bundleLoader = bundleLoader;
            _moduleName = moduleName;
        }

        public async UniTask Initialize()
        {
            string path = GetBundleDefaultAtlasPath(_moduleName);
            SpriteAtlas atlas = await _bundleLoader.LoadAssetAsync<SpriteAtlas>(path);
            _atlasInfo = new AtlasInfo(path, atlas);
        }

        private static string GetBundleDefaultAtlasPath(ModuleName moduleName)
        {
            return string.Format(AtlasBasePath, moduleName, moduleName);
        }

        public Sprite GetSprite(string spName)
        {
            try
            {
                return _atlasInfo.Atlas.GetSprite(spName);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public void Dispose()
        {
            _bundleLoader.ReleaseAsset(_atlasInfo.Path);
        }
    }

}
