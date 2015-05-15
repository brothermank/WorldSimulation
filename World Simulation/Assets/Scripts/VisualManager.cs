using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualManager : MonoBehaviour {

	WorldLayoutManager wlm = new WorldLayoutManager();
	public MeshFilter meshRendererPrefab;
	public Merchant merchantPrefab;
	List<MeshFilter> renderers = new List<MeshFilter>();

	// Use this for initialization
	void Awake () {
		wlm.cities = wlm.CreateCitiesRandomly (2,2,50,1000, 10);
		CreateVisualForAllCities ();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void CreateVisualForAllCities(){
		foreach (City city in wlm.cities) {
			Debug.Log("CreatingVisual");
			renderers.Add(CreateCityVisual(city));
		}
	}

	MeshFilter CreateCityVisual(City city){
		//City parameters
		Vector2 position = city.GetPosition ();
		float radius = Mathf.Sqrt (city.GetPopulation ()) / Mathf.PI / 10;

		//Needed variables
		Vector3[] vertices = new Vector3[3];
		Vector2[] UV = new Vector2[3];
		int[] triangle = new int[3];


		//Assign varaibles
		vertices[0] = new Vector3(0 - radius, 0, -radius);
		vertices[1] = new Vector3(0 + radius, 0, -radius);
		vertices[2] = new Vector3(0			, 0, radius);

		UV[0] = new Vector2(0,0);
		UV[1] = new Vector2(1,0);
		UV[2] = new Vector2(0.5f,1);

		triangle[0] = 0;
		triangle[1] = 2;
		triangle[2] = 1;


		//Create mesh
		Mesh newMesh = new Mesh();
		newMesh.vertices = vertices;
		newMesh.uv = UV;
		newMesh.triangles = triangle;

		//Create mesh renderer and return
		MeshFilter renderer = Instantiate(meshRendererPrefab) as MeshFilter;
		renderer.transform.position = new Vector3(position.x, 0, position.y);
		renderer.mesh = newMesh;

		return renderer;
	}
}
