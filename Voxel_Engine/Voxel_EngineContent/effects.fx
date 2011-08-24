//-------------------------------------//
//          Panacea Creations          //
// Textured Pixel Shader With Lighting //
//									   //
// version 1.0_0					   //
// Caleb Simpson					   //
// August 20th 2011					   //
//-------------------------------------//

#define MaxLights 20
bool xEnableLighting;
float xAmbient;
float3 xLightPosition[MaxLights];
float3 xLightDistance[MaxLights];
float3 xLightBrightness[MaxLights];
float4x4 xLightWorldViewProjection;
int xLightCount;

float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float4x4 xWorldViewProjection;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = POINT; minfilter = POINT; mipfilter = POINT; AddressU = mirror; AddressV = mirror;};

Texture xShadowMap;
sampler ShadowMapSampler = sampler_state { texture = <xShadowMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

//=========STANDARD TECHNIQUE==========//

struct VertexToPixel
{
    float4 Position   			: POSITION;    
    float4 Color				: COLOR0;
    float LightingFactor		: TEXCOORD0;
    float2 TextureCoords		: TEXCOORD1;	
	float4 Pos2DAsSeenByLight	: TEXCOORD2;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

VertexToPixel StandardVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	//float4x4 preViewProjection = mul (xView, xProjection);
	//float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
	//Output.Position = mul(inPos, preWorldViewProjection);
	
	Output.Pos2DAsSeenByLight = mul(inPos, xLightWorldViewProjection);
	Output.Position = mul(inPos, xWorldViewProjection);	

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

PixelToFrame StandardPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		

	float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
    ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;

	float lightingFactor = 0;
	if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
	{		
		float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;		
		float realDistance = PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;
		
		if ((realDistance - 1.0f/100.0f) <= depthStoredInShadowMap)		
        {			            
			lightingFactor = PSIn.LightingFactor;            
        }
	}
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(lightingFactor) + xAmbient;

	return Output;
}

technique Standard
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 StandardVS();
		PixelShader  = compile ps_2_0 StandardPS();
	}
}



//=========SHADOWMAP TECHNIQUE==========//

struct SMapVertexToPixel
{
    float4 Position     : POSITION;
    float4 Position2D    : TEXCOORD0;
};

struct SMapPixelToFrame
{
    float4 Color : COLOR0;
};


SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
{
    SMapVertexToPixel Output = (SMapVertexToPixel)0;

    Output.Position = mul(inPos, xLightWorldViewProjection);
    Output.Position2D = Output.Position;

    return Output;
}

SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
{
    SMapPixelToFrame Output = (SMapPixelToFrame)0;            

    Output.Color = PSIn.Position2D.z/PSIn.Position2D.w;
	Output.Color.a = 255;

    return Output;
}


technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowMapVertexShader();
        PixelShader = compile ps_2_0 ShadowMapPixelShader();
    }
}