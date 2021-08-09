Shader "Environment/Plant/EcoLeaves" 
{

	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	Category
	{
		SubShader 
		{
			Pass 
			{
				ColorMaterial AmbientAndDiffuse
				Lighting Off
				ZWrite On
				Alphatest Greater .3
				Cull Off
				SeparateSpecular Off

				//Blend SrcAlpha OneMinusSrcAlpha
	
				SetTexture [_MainTex] 
				{
					//constantColor [_Color]
					Combine texture * primary, texture * primary
					//Combine texture * constant DOUBLE, texture * constant
				}
			}
		}
	}
} 