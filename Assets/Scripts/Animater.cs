using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animater
{
    public SpriteRenderer Renderer;
    private Anime currentAnimation;
    private Coroutine coroutine;
    
    public Animater (SpriteRenderer renderer) => Renderer = renderer;
    public Coroutine Animate(MonoBehaviour mono, Anime animation)
    {
        if (currentAnimation == null || animation.name != currentAnimation.name || !currentAnimation.looped)
        {
            currentAnimation = animation;
            if (coroutine != null)
                mono.StopCoroutine(coroutine);
            return coroutine = mono.StartCoroutine(_Animate(animation));
        }
        return coroutine;
    }
    private IEnumerator _Animate(Anime animation)
    {
        if (animation.looped)
        while (animation.looped)
            foreach(Sprite sprite in animation.frameList)
            {
                Renderer.sprite = sprite;
                yield return new WaitForSeconds(1 / animation.FPS);
            }
        else
            foreach (Sprite sprite in animation.frameList)
            {
                Renderer.sprite = sprite;
                yield return new WaitForSeconds(1 / animation.FPS);
            }
    }
}
public class Anime
{
    public string name = "";
    public Sprite[] frameList;
    public bool looped = false;
    public float FPS = 10;

    public Anime(string name, Sprite[] frameList, bool looped, float fPS)
    {
        this.name = name;
        this.frameList = frameList;
        this.looped = looped;
        FPS = fPS;
    }
}