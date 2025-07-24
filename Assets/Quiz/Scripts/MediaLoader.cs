using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public static class MediaLoader
{
    // Carrega Sprite dos Resources (sem extensão no caminho)
    public static Sprite LoadSprite(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) return null;

        // Carrega a textura dos Resources
        Texture2D tex = Resources.Load<Texture2D>(resourcePath);
        if (tex == null)
        {
            Debug.LogWarning($"Não foi possível carregar a imagem: {resourcePath}");
            return null;
        }

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    }

    // Carrega AudioClip dos Resources
    public static AudioClip LoadAudio(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) return null;

        AudioClip clip = Resources.Load<AudioClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogWarning($"Não foi possível carregar o áudio: {resourcePath}");
        }
        return clip;
    }

    // Carrega VideoClip dos Resources
    public static VideoClip LoadVideo(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath)) return null;

        VideoClip clip = Resources.Load<VideoClip>(resourcePath);
        if (clip == null)
        {
            Debug.LogWarning($"Não foi possível carregar o vídeo: {resourcePath}");
        }
        return clip;
    }
}