using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public bool UseWield;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Renderer _renderer;
    public GameObject target;
    public enum MergeType
    {
        BASIC =0,
        ADVANCE = 1

    }

    [SerializeField] MergeType _mergeType;
    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _renderer = GetComponent<Renderer>();
    } 

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Alpha9)) return;
        
        switch (_mergeType)
        {
            case MergeType.BASIC:
                BasicMerge();
                break;
            case MergeType.ADVANCE:
                AdvancedMerge();
                break;
            default:
                Debug.LogError("Unkown Merge type",this);
                break;
                
            
        }

    }


    private void BasicMerge()
    { 
        Mesh mesh = _meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            _meshFilter.sharedMesh = mesh;

        }
        else
        {
            mesh.Clear();
        }

        MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>(false);
        Debug.Log("Merging " + (filters.Length-1) + " meshes... " );

        List<CombineInstance> combiners = new List<CombineInstance>();

        foreach (MeshFilter filter in filters)
        {
            //dont combine ourself
            if (filter == _meshFilter)
            {
                continue;
            }
            
            CombineInstance ci = new CombineInstance();
            ci.mesh = filter.sharedMesh;
            ci.subMeshIndex = 0;
            ci.transform = filter.transform.localToWorldMatrix;
            combiners.Add(ci);
            filter.gameObject.SetActive(false);
        }
        mesh.CombineMeshes(combiners.ToArray(),true);
    }
    
    
    
    private void AdvancedMerge()
    { 

        // All children of target
        MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>(false);
        Debug.Log("Merging " + (filters.Length-1) + " meshes... " );
        //All the meshes in our children
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>(false);
        Debug.Log("founds " + (renderers.Length-1) + " renderers... " );
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)
            {
                continue;
            }

            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
            {
                if (materials.Contains(localMat) || localMat == null) continue;
                materials.Add(localMat);
                Debug.Log(materials);
            }
        }
        //get materials

        _renderer.materials = materials.ToArray();
        //each material will have a mesh for it
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            //make a combiners for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter filter in filters)
            {
                // The filter doesn't know what material are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    Debug.LogError(filter.name + "has no meshRenderer");
                    continue;
                }

                //Let's see if their material are the one we want right now
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                    {
                        continue;
                    }

                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex;
                    ci.transform = filter.transform.localToWorldMatrix;
                    combiners.Add(ci);
                }
            }
            //Flattern into a single mesh
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.CombineMeshes(combiners.ToArray(),true);
            Debug.Log(mesh);
            submeshes.Add(mesh);
        }
        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance ();
            if(UseWield){
                ci.mesh = AutoWeld(mesh,0.1f,0.01f);
            }
            else
            {
                ci.mesh = mesh;
            }


            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            
            finalCombiners.Add(ci);
        }
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes (finalCombiners.ToArray(), false);
        _meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _meshFilter.sharedMesh = finalMesh;
        Debug.Log ("Final mesh has " + submeshes.Count + " materials.");
        target.SetActive(false);
        AddColider();


        
        
    }
    
       public static Mesh AutoWeld (Mesh mesh, float threshold, float bucketStep) {
     Vector3[] oldVertices = mesh.vertices;
     Vector3[] newVertices = new Vector3[oldVertices.Length];
     int[] old2new = new int[oldVertices.Length];
     int newSize = 0;
 
     // Find AABB
     Vector3 min = new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
     Vector3 max = new Vector3 (float.MinValue, float.MinValue, float.MinValue);
     for (int i = 0; i < oldVertices.Length; i++) {
       if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
       if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
       if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
       if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
       if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
       if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
     }
 
     // Make cubic buckets, each with dimensions "bucketStep"
     int bucketSizeX = Mathf.FloorToInt ((max.x - min.x) / bucketStep) + 1;
     int bucketSizeY = Mathf.FloorToInt ((max.y - min.y) / bucketStep) + 1;
     int bucketSizeZ = Mathf.FloorToInt ((max.z - min.z) / bucketStep) + 1;
     List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];
 
     // Make new vertices
     for (int i = 0; i < oldVertices.Length; i++) {
       // Determine which bucket it belongs to
       int x = Mathf.FloorToInt ((oldVertices[i].x - min.x) / bucketStep);
       int y = Mathf.FloorToInt ((oldVertices[i].y - min.y) / bucketStep);
       int z = Mathf.FloorToInt ((oldVertices[i].z - min.z) / bucketStep);
 
       // Check to see if it's already been added
       if (buckets[x, y, z] == null)
         buckets[x, y, z] = new List<int> (); // Make buckets lazily
 
       for (int j = 0; j < buckets[x, y, z].Count; j++) {
         Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
         if (Vector3.SqrMagnitude (to) < threshold) {
           old2new[i] = buckets[x, y, z][j];
           goto skip; // Skip to next old vertex if this one is already there
         }
       }
 
       // Add new vertex
       newVertices[newSize] = oldVertices[i];
       buckets[x, y, z].Add (newSize);
       old2new[i] = newSize;
       newSize++;
 
     skip:;
     }
 
     // Make new triangles
     int[] oldTris = mesh.triangles;
     int[] newTris = new int[oldTris.Length];
     for (int i = 0; i < oldTris.Length; i++) {
       newTris[i] = old2new[oldTris[i]];
     }
     
     Vector3[] finalVertices = new Vector3[newSize];
     for (int i = 0; i < newSize; i++)
       finalVertices[i] = newVertices[i];
 
     mesh.Clear();
     mesh.vertices = finalVertices;
     mesh.triangles = newTris;
     mesh.RecalculateNormals ();
     mesh.Optimize ();
     return mesh;
       }

    private void AddColider()
    {
        if (!gameObject.GetComponent<MeshCollider>())
        gameObject.AddComponent<MeshCollider>();
    }
}
