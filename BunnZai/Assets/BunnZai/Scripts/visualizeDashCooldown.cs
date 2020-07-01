using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class visualizeDashCooldown : MonoBehaviour
{
    private Player mPlayer;
    private Slider mSlider;
    // Start is called before the first frame update
    void Start()
    {
        mPlayer = FindObjectOfType<Player>();
        mSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        mSlider.value = mPlayer.mDashTimer / 3;
    }
}
