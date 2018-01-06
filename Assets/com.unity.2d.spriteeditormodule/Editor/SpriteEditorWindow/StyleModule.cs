using System;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Experimental.U2D
{
    public struct MLStyle
    {
        public string name;
        public Func<Texture2D,Color32[]> processFunc;
    }

    [RequireSpriteDataProvider(typeof(ITextureDataProvider))]
    public class StyleModule : SpriteEditorModuleBase
    {
        int m_CurrentStyleSelection;

        const float kToolbarHeight = 16f;
        const float kInspectorWindowMargin = 8f;
        const float kInspectorWidth = 200f;
        const float kInspectorHeight = 45f;

        private Rect toolbarWindowRect
        {
            get
            {
                Rect position = spriteEditor.windowDimension;
                return new Rect(
                    position.width - kInspectorWidth - kInspectorWindowMargin,
                    position.height - kInspectorHeight - kInspectorWindowMargin + kToolbarHeight,
                    kInspectorWidth,
                    kInspectorHeight);
            }
        }

        static public MLStyle[] kMLStyle = new[]
        {
            new MLStyle(){
                name = "No Style",
                processFunc = null
            },
            new MLStyle(){
                name = "Cell",
                processFunc = CellProcessFunc
            },
            new MLStyle(){
                name = "Ink",
                processFunc = InkProcessFunc
            },
            new MLStyle(){
                name = "Canvas",
                processFunc = CanvasProcessFunc
            },
        };

        string[] m_StyleSelectionStrings;
        AssetImporter m_AssetImporter;
        ITextureDataProvider m_TextureDataProvider;
        Texture2D m_ProcessedTexture;
        Material m_Material;

        static Color32[] CellProcessFunc(Texture2D tex)
        {            
            var pixels = tex.GetPixels32();

            for(int i = 0; i < pixels.Length; ++i)
                pixels[i] = new Color32(255, 0,0,255);

            return pixels;
        }

        static Color32[] InkProcessFunc(Texture2D tex)
        {
            var pixels = tex.GetPixels32();

            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new Color32(255, 255, 0, 255);

            return pixels;
        }

        static Color32[] CanvasProcessFunc(Texture2D tex)
        {
            var pixels = tex.GetPixels32();

            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = new Color32(0, 255, 0, 255);

            return pixels;
        }

        public override string moduleName { get { return "Style"; } }

        public override bool CanBeActivated()
        {
            return spriteEditor.GetDataProvider<ISpriteEditorDataProvider>() is AssetImporter;
        }

        public override void DoMainGUI()
        {
            if (m_ProcessedTexture)
            {
                m_Material.SetTexture("_MainTex", m_ProcessedTexture);
                m_Material.SetColor("_Color", Color.white);
                m_Material.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(Handles.matrix);
                GL.Begin(GL.QUADS);
                GL.TexCoord2(0,0);
                GL.Vertex3(0, 0, 0);
                GL.TexCoord2(0,1);
                GL.Vertex3(0, m_ProcessedTexture.height, 0);
                GL.TexCoord2(1,1);
                GL.Vertex3(m_ProcessedTexture.width, m_ProcessedTexture.height, 0);
                GL.TexCoord2(1,0);
                GL.Vertex3(m_ProcessedTexture.width, 0, 0);
                GL.End();
                GL.PopMatrix();
            }
        }

        public override void DoToolbarGUI(UnityEngine.Rect drawArea)
        {

        }

        public override void OnModuleActivate()
        {
            if (m_StyleSelectionStrings == null)
                m_StyleSelectionStrings = kMLStyle.Select(x => x.name).ToArray();

            if (m_Material == null)
            {
                m_Material = new Material(Shader.Find("Sprites/Default"));
            }
            m_AssetImporter = spriteEditor.GetDataProvider<ISpriteEditorDataProvider>() as AssetImporter;
            m_TextureDataProvider = spriteEditor.GetDataProvider<ITextureDataProvider>();

            if (!int.TryParse(m_AssetImporter.userData, out m_CurrentStyleSelection))
                m_CurrentStyleSelection = 0;

            var tex = m_TextureDataProvider.GetReadableTexture2D();
            if (kMLStyle[m_CurrentStyleSelection].processFunc != null)
            {
                m_ProcessedTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
                m_ProcessedTexture.SetPixels32(kMLStyle[m_CurrentStyleSelection].processFunc(tex));
                m_ProcessedTexture.Apply();
            }
            
        }

        public override void OnModuleDeactivate()
        {
            if (m_ProcessedTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(m_ProcessedTexture);
                m_ProcessedTexture = null;
            }
            if (m_Material != null)
            {
                UnityEngine.Object.DestroyImmediate(m_Material);
                m_Material = null;
            }
        }

        public override void DoPostGUI()
        {
            GUILayout.BeginArea(toolbarWindowRect, Styles.windowsTitle, GUI.skin.window);
            EditorGUI.BeginChangeCheck();
            m_CurrentStyleSelection = EditorGUILayout.Popup(m_CurrentStyleSelection, m_StyleSelectionStrings);
            if (EditorGUI.EndChangeCheck())
            {
                StyleChange();
            }
                
            GUILayout.EndArea();

        }

        public override bool ApplyRevert(bool apply)
        {
            if (apply)
            {
                m_AssetImporter.userData = m_CurrentStyleSelection.ToString();
            }
            
            return true;
        }

        private void StyleChange()
        {
            var tex = m_TextureDataProvider.GetReadableTexture2D();
            if(m_ProcessedTexture == null)
                m_ProcessedTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            if (kMLStyle[m_CurrentStyleSelection].processFunc != null)
            {
                m_ProcessedTexture.SetPixels32(kMLStyle[m_CurrentStyleSelection].processFunc(tex));
                m_ProcessedTexture.Apply();
            }
            else if(m_ProcessedTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(m_ProcessedTexture);
                m_ProcessedTexture = null;
            }
            spriteEditor.SetDataModified();
        }

        private class Styles
        {
            public static readonly GUIContent windowsTitle = new GUIContent("Styles");
        }
    }


    public class MLStylePostProcesssor : AssetPostprocessor
    {
        void OnPostprocessTexture(Texture2D texture)
        {
            var ai = AssetImporter.GetAtPath(assetPath);
            if (ai != null)
            {
                var style = 0;
                if (int.TryParse(ai.userData, out style))
                {
                    if (StyleModule.kMLStyle[style].processFunc != null)
                    {
                        // Only works when texture is set to readable in importer
                        texture.SetPixels32(StyleModule.kMLStyle[style].processFunc(texture));
                        texture.Apply();
                    }
                }
            }
        }
    }
}
