using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMaterial : MonoBehaviour
{
    //親 子オブジェクトを格納。
    private MeshRenderer[] meshRenderers;
    private Material[][] originalMaterials; // 各MeshRendererの元のマテリアルを保存
    private Material[][] transparentMaterials; // 透明化用のマテリアルを保存

    void Awake()
    {
        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        //子オブジェクトと親オブジェクトのmeshrendererを取得
        meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
        
        Debug.Log($"Found {meshRenderers.Length} MeshRenderers");
        
        // 元のマテリアルを保存し、透明化用のマテリアルを作成
        originalMaterials = new Material[meshRenderers.Length][];
        transparentMaterials = new Material[meshRenderers.Length][];
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Material[] mats = meshRenderers[i].materials;
            originalMaterials[i] = new Material[mats.Length];
            transparentMaterials[i] = new Material[mats.Length];
            
            Debug.Log($"MeshRenderer {i} ({meshRenderers[i].gameObject.name}) has {mats.Length} materials");
            
            for (int j = 0; j < mats.Length; j++)
            {
                // 元のマテリアルを保存
                originalMaterials[i][j] = mats[j];
                
                // 透明化用のマテリアルを作成（元のマテリアルのコピー）
                transparentMaterials[i][j] = new Material(mats[j]);
                
                Debug.Log($"  Material {j}: {mats[j].name}, Shader: {mats[j].shader.name}");
                
                // シェーダーのレンダリングモードを透明に設定
                SetMaterialTransparent(transparentMaterials[i][j]);
            }
        }
        
        Debug.Log("Material initialization complete");
    }

    private void SetMaterialTransparent(Material mat)
    {
        Debug.Log($"Processing shader: {mat.shader.name}, Current color: {mat.color}");
        
        // Nature/Treeシェーダーの場合（Unity標準の木シェーダー）
        if (mat.shader.name.Contains("Nature/Tree"))
        {
            Debug.Log("Detected Nature/Tree shader - Applying transparency");
            
            // アプローチ1: シェーダーを透明対応のものに変更
            Shader transparentShader = Shader.Find("Transparent/Cutout/Diffuse");
            if (transparentShader == null)
            {
                transparentShader = Shader.Find("Transparent/Diffuse");
            }
            
            if (transparentShader != null)
            {
                // テクスチャを保存
                Texture mainTex = mat.mainTexture;
                Color originalColor = mat.color;
                
                // シェーダーを変更
                mat.shader = transparentShader;
                
                // テクスチャを復元
                mat.mainTexture = mainTex;
                
                // 透明度を設定
                Color color = originalColor;
                color.a = 0.3f;
                mat.color = color;
                
                Debug.Log($"Changed to {transparentShader.name}, New color: {mat.color}");
            }
            else
            {
                Debug.LogError("Transparent shader not found!");
            }
            
            // レンダリングキューを透明オブジェクト用に設定
            mat.renderQueue = 3000;
        }
        // Standardシェーダーの場合（Fantasy Forest/StandardNoCullingを含む）
        else if (mat.shader.name.Contains("Standard"))
        {
            Debug.Log("Detected Standard shader");
            
            Color color = mat.color;
            
            // 元のアルファが0の場合は、シェーダーを完全に置き換える
            if (color.a < 0.1f)
            {
                Debug.Log("Material has alpha 0, replacing shader entirely");
                
                // テクスチャとプロパティを保存
                Texture mainTex = mat.mainTexture;
                Color originalColor = mat.color;
                
                // 透明シェーダーに置き換え
                Shader transparentShader = Shader.Find("Transparent/Diffuse");
                if (transparentShader != null)
                {
                    mat.shader = transparentShader;
                    mat.mainTexture = mainTex;
                    
                    // 色を設定（RGBはそのまま、アルファを0.3に）
                    Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
                    mat.color = newColor;
                    
                    Debug.Log($"Replaced with Transparent/Diffuse, New color: {mat.color}");
                }
                else
                {
                    Debug.LogError("Transparent/Diffuse shader not found!");
                }
            }
            else
            {
                // 通常のStandardシェーダーの透明化処理
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                
                color.a = 0.3f;
                mat.color = color;
            }
            
            mat.renderQueue = 3000;
        }
        // Cutoutシェーダーの場合
        else if (mat.shader.name.Contains("Cutout"))
        {
            Debug.Log("Detected Cutout shader");
            // Transparentシェーダーに変更
            mat.shader = Shader.Find("Transparent/Diffuse");
            
            // 透明度を設定
            Color color = mat.color;
            color.a = 0.3f;
            mat.color = color;
        }
        else
        {
            Debug.Log("Using default transparent shader");
            // その他のシェーダーの場合
            mat.shader = Shader.Find("Transparent/Diffuse");
            
            // 透明度を設定
            Color color = mat.color;
            color.a = 0.3f;
            mat.color = color;
        }
    }

    public void ClearMaterialInvoke()
    {
        Debug.Log($"ClearMaterialInvoke called on {gameObject.name}");
        
        // 初期化されていない場合は初期化
        if (transparentMaterials == null || transparentMaterials.Length == 0)
        {
            Debug.LogWarning("Materials not initialized, initializing now...");
            InitializeMaterials();
        }
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Debug.Log($"Making transparent: {meshRenderers[i].gameObject.name}");
            meshRenderers[i].materials = transparentMaterials[i];
        }
    }
    
    public void NotClearMaterialInvoke()
    {
        Debug.Log($"NotClearMaterialInvoke called on {gameObject.name}");
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            Debug.Log($"Restoring original materials on {meshRenderers[i].gameObject.name}");
            meshRenderers[i].materials = originalMaterials[i];
        }
    }
}
