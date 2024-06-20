using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedSingleStreamProceduralMesh : MonoBehaviour
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
            VertexAttribute.Normal, dimension: 3 
        );
        // Tangents
        vertexAttributes[2] = new VertexAttributeDescriptor(
            VertexAttribute.Tangent,
            VertexAttributeFormat.Float16, // For now, reduce the precision to half
            dimension: 4
        );
        // UVs
        vertexAttributes[3] = new VertexAttributeDescriptor(
            VertexAttribute.TexCoord0, 
            VertexAttributeFormat.Float16, // For now, reduce the precision to half
            dimension: 2
        );
        meshData.SetVertexBufferParams(vertexCount, vertexAttributes);

        vertexAttributes.Dispose(); // We have to manually dispose. Using a using statement, throws an error when we attempt to set the attributes descrioptor,
        
        NativeArray<Vertex> vertices = meshData.GetVertexData<Vertex>();
        
        half h0 = math.half(0f), h1 = math.half(1f);

        var vertex = new Vertex {
            Normal = math.back(),
            Tangent = math.half4(h1, h0, h0, math.half(-1f))
        };

        vertex.Position = 0f;
        vertex.TEXCoord0 = h0;
        vertices[0] = vertex;

        vertex.Position = math.right();
        vertex.TEXCoord0 = math.half2(h1, h0);
        vertices[1] = vertex;

        vertex.Position = math.up();
        vertex.TEXCoord0 = math.half2(h0, h1);
        vertices[2] = vertex;

        vertex.Position = math.float3(1f, 1f, 0f);
        vertex.TEXCoord0 = h1;
        vertices[3] = vertex;
        
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
