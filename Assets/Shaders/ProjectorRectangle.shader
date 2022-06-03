Shader "Projector/Rectangle"
{
  Properties {
    _Color ("Tint Color", Color) = (1, 1, 1, 1)
    _Width ("Width", Float) = 1
    _Length ("Length", Float) = 1
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
      float _Width;
      float _Length;
      
      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 outColor = _Color;
        float2 uv = i.uv - float2(0.5, 0.5);
        return outColor * (-_Width / 2 < uv.x) * (uv.x < _Width / 2) * (0 < uv.y) * (uv.y < _Length);
      }
      ENDCG
    }
  }
}
