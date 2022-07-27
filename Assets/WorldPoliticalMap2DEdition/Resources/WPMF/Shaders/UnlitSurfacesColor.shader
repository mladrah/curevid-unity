Shader "World Political Map 2D/Unlit Surface Single Color" {
 
Properties {
    [NoScaleOffset] _MainTex("Sprite", 2D) = "white" { }
    _Color ("Color", Color) = (1,1,1)
}
 
SubShader {
    Color  [_Color]
        Tags {
        "Queue"="Geometry+1"
        "RenderType"="Opaque"
    	}
    ZWrite Off
    Pass {
    }
}
 
}
