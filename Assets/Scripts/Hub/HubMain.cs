using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubMain : MonoBehaviour
{
    void Start()
    {
        ScreensController.Instance.OpenScreen(ScreenIdentifiers.HeroSelect);
    }
}
