using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using Pathfindingsystem;
using System.Runtime.CompilerServices;

/// <summary>
/// Classe que lida com opera��es envolvendo texturas.
/// </summary>
public class TextureFunction
{
    /// <summary>
    /// Fun��o que retorna a textura da textura fornecida
    /// </summary>
    /// <returns>A mesma textura fornecida.</returns>
    public static Texture2D GetTexture(Texture2D toReceive)
    {
        Texture2D toApply; 
        toApply = new Texture2D(toReceive.width, toReceive.height);
        for (int x = 0; x < toApply.width; x++)
        {
            for (int y = 0; y < toApply.height; y++)
            {
                toApply.SetPixel(x, y, toReceive.GetPixel(x, y));
            }
        }
        toApply.Apply();
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return toApply;
    }
    /// <summary>
    /// Fun��o que retorna a textura da textura fornecida.
    /// </summary>
    [Obsolete("M�todo ultrapassado por passagens por refer�ncias")]
    public static void TextureToOther(ref Texture2D toApply, Texture2D toReceive)
    {
        toApply = new Texture2D(toReceive.width, toReceive.height);
        for (int x = 0; x < toApply.width; x++)
        {
            for (int y = 0; y < toApply.height; y++)
            {
                toApply.SetPixel(x, y, toReceive.GetPixel(x, y));
            }
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
        toApply.Apply();
    }
    /// <summary>
    /// Fun��o que transforma Color.black (sem considerar o alfa) em Color.clear
    /// </summary>
    /// <returns>Nova textura.</returns>
    public static Texture2D TurnBlackToClear(Texture2D texture)
    {
        Texture2D tex; 
        tex = GetTexture(texture);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (tex.GetPixel(x, y).r == 0 && tex.GetPixel(x, y).g == 0 && tex.GetPixel(x, y).b == 0)
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }
        tex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Fun��o que usa o RGB como base para alfa
    /// </summary>
    /// <returns>Nova textura.</returns>
    [Obsolete("M�todo falho")]
    public static Texture2D TurnBlurIntoClear(Texture2D texture)
    {
        Texture2D tex; 
        tex = GetTexture(texture);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                float a = (tex.GetPixel(x, y).r + tex.GetPixel(x, y).g + tex.GetPixel(x, y).b) / 3;
                tex.SetPixel(x, y, new Color(tex.GetPixel(x, y).r, tex.GetPixel(x, y).g, tex.GetPixel(x, y).b, a));
            }
        }
        tex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Fun��o que usa o elemento red do alphaTex para deixar pixeis transparentes.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="alphaTex">Textura preto e branca ou com gradiente.</param>
    /// <returns>Nova textura transparente.</returns>
    public static Texture2D GetWhiteAlpha(Texture2D texture, Texture2D alphaTex)
    {
        Texture2D tex;
        tex = GetTexture(texture);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                tex.SetPixel(x, y, new Color(tex.GetPixel(x, y).r, tex.GetPixel(x, y).g, tex.GetPixel(x, y).b, alphaTex.GetPixel(x, y).r));
            }
        }
        tex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Fun��o que transforma uma cor em outra em uma textura.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="col1">Cor que ser� alterada.</param>
    /// <param name="col2">Cor que ir� substituir.</param>
    /// <returns>Nova textura com a cor alterada.</returns>
    public static Texture2D TurnColorTo(Texture2D texture, Color col1, Color col2)
    {
        Texture2D tex; 
        tex = GetTexture(texture);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (texture.GetPixel(x, y) == col1)
                    tex.SetPixel(x, y, col2);
            }
        }
        tex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Fun��o que expande cores exceto preto.
    /// </summary>
    /// <returns>Nova textura com as cores alteradas.</returns>
    [Obsolete("M�todo sem fun��o")]
    public static Texture2D ExpandColorsWhitoutBlack(Texture2D texture)
    {
        Texture2D tex;
        tex = GetTexture(texture);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {

            }
        }
        tex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Transforma cores diversas em preto ou branco.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="which">Se verdadeiro, ser� branco, se falso, ser� preto.</param>
    /// <returns>Nova textura com as cores alteradas.</returns>
    public static Texture2D OtherColorsToTwo(Texture2D texture, bool which)
    {
        Texture2D textureComparative = GetTexture(texture);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y) != Color.black && texture.GetPixel(x, y) != Color.white)
                {
                    if (which == true)
                        textureComparative.SetPixel(x, y, Color.white);
                    else
                        textureComparative.SetPixel(x, y, Color.black);
                }
            }
        }
        textureComparative.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return textureComparative;
    }
    /// <summary>
    /// Fun��o que transforma todas as cores em preto exceto a cor fornecida, que ser� branco.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="color">A cor que ser� usada para criar branco</param>
    /// <returns>Nova textura com as cores alteradas.</returns>
    public static Texture2D PreserveOneColor(Texture2D texture, Color color)
    {
        Texture2D textureComparative = new Texture2D(texture.width, texture.height);
        TextureToOther(ref textureComparative, texture);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (textureComparative.GetPixel(x, y) != color)
                    textureComparative.SetPixel(x, y, Color.black);
                if (textureComparative.GetPixel(x, y) == color)
                    textureComparative.SetPixel(x, y, Color.white);
            }
        }
        textureComparative.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return textureComparative;
    }
    /// <summary>
    /// Inverte preto e branco.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <returns>Nova textura com as cores invertidas.</returns>
    public static void InverseWhiteBlack(Texture2D texture)
    {
        Texture2D textureComparative = new Texture2D(texture.width, texture.height);
        TextureToOther(ref textureComparative, texture);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (textureComparative.GetPixel(x, y) == Color.black)
                    texture.SetPixel(x, y, Color.white);
                else if (textureComparative.GetPixel(x, y) == Color.white)
                    texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /// <summary>
    /// Polariza a escala cinza entre preto e branco usando a vari�vel tolerance como base.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="tolerance">Toler�ncia, se maior mais preto, quanto menor mais branco.</param>
    [Obsolete("M�todo ultrapassado por passagens por refer�ncias")]
    public static void PolarizeGray(Texture2D texture, float tolerance)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).grayscale > tolerance)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /// <summary>
    /// Polariza a escala cinza entre preto e branco usando a vari�vel tolerance como base.
    /// </summary>
    /// <param name="tex">Textura original.</param>
    /// <param name="tolerance">Toler�ncia, se maior mais preto, quanto menor mais branco.</param>
    /// <returns>Nova textura com as cores polarizadas.</returns>
    public static Texture2D PolarizedGray(Texture2D tex, float tolerance)
    {
        Texture2D texture;
        texture = GetTexture(tex);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).grayscale > tolerance)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return texture;
    }
    /// <summary>
    /// Qualquer pixel onde o elemento alfa n�o � igual a zero se torna Color.white.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <returns>Nova textura com a cor branca e transparente.</returns>
    public static Texture2D TurnWhite(Texture2D texture)
    {
        Texture2D tex = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y).a == 0)
                    tex.SetPixel(x, y, Color.clear);
                else
                    tex.SetPixel(x, y, Color.white);
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        //tex.Compress(false);
        tex.name = texture.name + "_Sub";
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Preenche a parte de baixo da textura com branco dependendo da altura definida em height.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="height">Altura do preenchimento.</param>
    /// <returns>Nova textura com a cor branca preenchendo e a parte transparente acima.</returns>
    public static Texture2D GenerateHeightWhiteTex(Texture2D texture, int height)
    {
        Texture2D tex = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (y >= height)
                    tex.SetPixel(x, y, Color.clear);
                else
                    tex.SetPixel(x, y, Color.white);
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        //tex.Compress(false);
        tex.name = texture.name + "_Heg";
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return tex;
    }
    /// <summary>
    /// Expande a �rea branca.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    [Obsolete("M�todo ultrapassado por passagens por refer�ncias")]
    public static void ExpandWhiteArea(Texture2D texture)
    {
        Resources.UnloadUnusedAssets();
        Texture2D textureComparative = new Texture2D(texture.width, texture.height);
        TextureToOther(ref textureComparative, texture);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (textureComparative.GetPixel(x, y) == Color.white)
                    continue;

                if (textureComparative.GetPixel(x - 1, y) == Color.white || textureComparative.GetPixel(x + 1, y) == Color.white || textureComparative.GetPixel(x, y - 1) == Color.white || textureComparative.GetPixel(x, y + 1) == Color.white)
                    texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /// <summary>
    /// Expande a �rea preta.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <returns>Nova textura com a cor preta expandida.</returns>
    public static Texture2D ExpandBlackArea(Texture2D tex)
    {
        Texture2D texture;
        texture = GetTexture(tex);
        Texture2D textureComparative = new Texture2D(texture.width, texture.height);
        TextureToOther(ref textureComparative, texture);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (textureComparative.GetPixel(x, y) == Color.black)
                    continue;

                if (textureComparative.GetPixel(x - 1, y) == Color.black || textureComparative.GetPixel(x + 1, y) == Color.black || textureComparative.GetPixel(x, y - 1) == Color.black || textureComparative.GetPixel(x, y + 1) == Color.black)
                    texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return texture;
    }
    /// <summary>
    /// Expande a �rea branca.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="times">Quantas vezes expandir�.</param>
    [Obsolete("M�todo ultrapassado por passagens por refer�ncias")]
    public static void ExpandWhiteArea(Texture2D texture, int times)
    {
        Texture2D textureComparative = new Texture2D(texture.width, texture.height);
        TextureToOther(ref textureComparative, texture);
        for (int i = 0; i < times; i++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (textureComparative.GetPixel(x, y) == Color.white)
                        continue;

                    if (textureComparative.GetPixel(x - 1, y) == Color.white || textureComparative.GetPixel(x + 1, y) == Color.white || textureComparative.GetPixel(x, y - 1) == Color.white || textureComparative.GetPixel(x, y + 1) == Color.white)
                        texture.SetPixel(x, y, Color.white);
                }
            }
            texture.Apply();
            TextureToOther(ref textureComparative, texture);
        }
        texture.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /// <summary>
    /// Expande a �rea branca, uma vers�o atualizada de ExpandWhiteArea.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="times">Quantas vezes expandir�.</param>
    /// <returns>Nova textura com a cor branca expandida</returns>
    public static Texture2D ExpandWhite(Texture2D texture, int times)
    {
        Texture2D previousTex = GetTexture(texture);
        Texture2D newTex = GetTexture(texture);
        for (int i = 0; i < times; i++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (previousTex.GetPixel(x, y) == Color.white)
                        continue;

                    if (previousTex.GetPixel(x - 1, y) == Color.white || previousTex.GetPixel(x + 1, y) == Color.white || previousTex.GetPixel(x, y - 1) == Color.white || previousTex.GetPixel(x, y + 1) == Color.white)
                        newTex.SetPixel(x, y, Color.white);
                }
            }
            newTex.Apply();
            previousTex = GetTexture(newTex);
        }
        newTex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return newTex;
    }
    /// <summary>
    /// Expande a �rea branca, s� que as pontas tamb�m, evitando pontas arredondads.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="times">Quantas vezes expandir�.</param>
    /// <returns>Nova textura com a cor branca expandida</returns>
    public static Texture2D ExpandWhiteSquare(Texture2D texture, int times)
    {
        Texture2D previousTex = GetTexture(texture);
        Texture2D newTex = GetTexture(texture);
        for (int i = 0; i < times; i++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (previousTex.GetPixel(x, y) == Color.white)
                        for (int j = -1; j <= 1; j++)
                        {
                            for (int k = -1; k <= 1; k++)
                            {
                                if (j == 0 && k == 0)
                                    continue;
                                newTex.SetPixel(x + j, y + k, Color.white);
                            }
                        }
                }
            }
            newTex.Apply();
            previousTex = GetTexture(newTex);
        }
        newTex.Apply();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return newTex;
    }
    /// <summary>
    /// Borra a textura.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <returns>Nova textura borrada.</returns>
    public static Texture2D ApplyBlur(Texture2D texture, int times)
    {
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        Texture2D previousTexture = texture;
        for (int a = 0; a < times; a++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color color = Color.black;
                    float[] rgb = { 0, 0, 0 };

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i + x >= texture.width || x + i < 0 || j + y >= texture.height || y + j < 0)
                                continue;
                            
                            rgb[0] += previousTexture.GetPixel(x + i, y + j).r;
                            rgb[1] += previousTexture.GetPixel(x + i, y + j).g;
                            rgb[2] += previousTexture.GetPixel(x + i, y + j).b;
                        }
                    }
                    for (int i = 0; i < rgb.Length; i++)
                    {
                        rgb[i] /= 9;
                    }
                    color = new Color(rgb[0], rgb[1], rgb[2]);
                    newTexture.SetPixel(x, y, color);
                }
            }
            newTexture.Apply();
            previousTexture = newTexture;
            previousTexture.Apply();
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return newTexture;
    }
    /// <summary>
    /// Borra a textura com o elemento alfa.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <returns>Nova textura borrada.</returns>
    public static Texture2D ApplyBlurAlpha(Texture2D texture, int times)
    {
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        Texture2D previousTexture = GetTexture(texture);
        for (int a = 0; a < times; a++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color color = Color.clear;
                    float[] rgba = { 0, 0, 0, 0};

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i + x >= texture.width || x + i < 0 || j + y >= texture.height || y + j < 0)
                                continue;

                            rgba[0] += previousTexture.GetPixel(x + i, y + j).r;
                            rgba[1] += previousTexture.GetPixel(x + i, y + j).g;
                            rgba[2] += previousTexture.GetPixel(x + i, y + j).b;
                            rgba[3] += previousTexture.GetPixel(x + i, y + j).a;
                        }
                    }
                    for (int i = 0; i < rgba.Length; i++)
                    {
                        rgba[i] /= 9;
                    }
                    color = new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
                    newTexture.SetPixel(x, y, color);
                }
            }
            newTexture.Apply();
            previousTexture = GetTexture(newTexture);
            previousTexture.Apply();
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return newTexture;
    }
    /// <summary>
    /// Borra a textura somente na cor preta.
    /// </summary>
    /// <param name="texture">Textura original.</param>
    /// <param name="times">Quantas vezes repetir o processo.</param>
    /// <returns>Nova textura borrada.</returns>
    public static Texture2D ApplyBlurOnlyInBlack(Texture2D texture, int times)
    {
        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        Texture2D previousTexture = texture;
        for (int a = 0; a < times; a++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color color = Color.black;
                    float[] rgb = { 0, 0, 0 };

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i + x >= texture.width || x + i < 0 || j + y >= texture.height || y + j < 0)
                                continue;

                            rgb[0] += previousTexture.GetPixel(x + i, y + j).r;
                            rgb[1] += previousTexture.GetPixel(x + i, y + j).g;
                            rgb[2] += previousTexture.GetPixel(x + i, y + j).b;
                        }
                    }
                    for (int i = 0; i < rgb.Length; i++)
                    {
                        rgb[i] /= 9;
                    }
                    color = new Color(rgb[0], rgb[1], rgb[2], 1);
                    if (texture.GetPixel(x, y) != Color.white)
                        newTexture.SetPixel(x, y, color);
                }
            }
            newTexture.Apply();
            previousTexture = newTexture;
            previousTexture.Apply();
        }
        Resources.UnloadUnusedAssets();
        GC.Collect();
        return newTexture;
    }
    /// <summary>
    /// Mescla m�ltiplos sprites.
    /// </summary>
    /// <param name="width">Largura.</param>
    /// <param name="height">Altura.</param>
    /// <param name="sprites">Lista de sprites a ser fornecida, cada uma ser� mesclada com a outra, a textura de menor index ser� escrevida por cima da textura de maior index.</param>
    /// <param name="name">Nome do sprite.</param>
    /// <returns>Novo sprite mesclado.</returns>
    public static Sprite MergeSprites(int width, int height, Sprite[] sprites, string name)
    {
        Texture2D newTexture = new Texture2D(width, height);
        for (int x = 0; x < newTexture.width; x++)
        {
            for (int y = 0; y < newTexture.height; y++)
            {
                newTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
            }
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            int xx = 0, yy = 0;
            for (int x = (int)sprites[i].textureRect.position.x; x < (int)sprites[i].textureRect.position.x + newTexture.width; x++)
            {
                for (int y = (int)sprites[i].textureRect.position.y; y < (int)sprites[i].textureRect.position.y + newTexture.height; y++)
                {
                    Color color = sprites[i].texture.GetPixel(x, y);
                    if (color.a > 0)
                        newTexture.SetPixel(x, y, color);
                    /*else if (i - 1 >= 0 && color.a == 0)
                        newTexture.SetPixel(x, y, sprites[i - 1].texture.GetPixel((int)sprites[i - 1].textureRect.position.x + xx, (int)sprites[i - 1].textureRect.position.y + yy));*/
                    yy++;
                }
                xx++;
            }
            newTexture.Apply();
        }
        newTexture.filterMode = FilterMode.Point;
        newTexture.Compress(false);
        Sprite finalSprite = Sprite.Create(newTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16, 0, 0, new Vector4(0, 0, 0, 0), true);
        finalSprite.name = name;
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return finalSprite;
    }
    /// <summary>
    /// Mescla m�ltiplos sprites com alpha.
    /// </summary>
    /// <param name="width">Largura.</param>
    /// <param name="height">Altura.</param>
    /// <param name="sprites">Lista de sprites a ser fornecida, cada uma ser� mesclada com a outra, a textura de menor index ser� escrevida por cima da textura de maior index.</param>
    /// <param name="name">Nome do sprite.</param>
    /// <returns>Novo sprite mesclado.</returns>
    [Obsolete("M�todo n�o funcional")]
    public static Sprite MergeSpritesWithAlpha(int width, int height, Sprite[] sprites, string name)
    {
        Texture2D newTexture = new Texture2D(width, height);
        for (int x = 0; x < newTexture.width; x++)
        {
            for (int y = 0; y < newTexture.height; y++)
            {
                newTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
            }
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            for (int x = (int)sprites[i].textureRect.position.x; x < (int)sprites[i].textureRect.position.x + newTexture.width; x++)
            {
                for (int y = (int)sprites[i].textureRect.position.y; y < (int)sprites[i].textureRect.position.y + newTexture.height; y++)
                {
                    float nA = 1f - sprites[i].texture.GetPixel(x, y).a;
                    Color color = sprites[i].texture.GetPixel(x, y);
                    if (i - 1 >= 0)
                    {
                        color = new Color(
                            (sprites[i].texture.GetPixel(x, y).r - nA) + (sprites[i - 1].texture.GetPixel(x, y).r - sprites[i].texture.GetPixel(x, y).a),
                            (sprites[i].texture.GetPixel(x, y).g - nA) + (sprites[i - 1].texture.GetPixel(x, y).g - sprites[i].texture.GetPixel(x, y).a),
                            (sprites[i].texture.GetPixel(x, y).b - nA) + (sprites[i - 1].texture.GetPixel(x, y).b - sprites[i].texture.GetPixel(x, y).a),
                            1f);
                        if (sprites[i].texture.GetPixel(x, y).a > 0 && sprites[i - 1].texture.GetPixel(x, y).a == 1)
                            newTexture.SetPixel(x, y, color);
                    }
                    else
                    {
                        if (color.a > 0)
                            newTexture.SetPixel(x, y, color);
                    }
                }
            }
            newTexture.Apply();
        }
        newTexture.filterMode = FilterMode.Point;
        newTexture.Compress(false);
        Sprite finalSprite = Sprite.Create(newTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16, 0, 0, new Vector4(0, 0, 0, 0), true);
        finalSprite.name = name;
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return finalSprite;
    }
    /// <summary>
    /// Move pixeis de um sprite para um lado X ou Y.
    /// </summary>
    /// <param name="sprite">Sprite que ser� modificado.</param>
    /// <param name="moveX">Dire��o X.</param>
    /// <param name="moveY">Dire��o Y.</param>
    /// <returns>Novo sprite com os pixeis movidos.</returns>
    public static Sprite MovePixelsInSprite(Sprite sprite, int moveX, int moveY)
    {
        Texture2D newTexture = new Texture2D(sprite.texture.width, sprite.texture.height);
        for (int x = (int)sprite.textureRect.position.x; x < (int)sprite.textureRect.position.x + newTexture.width; x++)
        {
            for (int y = (int)sprite.textureRect.position.y; y < (int)sprite.textureRect.position.y + newTexture.height; y++)
            {
                int whereX = x + moveX, whereY = y + moveY;

                if (x + moveX >= sprite.texture.width)//x
                    whereX = (x + moveX) - sprite.texture.width;
                else if (x + moveX < 0)
                    whereX = (x + moveX) + sprite.texture.width;
                if (y + moveY >= sprite.texture.height)//y
                    whereY = (y + moveY) - sprite.texture.height;
                else if (y + moveY < 0)
                    whereY = (y + moveY) + sprite.texture.height;

                newTexture.SetPixel(whereX, whereY, sprite.texture.GetPixel(x, y));
            }
        }
        newTexture.Apply();
        newTexture.filterMode = FilterMode.Point;
        newTexture.Compress(false);
        Sprite finalSprite = Sprite.Create(newTexture, new Rect(sprite.textureRect.position.x, sprite.textureRect.position.y, sprite.texture.width, sprite.texture.height), new Vector2(0.5f, 0.5f), 16, 0, 0, new Vector4(0, 0, 0, 0), true);
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return finalSprite;
    }
    /// <summary>
    /// Desenha um c�rculo na textura fornecida.
    /// </summary>
    /// <param name="tex">Textura que ser� modificada.</param>
    /// <param name="color">A cor do c�rculo.</param>
    /// <param name="x">Posi��o X.</param>
    /// <param name="y">Posi��o Y.</param>
    /// <param name="radius">Raio do c�rculo.</param>
    /// <returns>Nova textura com um c�rculo desenhado.</returns>
    public static Texture2D DrawCircle(ref Texture2D tex, Color color, int x, int y, int radius = 3)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    tex.SetPixel(u, v, color);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return tex;
    }
    /// <summary>
    /// Gera um PNG com a textura fornecida.
    /// </summary>
    /// <param name="tex">Textura que ser� usada.</param>
    public static void GeneratePng(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/Resources/" + tex.name + ".png", bytes);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /// <summary>
    /// Gera um PNG com a textura fornecida transformada em cinza, usado para tiles.
    /// </summary>
    /// <param name="tex">Textura que ser� usada.</param>
    public static void GeneratePngTilesAlpha(Texture2D tex)
    {
        GC.Collect();
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                tex.SetPixel(x, y, new Color(0.5f, 0.5f, 0.5f, tex.GetPixel(x, y).a));
            }
        }
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/Resources/a.png", bytes);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    /*public static Color[] GeneratePallete()
    {

    }
    public static Texture2D GenerateMap(GameObject[,] map, Texture2D tex)
    {

    }*/
    /// <summary>
    /// Passa o valor alfa do borderSprite para criar um tile.
    /// </summary>
    /// <param name="tile">Sprite que ser� usado como tile.</param>
    /// <param name="borderSprite">Sprite que ser� usado para criar a borda.</param>
    /// <returns>Novo sprite com bordas novas.</returns>
    public static Sprite PassBorderAlpha(Sprite tile, Sprite borderSprite)
    {
        Texture2D newTexture = new(tile.texture.width, tile.texture.height);
        newTexture = GetTexture(tile.texture);
        newTexture.Apply();
        for (int x = 0; x < tile.texture.width; x++)
        {
            for (int y = 0; y < tile.texture.height; y++)
            {
                newTexture.SetPixel(x, y, new Color(newTexture.GetPixel(x, y).r, newTexture.GetPixel(x, y).g, newTexture.GetPixel(x, y).b, borderSprite.texture.GetPixel(x + (int)borderSprite.textureRect.position.x, y + (int)borderSprite.textureRect.position.y).a));
            }
        }
        newTexture.Apply();
        newTexture.filterMode = FilterMode.Point;
        newTexture.Compress(false);
        Sprite finalSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 16, 0, 0, new Vector4(0, 0, 0, 0), true);
        //return borderSprite;
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
        return finalSprite;
    }
    /// <summary>
    /// Separa pontos brancos at� a dist�ncia fornecida
    /// </summary>
    /// <param name="texture">A textura com os pontos brancos.</param>
    /// <param name="distance">A dist�ncia dos pixeis.</param>
    /// <param name="padding">A dist�ncia para a borda da textura.</param>
    /// <returns>Nova textura com os pixeis separados.</returns>
    public static Texture2D SeparateWhiteDots(Texture2D texture, int distance, int padding)
    {
        var newTex = GetTexture(texture);
        Color[,] colors = new Color[texture.width,texture.height];
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (x <= padding || y <= padding || x >= texture.width - padding || y >= texture.height - padding)
                {
                    newTex.SetPixel(x, y, Color.black);
                    continue;
                }
                if (newTex.GetPixel(x, y).r == 1)
                    for (int eX = -distance; eX < distance; eX++)
                        for (int eY = -distance; eY < distance; eY++)
                        {
                            if (eX == 0 && eY == 0)
                                continue;
                            if (IsInside2DArray(x + eX, y + eY, colors))
                                if (newTex.GetPixel(x + eX, y + eY).r == 1)
                                    newTex.SetPixel(x + eX, y + eY, Color.black);
                        }
            }
        }
        newTex.Apply();
        return newTex;
    }
    /// <summary>
    /// Pega caminhos desconexos ou n�o e os cataloga, com as cores, nessa ordem:
    /// <para>0 = vermelho</para> 
    /// <para>1 = amarelo</para> 
    /// <para>2 = ciano</para> 
    /// <para>3 = magenta</para> 
    /// <para>4 = azul</para> 
    /// <para>5 = laranja.</para> 
    /// <para>Caso o caminho tenha 1 pixel de tamanho ele � catalogado com a cor cinza.</para> 
    /// </summary>
    /// <param name="texture">A textura que cont�m os caminhos em branco.</param>
    /// <returns></returns>
    public static Texture2D DefineColorListForPaths(Texture2D texture)
    {
        var newTex = GetTexture(texture);
        Color[] colList = new Color[7];
        colList[0] = Color.red;
        colList[1] = Color.yellow;
        colList[2] = Color.cyan;
        colList[3] = Color.magenta;
        colList[4] = Color.blue;
        colList[5] = new Color(1, 0.5f, 0, 1);

        int index = 0;
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (newTex.GetPixel(x, y) == Color.black || newTex.GetPixel(x, y) == Color.gray)
                    continue;

                foreach (Color c in colList)
                {
                    if (newTex.GetPixel(x, y) == c)
                        goto Exit;
                }
                GetNeighbours(x, y, colList[index]);
                index++;
            Exit:;
            }
        }
        newTex.Apply();
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (newTex.GetPixel(x, y) == Color.black)
                    continue;
                if (texture.GetPixel(x, y) == Color.white && !Color32Equals((Color32)newTex.GetPixel(x, y), new Color32(128, 128, 128, 255)))
                {
                    //Debug.Log(newTex.GetPixel(x, y));
                    newTex.SetPixel(x, y, Color.white);
                }
            }
        }
        newTex.Apply();
        /*newTex.name = "Dungeon";
        GeneratePng(newTex);*/
        return newTex;
        void GetNeighbours(int x, int y, Color color)
        {
            if (newTex.GetPixel(x + 1, y) == Color.black &&
                newTex.GetPixel(x - 1, y) == Color.black &&
                newTex.GetPixel(x, y + 1) == Color.black &&
                newTex.GetPixel(x, y - 1) == Color.black)
            {
                newTex.SetPixel(x, y, Color.gray);
                return;
            }
            newTex.SetPixel(x, y, color);
            /*if (newTex.GetPixel(x, y) != Color.black && newTex.GetPixel(x, y) != Color.gray && newTex.GetPixel(x, y) != color && newTex.GetPixel(x, y) != Color.white)
            {
            }
            else
            {
                Debug.Log("Make");
            }*/

            //Extra X
            int Xe = x + 1;
            if (newTex.GetPixel(Xe, y) != Color.black && newTex.GetPixel(Xe, y) != Color.gray && newTex.GetPixel(Xe, y) != color)
            {
                newTex.SetPixel(Xe, y, color);
                GetNeighbours(Xe, y, color);
            }
            Xe = x - 1;
            if (newTex.GetPixel(Xe, y) != Color.black && newTex.GetPixel(Xe, y) != Color.gray && newTex.GetPixel(Xe, y) != color)
            {
                newTex.SetPixel(Xe, y, color);
                GetNeighbours(Xe, y, color);
            }
            //Extra Y
            int Ye = y + 1;
            if (newTex.GetPixel(x, Ye) != Color.black && newTex.GetPixel(x, Ye) != Color.gray && newTex.GetPixel(x, Ye) != color)
            {
                newTex.SetPixel(x, Ye, color);
                GetNeighbours(x, Ye, color);
            }
            Ye = y - 1;
            if (newTex.GetPixel(x, Ye) != Color.black && newTex.GetPixel(x, Ye) != Color.gray && newTex.GetPixel(x, Ye) != color)
            {
                newTex.SetPixel(x, Ye, color);
                GetNeighbours(x, Ye, color);
            }
        }
    }
    /// <summary>
    /// Script que aumenta pontos brancos �nicos para quadrados/ret�ngulos maiores que oscilam entre os tamanhos m�nimos e m�ximos de x e y, definindo tamb�m um espa�o m�ximo entre esses ret�ngulos.
    /// </summary>
    /// <param name="texture">A textura com os pontos.</param>
    /// <param name="minX">Tamanho m�nimo em x.</param>
    /// <param name="minY">Tamanho m�nimo em y.</param>
    /// <param name="maxX">Tamanho m�ximo em x.</param>
    /// <param name="maxY">Tamanho m�ximo em y</param>
    /// <param name="space">Espa�o m�ximo entre os ret�ngulos.</param>
    /// <returns></returns>
    public static Texture2D ExpandWhiteDotsRandomly(Texture2D texture, int minX, int minY, int maxX, int maxY, int space)
    {
        var newTex = GetTexture(texture);
        Color[,] colors = new Color[texture.width, texture.height];
        int _space = space + 1;

        Color32 undefinedColor = new Color32(64, 64, 64, 255);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (texture.GetPixel(x, y) == Color.white)
                {
                    newTex.SetPixel(x, y, Color.black);
                    newTex.Apply();
                    int Left = UnityEngine.Random.Range(-minX, -maxX);
                    int Right = UnityEngine.Random.Range(minX, maxX);

                    int Up = UnityEngine.Random.Range(minY, maxY);
                    int Down = UnityEngine.Random.Range(-minY, -maxY);

                    for (int xE = Left + x; xE < Right + x; xE++)
                    {
                        for (int yE = Down + y; yE < Up + y; yE++)
                        {
                            //ponto atual
                            if (!IsInside2DArray(xE, yE, colors) ||
                                //retos
                                !IsInside2DArray(xE + _space, yE, colors) ||
                                !IsInside2DArray(xE - _space, yE, colors) ||
                                !IsInside2DArray(xE, yE + _space, colors) ||
                                !IsInside2DArray(xE, yE - _space, colors) ||
                                //diagonais
                                !IsInside2DArray(xE + _space, yE + _space, colors) ||
                                !IsInside2DArray(xE - _space, yE + _space, colors) ||
                                !IsInside2DArray(xE + _space, yE - _space, colors) ||
                                !IsInside2DArray(xE - _space, yE - _space, colors))
                                continue;
                            //reto
                            if ((newTex.GetPixel(xE + space, yE) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE) == Color.black) ||
                                (newTex.GetPixel(xE, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE, yE - space) == Color.black) ||
                                //diagonais
                                (newTex.GetPixel(xE + space, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE + space) == Color.black) ||
                                (newTex.GetPixel(xE + space, yE - space) == Color.black) ||
                                (newTex.GetPixel(xE - space, yE - space) == Color.black))
                                newTex.SetPixel(xE, yE, undefinedColor);
                        }
                    }
                    newTex.Apply();

                    for (int xR = 0; xR < texture.width; xR++)
                    {
                        for (int yR = 0; yR < texture.height; yR++)
                        {
                            if (newTex.GetPixel(xR, yR) == undefinedColor)
                                newTex.SetPixel(xR, yR, Color.white);
                        }
                    }
                }
            }
        }

        newTex.Apply();
        return newTex;
    }
    public static Texture2D ExpandBlackOnNonWhite(Texture2D tex, int size)
    {
        var newTex = GetTexture(tex);
        Color32 undefinedColor = new Color32(64, 64, 64, 255);

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (tex.GetPixel(x, y) == Color.white)
                {
                    newTex.SetPixel(x, y, Color.black);
                }
                else if (tex.GetPixel(x, y) != Color.black)
                {
                    newTex.SetPixel(x, y, Color.white);
                }
            }
        }
        newTex.Apply();

        newTex = ExpandWhiteSquare(newTex, size);

        return newTex;
    }
    /// <summary>
    /// Funde v�rias texturas enquanto conserva somente a cor branca.
    /// </summary>
    /// <param name="texList">Lista de texturas que cont�m branco.</param>
    /// <returns>Nova textura com a cor branca preservada</returns>
    public static Texture2D MergeWhite(Texture2D[] texList)
    {
        Texture2D newTex = GetTexture(texList[0]);

        foreach (Texture2D tex in texList)
        {
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    if (tex.GetPixel(x, y) == Color.white)
                        newTex.SetPixel(x, y, Color.white);
                }
            }
        }
        newTex.Apply();
        return newTex;
    }
    /// <summary>
    /// Um comparador de cores de 32Bits, que � ausente por algum motivo, ent�o criei o meu.
    /// </summary>
    /// <param name="col1">Cor 1.</param>
    /// <param name="col2">Cor 2.</param>
    /// <returns>Se as duas cores s�o iguais retorna true, se n�o false.</returns>
    public static bool Color32Equals(Color32 col1, Color32 col2)
    {
        if (col1.r == col2.r &&
            col1.g == col2.g &&
            col1.b == col2.b &&
            col1.a == col2.a)
            return true;
        return false;
    }
    /// <summary>
    /// Verifica se a posi��o fornecida est� dentro dos limites do array 2D fornecido.
    /// </summary>
    /// <typeparam name="T">O tipo do array, pra suportar todos os tipos e classes, n�o � necess�rio colocar o tipo.</typeparam>
    /// <param name="x">Localiza��o X.</param>
    /// <param name="y">Localiza��o Y.</param>
    /// <param name="a">O array 2D que ser� usado de medida.</param>
    /// <returns></returns>
    public static bool IsInside2DArray<T>(int x, int y, T[,] a) => x >= 0 && y >= 0 && x < a.GetLength(0) && y < a.GetLength(1);
    /// <summary>
    /// Verifica se a posi��o fornecida est� dentro dos limites do array 3D fornecido.
    /// </summary>
    /// <typeparam name="T">O tipo do array, pra suportar todos os tipos e classes, n�o � necess�rio colocar o tipo.</typeparam>
    /// <param name="x">Localiza��o X.</param>
    /// <param name="y">Localiza��o Y.</param>
    /// <param name="z">Localiza��o Z.</param>
    /// <param name="a">O array 3D que ser� usado de medida.</param>
    /// <returns></returns>
    public static bool IsInside3DArray<T>(int x, int y, int z, T[,,] a) => x >= 0 && y >= 0 && z >= 0 && x < a.GetLength(0) && y < a.GetLength(1) && z < a.GetLength(2);
    /// <summary>
    /// Gera texturas procedurais em perlin noise.
    /// </summary>
    /// <param name="width">Largura da textura.</param>
    /// <param name="height">Altura da textura</param>
    /// <param name="seed">A semente que vai definir o estado atual.</param>
    /// <param name="frequency">A frequ�ncia, quanto maior mais densidade de pixeis, quanto menor mais forma��es unificadas.</param>
    /// <param name="limit">Quanto maior o limite menor a exten��o da gera��o da �rea branca.</param>
    /// <param name="scattering">Gera efeito spray nas bordas</param>
    /// <param name="polate">Se verdadeiro a textura ser� preto e branco, se falso ser� uma escala de cinza.</param>
    /// <returns></returns>
    public static Texture2D GenerateNoiseTexture(int width, int height, int seed, float frequency, float limit, float scattering, bool polate)
    {
        Texture2D noiseTexture = new Texture2D(width, height);
        var sed = UnityEngine.Random.seed;
        UnityEngine.Random.InitState(seed);
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (!polate)
                    noiseTexture.SetPixel(x, y, new Color(v, v, v, 1));
                else
                    if (v + UnityEngine.Random.Range(-scattering, scattering) > limit)
                        noiseTexture.SetPixel(x, y, Color.white);
                    else
                        noiseTexture.SetPixel(x, y, Color.black);
            }
        }
        UnityEngine.Random.InitState(sed);
        noiseTexture.Apply();
        return noiseTexture;
    }
    /// <summary>
    /// Gera caminhos entre pixeis brancos, �timo para gerar Dungeons!
    /// </summary>
    /// <param name="texture">A textura com os pontos.</param>
    /// <returns>Nova textura com os caminhos em verde. (Color.green)</returns>
    public static Texture2D GeneratePathsOnMap(Texture2D texture)
    {
        Texture2D dotMap = GetTexture(texture);

        Pathfinding pathfinding = new Pathfinding(texture.width, texture.height);

        pathfinding.MOVE_DIAGONAL_COST = 25;
        List<Vector2> pointsFound = new List<Vector2>();
        List<Vector2> pointsReached = new List<Vector2>();
        List<Vector2> pointsAnalyzed = new List<Vector2>();

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (dotMap.GetPixel(x, y).r == 0)
                    continue;
                pointsFound.Add(new Vector2(x, y));
            }
        }
        Vector2 point1 = pointsFound[UnityEngine.Random.Range(0, pointsFound.Count - 1)];
        Vector2 point2 = new Vector2(-1000, -1000);

        for (int i = 0; i < pointsFound.Count - 1; i++)
        {
            int range = 10;
            bool Rx = UnityEngine.Random.Range(0, range) < range / 2 ? false : true;
            bool Ry = UnityEngine.Random.Range(0, range) < range / 2 ? false : true;

            //Debug.Log("The interaction " + i + " has the parameters: X = " + Rx + " Y = " + Ry);

            bool firstDot = true;
            for (int x = Rx == true ? 0 : texture.width - 1;
                x >= 0 && x < texture.height;
                x = Rx == true ? x + 1 : x - 1)
            {
                for (int y = Ry == true ? 0 : texture.width - 1;
                    y >= 0 && y < texture.height;
                    y = Ry == true ? y + 1 : y - 1)
                {
                    if (firstDot)
                    {
                        if (dotMap.GetPixel(x, y).r == 1)
                        {
                            if (IsReachedPoint(new Vector2(x, y)))
                                continue;
                            point1 = new Vector2(x, y);
                            pointsReached.Add(point1);
                            firstDot = false;
                        }
                    }
                    else
                    {
                        if (dotMap.GetPixel(x, y).r == 0 || (x == (int)point1.x && y == (int)point1.y) || IsReachedPoint(new Vector2(x, y)))
                            continue;
                        point2 = Vector2.Distance(point1, new Vector2(x, y)) < Vector2.Distance(point1, point2) ? new Vector2(x, y) : point2;
                    }
                }
            }
            foreach (PathNode node in pathfinding.FindPath((int)point1.x, (int)point1.y, (int)point2.x, (int)point2.y).ToArray())
            {
                if (node == null || dotMap.GetPixel(node.x, node.y) == Color.white)
                    continue;
                dotMap.SetPixel(node.x, node.y, Color.green);
            }
            point1 = point2;
            point2 = new Vector2(-1000, -1000);
        }
        /*float a = 1f;
        foreach (Vector2 v in pointsParenting)
        {
            dotMap.SetPixel((int)v.x, (int)v.y, new Color(1, a, a, 1));
            a -= 1f / pointsParenting.Count;
            Debug.Log(a);
        }*/
        dotMap.Apply();
        return dotMap;

        bool IsReachedPoint(Vector2 currentPoint)
        {
            foreach (Vector2 point in pointsReached)
            {
                if (point.Equals(currentPoint))
                    return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Multiplica o tamanho da textura.
    /// </summary>
    /// <param name="texture">A textura base.</param>
    /// <param name="widthMultiplier">Muliplicador da largura.</param>
    /// <param name="heightMultiplier">Muliplicador da altura.</param>
    /// <returns>Nova textura com o novo tamanho.</returns>
    public static Texture2D ResizeTextureUp(Texture2D texture, int widthMultiplier, int heightMultiplier)
    {
        var newTex = new Texture2D (texture.width * widthMultiplier, texture.height * heightMultiplier);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int eX = 0; eX <= 2; eX++)
                    for (int eY = 0; eY <= 2; eY++)
                    {
                        newTex.SetPixel((x * widthMultiplier) + eX, (y * heightMultiplier) + eY, texture.GetPixel(x, y));
                    }
            }
        }
        newTex.Apply();
        return newTex;
    }
}