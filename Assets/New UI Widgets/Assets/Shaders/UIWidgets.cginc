#ifndef UIWIDGETS_INCLUDED
#define UIWIDGETS_INCLUDED

// Convert from linear colorspace to gamma.
// linRGB - color in the linear colorspace.
inline float4 LinearToGammaSpace4(float4 linRGB)
{
	linRGB = max(linRGB, float4(0.h, 0.h, 0.h, 0.h));
	// An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

	// Exact version, useful for debugging.
	//return float4(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b), LinearToGammaSpaceExact(linRGB.a));
}

// Convert from gamma colorspace to linear.
// sRGB - color in the gamma colorspace.
inline float4 GammaToLinearSpace4(float4 sRGB)
{
	// Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);

	// Precise version, useful for debugging.
	//return float4(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b), GammaToLinearSpaceExact(sRGB.a));
}

// Convert hue to base rgb info.
// H - H parameter from the HSV color.
inline float4 Hue(float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float4(R,G,B,1));
}

// Get color of the specified point of the HSV circle.
// pos - point, relative to circle center.
// value - V parameter from HSV.
// quality - circle edges quality.
inline float4 CircleHSV(in float2 pos, in float value, in float quality)
{
	float pi2 = 6.28318530718;

	float saturation = sqrt(pos.x * pos.x * 4.0 + pos.y * pos.y * 4.0);
	float alpha = 1.0 - smoothstep(1.0 - quality, 1.0 + quality, dot(pos, pos) * 4.0);

	float hue = atan2(pos.x, pos.y) / pi2;
	if (hue < 0)
	{
		hue += 1;
	}

	return saturate(float4(hue, saturation, value, alpha));
}

// Convert HSV color to RGB.
inline float4 HSVtoRGB(in float4 HSV)
{
	float4 result = ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
	result.a = HSV.a;
	return result;
}

inline float3 HSVtoRGB(in float3 HSV)
{
	return ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
}

inline float3 HSVtoRGB3(in float3 HSV)
{
	return HSVtoRGB(HSV);
}

// Convert RGB color to HSV.
// http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
inline float4 RGBtoHSV(float4 color)
{
	const float4 rate = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(color.bg, rate.wz), float4(color.gb, rate.xy), step(color.b, color.g));
	float4 q = lerp(float4(p.xyw, color.r), float4(color.r, p.yzx), step(p.x, color.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, color.a);
}

inline float3 RGBtoHSV(float3 color)
{
	const float4 rate = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(color.bg, rate.wz), float4(color.gb, rate.xy), step(color.b, color.g));
	float4 q = lerp(float4(p.xyw, color.r), float4(color.r, p.yzx), step(p.x, color.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

inline float3 RGBtoHSV3(float3 color)
{
	return RGBtoHSV(color);
}

// Lerp RGB colors thought HSV.
inline float lerpHue(in float a, in float b, float t)
{
	if (a > b)
	{
		float c = a;
		a = b;
		b = c;
		
		t = 1 - t;
	}

	float delta = b - a;

	return (delta > 0.5)
		? (a + 1 + t * (b - a - 1)) % 1
		: a + t * delta;
}

inline float4 lerpHSV(in float4 colorA, in float4 colorB, float t)
{
	float4 a_hsv = RGBtoHSV(colorA);
	float4 b_hsv = RGBtoHSV(colorB);
	
	return HSVtoRGB(lerp(a_hsv, b_hsv, t));
}

inline float3 lerpHSV(in float3 colorA, in float3 colorB, float t)
{
	float3 a_hsv = RGBtoHSV(colorA);
	float3 b_hsv = RGBtoHSV(colorB);
	
	return HSVtoRGB(lerp(a_hsv, b_hsv, t));
}

inline float4 lerpHSVAlternative(in float4 colorA, in float4 colorB, float t)
{
	float4 a_hsv = RGBtoHSV(colorA);
	float4 b_hsv = RGBtoHSV(colorB);
	
	float h = lerpHue(a_hsv.r, b_hsv.r, t);
	return HSVtoRGB(float4(h, lerp(a_hsv.gba, b_hsv.gba, t)));
}

inline float3 lerpHSVAlternative(in float3 colorA, in float3 colorB, float t)
{
	float3 a_hsv = RGBtoHSV(colorA);
	float3 b_hsv = RGBtoHSV(colorB);
	
	float h = lerpHue(a_hsv.r, b_hsv.r, t);
	return HSVtoRGB(float3(h, lerp(a_hsv.gb, b_hsv.gb, t)));
}

inline float3 lerpHSV3(in float3 colorA, in float3 colorB, float t)
{
	return lerpHSV(colorA, colorB, t);
}

float flare_distance_old(float center, float size, float pos, float delay)
{
    float half_size = size / 2.0;
    float left = center - half_size;
    float right = center + half_size;
    if ((left < 0) && (pos > (left + delay)))
    {
        pos -= delay;
    }
    else if ((right > delay) && (pos < (right - delay)))
    {
        pos += delay;
    }

    return abs(smoothstep(left, right, pos) - 0.5) * 2.0;
}

float flare_distance(float center, float size, float pos, float delay)
{
    float half_size = size / 2.0f;
    float distance = min(min(abs(pos - center), abs(pos + 1.0f - center)), abs(pos - 1.0f - center));
    return clamp(distance / half_size, 0.0f, 1.0f);
}
#endif