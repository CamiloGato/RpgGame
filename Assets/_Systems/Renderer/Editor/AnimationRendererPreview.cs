using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Renderer.Editor
{
    public class AnimationRendererPreview : EditorWindow
    {
        public StyleSheet styleSheet;
        public VisualTreeAsset visualTreeAsset;
        
        private Texture2D _texture;
        private VisualElement _previewElement;
        private IntegerField _widthField;
        private IntegerField _heightField;
        
        [MenuItem("Tools/Renderer/AnimationRendererPreview")]
        private static void ShowWindow()
        {
            AnimationRendererPreview window = GetWindow<AnimationRendererPreview>();
            window.titleContent = new GUIContent("Renderer Preview");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = visualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
            
            // Get references to elements
            var objectField = root.Q<ObjectField>("PreviewTextureField");
            objectField.objectType = typeof(Texture2D);
            objectField.RegisterValueChangedCallback(evt => {
                _texture = (Texture2D)evt.newValue;
            });

            _widthField = root.Q<IntegerField>("PreviewWidthField");
            _heightField = root.Q<IntegerField>("PreviewHeightField");

            var generateButton = root.Q<Button>("PreviewGenerateButton");
            generateButton.clicked += UpdatePreview;

            _previewElement = root.Q<VisualElement>("PreviewImage");
        }

        private void UpdatePreview()
        {
            if (!_texture) return;
            
            if (!_texture.isReadable)
            {
                Debug.LogError("The selected texture is not readable. Please enable 'Read/Write Enabled' in the texture import settings.");
                return;
            }

            int width = _widthField.value > 0 ? _widthField.value : _texture.width;
            int height = _heightField.value > 0 ? _heightField.value : _texture.height;
            var previewTexture = ScaleTexture(_texture, width, height);

            var style = _previewElement.style;
            style.backgroundImage = new StyleBackground(previewTexture);
            style.width = width;
            style.height = height;
        }

        private Texture2D ScaleTexture(Texture2D source, int width, int height)
        {
            Texture2D result = new Texture2D(width, height, source.format, false);
            Color[] pixels = result.GetPixels(0);

            float incX = (1.0f / width);
            float incY = (1.0f / height);

            for (int px = 0; px < pixels.Length; px++)
            {
                pixels[px] = source.GetPixelBilinear(incX * (px % width), incY * (px / width % height));
            }

            result.SetPixels(pixels, 0);
            result.Apply();

            return result;
        }
    }
}