using UnityEngine;
using System.Collections;

public class City {

	private static float populationAdvancedRangeInfluenceLimiter = 0.9f;
	private static float inversePopulationAdvancedRangeInfluenceLimiter = 1 / populationAdvancedRangeInfluenceLimiter;
	private static float populationBaseRangeInfluenceLimiter = 10;
	private static float poweredRangeLimiter = Mathf.Pow (populationBaseRangeInfluenceLimiter, populationAdvancedRangeInfluenceLimiter) * 2;

	private Vector2 center;

	private int population = 1;
	public float neighborRangeParameter;



	//City stats
	public float gold;
	float[] goodsInStash = new float[GoodsSystem.types]{100, 100, 100};
	float[] goodsBuyPrices = new float[GoodsSystem.types]{1,1.3f, 0.8f};
	float[] goodsSellPrices = new float[GoodsSystem.types]{1,0.7f, 1.2f};
	float[] goodsProductionSpeed = new float[GoodsSystem.types]{1,1,1};
	float[] goodsUsageSpeed = new float[GoodsSystem.types]{0.9f,0.9f,0.9f};


	public City(Vector2 worldPosition){
		SetPopulation (1);
		center = worldPosition;
		gold = 0;
	}
	public City(Vector2 worldPosition, int initalPopulation){
		SetPopulation (initalPopulation);
		center = worldPosition;
		gold = initalPopulation;
	}


	//################################################ Trading 'n stuff ####################################################
	public void Tick(){
		for (int i = 0; i < GoodsSystem.types; i++) {
			goodsInStash[i] += goodsProductionSpeed[i] - goodsUsageSpeed[i];
		}
	}

	public float GetGoodsInStash(GoodsSystem.GoodsTypes type){
		return goodsInStash [(int)type];
	}
	public float GetGoodsBuyPrice (GoodsSystem.GoodsTypes type){
		return goodsBuyPrices[(int)type];
	}
	public float GetGoodsSellPrice (GoodsSystem.GoodsTypes type){
		return goodsSellPrices[(int)type];
	}
	public void AddGoods(GoodsSystem.GoodsTypes type, float amount){
		goodsInStash [(int)type] += amount;
	}



	//#################################################  Logistics #########################################################
	public void SetPopulation(int newPopulation){
		population = newPopulation;
		neighborRangeParameter = Mathf.Pow(population / populationBaseRangeInfluenceLimiter, populationAdvancedRangeInfluenceLimiter);
	}
	public int GetPopulation(){
		return population;
	}
	public Vector2 GetPosition(){
		return center;
	}

	public static bool ViolatesRangeLimits(City city1, City city2){
		float distance = (city1.GetPosition () - city2.GetPosition ()).magnitude;
		float rangeParameter = (city1.neighborRangeParameter + city2.neighborRangeParameter) * 0.5f;
		return distance > rangeParameter;
	}

	public static int MaxPopulationAt(City limitingCity, Vector2 positionToCheck){
		float distance = (limitingCity.GetPosition () - positionToCheck).magnitude;
		return (int)Mathf.Pow (distance * poweredRangeLimiter, inversePopulationAdvancedRangeInfluenceLimiter);
	}

	/// <summary>
	/// Finds the minimum distance between two populations.
	/// </summary>
	/// <returns>The minimum distance between two populations.</returns>
	/// <param name="city1">City containing population 1.</param>
	/// <param name="city2">City containing population 2.</param>
	public static float MinimumDistanceFor(City city1, City city2){
		return (city1.neighborRangeParameter + city2.neighborRangeParameter) * 0.5f;
	}
	/// <summary>
	/// Finds the minimum distance between two populations.
	/// </summary>
	/// <returns>The minimum distance between two populations.</returns>
	/// <param name="city1">City containing population 1.</param>
	/// <param name="comparedPopulation">Size of population 2.</param>
	public static float MinimumDistanceFor(City city, int comparedPopulation){
		float neighborRangeParameter = Mathf.Pow(comparedPopulation / populationBaseRangeInfluenceLimiter, populationAdvancedRangeInfluenceLimiter);
		return (city.neighborRangeParameter + neighborRangeParameter) * 0.5f;
	}
	/// <summary>
	/// Finds the minimum distance between two populations.
	/// </summary>
	/// <returns>The minimum distance between two populations.</returns>
	/// <param name="comparedPopulation1">Size of population 1.</param>
	/// <param name="comparedPopulation2">Size of population 2.</param>
	public static float MinimumDistanceFor(int comparedPopulation1, int comparedPopulation2){
		float neighborRangeParameter1 = Mathf.Pow(comparedPopulation1 / populationBaseRangeInfluenceLimiter, populationAdvancedRangeInfluenceLimiter);
		float neighborRangeParameter2 = Mathf.Pow(comparedPopulation2 / populationBaseRangeInfluenceLimiter, populationAdvancedRangeInfluenceLimiter);
		return (neighborRangeParameter1 + neighborRangeParameter2) * 0.5f;
	}

}
