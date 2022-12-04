//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/player"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Outline_Size_1("_Outline_Size_1", Range(1, 16)) = 1
_Outline_Color_1("_Outline_Color_1", COLOR) = (0,1,0.9708953,1)
_Outline_HDR_1("_Outline_HDR_1", Range(0, 2)) = 1
_Brightness_Fade_1("_Brightness_Fade_1", Range(0, 1)) = 0.22
_ColorRGBA_Color_2("_ColorRGBA_Color_2", COLOR) = (1,0.9598201,0,1)
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float _Outline_Size_1;
float4 _Outline_Color_1;
float _Outline_HDR_1;
float _Brightness_Fade_1;
float4 _ColorRGBA_Color_2;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 OutLine(float2 uv,sampler2D source, float value, float4 color, float HDR)
{

value*=0.01;
float4 mainColor = tex2D(source, uv + float2(-value, value))
+ tex2D(source, uv + float2(value, -value))
+ tex2D(source, uv + float2(value, value))
+ tex2D(source, uv - float2(value, value));

color *= HDR;
mainColor.rgb = color;
float4 addcolor = tex2D(source, uv);
if (mainColor.a > 0.40) { mainColor = color; }
if (addcolor.a > 0.40) { mainColor = addcolor; mainColor.a = addcolor.a; }
return mainColor;
}
float CMPFXrng2(float2 seed)
{
return frac(sin(dot(seed * floor(50 + (_Time + 0.1) * 12.), float2(127.1, 311.7))) * 43758.5453123);
}

float CMPFXrng(float seed)
{
return CMPFXrng2(float2(seed, 1.0));
}

float4 CompressionFX(float2 uv, sampler2D source,float Value)
{
float2 blockS = floor(uv * float2(24., 19.))*4.0;
float2 blockL = floor(uv * float2(38., 14.))*4.0;
float r = CMPFXrng2(uv);
float lineNoise = pow(CMPFXrng2(blockS), 3.0) *Value* pow(CMPFXrng2(blockL), 3.0);
float4 col1 = tex2D(source, uv + float2(lineNoise * 0.02 * CMPFXrng(2.0), 0));
float4 result = float4(float3(col1.x, col1.y, col1.z), 1.0);
result.a = col1.a;
return result;
}
float4 ColorRGBA(float4 txt, float4 color)
{
txt.rgb += color.rgb;
return txt;
}
float4 Brightness(float4 txt, float value)
{
txt.rgb += value;
return txt;
}
float4 ColorHSV(float4 RGBA, float HueShift, float Sat, float Val)
{

float4 RESULT = float4(RGBA);
float a1 = Val*Sat;
float a2 = HueShift*3.14159265 / 180;
float VSU = a1*cos(a2);
float VSW = a1*sin(a2);

RESULT.x = (.299*Val + .701*VSU + .168*VSW)*RGBA.x
+ (.587*Val - .587*VSU + .330*VSW)*RGBA.y
+ (.114*Val - .114*VSU - .497*VSW)*RGBA.z;

RESULT.y = (.299*Val - .299*VSU - .328*VSW)*RGBA.x
+ (.587*Val + .413*VSU + .035*VSW)*RGBA.y
+ (.114*Val - .114*VSU + .292*VSW)*RGBA.z;

RESULT.z = (.299*Val - .3*VSU + 1.25*VSW)*RGBA.x
+ (.587*Val - .588*VSU - 1.05*VSW)*RGBA.y
+ (.114*Val + .886*VSU - .203*VSW)*RGBA.z;

return RESULT;
}
float InnerGlowAlpha(sampler2D source, float2 uv)
{
return (1 - tex2D(source, uv).a);
}
float4 InnerGlow(float2 uv, sampler2D source, float Intensity, float size, float4 color)
{
float step1 = 0.00390625f * size*2;
float step2 = step1 * 2;
float4 result = float4 (0, 0, 0, 0);
float2 texCoord = float2(0, 0);
texCoord = uv + float2(-step2, -step2); result += InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step1, -step2); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(0, -step2); result += 6.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step1, -step2); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step2, -step2); result += InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step2, -step1); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step1, -step1); result += 16.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(0, -step1); result += 24.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step1, -step1); result += 16.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step2, -step1); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step2, 0); result += 6.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step1, 0); result += 24.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv; result += 36.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step1, 0); result += 24.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step2, 0); result += 6.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step2, step1); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step1, step1); result += 16.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(0, step1); result += 24.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step1, step1); result += 16.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step2, step1); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step2, step2); result += InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(-step1, step2); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(0, step2); result += 6.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step1, step2); result += 4.0 * InnerGlowAlpha(source, texCoord);
texCoord = uv + float2(step2, step2); result += InnerGlowAlpha(source, texCoord);
result = result*0.00390625;
result = lerp(tex2D(source,uv),color*Intensity,result*color.a);
result.a = tex2D(source, uv).a;
return saturate(result);
}
float4 frag (v2f i) : COLOR
{
float4 _Outline_1 = OutLine(i.texcoord,_MainTex,_Outline_Size_1,_Outline_Color_1,_Outline_HDR_1);
float4 Brightness_1 = Brightness(_Outline_1,_Brightness_Fade_1);
float4 ColorRGBA_2 = ColorRGBA(Brightness_1,_ColorRGBA_Color_2);
float4 FinalResult = ColorRGBA_2;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
