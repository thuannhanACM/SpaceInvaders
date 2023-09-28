
using UnityEngine.U2D;

namespace Core.Framework.Utilities
{
    public struct AtlasInfo
    {
        public string Path;
        public SpriteAtlas Atlas;
        public AtlasInfo(string path, SpriteAtlas atlas)
        {
            Path = path;
            Atlas = atlas;
        }
    }
}
