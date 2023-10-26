using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    bool framePassed;

    void Update()
    {
        if (framePassed)
            SceneLoader.loadingCallback();
        if (!framePassed)
            framePassed = true;
    }
}
