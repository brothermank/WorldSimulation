using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Merchant : MonoBehaviour {

	public float transactionCapacity = 10;
	float[] goodsInInventory = new float[GoodsSystem.types]{0,0,0};

	float travelRange = 10;
	float travelSpeed = 1;
	float travelStartTime;
	Vector3 travelStartPos;
	Vector3 travelEndPos;
	float currentJourneyLength = 0;
	bool traveling = false;
	City targetCity;

	// Use this for initialization
	void Start () {
		GoToRandomCity ();
	}
	
	// Update is called once per frame
	void Update () {
		if (traveling) {
			float distanceCovered = (Time.time - travelStartTime) * travelSpeed;
			float fracJournComplete = distanceCovered / currentJourneyLength;
			transform.position = Vector3.Lerp(travelStartPos, travelEndPos, fracJournComplete);
			if(fracJournComplete == 1	){
				traveling = false;
				GoodsSystem.PerformTransaction(targetCity, this);
				GoToRandomCity();
			}
		}
	}

	void GoToRandomCity(){
		List<List<City>> possibleCities = WorldLayoutManager.activeManager.GetCitiesInRadius (new Vector2 (transform.position.x, transform.position.z), travelRange);
		int i = 0;
		do{
			i = Random.Range (0, possibleCities.Count - 1);
			Debug.Log (possibleCities [i].Count);
		} while(possadsibleCities[i].Count == 0);
		int j = Random.Range (0, possibleCities [i].Count - 1);
		targetCity = possibleCities [i] [j];
		

		travelStartPos = transform.position;
		travelEndPos = new Vector3(targetCity.GetPosition().x, 0, targetCity.GetPosition().y);
		
		travelStartTime = Time.time;
		currentJourneyLength = (travelStartPos - travelEndPos).magnitude;
		traveling = true;

	}

	public float GetAmountOfGood(GoodsSystem.GoodsTypes type){
		return goodsInInventory[(int)type];
	}
	public void AddToStash(GoodsSystem.GoodsTypes type, float amount){
		goodsInInventory [(int)type] += amount;
	}


}
