## How to generate this shader cheat-sheet:

- Go to `Master_ToonShader`, look at the inspector and click on "View Generated Shader"
- Overwrite `ToonShaderCompiled.shader` contents with the generated shader.
- Rename the shader to `DCL/Toon Shader`
- Look for the passes with `UniversalPipeline` tag, there should be two of them.

### For each "UniversalPipeline" pass:

#### Replace everything under "Render State" comment for the following:

```
    Cull [_Cull]
    Blend [_SrcBlend] [_DstBlend]
    ZTest LEqual
    ZWrite [_ZWrite]
```

> NOTE: If this is not done, transparent wearables will not work.

#### Tags cleanup

Remove all the tags, the only tag left should be "UniversalPipeline". The tags for each pass should look like this:

```
Tags
{
  "LightMode" = "UniversalForward"
}
```

> NOTE: If the tags aren't removed, the avatars will be rendered on the Transparent stage of the pipeline. That's bad!.

#### Change forward pass for custom one

Do a search of replace of "PBRForwardPass.hlsl". You will note that the include takes the file from the URP package. The
custom pass in this project should be used instead.

To achieve this, you must replace all the includes of this file for just:

    #include "PBRForwardPass.hlsl"

> NOTE: If this is not done, avatars aren't going to look toon.

### You're done!.

Yes, the project needs a tool for automating this.  