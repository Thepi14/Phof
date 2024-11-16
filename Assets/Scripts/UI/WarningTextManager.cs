using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Configurador para mensagens de aviso.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class WarningTextManager : MonoBehaviour
{
    /// <summary>
    /// Referência única do configurador para mensagens de aviso.
    /// </summary>
    public static WarningTextManager warningTextManagerInstance { get; private set; }
    public TextMeshProUGUI warningText => GetComponent<TextMeshProUGUI>();
    private const float TIME_SCALE = 0.01f;
    private string currentMsg = "";
    private int msgRepetition = 0;

    void Start()
    {
        if (warningTextManagerInstance == null)
            warningTextManagerInstance = this;
        else
            Destroy(gameObject);
        warningText.color = new Color(1, 1, 1, 0);
        DontDestroyOnLoad(warningTextManagerInstance);
    }
    /// <summary>
    /// Mostra uma mensagem de aviso na tela por um dado tempo.
    /// </summary>
    /// <param name="text">Texto exibido.</param>
    /// <param name="time">Tem em que o texto ficará na tela, tirando o tempo do fade.</param>
    /// <param name="fade">Tempo do efeito de fade in e fade out.</param>
    /// <param name="col">Cor do texto.</param>
    public static void ShowWarning(string text, float time, float fade = 0, Color? col = null)
    {
        warningTextManagerInstance.StopAllCoroutines();

        if (col == null)
            col = Color.white;

        if (warningTextManagerInstance.currentMsg != text)
        {
            warningTextManagerInstance.msgRepetition = 1;
            warningTextManagerInstance.currentMsg = text;
        }
        else
        {
            warningTextManagerInstance.msgRepetition++;
        }
        warningTextManagerInstance.StartCoroutine(warningTextManagerInstance._ShowWarning(text, time, fade, (Color)col));
    }
    private IEnumerator _ShowWarning(string text, float time, float fade, Color col)
    {
        warningText.text = text + (msgRepetition > 1 ? (" (x" + msgRepetition + ")") : "");

        if (fade > 0)
            for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
            {
                warningText.color = new Color(col.r, col.g, col.b, i);
                yield return new WaitForSeconds(TIME_SCALE);
            }
        warningText.color = new Color(col.r, col.g, col.b, 1);
        for (int i = 0; i < 1; i++)
        {
            yield return new WaitForSeconds(time);
        }
        if (fade > 0)
            for (float i = 0; i <= 1; i += TIME_SCALE / fade)
            {
                warningText.color = new Color(col.r, col.g, col.b, 1 - i);
                yield return new WaitForSeconds(TIME_SCALE);
            }
        warningText.color = new Color(1, 1, 1, 0);
        warningText.text = "";
        msgRepetition = 0;
    }
    /// <summary>
    /// Mostra uma mensagem de aviso na tela até que a função HideWarning() seja chamada.
    /// </summary>
    /// <param name="text">Texto exibido.</param>
    /// <param name="fade">Tempo do efeito do fade in.</param>
    /// <param name="col">Cor do texto.</param>
    public static void ShowAndKeepWarning(string text, float fade = 0, Color? col = null)
    {
        warningTextManagerInstance.StopAllCoroutines();

        if (col == null)
            col = Color.white;

        if (warningTextManagerInstance.currentMsg != text)
        {
            warningTextManagerInstance.msgRepetition = 1;
            warningTextManagerInstance.currentMsg = text;
        }
        else
        {
            warningTextManagerInstance.msgRepetition++;
        }

        warningTextManagerInstance.StartCoroutine(warningTextManagerInstance._ShowAndKeepWarning(text, fade, (Color)col));
    }
    private IEnumerator _ShowAndKeepWarning(string text, float fade, Color col)
    {
        warningText.text = text + (msgRepetition > 1 ? (" (x" + msgRepetition + ")") : "");

        if (fade > 0)
            for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
            {
                warningText.color = new Color(col.r, col.g, col.b, i);
                yield return new WaitForSeconds(TIME_SCALE);
            }
        warningText.color = new Color(col.r, col.g, col.b, 1);
    }
    /// <summary>
    /// Retira a mensagem de aviso da tela.
    /// </summary>
    /// <param name="fade">Tempo do efeito do fade out.</param>
    public static void HideWarning(float fade = 0)
    {
        if (fade > 0)
        {
            warningTextManagerInstance.StopAllCoroutines();
            warningTextManagerInstance.StartCoroutine(warningTextManagerInstance.FadeOut(fade));
        }
        else
        {
            warningTextManagerInstance.StopAllCoroutines();
            warningTextManagerInstance.warningText.color = new Color(1, 1, 1, 0);
            warningTextManagerInstance.warningText.text = "";
            warningTextManagerInstance.msgRepetition = 0;
        }
    }
    private IEnumerator FadeOut(float fade)
    {
        if (fade > 0)
            for (float i = warningText.color.a; i <= 1; i += TIME_SCALE / fade)
            {
                warningText.color = new Color(warningText.color.r, warningText.color.g, warningText.color.b, 1 - i);
                yield return new WaitForSeconds(TIME_SCALE);
            }
        warningText.color = new Color(1, 1, 1, 0);
        warningText.text = "";
        msgRepetition = 0;
    }
}
