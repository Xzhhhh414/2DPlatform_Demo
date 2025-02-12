#include "Utils.hlsl"

#define LightProbeBlendReflectionRatio 0.5
#define LightMapBlendReflectionRatio 0.6

#ifdef LUMINANCE_GLOSSYENVIROMENTREFLECTION
    #define GLOSSYENVIRONMENTREFLECTION GlossyEnvironmentReflectionLuminance
#else
    #define GLOSSYENVIRONMENTREFLECTION GlossyEnvironmentReflection
#endif

int _DisneyRenderDebug_albedo;
int _DisneyRenderDebug_normalWS;
int _DisneyRenderDebug_emission;
int _DisneyRenderDebug_metal;
int _DisneyRenderDebug_roughness;
int _DisneyRenderDebug_occlusion;
int _DisneyRenderDebug_subsurfaceValue;
int _DisneyRenderDebug_diffuse;
int _DisneyRenderDebug_specular;
int _DisneyRenderDebug_gi;
int _DisneyRenderDebug_shadow;
int _DisneyRenderDebug_spRoughness;
int _DisneyRenderDebug_final;

uniform half _Lobby_june;

struct DBRDFData
{
    //half3 diffuse;
    half3 specular;
    half gi_grazingTerm;
    half perceptualRoughness;
    half roughness;
    half roughness2;
    half3 albedo;
    half metallic;
    half sheen;
    half sheenRange;
    half sheenTint;
    half3 L;
    half3 N;
    half3 V;
    half3 H;
    half NdotL;
    half NdotV;
    half NdotH;
    half LdotH;
    half LdotV;
    half3 Fs;
    half FH;
#ifdef _SMASH_DISNEY_TOON
    half NoLOffset;
    half specularMask;
#endif
};

struct AnistroData {
    #ifdef _SMASH_DISNEY_ANISOTROPIC_ON
        // #ifdef _SMASH_DISNEY_USEANISTROPIC
        //     half3 tangentWS;
        //     half3 biTangentWS;
        //     half anisotropic;
        // #elif defined (_SMASH_DISNEY_USEKAJIYAANISTROPIC)
        #ifdef _SMASH_DISNEY_USEKAJIYAANISTROPIC
            half3 T1;
            half3 T2;
            half perceptualRoughness;
            half perceptualRoughness2;
            half3 anisotropicColor;
            half3 anisotropicColor2;
            half anisotropicIntensity;
            half anisotropicMask;
            // half3 tangentWS;
            // half3 biTangentWS;
            // half anisotropic;
        #endif
    #endif
};

struct ClearcoatData {
    half3 normal;
    half clearcoat;
    half clearcoatGloss;
};

struct SubsurfaceData {
    half translucency;
    half transPower;
    half3 color;
};

#ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP
    struct CustomlightmapData{
        half lightmapFactor;
        half lightmaplerp;
        half lightmaptransLow;
        half lightmaptransHigh;
        half lightmapChrominance;
        half lightmapChroma;
        half3 lightmapshadowcolorup;
        half3 lightmapshadowcolorleft;
        half3 lightmapshadowcolorbuttom;
        half4 L;
        half3 lightmapssscolor;
        half lightmapssscolorintensity;
        half lightmapDirintensity;
    };

    half3 HSV2RGB( half3 c ){
        half3 rgb = clamp( abs(fmod(c.x*6.0+half3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
        rgb = rgb*rgb*(3.0-2.0*rgb);
        return c.z * lerp( half3(1,1,1), rgb, c.y);
    }

    half3 RGB2HSV(half3 c){
        half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
        half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));
        half d = q.x - min(q.w, q.y);
        half e = 1.0e-10;
        return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    }

    half3 Customlightmapping(CustomlightmapData customlightmapData,half3 lightmapColor, half NDL,half3 normal){
        // Adv start

        lightmapColor *= customlightmapData.lightmapFactor;
        lightmapColor = lerp(half3(1, 1, 1), lightmapColor, customlightmapData.lightmaplerp);

        //Adv end
        half halfNDL = NDL * 0.5 + 0.5;

        #ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_NORMAL
            // lightmapColor = _Enabledir == 1 ? lerp(lightmapColor,lerp(lightmapColor , NDL * lightmapColor, halfNDL),customlightmapData.lightmapDirintensity): lightmapColor;
            lightmapColor = lerp(lightmapColor,lerp(lightmapColor , NDL * lightmapColor, saturate(halfNDL)),customlightmapData.lightmapDirintensity);
        #endif
        // Adv start

        // half3 lpcolorhsv = RGB2HSV(lightmapColor * customlightmapData.lightmapssscolor);
        // //Chrominance
        // lpcolorhsv.x += customlightmapData.lightmapChrominance;
        // //Chroma
        // lpcolorhsv.z += customlightmapData.lightmapChroma ;
        // half3 lpcolor = HSV2RGB(lpcolorhsv);

        // half3 color = lightmapColor + lpcolor * SAMPLE_TEXTURE2D(_LightmapLUT, sampler_LightmapLUT, 
        // half2(smoothstep(customlightmapData.lightmaptransLow,customlightmapData.lightmaptransHigh, halfNDL), 0)).rgb;
        // half upST = max(0,dot(half3(0,1,0),normal));
        // half bottomST = max(0,dot(half3(0,-1,0),normal));
        // half leftST = saturate(1-upST-bottomST);

        // half shadowatten = saturate(dot(lightmapColor, half3(0.22, 0.707, 0.071)));
        // shadowatten = 1-(smoothstep(customlightmapData.L.x,customlightmapData.L.y,shadowatten)*(1-smoothstep(customlightmapData.L.z,customlightmapData.L.w,shadowatten)));
        // color = color * shadowatten + (1-shadowatten) *
        // (upST * color * customlightmapData.lightmapshadowcolorup + 
        // leftST * color * customlightmapData.lightmapshadowcolorleft + 
        // bottomST * color * customlightmapData.lightmapshadowcolorbuttom);

        //Adv end
        half3 color;
        color = lightmapColor;
        #ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_SSS
            color += customlightmapData.lightmapssscolor * SAMPLE_TEXTURE2D(_LightmapLUT, sampler_LightmapLUT, 
            half2(smoothstep(customlightmapData.lightmaptransLow,customlightmapData.lightmaptransHigh, halfNDL), 0)).rgb;
        #endif
        return color;
    }
#endif

half3 DirectSpecular(AnistroData anistroData, DBRDFData brdfData) {
    half3 L = brdfData.L;
    half3 V = brdfData.V;
    half3 H = brdfData.H;
    half NdotH = brdfData.NdotH;
    half NdotL = brdfData.NdotL;
    half NdotV = brdfData.NdotV;
    half roughness = brdfData.roughness;
    half3 Fs = brdfData.Fs;

    #ifdef _SMASH_DISNEY_ANISOTROPIC_ON
        // #ifdef _SMASH_DISNEY_USEANISTROPIC
        //     float aspect = sqrt(1 - anistroData.anisotropic * .9);
        //     float ax = max(.001, sqr(roughness) / aspect);
        //     float ay = max(.001, sqr(roughness) * aspect);
        //     half Ds = GTR2_aniso(NdotH, dot(H, anistroData.tangentWS), dot(H, anistroData.biTangentWS), ax, ay);
        //     half Gs = smithG_GGX_aniso(NdotL, dot(L, anistroData.tangentWS), dot(L, anistroData.biTangentWS), ax, ay);
        //     Gs *= smithG_GGX_aniso(NdotV, dot(V, anistroData.tangentWS), dot(V, anistroData.biTangentWS), ax, ay);
        //     Ds = min(Ds, 1e+2);
        //     Gs = min(Gs, 1e+2);
        //     return Gs * Fs * Ds;
        // #elif defined (_SMASH_DISNEY_USEKAJIYAANISTROPIC)
        #ifdef _SMASH_DISNEY_USEKAJIYAANISTROPIC
            half perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(anistroData.perceptualRoughness);
            half roughness1 = PerceptualRoughnessToRoughness(perceptualRoughness);
            float pbRoughness1 = RoughnessToBlinnPhongSpecularExponent(roughness1);

            half perceptualRoughness2 = PerceptualSmoothnessToPerceptualRoughness(anistroData.perceptualRoughness2);
            half roughness2 = PerceptualRoughnessToRoughness(perceptualRoughness2);
            float pbRoughness2 = RoughnessToBlinnPhongSpecularExponent(roughness2);

            half3 spec1 = KajiyaKay(anistroData.T1, H, pbRoughness1 * roughness)* anistroData.anisotropicColor * 10;
            half3 spec2 = KajiyaKay(anistroData.T2, H, pbRoughness2 * roughness)* anistroData.anisotropicColor2 * 5;
            half Ds = lerp(GTR2(NdotH, roughness), 1, anistroData.anisotropicMask);
            half Gs = smithG_GGX(NdotL, roughness) * smithG_GGX(NdotV, roughness);
            Gs = lerp(Gs, (spec1.x + spec2.x) * anistroData.anisotropicIntensity, anistroData.anisotropicMask);

            Ds = min(Ds, 1e+1);
            Gs = min(Gs, 1e+1);
            half3 finalColor= Gs * Fs * Ds;
            #ifdef  _SMASH_NO_FLOWDIR
                spec1 = KajiyaKay(anistroData.T1, H, _Highlight_1_Power) * anistroData.anisotropicColor* _Highlight_1_Strength;
                spec2 = KajiyaKay(anistroData.T2, H, _Highlight_2_Power) * anistroData.anisotropicColor2* _Highlight_2_Strength;
                 #ifdef _SMASH_DISNEY_TOON
					finalColor =  saturate((spec1 + spec2) * anistroData.anisotropicIntensity) * brdfData.albedo;
				#else
					finalColor =  lerp(Gs* Ds* Fs, saturate((spec1 + spec2) * anistroData.anisotropicIntensity), anistroData.anisotropicMask);
				#endif
            #endif
            return  finalColor;

        #endif
    #else
        half Ds = GTR2(NdotH, roughness);
        half Gs = smithG_GGX(NdotL, roughness) * smithG_GGX(NdotV, roughness);
        Ds = min(Ds, 1e+1);
        Gs = min(Gs, 1e+1);
        #ifdef _SMASH_DISNEY_TOON
            half3 toonMetalSpec = 0;
            if(brdfData.metallic > 0.5)
            {
                half customNoV = dot(brdfData.N, normalize(brdfData.V+half3(0.2,0.35,0)));
                if(customNoV > _MetalColorAndPos0.w)
                {
                    toonMetalSpec = _MetalColorAndPos0.rgb;
                }
                else if(customNoV > _MetalColorAndPos1.w)
                {
                    toonMetalSpec = _MetalColorAndPos1.rgb;
                }
                else
                {
                    toonMetalSpec = _MetalDarkColor.rgb;
                }
                toonMetalSpec = lerp(toonMetalSpec, toonMetalSpec*Fs, _MetalColorAlbedoTint);
            }
			half specStrength = Gs * Ds;
    		half3 toonSpec = specStrength * brdfData.specularMask;
		    toonSpec = Fs*step(_SpecularThreshold, specStrength)*_SpecularStrength;
            return max(toonSpec, toonMetalSpec);
		#else
            return Gs * Fs * Ds;
        #endif
    #endif
}

half DirectClearcoat(ClearcoatData clearcoatData, DBRDFData brdfData) {
    half3 H = brdfData.H;
    half3 L = brdfData.L;
    half3 V = brdfData.V;
    half FH = brdfData.FH;

    #ifdef _SMASH_DISNEY_CLEARCOAT_ON
        half ccNdotH = dot(clearcoatData.normal, H);
        half ccNdotL = dot(clearcoatData.normal, L);
        half ccNdotV = dot(clearcoatData.normal, V);

        half Dr = GTR1(ccNdotH, lerp(.1, .001, clearcoatData.clearcoatGloss));
        half Fr = lerp(kDieletricSpec.x, 1.0, FH);
        half Gr = smithG_GGX(ccNdotL, .25) * smithG_GGX(ccNdotV, .25);

        Dr = min(Dr, 1e+2);
        Gr = min(Gr, 1e+2);
        return .25 * clearcoatData.clearcoat * Gr * Fr * Dr;
    #else
        return 0;
    #endif
}

half3 DirectDiffuseAndSSS(SubsurfaceData sssData, DBRDFData brdfData, Light light) {
    half3 albedo = brdfData.albedo;
    half NdotL = brdfData.NdotL;
    half NdotV = brdfData.NdotV;
    half LdotH = brdfData.LdotH;
    half roughness = brdfData.roughness;

    half FL = SchlickFresnel(NdotL), FV = SchlickFresnel(NdotV);
    half Fd90 = 0.5 + 2 * LdotH * LdotH * roughness;

    #ifdef _SMASH_DISNEY_SUBSURFACE_ON
        #ifdef _SMASH_DISNEY_TOON
    		half3 color = albedo * SAMPLE_TEXTURE2D(_SubsurfaceLUT, sampler_SubsurfaceLUT, half2(saturate((dot(brdfData.N, brdfData.L)+brdfData.NoLOffset) * 0.5 + 0.5), 1)).rgb;
		    color *= 1 - brdfData.metallic;
        #else
            half3 color = albedo * SAMPLE_TEXTURE2D(_SubsurfaceLUT, sampler_SubsurfaceLUT, half2(dot(brdfData.N, brdfData.L) * 0.5 + 0.5, 1)).rgb;
		#endif
        half transDot = saturate(dot(-brdfData.L, brdfData.V));
        transDot = exp2((transDot - 1) * sssData.transPower);
        color += sssData.color * transDot * (1.0 - NdotL) * sssData.translucency;
        color *= light.color * light.shadowAttenuation * light.distanceAttenuation;
        return color;
    #else
        half Fd = lerp(1.0, Fd90, FL) * lerp(1.0, Fd90, FV);
        return Fd * albedo * light.color * light.shadowAttenuation * light.distanceAttenuation * NdotL;
    #endif

}

half3 DirectSheen(DBRDFData brdfData) {
    half3 albedo = brdfData.albedo;
    half LdotH = brdfData.LdotH;
    half sheen = brdfData.sheen;
    half sheenRange = brdfData.sheenRange;
    half sheenTint = brdfData.sheenTint;

    half Cdlum = .3 * albedo.x + .6 * albedo.y + .1 * albedo.z; // luminance approx.
    Cdlum = max(Cdlum,0.0001);
    half3 Ctint = Cdlum > 0 ? albedo / Cdlum : half3(1, 1, 1); // SafeNormalize lum. to isolate hue+sat
    half3 Csheen = lerp(half3(1, 1, 1), Ctint, sheenTint);

    half3 Fsheen = pow(clamp(1 - LdotH, 0.001, 1), sheenRange) * sheen * Csheen;
    return Fsheen;
}


half3 DisneyDirectBRDF(DBRDFData brdfData,
ClearcoatData clearcoatData,
AnistroData anistroData,
SubsurfaceData subData,
Light light,
half4 GlobalDirectColor)
{
    half3 L = brdfData.L = light.direction;
    half3 N = brdfData.N;
    half3 V = brdfData.V;
    half NdotL = brdfData.NdotL = max(saturate(dot(N, L)),0.0001);
    half NdotV = brdfData.NdotV = max(saturate(dot(N, V)),0.0001);

    #ifdef _SMASH_DISNEY_TOON
		NdotL += brdfData.NoLOffset;
        NdotL = clamp(NdotL,0.0001,1);
        brdfData.NdotL = NdotL;
	#endif

    half3 H = brdfData.H = Unity_SafeNormalize(L + V);
    brdfData.NdotH = dot(N, H);
    half LdotH = brdfData.LdotH = dot(L, H);
    brdfData.LdotV = dot(L, V);

    half FH = brdfData.FH = SchlickFresnel(LdotH);
    brdfData.Fs = lerp(brdfData.specular, half3(1, 1, 1), FH);
    half3 diffuse_shade = DirectDiffuseAndSSS(subData, brdfData, light) * (1 - brdfData.metallic);
    diffuse_shade = DesaturateColor(diffuse_shade, GlobalDirectColor.a) * GlobalDirectColor.rgb;
        #ifdef _SMASH_DISNEY_FULLROUGH
        if(brdfData.perceptualRoughness > 0.75)
        {
            return diffuse_shade;
        }
        #endif
    half3 specular_val = DirectSpecular(anistroData, brdfData);
    half clearcoatSpecular_val = DirectClearcoat(clearcoatData, brdfData);
    half3 Fsheen = 0.0;
#ifdef _SMASH_DISNEY_SHEEN
    Fsheen = DirectSheen(brdfData) * (1 - brdfData.metallic);
#endif

    half3 o = Fsheen + PI * (specular_val + clearcoatSpecular_val);
    o *= light.color * light.shadowAttenuation * light.distanceAttenuation * NdotL;
    o += diffuse_shade;
    return o;
}

half3 DirectBDRF(half roughness2MinusOne, half roughness2, half normalizationTerm, half3 diffuse, half3 specular, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS )
{
        #ifndef _SPECULARHIGHLIGHTS_OFF
        half3 halfDir = SafeNormalize(half3(lightDirectionWS) + half3(viewDirectionWS));
        half NoH = saturate(dot(normalWS, halfDir));
        half LoH = saturate(dot(lightDirectionWS, halfDir));
        half d = NoH * NoH * roughness2MinusOne + 1.00001f;
        half LoH2 = LoH * LoH;
        half specularTerm = roughness2 / ((d * d) * max(0.1h, LoH2) * normalizationTerm);
    
        #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
        specularTerm = specularTerm - HALF_MIN;
        specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
        #endif

        half3 color = specularTerm * specular + diffuse;
        return color;
        #else
        return brdfData.diffuse;
        #endif
}

half3 LightingPhysicallyBased(half roughness2, half3 albedo, half metallic, half3 specular, Light light, half3 normalWS, half3 viewDirectionWS)
{
        half3 diffuse = albedo * OneMinusReflectivityMetallic(metallic);
        half roughness2MinusOne = roughness2 - 1.0h;
        half normalizationTerm = roughness2 * 4.0 + 2.0;
        half NdotL = saturate(dot(normalWS, light.direction));
        half3 radiance = light.color * (light.distanceAttenuation * NdotL);
        return DirectBDRF(roughness2MinusOne, roughness2, normalizationTerm, diffuse, specular, normalWS, light.direction, viewDirectionWS) * radiance;
}

half3 LightingPhysicallyBased(half roughness2MinusOne, half roughness2, half normalizationTerm, half3 diffuse, half3 specular, half3 lightColor, half3 lightDirectionWS, half lightAttenuation, half3 normalWS, half3 viewDirectionWS)
{
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half3 radiance = lightColor * (lightAttenuation * NdotL);
    return DirectBDRF(roughness2MinusOne, roughness2, normalizationTerm, diffuse, specular, normalWS, lightDirectionWS, viewDirectionWS) * radiance;
}


half3 DisneyDirectBRDF_PointLight(DBRDFData brdfData,
// ClearcoatData clearcoatData,
AnistroData anistroData,
SubsurfaceData subData,
Light light)
    {
        half3 L = brdfData.L = light.direction;
        half3 N = brdfData.N;
        half3 V = brdfData.V;
        float NdotL = brdfData.NdotL = max(saturate(dot(N, L)),0.0001);
        // float NdotV = brdfData.NdotV = max(saturate(dot(N, V)),0.0001);

        half3 H = brdfData.H = SafeNormalize(L + V);
        brdfData.NdotH = dot(N, H);
        float LdotH = brdfData.LdotH = saturate(dot(L, H));
        brdfData.LdotV = dot(L, V);

        float FH = brdfData.FH = SchlickFresnel(LdotH);
        brdfData.Fs = lerp(brdfData.specular, float3(1, 1, 1), FH);
        half3 diffuse_shade = DirectDiffuseAndSSS(subData, brdfData, light) * (1 - brdfData.metallic);
        half3 specular_val = DirectSpecular(anistroData, brdfData);
        // half clearcoatSpecular_val = DirectClearcoat(clearcoatData, brdfData);
        // half3 Fsheen = 0.0;
        // #ifdef _SMASH_DISNEY_SHEEN
        // Fsheen = DirectSheen(brdfData) * (1 - brdfData.metallic);
        // #endif

        half3 o =  PI * (specular_val);
        o *= light.color * light.shadowAttenuation * light.distanceAttenuation * NdotL;
        o += diffuse_shade;
        return o;
    }

//BRDFData Initialize
inline void DisneyInitializeBRDFData(half3 albedo, half metallic, half smoothness, half alpha, half sheen, half sheenRange, half sheenTint, out DBRDFData outBRDFData)
{
    outBRDFData = (DBRDFData)0;
    half oneMinusReflectivity = OneMinusReflectivityMetallic(metallic);
    half reflectivity = 1.0 - oneMinusReflectivity;
    outBRDFData.albedo = albedo;
    //outBRDFData.diffuse = albedo * oneMinusReflectivity;
    outBRDFData.specular = lerp(kDieletricSpec.rgb, albedo, metallic);
    outBRDFData.metallic = metallic;
    outBRDFData.sheen = sheen;
    outBRDFData.sheenRange = sheenRange;
    outBRDFData.sheenTint = sheenTint;
    outBRDFData.gi_grazingTerm = saturate(smoothness + reflectivity);
    outBRDFData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(smoothness);
    outBRDFData.roughness = max(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness), HALF_MIN);
    outBRDFData.roughness2 = outBRDFData.roughness * outBRDFData.roughness;

    #ifdef _ALPHAPREMULTIPLY_ON
        outBRDFData.albedo *= alpha;
        //outBRDFData.diffuse *= alpha;
    #endif
}
half3 DisneyGlobalIllumination(DBRDFData brdfData, half3 bakedGI, half occlusion, half3 occlusionCol, /*ClearcoatData clearcoatData,*/  int IndSpeByDifToggle, float IndSpeByDifFac,float GIFresnel, half4 GlobalInDirectColor)
{
    half3 V = brdfData.V;
    half3 N = brdfData.N;
    half sheenTint = brdfData.sheenTint;
    half3 indirectDiffuse = lerp(occlusionCol, bakedGI, occlusion);//Art dark part of diffuse
   // indirectDiffuse = (DesaturateColor(indirectDiffuse, GlobalInDirectColor.a) * GlobalInDirectColor.rgb);
    indirectDiffuse = clamp(DesaturateColor(indirectDiffuse, GlobalInDirectColor.a) * GlobalInDirectColor.rgb, 0,10);
    //SPECULAR1
    half3 reflectVector = reflect(-V, N);
    #ifdef _SMASH_DISNEY_TOON
        half3 indirectSpecular = 0;
    #else
        half3 indirectSpecular = GLOSSYENVIRONMENTREFLECTION(reflectVector, brdfData.perceptualRoughness, occlusion);
    #endif
    if (IndSpeByDifToggle) {
        indirectSpecular *= max(0, lerp(1, min(0.99, 0.2125 * indirectDiffuse.r + 0.7154 * indirectDiffuse.g + 0.0721 * indirectDiffuse.b), IndSpeByDifFac));
    }
    half surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
    half fresnelTerm = Pow4(1.0 - saturate(dot(N, V)));
    fresnelTerm = lerp(0,fresnelTerm,GIFresnel);
        #ifdef _SMASH_DDLOBBY_JUNE
        if(_Lobby_june == 1)
        {
            fresnelTerm *= 0.65;
        }
        #endif
    half3 c = surfaceReduction * indirectSpecular * lerp(brdfData.specular, brdfData.gi_grazingTerm, fresnelTerm);
        #ifdef _SMASH_DISNEY_FULLROUGH
        if(brdfData.perceptualRoughness > 0.75)
        {
            if(brdfData.metallic < 0.9)
            {
                return brdfData.albedo * (1 - brdfData.metallic) * indirectDiffuse;
            }
            return c + brdfData.albedo * (1 - brdfData.metallic) * indirectDiffuse;
        }
        #endif
        #ifdef LIGHTMAP_ON
            return c + brdfData.albedo * (1 - brdfData.metallic) * indirectDiffuse;
        #endif
    //DIFFUSE
    half3 Cdlin = brdfData.albedo;
    half Cdlum = .3 * Cdlin.x + .6 * Cdlin.y + .1 * Cdlin.z; // luminance approx.
    Cdlum = max(Cdlum,0.0001);
    half3 Ctint = Cdlum > 0 ? Cdlin / Cdlum : half3(1, 1, 1); // normalize lum. to isolate hue+sat
    half3 Csheen = lerp(half3(1, 1, 1), Ctint, sheenTint);
    half3 Fsheen = 0.0;
#ifdef _SMASH_DISNEY_SHEEN
    Fsheen = pow(clamp(1 - dot(V, N), 0.001, 1), brdfData.sheenRange) * brdfData.sheen * Csheen;
#endif
    c += (brdfData.albedo + Fsheen) * (1 - brdfData.metallic) * indirectDiffuse;

// #ifdef _SMASH_DISNEY_CLEARCOAT_ON
//     half3 cc_reflectVector = reflect(-V, clearcoatData.normal);
//     half3 cc_indirectSpecular = GLOSSYENVIRONMENTREFLECTION(cc_reflectVector, 1 - clearcoatData.clearcoatGloss, occlusion);
//     half3 cc_specular = kDieletricSpec.rgb;
//     half3 cc_grazingTerm = saturate(2.0 - kDieletricSpec.a);
//     half cc_fresnelTerm = Pow4(1.0 - saturate(dot(clearcoatData.normal, V)));
//     c += 1 * cc_indirectSpecular * lerp(cc_specular, cc_grazingTerm, cc_fresnelTerm) * clearcoatData.clearcoat;// *reflectionIntensity;
// #endif
    return c;
}

half4 DisneyBRDF(InputData inputData,
half3 albedo,
half metallic,
half roughness,
half3 emission,
half alpha,
half occlusion = 1,
half3 occlusionCol = half3(1.0,1.0,1.0),
half sheen = 0,
half sheenRange = 5.0,
half sheenTint = 0,
AnistroData anistroData = (AnistroData)0,
ClearcoatData clearcoatData = (ClearcoatData)0,
SubsurfaceData subData = (SubsurfaceData)0,
half ShadowIntensity = 1,
half DirectLightIntensity = 1,
half GIIntensity = 1,
half GIFresnel = 1,
int IndSpeByDifToggle = 0, float IndSpeByDifFac = 0,
half4 GlobalDirectColor = 1, half4 GlobalInDirectColor = 1,
half NoLOffset = 0, half specularMask = 1)
    {
        //tmp close GlobalDirectColor and GlobalInDirectColor
        GlobalDirectColor = 1; GlobalInDirectColor = 1;

        DBRDFData brdfData;
        DisneyInitializeBRDFData(albedo, metallic, 1 - roughness, alpha, sheen, sheenRange, sheenTint, brdfData);
        brdfData.V = inputData.viewDirectionWS;
        brdfData.N = inputData.normalWS;

        #ifdef _SMASH_DISNEY_TOON
		        brdfData.NoLOffset = NoLOffset;
		        brdfData.specularMask = specularMask;
		#endif

        Light mainLight = GetMainLight(inputData.shadowCoord);
        
        mainLight.shadowAttenuation = lerp(1, mainLight.shadowAttenuation, ShadowIntensity);
        // return smoothstep(0.35,0.45,inputData.positionWS.y)+(1-smoothstep(0.1,0.25,inputData.positionWS.y));
        #ifdef _SMASH_DISNEY_SHADOWMASK
        if(_CustomShadowBias.w == 1)
        {
            half shadowmask = smoothstep(0.25,0.3,inputData.positionWS.y)+(1-smoothstep(0.15,0.2,inputData.positionWS.y));
            // return shadowmask;
            mainLight.shadowAttenuation = lerp(1,mainLight.shadowAttenuation,shadowmask);
        }
        else if(_CustomShadowBias.w == 0.9)
        {
            half shadowmask = smoothstep(0.9,0.95,inputData.positionWS.y)+(1-smoothstep(0.82,0.87,inputData.positionWS.y));
            // return shadowmask;
            mainLight.shadowAttenuation = lerp(1,mainLight.shadowAttenuation,shadowmask);
        }

        #endif
        MixRealtimeAndBakedGI(mainLight, brdfData.N, inputData.bakedGI, half4(0, 0, 0, 0));

        half3 giColor = DisneyGlobalIllumination(brdfData, inputData.bakedGI, occlusion, occlusionCol, IndSpeByDifToggle, IndSpeByDifFac, GIFresnel, GlobalInDirectColor) * GIIntensity;
        half3 directBRDFColor = 0;
        #ifdef LIGHTMAP_ON 
        #ifdef _SMASH_DISNEY_CUSTOMLIGHTMAP_ENHANCENORM
        if (_EnableHightLightNormal == 1) {
            half3 CustomLightmapDir = normalize(_CustomLightmapdir);
            half NdotL = dot(CustomLightmapDir, inputData.normalWS);
            half3 H = normalize(CustomLightmapDir + inputData.viewDirectionWS);
            half NdotH = saturate(dot(inputData.normalWS, H));
            half LdotH = saturate(dot(CustomLightmapDir, H));
            float d = NdotH * NdotH * (brdfData.roughness2 - 1)  + 1.00001f;
            half LdotH2 = LdotH * LdotH;
            half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LdotH2) * (brdfData.roughness * 4.0h + 2.0h));
            specularTerm = clamp(specularTerm, 0.0, 1e+1);
            half3 highlight = saturate(NdotL) * _CustomLightmapCol * _CustomLightSpecIntensity * PI * specularTerm * lerp(kDieletricSpec.rgb, albedo, metallic);
            emission += max(0, min(highlight, 2));
        }
        #endif
        return half4(giColor + emission, alpha);
        #else
        #ifndef _SMASH_NOMAINLIGHT
        directBRDFColor = DisneyDirectBRDF(brdfData, clearcoatData, anistroData, subData, mainLight, GlobalDirectColor);
        #endif

        #endif
        #ifdef _ADDITIONAL_LIGHTS
            #ifdef _SMASH_LOD_ADDLIGHT_OFF
                if(_CustomShadowBias.w != 0.88)
                    {
                        uint pixelLightCount = GetAdditionalLightsCount();
                        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                        {
                            Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
                            directBRDFColor += DisneyDirectBRDF(brdfData, clearcoatData, anistroData, subData, light, GlobalDirectColor);
                        }
                    }
            #else
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
                    directBRDFColor += DisneyDirectBRDF(brdfData, clearcoatData, anistroData, subData, light, GlobalDirectColor);
                }
            #endif
        #endif
        #ifdef _SMASH_NOMAINLIGHT
            directBRDFColor *= mainLight.shadowAttenuation;
        #endif
        half3 finalColor = directBRDFColor * DirectLightIntensity;
        finalColor += giColor;
        finalColor += emission;
        return half4(finalColor, alpha);
    }

half4 DisneySimpleBRDF(InputData inputData,
half3 albedo,
half3 emission,
half alpha,
half ShadowIntensity = 1,
half DirectLightIntensity = 1,
half GIIntensity = 1)
{
    DBRDFData brdfData;
    brdfData = (DBRDFData)0;
    brdfData.albedo = albedo;

    #ifdef _ALPHAPREMULTIPLY_ON
        brdfData.albedo *= alpha;
    #endif

    Light mainLight = GetMainLight(inputData.shadowCoord);
    half3 N = inputData.normalWS;
    half3 L = mainLight.direction;
    half NdotL = saturate(dot(N, L));

    mainLight.shadowAttenuation = lerp(1, mainLight.shadowAttenuation, ShadowIntensity);
    MixRealtimeAndBakedGI(mainLight, N, inputData.bakedGI, half4(0, 0, 0, 0));

    half3 giColor = brdfData.albedo * inputData.bakedGI * GIIntensity;

    #ifdef LIGHTMAP_ON 
        return half4(giColor + emission, 1);
    #else
        half3 directBRDFColor = albedo;
        directBRDFColor *= mainLight.color * mainLight.shadowAttenuation * mainLight.distanceAttenuation * NdotL;
        half3 finalColor = directBRDFColor * DirectLightIntensity;
        finalColor += giColor + emission;
        return half4(finalColor, alpha);
    #endif
}
