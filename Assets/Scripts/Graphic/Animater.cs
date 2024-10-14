using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Classe feita para a criação e controle de animações 2D por sprites mais simplificada.
/// </summary>
public class Animater
{
    /// <summary>
    /// O SpriteRenderer que vai renderizar a animação.
    /// </summary>
    public SpriteRenderer Renderer;
    /// <summary>
    /// A animação atual.
    /// </summary>
    public Anime currentAnimation { get; private set; }
    private Coroutine coroutine;
    /// <summary>
    /// Define a velocidade da animação.
    /// </summary>
    public float speed = 1f;
    /// <summary>
    /// Congela a animação.
    /// </summary>
    public bool freezeAnimation = false;

    /// <summary>
    /// Construtor do animador, necessário para fazer as animações.
    /// </summary>
    /// <param name="renderer">O SpriteRenderer que vai renderizar a animação.</param>
    public Animater(SpriteRenderer renderer)
    {
        Renderer = renderer;
    }

    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus parâmetros.
    /// </summary>
    /// <param name="mono">O Monobehaviour responsável por processar a coroutine.</param>
    /// <param name="animation">Elementos da animação no formato da classe Anime.</param>
    /// <returns>A corrotina da animação.</returns>
    public Coroutine Animate(MonoBehaviour mono, Anime animation)
    {
        //Se a animação atual for nula
        if (currentAnimation == null ||
            //ou se o nome for diferente da animação atual, ou se a animação não for loopeada, se não estiver congelado e se a animação atual não tem prioridade
            ((animation.name != currentAnimation.name || !currentAnimation.looped) && !freezeAnimation && !currentAnimation.prioritizeAnimation) 
            )
        {
            currentAnimation = animation;
            if (coroutine != null)
                mono.StopCoroutine(coroutine);
            return coroutine = mono.StartCoroutine(_Animate(animation));
        }
        return coroutine;
    }
    /// <summary>
    /// Criador das corrotinas de animação.
    /// </summary>
    /// <param name="animation">Instância da classe Anime que carregará as informações.</param>
    /// <returns></returns>
    private IEnumerator _Animate(Anime animation)
    {
        if (animation.looped)
        {
            currentAnimation.prioritizeAnimation = false;
            while (animation.looped)
                foreach (Sprite sprite in animation.frameList)
                {
                    Renderer.sprite = sprite;
                    if (freezeAnimation)
                        for (int i = 0; i < 2; i++)
                            yield return new WaitUntil(() => { return freezeAnimation == false; });
                    else
                        yield return new WaitForSeconds((1 / animation.FPS) / speed);
                }
        }
        else
        {
            foreach (Sprite sprite in animation.frameList)
            {
                Renderer.sprite = sprite;
                if (freezeAnimation)
                    for (int i = 0; i < 2; i++)
                        yield return new WaitUntil(() => { return freezeAnimation == false; });
                else
                    yield return new WaitForSeconds((1 / animation.FPS) / speed);
            }
        }
        currentAnimation.prioritizeAnimation = false;
    }
}

/// <summary>
/// Classe feita para a criação de animações 2D usando lista de sprites e parâmetros.
/// </summary>
public class Anime
{
    /// <summary>
    /// Nome da animação, usada para diferenciar as animações e trocar elas quando os nomes forem diferentes.
    /// </summary>
    public string name = "";
    /// <summary>
    /// Vetor de sprites da animação.
    /// </summary>
    public Sprite[] frameList;
    /// <summary>
    /// Define se a animação é em loop.
    /// </summary>
    public bool looped = false;
    /// <summary>
    /// Define a quantidade de frames por segundo. (frames per second)
    /// </summary>
    public float FPS = 10;
    /// <summary>
    /// Prioritiza a animação atual para não ser sobreposta por outra, (funciona só com animações sem loop) default false.
    /// </summary>
    public bool prioritizeAnimation = false;

    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus parâmetros.
    /// </summary>
    /// <param name="name">Nome da animação, usada para diferenciar as animações e trocar elas quando os nomes forem diferentes.</param>
    /// <param name="frameList">Vetor de sprites da animação.</param>
    /// <param name="looped">Define se a animação é em loop.</param>
    /// <param name="fPS">Define a quantidade de frames por segundo. (frames per second)</param>
    /// <returns>Nova classe de animação</returns>
    public Anime(string name, Sprite[] frameList, bool looped, float fPS)
    {
        this.name = name;
        this.frameList = frameList;
        this.looped = looped;
        FPS = fPS;
    }
    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus parâmetros.
    /// </summary>
    /// <param name="name">Nome da animação, usada para diferenciar as animações e trocar elas quando os nomes forem diferentes.</param>
    /// <param name="frameList">Vetor de sprites da animação.</param>
    /// <param name="looped">Define se a animação é em loop.</param>
    /// <param name="fPS">Define a quantidade de frames por segundo. (frames per second)</param>
    /// <param name="prioritize">Define se essa animação será priorizada sobre outras que não tem o mesmo privilégio.</param>
    /// <returns>Nova classe de animação</returns>
    public Anime(string name, Sprite[] frameList, bool looped, float fPS, bool prioritize)
    {
        this.name = name;
        this.frameList = frameList;
        this.looped = looped;
        FPS = fPS;
        prioritizeAnimation = looped == false && prioritize;
    }
}