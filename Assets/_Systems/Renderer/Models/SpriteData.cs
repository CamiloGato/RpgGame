using System;
using UnityEngine;

namespace Renderer.Models
{
    [Serializable]
    public class SpriteData
    {
        public Texture2D texture;
        public int width;
        public int height;
        public int amountOnWidth;
        public int amountOnHeight;
    }
}