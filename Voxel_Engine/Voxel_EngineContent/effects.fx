//-------------------------------------//
//          Panacea Creations          //
// Textured Pixel Shader With Lighting //
//									   //
// version 1.0_0					   //
// Caleb Simpson					   //
// August 20th 2011					   //
//-------------------------------------//

struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- Constants --------
#define MaxLights 20

float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightPosition[MaxLights];
float3 xLightDistance[MaxLights];
float3 xLightBrightness[MaxLights];
int xLightCount;
float xAmbient;
bool xEnableLighting;
float3 xCamPos;
float3 xCamUp;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = POINT; minfilter = POINT; mipfilter = POINT; AddressU = mirror; AddressV = mirror;};

//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);	
	Output.TextureCoords = inTexCoords;
	
	float3 Normal = normalize(mul(normalize(inNormal), xWorld));	

	Output.LightingFactor = 1;
	if (xEnableLighting)
	{
		Output.LightingFactor = 0;
		for (int i = 0; i < xLightCount; i += 1)
		{
			if (i < xLightCount)
			{
				float3 lightRay = inPos - xLightPosition[i];
				float rayLength = length(lightRay);
				//Output.LightingFactor = rayLength;
				lightRay = normalize(lightRay);

				float dotResult = dot(Normal, -lightRay);
				//Output.LightingFactor = max(0, dotResult);
				float distAdjust = max(0, (xLightDistance[i] - rayLength) / xLightDistance[i]);			

				Output.LightingFactor += max(0, dotResult * distAdjust * xLightBrightness[i]);
			}
		}				
	}	
    
	return Output;    
}

PixelToFrame TexturedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor) + xAmbient;

	return Output;
}

technique Textured
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 TexturedPS();
	}
}