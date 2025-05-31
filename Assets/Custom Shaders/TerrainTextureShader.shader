Shader "Custom/TerrainTextureShader"
{
    Properties
    {
        _GrassTex("Grass Texture", 2D) = "white" {}
        _DirtTex("Dirt Texture", 2D) = "white"{}
        _RockTex("Rock Texture", 2D) = "white"{}
        _SnowTex("Snow Texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 color : COLOR;
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _GrassTex;
            sampler2D _DirtTex;
            sampler2D _RockTex;
            sampler2D _SnowTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vpos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float height = i.color.y;
                fixed4 grassTex = tex2D(_GrassTex, i.uv) * smoothstep(0.2, 0.4, height);
                fixed4 dirtTex = tex2D(_DirtTex, i.uv) * smoothstep(0.4, 0.6, height);
                fixed4 rockTex = tex2D(_RockTex, i.uv) * smoothstep(0.6, 0.8, height);
                fixed4 snowTex = tex2D(_SnowTex, i.uv) * smoothstep(0.8, 1.0, height);
                return grassTex + dirtTex + rockTex + snowTex;
            }
            ENDCG
        }
    }
}
