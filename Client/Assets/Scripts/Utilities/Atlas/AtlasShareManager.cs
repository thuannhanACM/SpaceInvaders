
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using Zenject;

namespace Core.Framework.Utilities
{
    public class AtlasShareManager
    {
        private readonly GameStore.Atlas _atlas;
        private readonly IBundleLoader _bundleLoader;

        private Dictionary<string, AtlasInfo> _atlasDic = new Dictionary<string, AtlasInfo>();

        public AtlasShareManager(
            [Inject(Id = BundleLoaderName.Addressable)]
            IBundleLoader bundleLoader,
            GameStore.Atlas atlases)
        {
            _bundleLoader = bundleLoader;
            _atlas = atlases;
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        private async void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
        {
            SpriteAtlas atlas = await _bundleLoader.LoadAssetAsync<SpriteAtlas>($"Assets/Bundles/Views/{tag}/{tag}.spriteatlas");
            callback(atlas);
        }

        public async UniTask Initialize()
        {
            foreach (var atlasPath in _atlas.Atlases)
            {
                string atlasNameNoExt = Path.GetFileNameWithoutExtension(atlasPath);
                SpriteAtlas atlas = await _bundleLoader.LoadAssetAsync<SpriteAtlas>(atlasPath);
                AtlasInfo info = new AtlasInfo(atlasPath, atlas);
                _atlasDic.Add(atlasNameNoExt, info);
            }
        }

        public Sprite GetSpriteFromDataAtlas(string atlasName, string spriteName)
        {
            if (_atlasDic.TryGetValue(atlasName, out AtlasInfo info))
                return GetSprite(info, spriteName);

            throw new SpriteNotFound(atlasName + "/" + spriteName);
        }

        public Sprite GetSpriteFromDataAtlas(string spritePath)
        {
            (string atlas, string spName) atlasPath = GetPathAndName(spritePath);
            if (_atlasDic.TryGetValue(atlasPath.atlas, out AtlasInfo info))
                return GetSprite(info, atlasPath.spName);

            throw new SpriteNotFound(spritePath);
        }

        private Sprite GetSprite(AtlasInfo info, string spName)
        {
            try
            {
                return info.Atlas.GetSprite(spName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private (string, string) GetPathAndName(string path)
        {
            string[] p = path.Split('/');
            string atlas = p[0];
            string spName = p[1];
            return (atlas, spName);
        }

        private class SpriteNotFound : Exception
        {
            public SpriteNotFound(string mess) : base(mess) { }
        }
    }

}
