Shader"Scene/Decal_VolumeCloud"
{
    Properties
    {
        [HDR]_BaseColor("BaseColor",Color)=(1,1,1,1)
        [HDR]_GIColor("GIColor",Color)=(1,1,1,1)
        _CloudTrans("CloudTrans",Range(0,1))=0
        _ShadowPower("_ShadowPower",float)=1
        [NoScaleOffset]_3DNoise("3dNoise",3D) = "white"{}
        
        [HDR]_AddColor("AddColor",Color)=(1,1,1,1)
        _AddPoint("_AddPoint",vector)=(0,0,0,1)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Overlay"
            "Queue"="Transparent"
            "DisableBatch"="True"
        }

        Cull back
        ZTest Less

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include"Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include"Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
        #include"Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
        CBUFFER_START(UnityPerMaterial)
        half4 _Color;
        half4 _BaseColor;
        half4 _GIColor;
        half _CloudTrans;
        half _ShadowPower;
        half3 _AddColor;
        float4 _AddPoint;
        CBUFFER_END

        struct a2v
        {
            float4 positionOS:POSITION;
        };

        struct v2f
        {
            float4 positionCS:SV_POSITION;
            float4 SStexcoord:TEXCOORD;
            float4 cameraPosOS:TEXCOORD1;
            float4 cam2vertexRayOS:TEXCOORD2;
            float3 positionOS   :TEXCOORD3;
        };
        
        float4 GetTheWorldPos(float2 ScreenUV , float Depth)
        {
        	if(_ProjectionParams.x<0)
            {
                Depth =1- Depth;
            }
        	//获取像素的屏幕空间位置
        	float3 ScreenPos = float3(ScreenUV , Depth);
        	float4 normalScreenPos = float4(ScreenPos * 2.0 - 1.0 , 1.0);
        	//得到ndc空间下像素位置
        	float4 ndcPos = mul( unity_CameraInvProjection , normalScreenPos );
        	ndcPos = float4(ndcPos.xyz / ndcPos.w , 1.0);
        	//获取世界空间下像素位置
        	float4 sencePos = mul( unity_CameraToWorld , ndcPos * float4(1,1,-1,1));
        	sencePos = float4(sencePos.xyz , 1.0);
        	return sencePos;
        }
        float Dither8x8Bayer(float2 uv)
        {
            uint2 seed =fmod(uv,8);
            
            const float dither[ 64 ] =
                {
                1/ 64.0, 49/ 64.0, 13/ 64.0, 61/ 64.0,  4/ 64.0, 52/ 64.0, 16/ 64.0, 64/ 64.0,
                33/ 64.0, 17/ 64.0, 45/ 64.0, 29/ 64.0, 36/ 64.0, 20/ 64.0, 48/ 64.0, 32/ 64.0,
                9/ 64.0, 57/ 64.0,  5/ 64.0, 53/ 64.0, 12/ 64.0, 60/ 64.0,  8/ 64.0, 56/ 64.0,
                41/ 64.0, 25/ 64.0, 37/ 64.0, 21/ 64.0, 44/ 64.0, 28/ 64.0, 40/ 64.0, 24/ 64.0,
                3/ 64.0, 51/ 64.0, 15/ 64.0, 63/ 64.0,  2/ 64.0, 50/ 64.0, 14/ 64.0, 62/ 64.0,
                35/ 64.0, 19/ 64.0, 47/ 64.0, 31/ 64.0, 34/ 64.0, 18/ 64.0, 46/ 64.0, 30/ 64.0,
                11/ 64.0, 59/ 64.0,  7/ 64.0, 55/ 64.0, 10/ 64.0, 58/ 64.0,  6/ 64.0, 54/ 64.0,
                43/ 64.0, 27/ 64.0, 39/ 64.0, 23/ 64.0, 42/ 64.0, 26/ 64.0, 38/ 64.0, 22/ 64.0
                };
            int r = seed.y * 8 + seed.x;
            return dither[r];
        }
        
        ENDHLSL

        pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            //Blend SrcAlpha One
            //Blend DstColor Zero
            ZWrite Off
            Tags{"LightMode"="UniversalForward"}

            HLSLPROGRAM
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            
            #pragma vertex VERT
            #pragma fragment FRAG
            #pragma target 3.0
            #pragma multi_compile _ _FOG_ON

            float4 ComputeScreenUV(float4 positionCS)
            {
                float4 ndc;
                ndc.xy=positionCS.xy*0.5+0.5*positionCS.w;
                #ifdef UNITY_UV_STARTS_AT_TOP
                ndc.y=positionCS.w-ndc.y;
                #endif
                ndc.zw=positionCS.zw;
                return ndc;
            }
            TEXTURE3D(_3DNoise);
            SAMPLER(sampler_3DNoise);
            v2f VERT(a2v i)
            {
                v2f o = (v2f)0;
                o.positionCS=TransformObjectToHClip(i.positionOS.xyz);
                o.SStexcoord.xy=o.positionCS.xy*0.5+0.5*o.positionCS.w;
                #ifdef UNITY_UV_STARTS_AT_TOP
                o.SStexcoord.y=o.positionCS.w-o.SStexcoord.y;
                #endif
                o.SStexcoord.zw=o.positionCS.zw;
                float4 posVS =mul(UNITY_MATRIX_V,mul(UNITY_MATRIX_M,i.positionOS));//得到相机空间顶点坐标//ObjectToWorldToView
                o.cam2vertexRayOS.w=-posVS.z;//相机空间下的z是线性深度，取负 
                o.cam2vertexRayOS.xyz=mul(UNITY_MATRIX_I_M,mul(UNITY_MATRIX_I_V,float4(posVS.xyz,0))).xyz;//忽略平移矩阵当成向量处理//ViewTooWorldToObject
                o.cameraPosOS.xyz=mul(UNITY_MATRIX_I_M,mul(UNITY_MATRIX_I_V,float4(0,0,0,1))).xyz;//计算模型空间下的相机坐标
                o.positionOS=i.positionOS.xyz;
                return o;
            }

            float GetCurrentPositionLum(float3 currentPos,float3 lightDir,float dither)
            {
                float marchingLength = 0;
                float totalDensity = 0;
                float l =lerp(0.025,0.5,dither);
                [unroll(4)]for(int march = 0; march <= 4; march++)
                {
                    marchingLength += l;
                    float3 pos = currentPos + lightDir * marchingLength;
                    float density = SAMPLE_TEXTURE3D(_3DNoise,sampler_3DNoise,pos).r;
                    totalDensity  += density * l;
                    if(totalDensity>=1)break;
                }
                return saturate(totalDensity);
            }
            half4 FRAG(v2f i):SV_TARGET
            {
                if(_BaseColor.a<=0)return 0;
                float2 SSUV=i.SStexcoord.xy/i.SStexcoord.w;//在片元里进行透除
                float SSdepth=SampleSceneDepth(SSUV);
                float3 positionWS =GetTheWorldPos(SSUV,SSdepth).xyz;
                SSdepth = LinearEyeDepth(SSdepth,_ZBufferParams);
                i.cam2vertexRayOS.xyz/=i.cam2vertexRayOS.w;//在片元里进行透除
                float3 decalPos=i.cameraPosOS.xyz + i.cam2vertexRayOS.xyz * SSdepth;//模型空间下的计算：相机坐标+相机朝着顶点的射线（已透除）*相机空间的线性深度
                
                float dither =InterleavedGradientNoise(i.positionCS.xy,1);
                dither=frac(dither+_Time.y*0.5);
                
                float3 cameraOS = TransformWorldToObject(_WorldSpaceCameraPos);
                float3 viewDirOS = SafeNormalize(i.positionOS-cameraOS);
                float3 lightDirOS = TransformWorldToObjectDir(GetMainLight().direction);
                float3 startPos = i.positionOS+0.5;
                float maxLength = length(startPos-decalPos);
                float marchingLength = 0;
                float totalDensity = 0;
                float totalShadow=0;
                float addShadow=0;
                float l =lerp(0.025,0.2,dither);
                [unroll(16)]for(int march = 0; march <= 16; march++)
                {
                    marchingLength += l;
                    float3 currentPos = startPos + viewDirOS * marchingLength;
                    if(marchingLength > maxLength)break;
                    half4 Tex3d = SAMPLE_TEXTURE3D(_3DNoise,sampler_3DNoise,currentPos);
                    float density = Tex3d.r* l *5;
                    totalDensity += density;
                    totalDensity=saturate(totalDensity);
                    if(totalShadow<1&&Tex3d.r>0.1)
                    {
                        float curshadow = GetCurrentPositionLum(currentPos,lightDirOS,dither);
                        totalShadow += totalDensity * saturate(1-curshadow);
                    }
                    if(addShadow<1&&Tex3d.r>0.1)
                    {
                        float3 addDir = _AddPoint.xyz-(currentPos-0.5);
                        float addLength =saturate((_AddPoint.w-length(addDir))/_AddPoint.w);
                        float curshadow = GetCurrentPositionLum(currentPos,SafeNormalize(addDir),dither);
                        addShadow += totalDensity *saturate(1-curshadow)*addLength;
                    }
                    if(totalDensity>=1)break;
                }
                half shadow = pow(saturate(totalShadow),_ShadowPower);
                half addshadow = pow(saturate(addShadow),_ShadowPower);
                half3 cloudCol = _BaseColor.rgb*(GetMainLight().color*shadow+_AddColor.rgb*addshadow)+lerp(_GIColor.rgb,GetMainLight().color,_CloudTrans);
                return float4(cloudCol,saturate(totalDensity)*_BaseColor.a);
            }
        ENDHLSL
        }

    }

}