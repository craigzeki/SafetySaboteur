using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iCanvasController
{
    public void DoLoad(Action callback = null);
    public void DoUnload(Action callback = null);
}
