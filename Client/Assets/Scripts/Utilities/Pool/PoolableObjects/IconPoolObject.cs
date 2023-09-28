
using Core.Framework.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Framework
{
    public class IconPoolObject : BasePoolObj<string>
    {
        private readonly AtlasShareManager _atlasManager;
        private string _spritePath;
        private SpriteRenderer _sprite;

        public IconPoolObject(AtlasShareManager atlasManager)
        {
            _atlasManager = atlasManager;
        }

        public override void InitDefinition(string spritePath)
        {
            _spritePath = spritePath;
        }

        public override UniTask Reinitialize()
        {
            if(_sprite == null)
                _sprite = _modelObj.GetComponent<SpriteRenderer>();
            _sprite.sprite = _atlasManager.GetSpriteFromDataAtlas(_spritePath);
            return UniTask.CompletedTask;
        }
    }
}
