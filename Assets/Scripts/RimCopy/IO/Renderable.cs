using UnityEngine.Tilemaps;

namespace RimCopy.IO
{
    public interface IRenderable
    {
        /**
         * Being called first tick to render an object.
         * Param tilemaps is the tilemaps where the object can be rendered.
         * 0th index means 0th layer(under all others), 1st means 1st and so on.
         */
        void StartRender(Tilemap[] tilemaps);

        /**
         * Being called every tick to render an object.
         */
        void Render(Tilemap[] tilemaps);
    }
}