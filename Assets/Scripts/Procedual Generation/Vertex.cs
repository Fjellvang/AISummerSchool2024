using System.Runtime.InteropServices;
using Unity.Mathematics;

[StructLayout(LayoutKind.Sequential)] // This is required to ensure the struct is layed out in memory as we expect, as its copied to the GPU directly
public struct Vertex {
    public float3 Position, Normal;
    public half4 Tangent;
    public half2 TEXCoord0;
}