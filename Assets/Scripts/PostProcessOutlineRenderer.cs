using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    public override void Render(PostProcessRenderContext context)
    {
        PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/Outline"));
        propertySheet.properties.SetFloat("_Thickness",settings.thickness);
        propertySheet.properties.SetFloat("_MinDepth",settings.depthMin);
        propertySheet.properties.SetFloat("_MaxDepth",settings.depthMax);
        
        context.command.BlitFullscreenTriangle(context.source,context.destination,propertySheet,0);
    }
}
