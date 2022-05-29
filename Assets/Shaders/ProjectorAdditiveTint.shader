// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/AdditiveTint" {
  Properties {
    _Color ("Tint Color", Color) = (1, 1, 1, 1)
    _Angle ("Angle", Range(0, 360)) = 0
    _InnerRadius("Inner Radius", Range(0, 0.5)) = 0
  }
  Subshader {
    Tags {"Queue"="Transparent"}
    Pass {
      ZWrite Off
      ColorMask RGB
      Blend SrcAlpha OneMinusSrcAlpha
      Offset -1, -1

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"
      
      struct v2f {
        float2 uv: TEXCOORD0;
        float4 pos : SV_POSITION;
      };
      
      float4x4 unity_Projector;
      
      v2f vert (float4 vertex : POSITION)
      {
        v2f o;
        o.pos = UnityObjectToClipPos (vertex);
        o.uv = mul (unity_Projector, vertex);
        return o;
      }
      
      fixed4 _Color;
      float _Angle;
      float _InnerRadius;
      
      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 outColor = _Color;
        float2 uv = i.uv - float2(0.5, 0.5);
        float angle = degrees(atan2(uv.x, uv.y)) + 180;
        float magnitude = length(uv);
        return outColor * (angle < _Angle) * (_InnerRadius < magnitude) * (magnitude < 0.5);
      }
      ENDCG
    }
  }
}
