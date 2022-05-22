// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Projector/AdditiveTint" {
  Properties {
    _Color ("Tint Color", Color) = (1, 1, 1, 1)
    _ShadowTex ("Cookie", 2D) = "gray" {}
    _Angle ("Angle", Range(0, 360)) = 0
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
      #pragma enable_d3d11_debug_symbols

      #include "UnityCG.cginc"
      
      struct v2f {
        float4 uvShadow : TEXCOORD0;
        float4 pos : SV_POSITION;
      };
      
      float4x4 unity_Projector;
      
      v2f vert (float4 vertex : POSITION)
      {
        v2f o;
        o.pos = UnityObjectToClipPos (vertex);
        o.uvShadow = mul (unity_Projector, vertex);
        return o;
      }
      
      sampler2D _ShadowTex;
      fixed4 _Color;
      float _Angle;
      
      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 texCookie = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
        fixed4 outColor = _Color * texCookie.a;
        float angle = degrees(atan2(i.uvShadow.x - 0.5, i.uvShadow.y - 0.5)) + 180;
        return outColor * (angle < _Angle);
      }
      ENDCG
    }
  }
}
