using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontiniApe : NPC
{
    public override void InitializeNPC(NPCAnchor anchor)
    {
        if (anchor != null) MoveToAnchor(anchor);
    }
}
