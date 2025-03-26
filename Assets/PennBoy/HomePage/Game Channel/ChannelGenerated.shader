Shader "Custom/ChannelGenerated"
{
    Properties
    {
        _Stencil("Stencil ID", Float) = 0
        _StencilComp("StencilComp", Float) = 8
        _StencilOp("StencilOp", Float) = 0
        _StencilReadMask("StencilReadMask", Float) = 255
        _StencilWriteMask("StencilWriteMask", Float) = 255
        _ColorMask("ColorMask", Float) = 15

        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        _Speed("Speed", Float) = 0.5
        _Bar_Number("Bar Number", Float) = 2
        _Bar_Size("Bar Size", Range(0, 1)) = 0.5
        _Bar_Strength("Bar Strength", Range(0, 1)) = 0.1
        _Flicker_Strength("Flicker Strength", Range(0, 1)) = 0.2
        _Offset("Offset", Float) = 0
        [ToggleUI]_Currently_Hovered("Currently Hovered", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }

            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float4 texCoord0;
                float4 color;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float4 color : INTERP1;
                float3 positionWS : INTERP2;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.color.xyzw = input.color;
                output.positionWS.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                output.color = input.color.xyzw;
                output.positionWS = input.positionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Speed;
                float _Currently_Hovered;
                float _Bar_Number;
                float _Bar_Size;
                float _Bar_Strength;
                float _Flicker_Strength;
                float _Offset;
            CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
            #endif

            // Graph Functions

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            float Unity_SimpleNoise_ValueNoise_Deterministic_float(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0;
                Hash_Tchou_2_1_float(c0, r0);
                float r1;
                Hash_Tchou_2_1_float(c1, r1);
                float r2;
                Hash_Tchou_2_1_float(c2, r2);
                float r3;
                Hash_Tchou_2_1_float(c3, r3);
                float bottomOfGrid = lerp(r0, r1, f.x);
                float topOfGrid = lerp(r2, r3, f.x);
                float t = lerp(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
            {
                float freq, amp;
                Out = 0.0f;
                freq = pow(2.0, float(0));
                amp = pow(0.5, float(3 - 0));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Fraction_float(float In, out float Out)
            {
                Out = frac(In);
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
            {
                Out = Predicate ? True : False;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _Property_457c26f07f6d46d2a7dd61cd0883b3e5_Out_0_Boolean = _Currently_Hovered;
                UnityTexture2D _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D =
                    UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.tex,
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.samplerstate,
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_R_4_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.r;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_G_5_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.g;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_B_6_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.b;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_A_7_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.a;
                float _Property_1e290f8ec7c344e197a1c721aa854d11_Out_0_Float = _Offset;
                float _Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float;
                Unity_Add_float(IN.TimeParameters.x, _Property_1e290f8ec7c344e197a1c721aa854d11_Out_0_Float,
                                _Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float);
                float _SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float;
                Unity_SimpleNoise_Deterministic_float((_Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float.xx), 500,
                                                          _SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float);
                float _Property_d2698a172c1a4090b54c5d7367fad38e_Out_0_Float = _Flicker_Strength;
                float _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float;
                Unity_OneMinus_float(_Property_d2698a172c1a4090b54c5d7367fad38e_Out_0_Float,
                                         _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float);
                float2 _Vector2_b872038581174b449d8e89ddb9302c12_Out_0_Vector2 = float2(
                    _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float, 1);
                float _Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float;
                Unity_Remap_float(_SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float, float2(0, 1),
                                                           _Vector2_b872038581174b449d8e89ddb9302c12_Out_0_Vector2,
                                                           _Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float);
                float _Property_d59fcee106f143f09a4257d3aed5b660_Out_0_Float = _Bar_Size;
                float4 _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4 = IN.uv0;
                float _Split_300b216908104890bcc30cfc31ae26ac_R_1_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[0];
                float _Split_300b216908104890bcc30cfc31ae26ac_G_2_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[1];
                float _Split_300b216908104890bcc30cfc31ae26ac_B_3_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[2];
                float _Split_300b216908104890bcc30cfc31ae26ac_A_4_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[3];
                float _Property_156934309a954da5b4e7738a9f22fc66_Out_0_Float = _Bar_Number;
                float _Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float;
                Unity_Multiply_float_float(_Split_300b216908104890bcc30cfc31ae26ac_G_2_Float,
                                                    _Property_156934309a954da5b4e7738a9f22fc66_Out_0_Float,
                                                    _Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float);
                float _Property_daa34ccc3d7141d6bf58f2f9d29ecd17_Out_0_Float = _Speed;
                float _Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float;
                Unity_Multiply_float_float(_Property_daa34ccc3d7141d6bf58f2f9d29ecd17_Out_0_Float, IN.TimeParameters.x,
             _Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float);
                float _Property_424b57d1e84747248c5ce16575a9c7f7_Out_0_Float = _Offset;
                float _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float;
                Unity_Add_float(_Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float,
                    _Property_424b57d1e84747248c5ce16575a9c7f7_Out_0_Float,
                    _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float);
                float _Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float;
                Unity_Add_float(_Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float,
                    _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float,
                    _Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float);
                float _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float;
                Unity_Fraction_float(_Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float,
                                                                   _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float);
                float _Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float;
                Unity_Step_float(_Property_d59fcee106f143f09a4257d3aed5b660_Out_0_Float,
                                                                  _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float,
                                                                  _Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float);
                float _Property_20befc80fbd4434e89f5a87806f4ed7f_Out_0_Float = _Bar_Strength;
                float _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float;
                Unity_OneMinus_float(_Property_20befc80fbd4434e89f5a87806f4ed7f_Out_0_Float,
                                               _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float);
                float _Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float;
                Unity_Add_float(_Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float,
                                              _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float,
                                              _Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float);
                float _Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float;
                Unity_Saturate_float(_Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float,
                    _Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float);
                float4 _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4;
                Unity_Multiply_float4_float4(_SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4,
                                          (_Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float.
                                              xxxx),
                                          _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4);
                float4 _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4;
                Unity_Multiply_float4_float4((_Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float.xxxx),
                                    _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4,
                                    _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4);
                float4 _Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4;
                Unity_Branch_float4(_Property_457c26f07f6d46d2a7dd61cd0883b3e5_Out_0_Boolean,
                                                                      _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4,
                                                                      _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4,
                                                                      _Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4);
                surface.BaseColor = (_Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4.xyz);
                surface.Alpha = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }

            // Render State
            Cull Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENESELECTIONPASS 1


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Speed;
                float _Currently_Hovered;
                float _Bar_Number;
                float _Bar_Size;
                float _Bar_Strength;
                float _Flicker_Strength;
                float _Offset;
            CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                surface.Alpha = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }

            // Render State
            Cull Back

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
            #define SCENEPICKINGPASS 1


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Speed;
                float _Currently_Hovered;
                float _Bar_Number;
                float _Bar_Size;
                float _Bar_Strength;
                float _Flicker_Strength;
                float _Offset;
            CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
            #endif

            // Graph Functions
            // GraphFunctions: <None>

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                surface.Alpha = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 color : COLOR;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float4 texCoord0;
                float4 color;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0;
                float3 TimeParameters;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float4 color : INTERP1;
                float3 positionWS : INTERP2;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                ZERO_INITIALIZE(PackedVaryings, output);
                output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.color.xyzw = input.color;
                output.positionWS.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }

            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                output.color = input.color.xyzw;
                output.positionWS = input.positionWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Speed;
                float _Currently_Hovered;
                float _Bar_Number;
                float _Bar_Size;
                float _Bar_Strength;
                float _Flicker_Strength;
                float _Offset;
            CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
            #endif

            // Graph Functions

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            float Unity_SimpleNoise_ValueNoise_Deterministic_float(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0;
                Hash_Tchou_2_1_float(c0, r0);
                float r1;
                Hash_Tchou_2_1_float(c1, r1);
                float r2;
                Hash_Tchou_2_1_float(c2, r2);
                float r3;
                Hash_Tchou_2_1_float(c3, r3);
                float bottomOfGrid = lerp(r0, r1, f.x);
                float topOfGrid = lerp(r2, r3, f.x);
                float t = lerp(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
            {
                float freq, amp;
                Out = 0.0f;
                freq = pow(2.0, float(0));
                amp = pow(0.5, float(3 - 0));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3 - 1));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3 - 2));
                Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy * (Scale / freq))) * amp;
            }

            void Unity_OneMinus_float(float In, out float Out)
            {
                Out = 1 - In;
            }

            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Fraction_float(float In, out float Out)
            {
                Out = frac(In);
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
            {
                Out = Predicate ? True : False;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _Property_457c26f07f6d46d2a7dd61cd0883b3e5_Out_0_Boolean = _Currently_Hovered;
                UnityTexture2D _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D =
                    UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.tex,
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.samplerstate,
                    _Property_77ac6a57caad4367954ed21e2ae89d14_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_R_4_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.r;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_G_5_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.g;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_B_6_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.b;
                float _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_A_7_Float =
                    _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4.a;
                float _Property_1e290f8ec7c344e197a1c721aa854d11_Out_0_Float = _Offset;
                float _Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float;
                Unity_Add_float(IN.TimeParameters.x, _Property_1e290f8ec7c344e197a1c721aa854d11_Out_0_Float,
                                _Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float);
                float _SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float;
                Unity_SimpleNoise_Deterministic_float((_Add_f78c39c50d944c77bb319c17a8e615be_Out_2_Float.xx), 500,
                                                          _SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float);
                float _Property_d2698a172c1a4090b54c5d7367fad38e_Out_0_Float = _Flicker_Strength;
                float _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float;
                Unity_OneMinus_float(_Property_d2698a172c1a4090b54c5d7367fad38e_Out_0_Float,
                                         _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float);
                float2 _Vector2_b872038581174b449d8e89ddb9302c12_Out_0_Vector2 = float2(
                    _OneMinus_e6fe1fcfda054598beff5203d4b41aa1_Out_1_Float, 1);
                float _Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float;
                Unity_Remap_float(_SimpleNoise_7fb8c7041bee4cf791ddbbb8d5fa3ea7_Out_2_Float, float2(0, 1),
                                                           _Vector2_b872038581174b449d8e89ddb9302c12_Out_0_Vector2,
                                                           _Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float);
                float _Property_d59fcee106f143f09a4257d3aed5b660_Out_0_Float = _Bar_Size;
                float4 _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4 = IN.uv0;
                float _Split_300b216908104890bcc30cfc31ae26ac_R_1_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[0];
                float _Split_300b216908104890bcc30cfc31ae26ac_G_2_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[1];
                float _Split_300b216908104890bcc30cfc31ae26ac_B_3_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[2];
                float _Split_300b216908104890bcc30cfc31ae26ac_A_4_Float =
                    _UV_73acc499eaa84faeaedf53c74de7c054_Out_0_Vector4[3];
                float _Property_156934309a954da5b4e7738a9f22fc66_Out_0_Float = _Bar_Number;
                float _Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float;
                Unity_Multiply_float_float(_Split_300b216908104890bcc30cfc31ae26ac_G_2_Float,
                                                    _Property_156934309a954da5b4e7738a9f22fc66_Out_0_Float,
                                                    _Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float);
                float _Property_daa34ccc3d7141d6bf58f2f9d29ecd17_Out_0_Float = _Speed;
                float _Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float;
                Unity_Multiply_float_float(_Property_daa34ccc3d7141d6bf58f2f9d29ecd17_Out_0_Float, IN.TimeParameters.x,
             _Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float);
                float _Property_424b57d1e84747248c5ce16575a9c7f7_Out_0_Float = _Offset;
                float _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float;
                Unity_Add_float(_Multiply_f958b44c9cf74dd887bb8ed594006f7a_Out_2_Float,
                    _Property_424b57d1e84747248c5ce16575a9c7f7_Out_0_Float,
                    _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float);
                float _Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float;
                Unity_Add_float(_Multiply_fee12fab2cdb49ecbab849d808772368_Out_2_Float,
                    _Add_30f07a6a9b004ea698364863cff6a1d2_Out_2_Float,
                    _Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float);
                float _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float;
                Unity_Fraction_float(_Add_90da9a600a9f47b7a5a4ce0cb863dd65_Out_2_Float,
                                                                   _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float);
                float _Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float;
                Unity_Step_float(_Property_d59fcee106f143f09a4257d3aed5b660_Out_0_Float,
                                                                  _Fraction_1298b297c6844f1fac43f68aef53758e_Out_1_Float,
                                                                  _Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float);
                float _Property_20befc80fbd4434e89f5a87806f4ed7f_Out_0_Float = _Bar_Strength;
                float _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float;
                Unity_OneMinus_float(_Property_20befc80fbd4434e89f5a87806f4ed7f_Out_0_Float,
                                               _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float);
                float _Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float;
                Unity_Add_float(_Step_daf760da95e64981a1c36c28f7d8059a_Out_2_Float,
                                              _OneMinus_2df75aaad109409586daedb94a2032de_Out_1_Float,
                                              _Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float);
                float _Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float;
                Unity_Saturate_float(_Add_9534df68bc304eeeafbd95185f926c27_Out_2_Float,
                    _Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float);
                float4 _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4;
                Unity_Multiply_float4_float4(_SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4,
                                          (_Saturate_7b4ffcc335ad47189aa9d8a2cc883c21_Out_1_Float.
                                              xxxx),
                                          _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4);
                float4 _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4;
                Unity_Multiply_float4_float4((_Remap_853d0444460c4a9ab6c3353295e645c6_Out_3_Float.xxxx),
                                    _Multiply_1873aedd635e4a07abc6e16c9f6ce7bd_Out_2_Vector4,
                                    _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4);
                float4 _Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4;
                Unity_Branch_float4(_Property_457c26f07f6d46d2a7dd61cd0883b3e5_Out_0_Boolean,
                                                                      _SampleTexture2D_bc7223c58e0f40efbb3dcd157be110d8_RGBA_0_Vector4,
                                                                      _Multiply_bab1a852e9a74c78bb7f0c887f97fa2a_Out_2_Vector4,
                                                                      _Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4);
                surface.BaseColor = (_Branch_daf499d848f848e0adab62c070d193aa_Out_3_Vector4.xyz);
                surface.Alpha = 1;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}