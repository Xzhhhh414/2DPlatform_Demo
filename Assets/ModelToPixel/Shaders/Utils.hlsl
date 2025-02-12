float sqr(float x) { return x * x; }

float min_v3(float3 v) { return min(v.x, min(v.y, v.z)); }

float3 DesaturateColor(float3 inputRGB, float frac) {
    float3 dcolor = dot(inputRGB, float3(0.299, 0.587, 0.114));
    return lerp(dcolor, inputRGB, frac);
}

float2 voronoi_fun(float2 p)
{
    p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
    return frac(sin(p) * 43758.5453);
}

float voronoi(float2 v, float time)
{
    float2 n = floor(v);
    float2 f = frac(v);
    float F1 = 8.0;
    float F2 = 8.0; float2 mg = 0;
    for (int j = -1; j <= 1; j++)
    {
        for (int i = -1; i <= 1; i++)
        {
            float2 g = float2(i, j);
            float2 o = voronoi_fun(n + g);
            o = (sin(time + o * 6.2831) * 0.5 + 0.5); float2 r = f - g - o;
            float d = 0.5 * dot(r, r);
            if (d < F1) {
                F2 = F1;
                F1 = d; mg = g;
            }
            else if (d < F2) {
                F2 = d;
            }
        }
    }
    return F1;
}

half overlayLerpBlend(half base, half overlay, half lerpFactor) {
    int tmp = step(base, 0.5f);
    return lerp(base, lerp(2 * base * overlay, 1 - 2 * (1 - base) * (1 - overlay), tmp), lerpFactor);
}

float SchlickFresnel(float u)
{
    float m = clamp(1 - u, 0, 1);
    float m2 = m * m;
    return m2 * m2 * m; // pow(m,5)
}

float3 HeightToNormal(float height, float strength, float3 posVS, float3 normalWS) {
    //ASE type
    //float dPdx = ddx(posWS * 100.0);
    //float dPdy = ddy(posWS * 100.0);
    //float len = abs(dot(cross(normalWS, dPdy), dPdx));
    //float3 dheight = (ddx(height) * cross(normalWS, dPdy) + ddy(height) * cross(normalWS, dPdx));
    //dheight.y *= -1;
    //return normalize(((len * normalWS) - dheight));

    //blender type
    float3 N = mul(unity_MatrixV, half4(normalize(normalWS), 0)).xyz;
    float3 dPdx = ddx(posVS);
    float3 dPdy = ddy(posVS);
    float dHdx = ddx(height);
    float dHdy = ddy(height);

    /* Get surface tangents from normal. */
    float3 Rx = cross(dPdy, N);
    float3 Ry = cross(N, dPdx);

    /* Compute surface gradient and determinant. */
    float det = dot(dPdx, Rx);

    float3 surfgrad = dHdx * Rx + dHdy * Ry;

    strength = max(strength, 0.0);

    float3 result = normalize(abs(det) * N - sign(det) * surfgrad);
    result = normalize(lerp(N, result, strength));

    return mul(unity_MatrixInvV, half4(result, 0)).xyz;
}

float GTR1(float NdotH, float a)
{
    if (a >= 1) return 1 / PI;
    float a2 = a * a;
    float t = 1 + (a2 - 1) * NdotH * NdotH;
    return (a2 - 1) / (PI * log(a2) * t);
}

float GTR2(float NdotH, float a)
{
    float a2 = a * a;
    float t = 1 + (a2 - 1) * NdotH * NdotH;
    return (HALF_MIN + a2) / (PI * t * t);
}

float GTR2_aniso(float NdotH, float HdotX, float HdotY, float ax, float ay)
{
    return 1 / max((PI * ax * ay * sqr(sqr(HdotX / ax) + sqr(HdotY / ay) + NdotH * NdotH)),  1e-6);
}

float smithG_GGX(float NdotV, float alphaG)
{
    float a = alphaG * alphaG;
    float b = NdotV * NdotV;
    return 1 / max((NdotV + sqrt(a + b - a * b)), 1e-6);
}

float smithG_GGX_aniso(float NdotV, float VdotX, float VdotY, float ax, float ay)
{
    return 1 / max((NdotV + sqrt(sqr(VdotX * ax) + sqr(VdotY * ay) + sqr(NdotV))), 1e-6);
}

float3 mon2lin(float3 x)
{
    return float3(pow(x[0], 2.2), pow(x[1], 2.2), pow(x[2], 2.2));
}

float RoughnessToBlinnPhongSpecularExponent(float roughness)
{
    return clamp(2 * rcp(roughness * roughness) - 2, FLT_EPS, rcp(FLT_EPS));//FLT_EPS  5.960464478e-8  // 2^-24, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)
}
//http://web.engr.oregonstate.edu/~mjb/cs519/Projects/Papers/HairRendering.pdf
float3 ShiftTangentX(float3 T, float3 N, float shift)
{
    return SafeNormalize(T + N * shift);
}

// Note: this is Blinn-Phong, the original paper uses Phong.
float KajiyaKay(float3 T, float3 H, float specularExponent)
{
    float TdotH = clamp(dot(T, H), -(1.0 - 1e-9), 1.0 - 1e-9);
    float sinTH = sqrt(1.0 - TdotH * TdotH);
    float dirAtten = smoothstep(-1, 0, dot(T, H));
    return dirAtten * pow(sinTH, specularExponent);
}

half3 AnimateVertex(half3 pos, half3 normal, half3 animParams, half4 wind)
{
    // animParams stored in color
    // animParams.x = random phase 
    // animParams.y = primary factor
    // animParams.z = secondary factor

    //wind.xyz = direction * main
    //wind.w = slef shake factor

    half fDetailAmp = 0.1f;
    half fBranchAmp = 0.3f;

    // Phases (object, vertex, branch)
    half fObjPhase = dot(unity_ObjectToWorld._14_24_34, 1);
    half fBranchPhase = fObjPhase + animParams.x;

    half fVtxPhase = dot(pos, fBranchPhase);

    half2 vWavesIn = _Time.yy + half2(fVtxPhase, fBranchPhase);

    // 1.975, 0.793, 0.375, 0.193 are good frequencies
    half4 vWaves = (frac(vWavesIn.xxyy * half4(1.975, 0.793, 0.375, 0.193)) * 2.0 - 1.0);

    vWaves = 0.5 * sin(2 * PI * (vWaves - 1.25)) + 0.5;
    half2 vWavesSum = vWaves.xz + vWaves.yw;

    // Edge (xz) and branch bending (y)
    half3 bend = fDetailAmp * normal.xyz;
    bend.y = animParams.z * fBranchAmp;
    pos += ((vWavesSum.xyx * bend) + (wind.xyz * vWavesSum.y * animParams.z)) * wind.w;

    // Primary bending
    // Displace position
    pos += animParams.y * wind.xyz;

    return pos;
}

float F_eta(float eta, float cos_theta) {
    float f0 = pow(eta - 1, 2) / pow(eta + 1, 2);
    return lerp(f0, 1, SchlickFresnel(cos_theta));
}

inline half3 Unity_SafeNormalize(half3 inVec)
{
    half dp3 = max(HALF_MIN, dot(inVec, inVec));
    return inVec * rsqrt(dp3);
}

#if defined(_Glass) 
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

#define BTDF_BIAS 0.85
#define _MAX_STEP 4

float3 project_point(float4x4 projMat, float3 vpos) {
    float4 ps = mul(projMat, float4(vpos, 1));
    return ps.xyz / ps.w;
}

float line_unit_box_intersect_dist(float3 lineorigin, float3 linedirection)
{
    /* https://seblagarde.wordpress.com/2012/09/29/image-based-lighting-approaches-and-parallax-corrected-cubemap/
     */
    float3 firstplane = (1.0 - lineorigin) / linedirection;
    float3 secondplane = (-1.0 - lineorigin) / linedirection;
    float3 furthestplane = max(firstplane, secondplane);

    return min_v3(furthestplane);
}

/* See times_and_deltas. */
#define curr_time times_and_deltas.x
#define prev_time times_and_deltas.y
#define curr_delta times_and_deltas.z
#define prev_delta times_and_deltas.w

float3 tangent_to_world(float3 v, float3 N, float3 T, float3 B)
{
    return T * v.x + B * v.y + N * v.z;
}

void make_orthonormal_basis(float3 N, out float3 T, out float3 B)
{
    float3 UpVector = abs(N.z) < 0.99999 ? float3(0.0, 0.0, 1.0) : float3(1.0, 0.0, 0.0);
    T = normalize(cross(UpVector, N));
    B = cross(N, T);
}

float3 sample_ggx(float3 rand, float a2)
{
    /* Theta is the aperture angle of the cone */
    float z = sqrt((1.0 - rand.x) / (1.0 + a2 * rand.x - rand.x)); /* cos theta */
    float r = sqrt(max(0.0, 1.0f - z * z));                        /* sin theta */
    float x = r * rand.y;
    float y = r * rand.z;

    /* Microfacet Normal */
    return float3(x, y, z);
}

float3 sample_ggx(float3 rand, float a2, float3 N, float3 T, float3 B, out float NH)
{
    float3 Ht = sample_ggx(rand, a2);
    NH = Ht.z;
    return tangent_to_world(Ht, N, T, B);
}

/* GGX */
float D_ggx_opti(float NH, float a2)
{
    float tmp = (NH * a2 - NH) * NH + 1.0;
    return PI * tmp * tmp; /* Doing RCP and mul a2 at the end */
}

float pdf_ggx_reflect(float NH, float a2)
{
    return NH * a2 / D_ggx_opti(NH, a2);
}


float3 get_view_space_from_depth(float2 uv, float depth_fromTex)
{
    //opengl input to dx input if REVERSED_Z
#if UNITY_REVERSED_Z
    depth_fromTex = 1 - depth_fromTex;
#endif
    float zView = LinearEyeDepth(depth_fromTex, _ZBufferParams);
    float xView = (2 * uv.x - 1) * zView / UNITY_MATRIX_P[0][0];
    float yView = (2 * uv.y - 1) * zView / abs(UNITY_MATRIX_P[1][1]);
    float4 posView = float4(xView, yView, -zView, 1);
    return posView.xyz;
}

/* [Drobot2014a] Low Level Optimizations for GCN */
float fast_sqrt(float x)
{
    int i = asint(x);
    i = 0x1FBD1DF5 + (i >> 1);
    return asfloat(i);
}

float2 fast_sqrt(float2 v)
{
    return float2(fast_sqrt(v.x), fast_sqrt(v.y));
}

void prepare_raycast(float4x4 projMat, float3 ray_origin,
    float3 ray_dir,
    float thickness,
    int index,
    out float4 ss_step,
    out float4 ss_ray,
    out float max_time)
{
    float z_sign = -sign(ray_dir.z);
    float3 ray_end = ray_origin + z_sign * ray_dir;

    float4 ss_start, ss_end;
    ss_start.xyz = project_point(projMat, ray_origin);
    ss_end.xyz = project_point(projMat, ray_end);

    ray_origin.z -= thickness;
    ray_end.z -= thickness;
    ss_start.w = project_point(projMat, ray_origin).z;
    ss_end.w = project_point(projMat, ray_end).z;

    ss_start.w = 2.0 * ss_start.z - ss_start.w;
    ss_end.w = 2.0 * ss_end.z - ss_end.w;
    ss_step = ss_end - ss_start;

    max_time = length(ss_step.xyz);
    ss_step = z_sign * ss_step / length(ss_step.xyz);
    /* If the line is degenerate, make it cover at least one pixel
     * to not have to handle zero-pixel extent as a special case later */
    ss_step.xy += (dot(ss_step.xy, ss_step.xy) < 0.00001) ? 0.001 : 0.0;
    /* Make ss_step cover one pixel. */
    ss_step /= max(abs(ss_step.x), abs(ss_step.y));
    ss_step *= (abs(ss_step.x) > abs(ss_step.y)) ? 1.0 / _ScreenParams.x : 1.0 / _ScreenParams.y;
    /* Clip to segment's end. */
    max_time /= length(ss_step.xyz);
    /* Clipping to frustum sides. */
    max_time = min(max_time, line_unit_box_intersect_dist(ss_start.xyz, ss_step.xyz));
    ss_ray = ss_start * 0.5 + 0.5;
    ss_step *= 0.5;
}

float SampleSceneDepthOpengl(float2 uv) {
#if UNITY_REVERSED_Z
    return 1 - SampleSceneDepth(uv);
#else
    return SampleSceneDepth(uv);
#endif
}

/* Return the hit position, and negate the z component (making it positive) if not hit occurred. */
/* __ray_dir__ is the ray direction premultiplied by it's maximum length */
float3 raycast(float4x4 projMat,
    int index,
    float3 ray_origin,
    float3 ray_dir,
    float thickness,
    float ray_jitter,
    float trace_quality)
{
    float4 ss_step, ss_start;
    float max_time;
    prepare_raycast(projMat, ray_origin, ray_dir, thickness, index, ss_step, ss_start, max_time);

    float max_trace_time = max(0.01, max_time - 0.01);
    float4 times_and_deltas = 0.0;
    float ray_time = 0.0;
    float depth_sample = SampleSceneDepthOpengl(ss_start.xy);
    curr_delta = depth_sample - ss_start.z;

    bool hit = false;
    float iter;
    for (iter = 1.0; !hit && (ray_time < max_time) && (iter < _MAX_STEP); iter++) {
        float stride = max(1.0, iter * trace_quality);
        ray_time += stride;
        times_and_deltas.xyzw = times_and_deltas.yxwz;
        float jit_stride = lerp(2.0, stride, ray_jitter);
        curr_time = min(ray_time + jit_stride, max_trace_time);
        float4 ss_ray = ss_start + ss_step * curr_time;
        depth_sample = SampleSceneDepthOpengl(ss_start.xy);
        float prev_w = ss_start.w + ss_step.w * prev_time;
        curr_delta = depth_sample - ss_ray.z;
        hit = (prev_w <= depth_sample) && (depth_sample <= ss_ray.z);
    }
    hit = hit && (depth_sample != 1.0);
    curr_time = (hit) ? lerp(prev_time, curr_time, saturate(prev_delta / (prev_delta - curr_delta))) : curr_time;
    ray_time = (hit) ? curr_time : ray_time;

    /* Clip to frustum. */
    ray_time = max(0.001, min(ray_time, max_time - 1.5));
    float4 ss_ray = ss_start + ss_step * ray_time;

    /* Tag Z if ray failed. */
    ss_ray.z *= (hit) ? 1.0 : -1.0;
    return ss_ray.xyz;
}

float screen_border_mask(float2 hit_co)
{
    const float margin = 0.003;
    float atten = _SSRBorderFac + margin; /* Screen percentage */
    hit_co = smoothstep(margin, atten, hit_co) * (1 - smoothstep(1.0 - atten, 1.0 - margin, hit_co));

    float screenfade = hit_co.x * hit_co.y;

    return screenfade;
}


float4 screen_space_refraction(float3 viewPosition, float3 N, float3 V, float ior, float roughnessSquared, float4 rand)
{
    float4x4 projMat = (float4x4)0;
    projMat[0][0] = abs(UNITY_MATRIX_P[0][0]);
    projMat[1][1] = abs(UNITY_MATRIX_P[1][1]);
    projMat[2][2] = (_ProjectionParams.y + _ProjectionParams.z) / (_ProjectionParams.y - _ProjectionParams.z);
    projMat[2][3] = 2 * _ProjectionParams.y * _ProjectionParams.z / (_ProjectionParams.y - _ProjectionParams.z);
    projMat[3][2] = -1;

    float a2 = max(5e-6, roughnessSquared * roughnessSquared);

    /* Importance sampling bias */
    rand.x = lerp(rand.x, 0.0, BTDF_BIAS);

    float3 T, B;
    float NH;
    make_orthonormal_basis(N, T, B);
    float3 H = sample_ggx(rand.xzw, a2, N, T, B, NH); /* Microfacet normal */
    float HV = dot(H, V);
    float frenel = F_eta(ior, HV);
    float pdf = pdf_ggx_reflect(NH, a2);

    /* If ray is bad (i.e. going below the plane) regenerate. */
    if (frenel < 1.0) {
        H = sample_ggx(rand.xzw * float3(1.0, -1.0, -1.0), a2, N, T, B, NH); /* Microfacet normal */
        pdf = pdf_ggx_reflect(NH, a2);
    }

    float eta = 1.0 / ior;
    if (HV < 0.0) {
        H = -H;
        eta = ior;
    }
    float3 R = refract(-V, H, 1.0 / ior);
    R = mul(UNITY_MATRIX_V, float4(R, 0.0)).xyz;
    float3 hit_pos = raycast(projMat,
        -1, viewPosition, R * 1e16, _SSRThickness, rand.y, _SSRQuality);
    //return half4(hit_pos, 1);
    if ((hit_pos.z > 0.0) && (frenel < 1.0)) {
        hit_pos = get_view_space_from_depth(hit_pos.xy, hit_pos.z);
        float2 hit_uvs = project_point(projMat, hit_pos).xy * 0.5 + 0.5;
        float3 spec = SampleSceneColor(hit_uvs).xyz;
        //return half4(hit_uvs,0,1);
        float mask = screen_border_mask(hit_uvs);
        return float4(spec, mask);
    }
    return 0.0;
}


float3 refraction_dominant_dir(float3 N, float3 V, float roughness, float ior)
{
    /* TODO: This a bad approximation. Better approximation should fit
     * the refracted vector and roughness into the best prefiltered reflection
     * lobe. */
     /* Correct the IOR for ior < 1.0 to not see the abrupt delimitation or the TIR */
    float eta = 1.0 / ior;
    float NV = dot(N, -V);
    /* Custom Refraction. */
    float k = 1.0 - eta * eta * (1.0 - NV * NV);
    k = max(0.0, k); /* Only this changes. */
    float3 R = eta * -V - (eta * NV + sqrt(k)) * N;
    return R;
}
#endif




//----------------------3d perline noise------------------------------------
#define rot(x, k) (((x) << (k)) | ((x) >> (32 - (k))))
#define FLOORFRAC(x, x_int, x_fract) { float x_floor = floor(x); x_int = int(x_floor); x_fract = x - x_floor; }

#define final(a, b, c) \
  { \
    c ^= b; \
    c -= rot(b, 14); \
    a ^= c; \
    a -= rot(c, 11); \
    b ^= a; \
    b -= rot(a, 25); \
    c ^= b; \
    c -= rot(b, 16); \
    a ^= c; \
    a -= rot(c, 4); \
    b ^= a; \
    b -= rot(a, 14); \
    c ^= b; \
    c -= rot(b, 24); \
  }

uint hash_uint3(uint kx, uint ky, uint kz)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (3u << 2u) + 13u;

    c += kz;
    b += ky;
    a += kx;
    final(a, b, c);

    return c;
}

uint hash_int3(int kx, int ky, int kz)
{
    return hash_uint3(uint(kx), uint(ky), uint(kz));
}

float fade(float t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float negate_if(float value, uint condition)
{
    return (condition != 0u) ? -value : value;
}

float tri_mix(float v0,
    float v1,
    float v2,
    float v3,
    float v4,
    float v5,
    float v6,
    float v7,
    float x,
    float y,
    float z)
{
    float x1 = 1.0 - x;
    float y1 = 1.0 - y;
    float z1 = 1.0 - z;
    return z1 * (y1 * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x)) +
        z * (y1 * (v4 * x1 + v5 * x) + y * (v6 * x1 + v7 * x));
}

float noise_grad(uint hash, float x, float y, float z)
{
    uint h = hash & 15u;
    float u = h < 8u ? x : y;
    float vt = ((h == 12u) || (h == 14u)) ? x : z;
    float v = h < 4u ? y : vt;
    return negate_if(u, h & 1u) + negate_if(v, h & 2u);
}


float noise_perlin(float3 vec)
{
    int X, Y, Z;
    float fx, fy, fz;

    FLOORFRAC(vec.x, X, fx);
    FLOORFRAC(vec.y, Y, fy);
    FLOORFRAC(vec.z, Z, fz);

    float u = fade(fx);
    float v = fade(fy);
    float w = fade(fz);

    float r = tri_mix(noise_grad(hash_int3(X, Y, Z), fx, fy, fz),
        noise_grad(hash_int3(X + 1, Y, Z), fx - 1, fy, fz),
        noise_grad(hash_int3(X, Y + 1, Z), fx, fy - 1, fz),
        noise_grad(hash_int3(X + 1, Y + 1, Z), fx - 1, fy - 1, fz),
        noise_grad(hash_int3(X, Y, Z + 1), fx, fy, fz - 1),
        noise_grad(hash_int3(X + 1, Y, Z + 1), fx - 1, fy, fz - 1),
        noise_grad(hash_int3(X, Y + 1, Z + 1), fx, fy - 1, fz - 1),
        noise_grad(hash_int3(X + 1, Y + 1, Z + 1), fx - 1, fy - 1, fz - 1),
        u,
        v,
        w);

    return r;
}

float noise_scale3(float result)
{
    return 0.9820 * result;
}


float snoise(float3 p)
{
    float r = noise_perlin(p);
    return (isinf(r)) ? 0.0 : noise_scale3(r);
}

float noise(float3 p)
{
    return 0.5 * snoise(p) + 0.5;
}

/* The fractal_noise functions are all exactly the same except for the input type. */
float fractal_noise(float3 p, float octaves, float roughness)
{
    float fscale = 1.0;
    float amp = 1.0;
    float maxamp = 0.0;
    float sum = 0.0;
    octaves = clamp(octaves, 0.0, 16.0);
    int n = int(octaves);
    for (int i = 0; i <= n; i++) {
        float t = noise(fscale * p);
        sum += t * amp;
        maxamp += amp;
        amp *= clamp(roughness, 0.0, 1.0);
        fscale *= 2.0;
    }
    float rmd = octaves - floor(octaves);
    if (rmd != 0.0) {
        float t = noise(fscale * p);
        float sum2 = sum + t * amp;
        sum /= maxamp;
        sum2 /= maxamp + amp;
        return (1.0 - rmd) * sum + rmd * sum2;
    }
    else {
        return sum / maxamp;
    }
}

uint hash_uint2(uint kx, uint ky)
{
    uint a, b, c;
    a = b = c = 0xdeadbeefu + (2u << 2u) + 13u;

    b += ky;
    a += kx;
    final(a, b, c);

    return c;
}

float hash_uint2_to_float(uint kx, uint ky)
{
    return float(hash_uint2(kx, ky)) / float(0xFFFFFFFFu);
}


float hash_vec2_to_float(float2 k)
{
    //return hash_uint2_to_float(floatBitsToUint(k.x), floatBitsToUint(k.y));
    return hash_uint2_to_float(asuint(k.x), asuint(k.y));
}

float3 random_vec3_offset(float seed)
{
    return float3(100.0 + hash_vec2_to_float(float2(seed, 0.0)) * 100.0,
        100.0 + hash_vec2_to_float(float2(seed, 1.0)) * 100.0,
        100.0 + hash_vec2_to_float(float2(seed, 2.0)) * 100.0);
}

void node_noise_texture_3d(float3 co, float scale, float detail, float roughness, float distortion, out float value)
{
    float3 p = co * scale;
    if (distortion != 0.0) {
        p += float3(snoise(p + random_vec3_offset(0.0)) * distortion,
            snoise(p + random_vec3_offset(1.0)) * distortion,
            snoise(p + random_vec3_offset(2.0)) * distortion);
    }
    value = fractal_noise(p, detail, roughness);
}

float3 LocalToBoundingPos(float3 localPos, float3 bmin, float3 bsize) {
    return (localPos - bmin) / bsize;
}

float3 UnityCoord2UE4(float3 inPos) {
    return float3(-inPos.x, inPos.z, inPos.y);
}


//converts an input 1d to 2d position. Useful for locating z frames that have been laid out in a 2d grid like a flipbook.
float2 Convert1dto2d(float XSize, float idx)
{
    float2 xyidx = 0;
    xyidx.x = fmod(idx, XSize);
    xyidx.y = floor(idx / XSize);

    return xyidx;
}

// return a pseudovolume textuer sample. treats 2d layout of frames a 3d texture and performs bilinear filtering by blending with an offset Z frame.
// @param Tex       = Input Texture Object storing Volume Data
// @param inPos     = Input float3 for Position, 0-1
// @param xsize     = Input float for num frames in x,y directions
// @param numFrames = Input float for num total frames
float4 PseudoVolumeTexture(Texture2D Tex, SamplerState TexSampler, float3 inPos, float xsize, float numframes)
{
    inPos = UnityCoord2UE4(inPos);
    float zframe = ceil(inPos.z * numframes);
    float zphase = frac(inPos.z * numframes);

    float2 uv = frac(inPos.xy) / xsize;

    float2 curframe = Convert1dto2d(xsize, zframe) / xsize;
    float2 nextframe = Convert1dto2d(xsize, zframe + 1) / xsize;

    float4 sampleA = Tex.SampleLevel(TexSampler, uv + curframe, 0);
    float4 sampleB = Tex.SampleLevel(TexSampler, uv + nextframe, 0);

    return lerp(sampleA, sampleB, zphase);
}


