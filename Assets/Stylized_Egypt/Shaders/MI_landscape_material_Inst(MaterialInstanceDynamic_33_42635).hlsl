#define NUM_TEX_COORD_INTERPOLATORS 1
#define NUM_MATERIAL_TEXCOORDS_VERTEX 1
#define NUM_CUSTOM_VERTEX_INTERPOLATORS 0

struct Input
{
	//float3 Normal;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv2_Material_Texture2D_0 : TEXCOORD1;
	float4 color : COLOR;
	float4 tangent;
	//float4 normal;
	float3 viewDir;
	float4 screenPos;
	float3 worldPos;
	//float3 worldNormal;
	float3 normal2;
};
struct SurfaceOutputStandard
{
	float3 Albedo;		// base (diffuse or specular) color
	float3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Metallic;		// 0=non-metal, 1=metal
	// Smoothness is the user facing name, it should be perceptual smoothness but user should not have to deal with it.
	// Everywhere in the code you meet smoothness it is perceptual smoothness
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;		// occlusion (default 1)
	float Alpha;		// alpha for transparencies
};

//#define HDRP 1
#define URP 1
#define UE5
//#define HAS_CUSTOMIZED_UVS 1
#define MATERIAL_TANGENTSPACENORMAL 1
//struct Material
//{
	//samplers start
SAMPLER( SamplerState_Linear_Repeat );
SAMPLER( SamplerState_Linear_Clamp );
TEXTURE2D(       Material_Texture2D_0 );
SAMPLER( sampler_Material_Texture2D_0 );
TEXTURE2D(       Material_Texture2D_1 );
SAMPLER( sampler_Material_Texture2D_1 );
TEXTURE2D(       Material_Texture2D_2 );
SAMPLER( sampler_Material_Texture2D_2 );
TEXTURE2D(       Material_Texture2D_3 );
SAMPLER( sampler_Material_Texture2D_3 );
TEXTURE2D(       Material_Texture2D_4 );
SAMPLER( sampler_Material_Texture2D_4 );
TEXTURE2D(       Material_Texture2D_5 );
SAMPLER( sampler_Material_Texture2D_5 );
TEXTURE2D(       Material_Texture2D_6 );
SAMPLER( sampler_Material_Texture2D_6 );
TEXTURE2D(       Material_Texture2D_7 );
SAMPLER( sampler_Material_Texture2D_7 );
TEXTURE2D(       Material_Texture2D_8 );
SAMPLER( sampler_Material_Texture2D_8 );
TEXTURE2D(       Material_Texture2D_9 );
SAMPLER( sampler_Material_Texture2D_9 );

//};

#ifdef UE5
	#define UE_LWC_RENDER_TILE_SIZE			2097152.0
	#define UE_LWC_RENDER_TILE_SIZE_SQRT	1448.15466
	#define UE_LWC_RENDER_TILE_SIZE_RSQRT	0.000690533954
	#define UE_LWC_RENDER_TILE_SIZE_RCP		4.76837158e-07
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_PI		0.673652053
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_2PI	0.673652053
	#define INVARIANT(X) X
	#define PI 					(3.1415926535897932)

	#include "LargeWorldCoordinates.hlsl"
#endif
struct MaterialStruct
{
	float4 PreshaderBuffer[22];
	float4 ScalarExpressions[1];
	float VTPackedPageTableUniform[2];
	float VTPackedUniform[1];
};
static SamplerState View_MaterialTextureBilinearWrapedSampler;
static SamplerState View_MaterialTextureBilinearClampedSampler;
struct ViewStruct
{
	float GameTime;
	float RealTime;
	float DeltaTime;
	float PrevFrameGameTime;
	float PrevFrameRealTime;
	float MaterialTextureMipBias;	
	float4 PrimitiveSceneData[ 40 ];
	float4 TemporalAAParams;
	float2 ViewRectMin;
	float4 ViewSizeAndInvSize;
	float MaterialTextureDerivativeMultiply;
	uint StateFrameIndexMod8;
	float FrameNumber;
	float2 FieldOfViewWideAngles;
	float4 RuntimeVirtualTextureMipLevel;
	float PreExposure;
	float4 BufferBilinearUVMinMax;
};
struct ResolvedViewStruct
{
	#ifdef UE5
		FLWCVector3 WorldCameraOrigin;
		FLWCVector3 PrevWorldCameraOrigin;
		FLWCVector3 PreViewTranslation;
		FLWCVector3 WorldViewOrigin;
	#else
		float3 WorldCameraOrigin;
		float3 PrevWorldCameraOrigin;
		float3 PreViewTranslation;
		float3 WorldViewOrigin;
	#endif
	float4 ScreenPositionScaleBias;
	float4x4 TranslatedWorldToView;
	float4x4 TranslatedWorldToCameraView;
	float4x4 TranslatedWorldToClip;
	float4x4 ViewToTranslatedWorld;
	float4x4 PrevViewToTranslatedWorld;
	float4x4 CameraViewToTranslatedWorld;
	float4 BufferBilinearUVMinMax;
	float4 XRPassthroughCameraUVs[ 2 ];
};
struct PrimitiveStruct
{
	float4x4 WorldToLocal;
	float4x4 LocalToWorld;
};

static ViewStruct View;
static ResolvedViewStruct ResolvedView;
static PrimitiveStruct Primitive;
uniform float4 View_BufferSizeAndInvSize;
uniform float4 LocalObjectBoundsMin;
uniform float4 LocalObjectBoundsMax;
static SamplerState Material_Wrap_WorldGroupSettings;
static SamplerState Material_Clamp_WorldGroupSettings;

#include "UnrealCommon.cginc"

static MaterialStruct Material;
void InitializeExpressions()
{
	Material.PreshaderBuffer[0] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[1] = float4(1.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1009.000000,1009.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(1009.000000,1009.000000,4.000000,0.250000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(13.000000,0.076923,12000.000000,0.000083);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.000000,-24.000000,-12.000000,9.500000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(21.500000,4.000000,0.250000,0.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(0.496933,0.584078,0.630757,1.000000);//(Unknown)
	Material.PreshaderBuffer[9] = float4(0.496933,0.584078,0.630757,0.000000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(0.503067,0.415922,0.369243,-1200.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(-0.000833,-13000.000000,-0.000077,12000.000000);//(Unknown)
	Material.PreshaderBuffer[12] = float4(0.000083,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(0.500000,0.500000,0.500000,0.000000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(0.500000,0.500000,0.500000,0.000000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(0.500000,0.500000,0.500000,0.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(0.434154,0.391572,0.212231,1.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(0.434154,0.391572,0.212231,0.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(0.565846,0.608428,0.787769,0.500000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(15.000000,1.500000,-0.500000,0.200000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(1.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[21] = float4(1.000000,0.000000,0.000000,0.000000);//(Unknown)
}
MaterialFloat3 CustomExpression2(FMaterialPixelParameters Parameters,MaterialFloat auto_renamedWeight,MaterialFloat layer_01Weight,MaterialFloat layer_02Weight,MaterialFloat layer_03Weight,MaterialFloat auto_renamed,MaterialFloat layer_01,MaterialFloat layer_02,MaterialFloat layer_03,MaterialFloat3 layer_03Height)
{
float  lerpres;
float  Local0;



lerpres = lerp( -1.0, 1.0, layer_03Weight );
Local0 = ( lerpres + layer_03Height );
float Layer3WithHeight = clamp(Local0, 0.0001, 1.0);

float  AllWeightsAndHeights = auto_renamedWeight.r + layer_01Weight.r + layer_02Weight.r + Layer3WithHeight + 0;
float  Divider = ( 1.0 / AllWeightsAndHeights );
float3  Layer0Contribution = Divider.rrr * auto_renamedWeight.rrr * auto_renamed;
float3  Layer1Contribution = Divider.rrr * layer_01Weight.rrr * layer_01;
float3  Layer2Contribution = Divider.rrr * layer_02Weight.rrr * layer_02;
float3  Layer3Contribution = Divider.rrr * Layer3WithHeight.rrr * layer_03;
float3  Result = Layer0Contribution + Layer1Contribution + Layer2Contribution + Layer3Contribution + float3(0,0,0);
return Result;
}

MaterialFloat3 CustomExpression1(FMaterialPixelParameters Parameters,MaterialFloat auto_renamedWeight,MaterialFloat layer_01Weight,MaterialFloat layer_02Weight,MaterialFloat layer_03Weight,MaterialFloat3 auto_renamed,MaterialFloat3 layer_01,MaterialFloat3 layer_02,MaterialFloat3 layer_03,MaterialFloat3 layer_03Height)
{
float  lerpres;
float  Local0;



lerpres = lerp( -1.0, 1.0, layer_03Weight );
Local0 = ( lerpres + layer_03Height );
float Layer3WithHeight = clamp(Local0, 0.0001, 1.0);

float  AllWeightsAndHeights = auto_renamedWeight.r + layer_01Weight.r + layer_02Weight.r + Layer3WithHeight + 0;
float  Divider = ( 1.0 / AllWeightsAndHeights );
float3  Layer0Contribution = Divider.rrr * auto_renamedWeight.rrr * auto_renamed;
float3  Layer1Contribution = Divider.rrr * layer_01Weight.rrr * layer_01;
float3  Layer2Contribution = Divider.rrr * layer_02Weight.rrr * layer_02;
float3  Layer3Contribution = Divider.rrr * Layer3WithHeight.rrr * layer_03;
float3  Result = Layer0Contribution + Layer1Contribution + Layer2Contribution + Layer3Contribution + float3(0,0,0);
return Result;
}

MaterialFloat3 CustomExpression0(FMaterialPixelParameters Parameters,MaterialFloat auto_renamedWeight,MaterialFloat layer_01Weight,MaterialFloat layer_02Weight,MaterialFloat layer_03Weight,MaterialFloat3 auto_renamed,MaterialFloat3 layer_01,MaterialFloat3 layer_02,MaterialFloat3 layer_03,MaterialFloat3 layer_03Height)
{
float  lerpres;
float  Local0;



lerpres = lerp( -1.0, 1.0, layer_03Weight );
Local0 = ( lerpres + layer_03Height );
float Layer3WithHeight = clamp(Local0, 0.0001, 1.0);

float  AllWeightsAndHeights = auto_renamedWeight.r + layer_01Weight.r + layer_02Weight.r + Layer3WithHeight + 0;
float  Divider = ( 1.0 / AllWeightsAndHeights );
float3  Layer0Contribution = Divider.rrr * auto_renamedWeight.rrr * auto_renamed;
float3  Layer1Contribution = Divider.rrr * layer_01Weight.rrr * layer_01;
float3  Layer2Contribution = Divider.rrr * layer_02Weight.rrr * layer_02;
float3  Layer3Contribution = Divider.rrr * Layer3WithHeight.rrr * layer_03;
float3  Result = Layer0Contribution + Layer1Contribution + Layer2Contribution + Layer3Contribution + float3(0,0,0);
return Result;
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
	return MaterialFloat3(0.00000000,0.00000000,0.00000000);;
}
void CalcPixelMaterialInputs(in out FMaterialPixelParameters Parameters, in out FPixelMaterialInputs PixelMaterialInputs)
{
	//WorldAligned texturing & others use normals & stuff that think Z is up
	Parameters.TangentToWorld[0] = Parameters.TangentToWorld[0].xzy;
	Parameters.TangentToWorld[1] = Parameters.TangentToWorld[1].xzy;
	Parameters.TangentToWorld[2] = Parameters.TangentToWorld[2].xzy;

	float3 WorldNormalCopy = Parameters.WorldNormal;

	// Initial calculations (required for Normal)
	MaterialFloat2 Local0 = Parameters.TexCoords[0].xy;
	MaterialFloat2 Local1 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[1].x));
	MaterialFloat Local2 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local1), 10);
	MaterialFloat4 Local3 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(sampler_Material_Texture2D_0,View_MaterialTextureBilinearClampedSampler),DERIV_BASE_VALUE(Local1)));
	MaterialFloat Local4 = MaterialStoreTexSample(Parameters, Local3, 10);
	MaterialFloat4 Local5 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(sampler_Material_Texture2D_1,View_MaterialTextureBilinearClampedSampler),DERIV_BASE_VALUE(Local1)));
	MaterialFloat Local6 = MaterialStoreTexSample(Parameters, Local5, 10);
	MaterialFloat4 Local7 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(sampler_Material_Texture2D_2,View_MaterialTextureBilinearClampedSampler),DERIV_BASE_VALUE(Local1)));
	MaterialFloat Local8 = MaterialStoreTexSample(Parameters, Local7, 10);
	MaterialFloat4 Local9 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(sampler_Material_Texture2D_3,View_MaterialTextureBilinearClampedSampler),DERIV_BASE_VALUE(Local1)));
	MaterialFloat Local10 = MaterialStoreTexSample(Parameters, Local9, 10);
	MaterialFloat2 Local11 = (DERIV_BASE_VALUE(Local0) + MaterialFloat4(0.00198216,0.00198216,0.00198216,0.00198216).rg);
	MaterialFloat2 Local12 = (DERIV_BASE_VALUE(Local11) * Material.PreshaderBuffer[3].xy);
	MaterialFloat2 Local13 = (DERIV_BASE_VALUE(Local12) * ((MaterialFloat2)Material.PreshaderBuffer[3].w));
	MaterialFloat Local14 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local13), 6);
	MaterialFloat4 Local15 = UnpackNormalMap(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(sampler_Material_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local13)));
	MaterialFloat Local16 = MaterialStoreTexSample(Parameters, Local15, 6);
	MaterialFloat2 Local17 = (DERIV_BASE_VALUE(Local12) * ((MaterialFloat2)Material.PreshaderBuffer[4].y));
	MaterialFloat Local18 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local17), 6);
	MaterialFloat4 Local19 = UnpackNormalMap(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(sampler_Material_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local17)));
	MaterialFloat Local20 = MaterialStoreTexSample(Parameters, Local19, 6);
	FLWCVector3 Local21 = ResolvedView.WorldCameraOrigin;
	FLWCVector3 Local22 = GetWorldPosition(Parameters);
	FLWCVector3 Local23 = LWCSubtract(Local21, DERIV_BASE_VALUE(Local22));
	MaterialFloat3 Local24 = LWCToFloat(DERIV_BASE_VALUE(Local23));
	MaterialFloat2 Local25 = DERIV_BASE_VALUE(Local24).xy;
	MaterialFloat2 Local26 = (DERIV_BASE_VALUE(Local25) - ((MaterialFloat2)0.00000000));
	MaterialFloat Local27 = length(DERIV_BASE_VALUE(Local26));
	MaterialFloat Local28 = (DERIV_BASE_VALUE(Local27) - 0.00000000);
	MaterialFloat Local29 = (DERIV_BASE_VALUE(Local28) * Material.PreshaderBuffer[4].w);
	MaterialFloat Local30 = saturate(DERIV_BASE_VALUE(Local29));
	MaterialFloat3 Local31 = lerp(Local15.rgb,Local19.rgb,DERIV_BASE_VALUE(Local30));
	MaterialFloat3 Local32 = lerp(Local31,MaterialFloat3(0.00000000,0.00000000,1.00000000),Material.PreshaderBuffer[5].x);
	MaterialFloat Local33 = dot(Parameters.TangentToWorld[2],normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000)));
	MaterialFloat Local34 = (Local33 * 0.50000000);
	MaterialFloat Local35 = (Local34 + 0.50000000);
	MaterialFloat Local36 = (Local35 * Material.PreshaderBuffer[5].y);
	MaterialFloat Local37 = (Local36 + Material.PreshaderBuffer[6].x);
	MaterialFloat Local38 = saturate(Local37);
	MaterialFloat3 Local39 = lerp(Local32.rgb,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Local38);
	MaterialFloat2 Local40 = (DERIV_BASE_VALUE(Local12) * ((MaterialFloat2)Material.PreshaderBuffer[6].z));
	MaterialFloat Local41 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local40), 8);
	MaterialFloat4 Local42 = UnpackNormalMap(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(sampler_Material_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local40)));
	MaterialFloat Local43 = MaterialStoreTexSample(Parameters, Local42, 8);
	MaterialFloat3 Local44 = lerp(Local42.rgb,MaterialFloat3(0.00000000,0.00000000,1.00000000),Material.PreshaderBuffer[6].w);
	MaterialFloat Local45 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local40), 9);
	MaterialFloat4 Local46 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSampleBias(Material_Texture2D_6,sampler_Material_Texture2D_6,DERIV_BASE_VALUE(Local40),View.MaterialTextureMipBias));
	MaterialFloat Local47 = MaterialStoreTexSample(Parameters, Local46, 9);
	MaterialFloat3 Local48 = CustomExpression0(Parameters,Local3.r,Local5.r,Local7.r,Local9.r,Local39,MaterialFloat3(0.00000000,0.00000000,1.00000000),Local32,Local44,Local46.rgb);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local48.rgb;


#if TEMPLATE_USES_STRATA
	Parameters.StrataPixelFootprint = StrataGetPixelFootprint(Parameters.WorldPosition_CamRelative, GetRoughnessFromNormalCurvature(Parameters));
	Parameters.SharedLocalBases = StrataInitialiseSharedLocalBases();
	Parameters.StrataTree = GetInitialisedStrataTree();
#if STRATA_USE_FULLYSIMPLIFIED_MATERIAL == 1
	Parameters.SharedLocalBasesFullySimplified = StrataInitialiseSharedLocalBases();
	Parameters.StrataTreeFullySimplified = GetInitialisedStrataTree();
#endif
#endif

	// Note that here MaterialNormal can be in world space or tangent space
	float3 MaterialNormal = GetMaterialNormal(Parameters, PixelMaterialInputs);

#if MATERIAL_TANGENTSPACENORMAL

#if FEATURE_LEVEL >= FEATURE_LEVEL_SM4
	// Mobile will rely on only the final normalize for performance
	MaterialNormal = normalize(MaterialNormal);
#endif

	// normalizing after the tangent space to world space conversion improves quality with sheared bases (UV layout to WS causes shrearing)
	// use full precision normalize to avoid overflows
	Parameters.WorldNormal = TransformTangentNormalToWorld(Parameters.TangentToWorld, MaterialNormal);

#else //MATERIAL_TANGENTSPACENORMAL

	Parameters.WorldNormal = normalize(MaterialNormal);

#endif //MATERIAL_TANGENTSPACENORMAL

#if MATERIAL_TANGENTSPACENORMAL
	// flip the normal for backfaces being rendered with a two-sided material
	Parameters.WorldNormal *= Parameters.TwoSidedSign;
#endif

	Parameters.ReflectionVector = ReflectionAboutCustomWorldNormal(Parameters, Parameters.WorldNormal, false);

#if !PARTICLE_SPRITE_FACTORY
	Parameters.Particle.MotionBlurFade = 1.0f;
#endif // !PARTICLE_SPRITE_FACTORY

	// Now the rest of the inputs
	MaterialFloat3 Local49 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[7].yzw,Material.PreshaderBuffer[7].x);
	MaterialFloat Local50 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local13), 5);
	MaterialFloat4 Local51 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(sampler_Material_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local13)));
	MaterialFloat Local52 = MaterialStoreTexSample(Parameters, Local51, 5);
	MaterialFloat3 Local53 = (((MaterialFloat3)1.00000000) - Local51.rgb);
	MaterialFloat3 Local54 = (Local53 * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local55 = (Local54 * Material.PreshaderBuffer[10].xyz);
	MaterialFloat3 Local56 = (((MaterialFloat3)1.00000000) - Local55);
	MaterialFloat3 Local57 = (Local51.rgb * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local58 = (Local57 * Material.PreshaderBuffer[9].xyz);
	MaterialFloat Local59 = select((Local51.rgb.r >= 0.50000000), Local56.r, Local58.r);
	MaterialFloat Local60 = select((Local51.rgb.g >= 0.50000000), Local56.g, Local58.g);
	MaterialFloat Local61 = select((Local51.rgb.b >= 0.50000000), Local56.b, Local58.b);
	FLWCVector3 Local62 = LWCMultiply(DERIV_BASE_VALUE(Local22), LWCPromote(((MaterialFloat3)Material.PreshaderBuffer[11].x)));
	FLWCVector3 Local63 = MakeLWCVector(LWCGetX(DERIV_BASE_VALUE(Local62)), LWCGetY(DERIV_BASE_VALUE(Local62)), LWCGetZ(DERIV_BASE_VALUE(Local62)));
	FLWCVector2 Local64 = MakeLWCVector(LWCGetY(DERIV_BASE_VALUE(Local63)), LWCGetZ(DERIV_BASE_VALUE(Local63)));
	MaterialFloat2 Local65 = LWCApplyAddressMode(DERIV_BASE_VALUE(Local64), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local66 = MaterialStoreTexCoordScale(Parameters, Local65, 1);
	MaterialFloat4 Local67 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(sampler_Material_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),Local65));
	MaterialFloat Local68 = MaterialStoreTexSample(Parameters, Local67, 1);
	FLWCVector3 Local69 = LWCMultiply(DERIV_BASE_VALUE(Local22), LWCPromote(((MaterialFloat3)Material.PreshaderBuffer[11].z)));
	FLWCVector3 Local70 = MakeLWCVector(LWCGetX(DERIV_BASE_VALUE(Local69)), LWCGetY(DERIV_BASE_VALUE(Local69)), LWCGetZ(DERIV_BASE_VALUE(Local69)));
	FLWCVector2 Local71 = MakeLWCVector(LWCGetY(DERIV_BASE_VALUE(Local70)), LWCGetZ(DERIV_BASE_VALUE(Local70)));
	MaterialFloat2 Local72 = LWCApplyAddressMode(DERIV_BASE_VALUE(Local71), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local73 = MaterialStoreTexCoordScale(Parameters, Local72, 1);
	MaterialFloat4 Local74 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(sampler_Material_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),Local72));
	MaterialFloat Local75 = MaterialStoreTexSample(Parameters, Local74, 1);
	MaterialFloat Local76 = (DERIV_BASE_VALUE(Local28) * Material.PreshaderBuffer[12].x);
	MaterialFloat Local77 = saturate(DERIV_BASE_VALUE(Local76));
	MaterialFloat3 Local78 = lerp(Local67.rgb,Local74.rgb,DERIV_BASE_VALUE(Local77));
	MaterialFloat3 Local79 = abs(Parameters.TangentToWorld[2]);
	MaterialFloat3 Local80 = PositiveClampedPow(Local79,((MaterialFloat3)10.00000000));
	MaterialFloat Local81 = (Local80.r + Local80.g);
	MaterialFloat Local82 = (Local81 + Local80.b);
	MaterialFloat3 Local83 = (Local80 / ((MaterialFloat3)Local82));
	MaterialFloat3 Local84 = (Local78 * ((MaterialFloat3)Local83.g));
	MaterialFloat3 Local85 = (Local78 * ((MaterialFloat3)Local83.b));
	MaterialFloat3 Local86 = (Local84 + Local85);
	MaterialFloat3 Local87 = (Local78 * ((MaterialFloat3)Local83.r));
	MaterialFloat3 Local88 = (Local86 + Local87);
	MaterialFloat3 Local89 = lerp(MaterialFloat3(MaterialFloat2(Local59,Local60),Local61).rgb,Local88.rgb,Local38);
	MaterialFloat Local90 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local40), 7);
	MaterialFloat4 Local91 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_9,GetMaterialSharedSampler(sampler_Material_Texture2D_9,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local40)));
	MaterialFloat Local92 = MaterialStoreTexSample(Parameters, Local91, 7);
	MaterialFloat3 Local93 = (((MaterialFloat3)1.00000000) - Local91.rgb);
	MaterialFloat3 Local94 = (Local93 * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local95 = (Local94 * Material.PreshaderBuffer[15].xyz);
	MaterialFloat3 Local96 = (((MaterialFloat3)1.00000000) - Local95);
	MaterialFloat3 Local97 = (Local91.rgb * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local98 = (Local97 * Material.PreshaderBuffer[14].xyz);
	MaterialFloat Local99 = select((Local91.rgb.r >= 0.50000000), Local96.r, Local98.r);
	MaterialFloat Local100 = select((Local91.rgb.g >= 0.50000000), Local96.g, Local98.g);
	MaterialFloat Local101 = select((Local91.rgb.b >= 0.50000000), Local96.b, Local98.b);
	MaterialFloat3 Local102 = (((MaterialFloat3)1.00000000) - MaterialFloat3(MaterialFloat2(Local99,Local100),Local101));
	MaterialFloat3 Local103 = (Local102 * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local104 = (Local103 * Material.PreshaderBuffer[18].xyz);
	MaterialFloat3 Local105 = (((MaterialFloat3)1.00000000) - Local104);
	MaterialFloat3 Local106 = (MaterialFloat3(MaterialFloat2(Local99,Local100),Local101) * ((MaterialFloat3)2.00000000));
	MaterialFloat3 Local107 = (Local106 * Material.PreshaderBuffer[17].xyz);
	MaterialFloat Local108 = select((MaterialFloat3(MaterialFloat2(Local99,Local100),Local101).r >= 0.50000000), Local105.r, Local107.r);
	MaterialFloat Local109 = select((MaterialFloat3(MaterialFloat2(Local99,Local100),Local101).g >= 0.50000000), Local105.g, Local107.g);
	MaterialFloat Local110 = select((MaterialFloat3(MaterialFloat2(Local99,Local100),Local101).b >= 0.50000000), Local105.b, Local107.b);
	MaterialFloat Local111 = PositiveClampedPow(Local42.b,Material.PreshaderBuffer[19].x);
	MaterialFloat Local112 = lerp(Material.PreshaderBuffer[19].z,Material.PreshaderBuffer[19].y,Local111);
	MaterialFloat Local113 = saturate(Local112);
	MaterialFloat3 Local114 = lerp(MaterialFloat3(MaterialFloat2(Local108,Local109),Local110),MaterialFloat3(MaterialFloat2(Local99,Local100),Local101),Local113.r);
	MaterialFloat3 Local115 = CustomExpression1(Parameters,Local3.r,Local5.r,Local7.r,Local9.r,Local89,Local88,MaterialFloat3(MaterialFloat2(Local59,Local60),Local61),Local114,Local46.rgb);
	MaterialFloat Local116 = lerp(Material.PreshaderBuffer[20].w,Material.PreshaderBuffer[20].z,Local38);
	MaterialFloat3 Local117 = CustomExpression2(Parameters,Local3.r,Local5.r,Local7.r,Local9.r,Local116,Material.PreshaderBuffer[20].y,Material.PreshaderBuffer[20].x,Material.PreshaderBuffer[21].x,Local46.rgb);

	PixelMaterialInputs.EmissiveColor = Local49;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local115.rgb;
	PixelMaterialInputs.Metallic = 0.00000000;
	PixelMaterialInputs.Specular = Material.PreshaderBuffer[19].w;
	PixelMaterialInputs.Roughness = Local117.r;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local48.rgb;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = 1.00000000;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 1;
	PixelMaterialInputs.FrontMaterial = GetInitialisedStrataData();
	PixelMaterialInputs.SurfaceThickness = 0.01000000;
	PixelMaterialInputs.Displacement = 0.00000000;


#if MATERIAL_USES_ANISOTROPY
	Parameters.WorldTangent = CalculateAnisotropyTangent(Parameters, PixelMaterialInputs);
#else
	Parameters.WorldTangent = 0;
#endif
}

#define UnityObjectToWorldDir TransformObjectToWorld

void SetupCommonData( int Parameters_PrimitiveId )
{
	View_MaterialTextureBilinearWrapedSampler = SamplerState_Linear_Repeat;
	View_MaterialTextureBilinearClampedSampler = SamplerState_Linear_Clamp;

	Material_Wrap_WorldGroupSettings = SamplerState_Linear_Repeat;
	Material_Clamp_WorldGroupSettings = SamplerState_Linear_Clamp;

	View.GameTime = View.RealTime = _Time.y;// _Time is (t/20, t, t*2, t*3)
	View.PrevFrameGameTime = View.GameTime - unity_DeltaTime.x;//(dt, 1/dt, smoothDt, 1/smoothDt)
	View.PrevFrameRealTime = View.RealTime;
	View.DeltaTime = unity_DeltaTime.x;
	View.MaterialTextureMipBias = 0.0;
	View.TemporalAAParams = float4( 0, 0, 0, 0 );
	View.ViewRectMin = float2( 0, 0 );
	View.ViewSizeAndInvSize = View_BufferSizeAndInvSize;
	View.MaterialTextureDerivativeMultiply = 1.0f;
	View.StateFrameIndexMod8 = 0;
	View.FrameNumber = (int)_Time.y;
	View.FieldOfViewWideAngles = float2( PI * 0.42f, PI * 0.42f );//75degrees, default unity
	View.RuntimeVirtualTextureMipLevel = float4( 0, 0, 0, 0 );
	View.PreExposure = 0;
	View.BufferBilinearUVMinMax = float4(
		View_BufferSizeAndInvSize.z * ( 0 + 0.5 ),//EffectiveViewRect.Min.X
		View_BufferSizeAndInvSize.w * ( 0 + 0.5 ),//EffectiveViewRect.Min.Y
		View_BufferSizeAndInvSize.z * ( View_BufferSizeAndInvSize.x - 0.5 ),//EffectiveViewRect.Max.X
		View_BufferSizeAndInvSize.w * ( View_BufferSizeAndInvSize.y - 0.5 ) );//EffectiveViewRect.Max.Y

	for( int i2 = 0; i2 < 40; i2++ )
		View.PrimitiveSceneData[ i2 ] = float4( 0, 0, 0, 0 );

	float4x4 LocalToWorld = transpose( UNITY_MATRIX_M );
    LocalToWorld[3] = float4(ToUnrealPos(LocalToWorld[3]), LocalToWorld[3].w);
	float4x4 WorldToLocal = transpose( UNITY_MATRIX_I_M );
	float4x4 ViewMatrix = transpose( UNITY_MATRIX_V );
	float4x4 InverseViewMatrix = transpose( UNITY_MATRIX_I_V );
	float4x4 ViewProjectionMatrix = transpose( UNITY_MATRIX_VP );
	uint PrimitiveBaseOffset = Parameters_PrimitiveId * PRIMITIVE_SCENE_DATA_STRIDE;
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 0 ] = LocalToWorld[ 0 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 1 ] = LocalToWorld[ 1 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 2 ] = LocalToWorld[ 2 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 3 ] = LocalToWorld[ 3 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 5 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 100.0 );//ObjectWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 6 ] = WorldToLocal[ 0 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 7 ] = WorldToLocal[ 1 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 8 ] = WorldToLocal[ 2 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 9 ] = WorldToLocal[ 3 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 10 ] = LocalToWorld[ 0 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 11 ] = LocalToWorld[ 1 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 12 ] = LocalToWorld[ 2 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 13 ] = LocalToWorld[ 3 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 18 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 0 );//ActorWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 19 ] = LocalObjectBoundsMax - LocalObjectBoundsMin;//ObjectBounds
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 21 ] = mul( LocalToWorld, float3( 1, 0, 0 ) );
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 23 ] = LocalObjectBoundsMin;//LocalObjectBoundsMin 
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 24 ] = LocalObjectBoundsMax;//LocalObjectBoundsMax

#ifdef UE5
	ResolvedView.WorldCameraOrigin = LWCPromote( ToUnrealPos( _WorldSpaceCameraPos.xyz ) );
	ResolvedView.PreViewTranslation = LWCPromote( float3( 0, 0, 0 ) );
	ResolvedView.WorldViewOrigin = LWCPromote( float3( 0, 0, 0 ) );
#else
	ResolvedView.WorldCameraOrigin = ToUnrealPos( _WorldSpaceCameraPos.xyz );
	ResolvedView.PreViewTranslation = float3( 0, 0, 0 );
	ResolvedView.WorldViewOrigin = float3( 0, 0, 0 );
#endif
	ResolvedView.PrevWorldCameraOrigin = ResolvedView.WorldCameraOrigin;
	ResolvedView.ScreenPositionScaleBias = float4( 1, 1, 0, 0 );
	ResolvedView.TranslatedWorldToView		 = ViewMatrix;
	ResolvedView.TranslatedWorldToCameraView = ViewMatrix;
	ResolvedView.TranslatedWorldToClip		 = ViewProjectionMatrix;
	ResolvedView.ViewToTranslatedWorld		 = InverseViewMatrix;
	ResolvedView.PrevViewToTranslatedWorld = ResolvedView.ViewToTranslatedWorld;
	ResolvedView.CameraViewToTranslatedWorld = InverseViewMatrix;
	ResolvedView.BufferBilinearUVMinMax = View.BufferBilinearUVMinMax;
	Primitive.WorldToLocal = WorldToLocal;
	Primitive.LocalToWorld = LocalToWorld;
}
#define VS_USES_UNREAL_SPACE 1
float3 PrepareAndGetWPO( float4 VertexColor, float3 UnrealWorldPos, float3 UnrealNormal, float4 InTangent,
						 float4 UV0, float4 UV1 )
{
	InitializeExpressions();
	FMaterialVertexParameters Parameters = (FMaterialVertexParameters)0;

	float3 InWorldNormal = UnrealNormal;
	float4 tangentWorld = InTangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	
	#ifdef VS_USES_UNREAL_SPACE
		UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	#endif
	Parameters.WorldPosition = UnrealWorldPos;
	#ifdef VS_USES_UNREAL_SPACE
		Parameters.TangentToWorld[ 0 ] = Parameters.TangentToWorld[ 0 ].xzy;
		Parameters.TangentToWorld[ 1 ] = Parameters.TangentToWorld[ 1 ].xzy;
		Parameters.TangentToWorld[ 2 ] = Parameters.TangentToWorld[ 2 ].xzy;//WorldAligned texturing uses normals that think Z is up
	#endif

	Parameters.VertexColor = VertexColor;

#if NUM_MATERIAL_TEXCOORDS_VERTEX > 0			
	Parameters.TexCoords[ 0 ] = float2( UV0.x, UV0.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 1
	Parameters.TexCoords[ 1 ] = float2( UV1.x, UV1.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( UV0.x, UV0.y );
	}
#endif

	Parameters.PrimitiveId = 0;

	SetupCommonData( Parameters.PrimitiveId );

#ifdef UE5
	Parameters.PrevFrameLocalToWorld = MakeLWCMatrix( float3( 0, 0, 0 ), Primitive.LocalToWorld );
#else
	Parameters.PrevFrameLocalToWorld = Primitive.LocalToWorld;
#endif
	
	float3 Offset = float3( 0, 0, 0 );
	Offset = GetMaterialWorldPositionOffset( Parameters );
	#ifdef VS_USES_UNREAL_SPACE
		//Convert from unreal units to unity
		Offset /= float3( 100, 100, 100 );
		Offset = Offset.xzy;
	#endif
	return Offset;
}

void SurfaceReplacement( Input In, out SurfaceOutputStandard o )
{
	InitializeExpressions();

	float3 Z3 = float3( 0, 0, 0 );
	float4 Z4 = float4( 0, 0, 0, 0 );

	float3 UnrealWorldPos = float3( In.worldPos.x, In.worldPos.y, In.worldPos.z );

	float3 UnrealNormal = In.normal2;	

	FMaterialPixelParameters Parameters = (FMaterialPixelParameters)0;
#if NUM_TEX_COORD_INTERPOLATORS > 0			
	Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 1
	Parameters.TexCoords[ 1 ] = float2( In.uv2_Material_Texture2D_0.x, 1.0 - In.uv2_Material_Texture2D_0.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	}
#endif
	Parameters.VertexColor = In.color;
	Parameters.WorldNormal = UnrealNormal;
	Parameters.ReflectionVector = half3( 0, 0, 1 );
	Parameters.CameraVector = normalize( _WorldSpaceCameraPos.xyz - UnrealWorldPos.xyz );
	//Parameters.CameraVector = mul( ( float3x3 )unity_CameraToWorld, float3( 0, 0, 1 ) ) * -1;
	Parameters.LightVector = half3( 0, 0, 0 );
	//float4 screenpos = In.screenPos;
	//screenpos /= screenpos.w;
	Parameters.SvPosition = In.screenPos;
	Parameters.ScreenPosition = Parameters.SvPosition;

	Parameters.UnMirrored = 1;

	Parameters.TwoSidedSign = 1;


	float3 InWorldNormal = UnrealNormal;	
	float4 tangentWorld = In.tangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	//WorldAlignedTexturing in UE relies on the fact that coords there are 100x larger, prepare values for that
	//but watch out for any computation that might get skewed as a side effect
	UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	
	Parameters.AbsoluteWorldPosition = UnrealWorldPos;
	Parameters.WorldPosition_CamRelative = UnrealWorldPos;
	Parameters.WorldPosition_NoOffsets = UnrealWorldPos;

	Parameters.WorldPosition_NoOffsets_CamRelative = Parameters.WorldPosition_CamRelative;
	Parameters.LightingPositionOffset = float3( 0, 0, 0 );

	Parameters.AOMaterialMask = 0;

	Parameters.Particle.RelativeTime = 0;
	Parameters.Particle.MotionBlurFade;
	Parameters.Particle.Random = 0;
	Parameters.Particle.Velocity = half4( 1, 1, 1, 1 );
	Parameters.Particle.Color = half4( 1, 1, 1, 1 );
	Parameters.Particle.TranslatedWorldPositionAndSize = float4( UnrealWorldPos, 0 );
	Parameters.Particle.MacroUV = half4( 0, 0, 1, 1 );
	Parameters.Particle.DynamicParameter = half4( 0, 0, 0, 0 );
	Parameters.Particle.LocalToWorld = float4x4( Z4, Z4, Z4, Z4 );
	Parameters.Particle.Size = float2( 1, 1 );
	Parameters.Particle.SubUVCoords[ 0 ] = Parameters.Particle.SubUVCoords[ 1 ] = float2( 0, 0 );
	Parameters.Particle.SubUVLerp = 0.0;
	Parameters.TexCoordScalesParams = float2( 0, 0 );
	Parameters.PrimitiveId = 0;
	Parameters.VirtualTextureFeedback = 0;

	FPixelMaterialInputs PixelMaterialInputs = (FPixelMaterialInputs)0;
	PixelMaterialInputs.Normal = float3( 0, 0, 1 );
	PixelMaterialInputs.ShadingModel = 0;
	PixelMaterialInputs.FrontMaterial = 0;

	SetupCommonData( Parameters.PrimitiveId );
	//CustomizedUVs
	#if NUM_TEX_COORD_INTERPOLATORS > 0 && HAS_CUSTOMIZED_UVS
		float2 OutTexCoords[ NUM_TEX_COORD_INTERPOLATORS ];
		//Prevent uninitialized reads
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			OutTexCoords[ i ] = float2( 0, 0 );
		}
		GetMaterialCustomizedUVs( Parameters, OutTexCoords );
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			Parameters.TexCoords[ i ] = OutTexCoords[ i ];
		}
	#endif
	//<-
	CalcPixelMaterialInputs( Parameters, PixelMaterialInputs );

	#define HAS_WORLDSPACE_NORMAL 0
	#if HAS_WORLDSPACE_NORMAL
		PixelMaterialInputs.Normal = mul( PixelMaterialInputs.Normal, (MaterialFloat3x3)( transpose( Parameters.TangentToWorld ) ) );
	#endif

	o.Albedo = PixelMaterialInputs.BaseColor.rgb;
	o.Alpha = PixelMaterialInputs.Opacity;
	//if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;

	//BLEND_ADDITIVE o.Alpha = ( o.Emission.r + o.Emission.g + o.Emission.b ) / 3;
}