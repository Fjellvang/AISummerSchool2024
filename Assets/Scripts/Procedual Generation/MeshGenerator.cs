using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    void OnEnable () {
        
        var vertexAttributeCount = 4;
        int vertexCount = 4;
        int triangleIndexCount = 6;
        
        var meshDataArray = Mesh.AllocateWritableMeshData(1);
        var meshData = meshDataArray[0];
        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
            vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );
        // Positions
        vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
        // Normals
        vertexAttributes[1] = new VertexAttributeDescriptor(
            VertexAttribute.Normal, dimension: 3, stream: 1
        );
        // Tangents
        vertexAttributes[2] = new VertexAttributeDescriptor(
            VertexAttribute.Tangent,
            VertexAttributeFormat.Float16, // For now, reduce the precision to half
            dimension: 4, stream: 2
        );
        // UVs
        vertexAttributes[3] = new VertexAttributeDescriptor(
            VertexAttribute.TexCoord0, 
            VertexAttributeFormat.Float16, // For now, reduce the precision to half
            dimension: 2, stream: 3
        );
        meshData.SetVertexBufferParams(vertexCount, vertexAttributes);

        vertexAttributes.Dispose(); // We have to manually dispose. Using a using statement, throws an error when we attempt to set the attributes descrioptor,
        
        // TODO: this should be unhardcoded
        NativeArray<float3> positions = meshData.GetVertexData<float3>();
        positions[0] = 0f;
        positions[1] = math.right();
        positions[2] = math.up();
        positions[3] = math.float3(1f, 1f, 0f);
        
        NativeArray<float3> normals = meshData.GetVertexData<float3>(1);
        normals[0] = normals[1] = normals[2] = normals[3] = math.back();

        half h0 = math.half(0f), h1 = math.half(1f); // Since we reduced the size of the tangent, we have to use half precision
        
        NativeArray<half4> tangents = meshData.GetVertexData<half4>(2);
        tangents[0] = tangents[1] = tangents[2] = tangents[3] =
            math.half4(h1, h0, h0, math.half(-1f));
        // NativeArray<float4> tangents = meshData.GetVertexData<float4>(2);
        // tangents[0] = tangents[1] = tangents[2] = tangents[3] = math.float4(1f, 0f, 0f, -1f);

        
        NativeArray<half2> texCoords = meshData.GetVertexData<half2>(3);
        texCoords[0] = h0;
        texCoords[1] = math.half2(h1, h0);
        texCoords[2] = math.half2(h0, h1);
        texCoords[3] = h1;
        // NativeArray<float2> texCoords = meshData.GetVertexData<float2>(3);
        // texCoords[0] = 0f;
        // texCoords[1] = math.float2(1f, 0f);
        // texCoords[2] = math.float2(0f, 1f);
        // texCoords[3] = 1f;
        
        // TODO: We could reduce to UInt16, but since we want to generate the mesh almost indefinetly, lets continue with UInt32 for now
        meshData.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt32);
        NativeArray<uint> triangleIndices = meshData.GetIndexData<uint>();
        triangleIndices[0] = 0;
        triangleIndices[1] = 2;
        triangleIndices[2] = 1;
        triangleIndices[3] = 1;
        triangleIndices[4] = 2;
        triangleIndices[5] = 3;
        
        meshData.subMeshCount = 1;
        var bounds = new Bounds(new Vector3(0.5f, 0.5f), new Vector3(1f, 1f));
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
        {
            bounds = bounds,
            vertexCount = vertexCount
        }, MeshUpdateFlags.DontRecalculateBounds); // We are setting the bounds manually, so we don't want Unity to recalculate them
        
        var mesh = new Mesh {
            bounds = bounds,
            name = "Procedural Mesh"
        };
        
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

        GetComponent<MeshFilter>().mesh = mesh;
    }
    
    void Update()
    {
        
    }
}
