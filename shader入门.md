# 1. 渲染流水线

## 1.1 渲染流水线

渲染分为3个阶段：

(1) **应用阶段：**准备数据，如模型，摄像机位置，光源等；看不见物体剔除；设置渲染状态，包括但不限于材质，纹理，Shader等。

(2) **几何阶段：**通常在GPU上执行，进行逐顶点，逐多边形操作，输出屏幕空间的二维顶点坐标，深度值，着色等信息。

(3) **光栅化阶段：**根据上一阶段的数据渲染最终图像。

## 1.2 CPU和GPU的通信

渲染流水线的起点是CPU，即应用阶段，分为：

(1) 把数据加载到显存：所需的数据都从硬盘加载到内存，网格信息，法线方向，顶点颜色，纹理坐标等加载到显存。

(2) 设置渲染状态：包括顶点着色器，片源着色器，光源属性，材质等。

(3) 调用Draw Call：调用后GPU会把图元渲染到屏幕上。

## 1.3 GPU流水线

流水线阶段：



顶点数据->

**几何阶段：**顶点着色器(可编程)->曲面细分着色器(可编程)->几何着色器(可编程)->剪裁->屏幕映射(可配置)

**光栅化：**三角形设置->三角形遍历->片元着色器->逐片元操作

-> 屏幕图像



### 1.3.1 顶点着色器

**顶点着色器**：输入来自CPU，处理单位是顶点。不能创建或销毁顶点，无法得到顶点间的关系。

主要工作是：坐标变换和逐顶点光照。

**必要**：把顶点坐标从模型空间转换到齐次剪裁空间。



### 1.3.2 裁剪

视野外的图元去除，只有一部分在视野外的会重新设置图元。



### 1.3.3 屏幕映射

将图元x,y坐标转换为屏幕坐标系（二维坐标），z值会保留



### 1.3.4 三角形设置

计算三角形边界。



### 1.3.5 三角形遍历

计算哪些顶点在三角形中，并插值得到深度等信息。



### 1.3.6 片元着色器

片元着色器的输入是顶点信息的插值。该阶段输出像素的颜色值。



### 1.3.7 逐片元操作

OpenGL叫逐片元操作，DirectX叫输出合并阶段。

(1) 决定每个片元的可见性，包括深度测试，模板测试等

(2) 如果通过了所有测试，就把这个片元的颜色值和已经存储在缓冲区的颜色进行合并，或混合





## 1.4 HLSL、GLSL、Cg

这些都是着色语言。

HSLS：DirectX的着色语言

GLSL：OpenGL

Cg：NVIDIA

这些都是高级语言，他们被翻译为IL，然后交给驱动程序执行。

GLSL跨平台，编译结果取决于硬件供应商。

HLSL仅支持微软自己的产品。

Cg真正的跨平台，根据平台不同，编译成相应的中间语言。但无法发挥OpenGL最新特性。



## 1.5 Draw Call

Draw Call将渲染模型放入GPU的命令缓存区。

Draw Call调用有额外开销，应当使用批处理的方式发起Draw Call。

在开发过程中，

(1) 避免使用过多小网格

(2) 避免使用过多材质

## 1.6 固定管线渲染

固定管线在较旧的GPU上使用，流水线只提供一些配置操作。



# 2. UnityShader基础

### 2.1 Unity Shader 概述

**Unity可创建Shader类型：**

(1) Standard Surface Shader：标准光照模型

(2) Image Effect Shader：用于屏幕后处理

(3) Compute Shader：利用GPU并行性做些和流水线无关的操作

(4) Unity Shader：一个不含光照，但包含雾效的Shader

## 2.2 ShaderLab

Unity Shader是Unity为开发者提供的高层级的渲染抽象层。ShaderLab是Unity提供的编写Unity Shader的一种说明性语言。

### 2.2.1 Shader 名字

Custom/MyShader对应材质面板中的Shader选择菜单层级

```shader
Shader "Custom/MyShader" {}
```

### 2.2.2 Properties

Properties定义的属性在材质面板中可填写，并且可在Shader中访问。

```
Properties {
  Name ("display name", PropertyType) = DefaultValue
  Name ("display name", PropertyType) = DefaultValue
}
```

Shader支持的类型：

```
Properties {
  _Int ("Int", Int) = 2
  _Float ("Float", Float) = 1.5
  _Range ("Range", Range(0.0, 5.0)) = 3.0
  _Color ("Color", Color) = (1,1,1,1)
  _Vector ("Vector", Vector) = (2, 3, 6, 1)
  _2D ("2D", 2D) = "" {}		// 2D纹理
  _Cube ("Cube", Cube) = "white" {}
  _3D ("3D", 3D) = "black" {}
}
```

### 2.2.3 SubShader

```
SubShader {
  // 可选的
  [Tags]
  // 可选的
  [RenderSetup]
  Pass {
    [Name]
    [Tags]
    [RenderSetup]
  }
}
```

Pass定义一次完整的渲染流程。

Tags是一个键值对，告诉Unity引擎怎样以及何时渲染这个对象。

```
Queue——控制渲染顺序，指定属于哪个渲染队列——如，"Queue"="Transparent"
RenderType——对着色器进行分类，可以被用于着色器替换功能——如，"RenderType"="Opaque"
```

**Pass**

Name中定义的名称，可以在其它地方直接引用

```
Name "MyPassName"

UsePass "MyShader/MYPASSNAME"	// Pass的名称会自动转换成大写
```

Pass中可以设置渲染状态：

```
LightMode：定义在渲染流水线中的角色
RequireOptions：指定满足某些条件才渲染该Pass
```

**UsePass**：引用其它Pass

**GrabPass**：抓取屏幕并将结果存到一张纹理中



### 2.2.4 Fallback

如果所有的SubShader在该显卡上都不能运行，使用的Shader

例如：

```
Fallback "VertexLit"
```

## 2.3 Unity Shader的形式

Shader代码写在：

```
CGPROGRAM
#pragma ...

ENCG
```

这之间的代码采用Cg/HLSL编写

### 2.3.1 表面着色器

Unity自创的代码类型，代码量少，但渲染代价大，它会被自动转换成顶点/片元着色器。Unity为我们处理了更多光照细节。

### 2.3.2 顶点片元着色器

顶点片元着色器在Pass语义块里，需要自己定义每个Pass需要的代码，灵活性更高。

### 2.3.3 固定函数着色器

自能做一些配置，使用固定管线。

# 3. 数学基础

## 3.1 笛卡尔系

**左手右手坐标**：拇指x轴，食指y轴，中指z轴

模型空间和世界空间使用左手系。

观察空间使用右手系。

## 3.2 变换

我们可以用$4\times4$表示平移，旋转和缩放。
$$
\begin {vmatrix}
M_{3\times3} & t_{3\times1} \\
0_{1\times3} & 1
\end{vmatrix}
\begin {vmatrix}
x \\
y \\
z \\
1
\end{vmatrix}
$$
$M_{3\times3}$表示旋转和缩放，$t_{3\times1}$表示平移。得到的矩阵为变换后的矩阵。

**平移矩阵：**
$$
\begin{vmatrix}
1 & 0 & 0 & t_x \\
0 & 1 & 0 & t_y \\
0 & 0 & 1 & t_z \\
0 & 0 & 0 & 1
\end{vmatrix}
$$
**缩放矩阵：**
$$
\begin{vmatrix}
k_x  & 0 & 0 & 0 \\
0 & k_y & 0 & 0 \\
0 & 0 & k_z & 0 \\
0 & 0 & 0 & 1
\end{vmatrix}
$$
**旋转矩阵：**

x轴旋转：
$$
\begin{vmatrix}
1 & 0 & 0 & 0 \\
0 & cos(\theta) & -sin(\theta) & 0 \\
0 & sin(\theta) & cose(\theta) & 0 \\
0 & 0 & 0 & 1
\end{vmatrix}
$$
y轴旋转：
$$
\begin{vmatrix}
cos(\theta) & 0 & sin(\theta) & 0 \\
0 & 1 & 0 & 0 \\
-sin(\theta) & 0 & cos(\theta) & 0 \\
0 & 0 & 0 & 1
\end{vmatrix}
$$
z轴旋转：
$$
\begin{vmatrix}
cos(\theta) & -sin(\theta) & 0 & 0 \\
sin(\theta) & cos(\theta) & 0 & 0 \\
0 & 0 & 1 & 0 \\
0 & 0 & 0 & 1
\end{vmatrix}
$$

## 3.3 坐标空间

假设P中有子坐标空间C，C的原点为$O_c$，C的坐标轴矢量分别为：$x_c,y_c,z_c$

则C中的坐标，可以左乘矩阵，来获得P中的坐标。
$$
\begin{vmatrix}
| & | & | & | \\
x_c & y_c & z_c & O_c \\
| & | & | & | \\
0 & 0 & 0 & 1
\end{vmatrix}
$$
(1) **模型空间**

依据对象的坐标空间，又叫对象空间或局部空间

(2) **世界空间**

一个特殊坐标系，Unity中最大的坐标空间，不同模型的世界空间相同。

(3) **观察空间**

又称摄像机空间，观察空间为右手坐标系，+x向右，+y向上，+z为摄像机后方。

(4) **剪裁空间**

分透视投影和正交投影。表示一个视锥体区域，正交投影剪裁空间为矩形。

(5) **屏幕空间**

屏幕空间将剪裁空间投入2维空间

## 3.4 法线变换

法线变换使用：$M^{-1}$，即M逆矩阵



## 3.5 内置变量

### 3.5.1 变换矩阵

UNITY_MATRIX_MVP：模型空间->剪裁空间

UNITY_MATRIX_MV：模型空间->观察空间

_Object2World：模型->世界

_World2Object：世界->模型

### 3.5.2 摄像机和屏幕参数

_WorldSpaceCameraPos：摄像机在世界空间中的位置

## 3.6 Cg中的矢量和矩阵类型

mul：矩阵相乘

dot：矢量点积

# 4. UnityShader基础

## 4.1 顶点片元着色器基本结构

```
Shader "MyShaderName" {
  Properties {
    // 属性
  }
  SubShader {
    // 针对显卡A的SubShader
    Pass {
      // 设置渲染状态和标签
      
      // 开始Cg代码片段
      CGPROGRAM
      // 编译指令
      #pragma vertex vert
      #pragma fragment frag
      
      ENDCG
      // 其它设置
    }
    // 其它Pass
  }
  SubShader {
    // 显卡B
  }
  
  // 上述Shader全失效时调用
  Fallback "VertexLit"
}
```



```
#pragma vertex vert	// 告诉哪个函数包含了顶点着色器代码
#pragma fragment frag	// 片元着色器代码
```

顶点着色器示例：

```
float4 vert(float4 v : POSITION) : SV_POSITION {
  return mul(UNITY_MATRIX_MVP, v);
}
```

片元着色器：

```
fixed4 frag() : SV_TARGET {
  return fixed4(1.0,1.0,1.0,1.0);
}
```

## 4.2 ShaderLab类型与Cg类型对应

Color,Vector -> float4, half4, fixed4

Range, Float -> float, half, fixed

2D -> sampler2D

Cude -> samplerCube

3D -> sampler3D



## 4.3 Unity提供的内置文件和变量

可以使用#inclue 引入其它文件，自动包含一些变量和帮助函数

```
CGPROGRAM
// ..
#include "UnityCG.cginc"
//..
ENDCG
```

位于Data/CGIncludes

UnityCG.cginc中常用函数：

float3 WorldSpaceViewDir(float4 v)：模型空间顶点位置->世界空间从该点到摄像机的观察方向

ObjSpaceViewDir：模型空间顶点位置->模型空间从改点到摄像机的观察方向

## 4.4 Unity支持的语义

**应用阶段->顶点着色阶段：**

POSITION->模型空间顶点位置

NORMAL->顶点法线

TANGENT->顶点切线

TEXCOORDn -> 第n组纹理坐标

COLOR -> 顶点颜色



**顶点着色器->片元着色器：**

SV_POSITION->剪裁空间顶点坐标

COLOR0 -> 第1组顶点颜色

COLOR1 -> 第2组顶点颜色

TEXCOORD0 ~ TEXCOORD7 -> 纹理坐标



**片元着色器输出：**

SV_Target：输出值存储到渲染目标



# 5. 基础光照

## 5.1 标准光照模型

**自发光**：给定一个方向，一个表面会向该方向发射多少辐射量，用$c_{emissive}$表示。没有全局光照的清空下，不会照亮周围的物体。

**高光反射**：使用$c_{sepcular}$表示，表示光线会完全镜面反射多少辐射量。

**漫反射**：使用$c_{diffuse}$表示，表示表面会向每个方向散射多少辐射量。

**环境光**：$c_{ambient}$藐视其它所有间接光照。

### 5.1.1 环境光

通常是一个环境变量。

### 5.1.2 自发光

### 5.1.3 漫反射

漫反射满足兰伯特定律：
$$
c_{diffuse} = (c_{light}\cdot m_{diffuse})max(0, \hat n\cdot \hat l)
$$
$\hat n$是表面法线，$\hat l$是指向光源的单位矢量。

### 5.1.4 高光

高光反射：

反射方向：$\hat r = 2 (\hat n \cdot \hat l) - \hat l$

Phong模型计算高光反射：$c_{specular} = (c_{light} \cdot m_{specular})max(0, \hat v \cdot \hat r)^{m_{gloss}}$

$m_{gloss}$称为反光度，$\hat v$是指向摄像机的矢量。

Blinn模型：$\hat h = \frac{\hat v + \hat l}{|\hat v + \hat l |}$，$c_{specular}=(c_light \cdot m_{specular})max(0, \hat n \cdot \hat h)^{m_{gloss}}$

Blinn模型在物体离光源和摄像机都很远的情况下计算较快，此时$\hat h$是定值。反之，Phong模型更快。

### 5.1.5 逐像素，逐顶点

逐像素：在片元着色器中处理，速度慢，根据每个顶点对法线进行插值。

逐顶点：在顶点着色器中处理，速度快，根据每个顶点的颜色进行线性插值。如果是非线性变换，可能会出现色块。

## 5.2 Unity Shader实现标准光线模型

`saturate(x)`：将x标量或矢量截取到[0,1]的范围。

### 5.2.1 漫反射

属性声明：

```
_Diffuse ("Diffuse", Color) = (1,1,1,1)
```

定义标签：

```
Tags {"LightMode"="ForwardBase"}
```

引入内置变量：

```
#include "Lighting.cginc"
```

定义着色器输入输出结构体：

```
struct a2v {
  float4 vertex : POSITION;
  float3 normal : NORMAL;
};
struct v2f {
  float4 pos : SV_POSITION;
  fixed3 color : COLOR;
}
```

**顶点着色**：

```
v2f vert(a2v v){
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;												// 环境光
	fixed3 worldNormal = normalize(mul(v.normal , (float3x3)_World2Object));					// 法线的世界坐标
	fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);									// 光线的世界坐标
	fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));	// 计算漫反射
	o.color = ambient + diffuse;
	return o;
}

fixed4 frag(v2f i) : SV_TARGET {
	return fixed4 (i.color, 1);
}
```

**片元着色：**

```
struct v2f {
	float4 pos : SV_POSITION;
	float3 worldNormal : TEXCOORD0;
};

v2f vert(a2v v) {
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.worldNormal = mul(v.normal, (float3x3)_World2Object);
	return o;
}

fixed4 frag(v2f i) : SV_TARGET {
	fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
	fixed3 worldNormal = normalize(i.worldNormal);
	fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
	fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));
	fixed3 color = ambient + diffuse;
	return fixed4(color, 1);
}
```

### 5.2.2 高光反射

```
_Specular ("Specular", Color) = (1,1,1,1)		// 设置高光颜色
_Gloss("Gloss",Range(8,25)) = 8					// 设置高光度
```

在v2f中添加worldPos

```
o.worldPos = mul((float3x3)_Object2World, v.vertex).xyz;		// vert()
```

计算高光：

```
fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
fixed3 viewDir = normalize(_WorldSpaceCameraPOs.xyz-i.worldPos.xyz);
fixed3 specular = _LightColor0.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);
```

# 6. 基础纹理

