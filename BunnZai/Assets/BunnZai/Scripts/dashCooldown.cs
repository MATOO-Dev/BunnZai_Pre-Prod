using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dashCooldown : MonoBehaviour
{
    private Slider slider;
    private Player player;

    void Awake()
    {
        slider = GetComponent<Slider>();
        player = FindObjectOfType<Player>();
    }
    // Update is called once per frame
    void Update()
    {
        slider.value = player.mDashTimer / player.mDashCooldown;
    }
}
