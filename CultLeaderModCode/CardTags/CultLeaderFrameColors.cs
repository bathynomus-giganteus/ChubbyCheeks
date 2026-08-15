using Godot;
using STS2RitsuLib.Utils;

namespace CultLeaderMod.CultLeaderModCode.CardTags;

public static class CultLeaderFrameColors
{
    private static ShaderMaterial? _rainbowMaterial;
    private static ShaderMaterial? _loopFrameMaterial;

    public static Material Loop
    {
        get
        {
            if (_loopFrameMaterial != null && GodotObject.IsInstanceValid(_loopFrameMaterial))
                return _loopFrameMaterial;

            var shader = new Shader
            {
                Code = @"shader_type canvas_item;

uniform float alpha : hint_range(0.0, 1.0) = 0.55;

varying vec4 modulate_color;

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 loop_color(float t) {
    t = clamp(t, 0.0, 1.0);
    const float stops[3] = float[3](0.0, 0.55, 1.0);
    const float hues[3]  = float[3](0.33, 0.66, 0.78);
    int i = 0;
    for (int j = 0; j < 2; j++) { if (t >= stops[j]) i = j; }
    float lt = (t - stops[i]) / (stops[i+1] - stops[i]);
    float hue = mix(hues[i], hues[i+1], lt);
    return hsv2rgb(vec3(hue, 1.0, 1.0));
}

void vertex() {
    modulate_color = COLOR;
}

void fragment() {
    vec4 col = texture(TEXTURE, UV);
    vec3 loop = loop_color(UV.x);
    float lum = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    vec3 tinted = mix(vec3(lum), loop, alpha);
    COLOR = vec4(tinted, col.a) * modulate_color;
}"
            };

            _loopFrameMaterial = new ShaderMaterial { Shader = shader };
            _loopFrameMaterial.SetShaderParameter("alpha", Variant.From(0.55f));
            return _loopFrameMaterial;
        }
    }

    public static Material Rainbow
    {
        get
        {
            if (_rainbowMaterial != null && GodotObject.IsInstanceValid(_rainbowMaterial))
                return _rainbowMaterial;

            var shader = new Shader
            {
                Code = @"shader_type canvas_item;

uniform float alpha : hint_range(0.0, 1.0) = 0.55;

varying vec4 modulate_color;

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 rainbow_color(float t) {
    t = clamp(t, 0.0, 1.0);
    const float stops[8] = float[8](0.0, 0.14, 0.28, 0.42, 0.57, 0.71, 0.85, 1.0);
    const float hues[8]  = float[8](0.0, 0.08, 0.16, 0.33, 0.52, 0.66, 0.78, 1.0);
    int i = 0;
    for (int j = 0; j < 7; j++) { if (t >= stops[j]) i = j; }
    float lt = (t - stops[i]) / (stops[i+1] - stops[i]);
    float hue = mix(hues[i], hues[i+1], lt);
    return hsv2rgb(vec3(hue, 1.0, 1.0));
}

void vertex() {
    modulate_color = COLOR;
}

void fragment() {
    vec4 col = texture(TEXTURE, UV);
    vec3 rainbow = rainbow_color(UV.x);
    float lum = dot(col.rgb, vec3(0.299, 0.587, 0.114));
    vec3 tinted = mix(vec3(lum), rainbow, alpha);
    COLOR = vec4(tinted, col.a) * modulate_color;
}"
            };

            _rainbowMaterial = new ShaderMaterial { Shader = shader };
            _rainbowMaterial.SetShaderParameter("alpha", Variant.From(0.55f));
            return _rainbowMaterial;
        }
    }

    // These five lines are the only place that should be tuned for personality frame color.
    // Keep the NCard/%Frame patch stable; do not replace it with overlay UI.
    public static readonly Material Pure = MaterialUtils.CreateReplaceHueShaderMaterial(0.65f, 1.12f, 0.55f);
    public static readonly Material Calm = MaterialUtils.CreateReplaceHueShaderMaterial(0.37f, 1.13f, 1.29f);
    public static readonly Material Frenzy = MaterialUtils.CreateReplaceHueShaderMaterial(1.25f, 0.55f, 0.55f);
    public static readonly Material Lively = MaterialUtils.CreateReplaceHueShaderMaterial(1.27f, 1.12f, 0.27f);
    public static readonly Material Melancholy = MaterialUtils.CreateReplaceHueShaderMaterial(0.77f, 0.58f, 1.26f);
}
