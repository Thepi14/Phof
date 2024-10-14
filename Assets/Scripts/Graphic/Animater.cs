using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Classe feita para a cria��o e controle de anima��es 2D por sprites mais simplificada.
/// </summary>
public class Animater
{
    /// <summary>
    /// O SpriteRenderer que vai renderizar a anima��o.
    /// </summary>
    public SpriteRenderer Renderer;
    /// <summary>
    /// A anima��o atual.
    /// </summary>
    public Anime currentAnimation { get; private set; }
    private Coroutine coroutine;
    /// <summary>
    /// Define a velocidade da anima��o.
    /// </summary>
    public float speed = 1f;
    /// <summary>
    /// Congela a anima��o.
    /// </summary>
    public bool freezeAnimation = false;

    /// <summary>
    /// Construtor do animador, necess�rio para fazer as anima��es.
    /// </summary>
    /// <param name="renderer">O SpriteRenderer que vai renderizar a anima��o.</param>
    public Animater(SpriteRenderer renderer)
    {
        Renderer = renderer;
    }

    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus par�metros.
    /// </summary>
    /// <param name="mono">O Monobehaviour respons�vel por processar a coroutine.</param>
    /// <param name="animation">Elementos da anima��o no formato da classe Anime.</param>
    /// <returns>A corrotina da anima��o.</returns>
    public Coroutine Animate(MonoBehaviour mono, Anime animation)
    {
        //Se a anima��o atual for nula
        if (currentAnimation == null ||
            //ou se o nome for diferente da anima��o atual, ou se a anima��o n�o for loopeada, se n�o estiver congelado e se a anima��o atual n�o tem prioridade
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
    /// Criador das corrotinas de anima��o.
    /// </summary>
    /// <param name="animation">Inst�ncia da classe Anime que carregar� as informa��es.</param>
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
/// Classe feita para a cria��o de anima��es 2D usando lista de sprites e par�metros.
/// </summary>
public class Anime
{
    /// <summary>
    /// Nome da anima��o, usada para diferenciar as anima��es e trocar elas quando os nomes forem diferentes.
    /// </summary>
    public string name = "";
    /// <summary>
    /// Vetor de sprites da anima��o.
    /// </summary>
    public Sprite[] frameList;
    /// <summary>
    /// Define se a anima��o � em loop.
    /// </summary>
    public bool looped = false;
    /// <summary>
    /// Define a quantidade de frames por segundo. (frames per second)
    /// </summary>
    public float FPS = 10;
    /// <summary>
    /// Prioritiza a anima��o atual para n�o ser sobreposta por outra, (funciona s� com anima��es sem loop) default false.
    /// </summary>
    public bool prioritizeAnimation = false;

    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus par�metros.
    /// </summary>
    /// <param name="name">Nome da anima��o, usada para diferenciar as anima��es e trocar elas quando os nomes forem diferentes.</param>
    /// <param name="frameList">Vetor de sprites da anima��o.</param>
    /// <param name="looped">Define se a anima��o � em loop.</param>
    /// <param name="fPS">Define a quantidade de frames por segundo. (frames per second)</param>
    /// <returns>Nova classe de anima��o</returns>
    public Anime(string name, Sprite[] frameList, bool looped, float fPS)
    {
        this.name = name;
        this.frameList = frameList;
        this.looped = looped;
        FPS = fPS;
    }
    /// <summary>
    /// Anima o SpriteRenderer escolhido criando uma corrotina usando a classe Anime e seus par�metros.
    /// </summary>
    /// <param name="name">Nome da anima��o, usada para diferenciar as anima��es e trocar elas quando os nomes forem diferentes.</param>
    /// <param name="frameList">Vetor de sprites da anima��o.</param>
    /// <param name="looped">Define se a anima��o � em loop.</param>
    /// <param name="fPS">Define a quantidade de frames por segundo. (frames per second)</param>
    /// <param name="prioritize">Define se essa anima��o ser� priorizada sobre outras que n�o tem o mesmo privil�gio.</param>
    /// <returns>Nova classe de anima��o</returns>
    public Anime(string name, Sprite[] frameList, bool looped, float fPS, bool prioritize)
    {
        this.name = name;
        this.frameList = frameList;
        this.looped = looped;
        FPS = fPS;
        prioritizeAnimation = looped == false && prioritize;
    }
}