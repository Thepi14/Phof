using System.Collections;
using System.Collections.Generic;
using ObjectUtils;
using TMPro;
using UnityEngine;
using static GamePlayer;

public class Coordinates : MonoBehaviour
{
    public TextMeshProUGUI Text => gameObject.GetGameObjectComponent<TextMeshProUGUI>("Text");
    public void Update()
    {
        Text.gameObject.SetActive(PlayerPreferences.ShowCoordinates);
        if (player != null && PlayerPreferences.ShowCoordinates)
        {
            var x = ((int)player.transform.position.x).ToString();
            var y = ((int)player.transform.position.z).ToString();
            var xT = player.transform.position.x.ToString();
            var yT = player.transform.position.z.ToString();
            if (xT.Length <= 4 || yT.Length <= 4)
                Text.text = $"X: {xT} Y: {yT}";
            else if (xT.Length > x.Length + 3 && yT.Length > y.Length + 3)
                Text.text = $"X: {xT.Remove(x.Length + 3)} Y: {yT.Remove(y.Length + 3)}";
        }
    }
}
