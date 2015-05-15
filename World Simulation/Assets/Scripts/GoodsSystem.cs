using UnityEngine;
using System.Collections;

public abstract class GoodsSystem{

	public enum GoodsTypes{metal, lumber, stone};
	public const int types = 3;
	

	public static void PerformTransaction(City city, Merchant merchant){
		GoodsTypes[] prefereableGoods = GetBestBuySellGoods (city);
		float sellGoodAmountInStash = merchant.GetAmountOfGood (prefereableGoods [0]);
		if(sellGoodAmountInStash > 0){
			//Merchant will sell this good
			float buyPrice = city.GetGoodsBuyPrice(prefereableGoods[0]);
			float totalPrice = city.gold;
			float amountToBuy = city.gold / buyPrice;
			if(amountToBuy > sellGoodAmountInStash){
				amountToBuy = sellGoodAmountInStash;
				totalPrice = amountToBuy * buyPrice;
			}
			merchant.AddToStash(prefereableGoods[0], -amountToBuy);
			city.gold -= totalPrice;
			city.AddGoods(prefereableGoods[0], amountToBuy);
		}
		float buyGoodAmountInCity = city.GetGoodsInStash (prefereableGoods [1]);
		if (buyGoodAmountInCity > 0) {
			//Merchant will buy this good
			float buyPrice = city.GetGoodsSellPrice(prefereableGoods[1]);
			float buyAmount = merchant.transactionCapacity;
			if(buyAmount > buyGoodAmountInCity){
				buyAmount = buyGoodAmountInCity;
			}
			city.gold += buyAmount * buyPrice;
			city.AddGoods(prefereableGoods[1], -buyAmount);
			merchant.AddToStash(prefereableGoods[1], buyAmount);
		}
	}

	private static GoodsSystem.GoodsTypes[] GetBestBuySellGoods(City targetCity){
		GoodsSystem.GoodsTypes[] results = new GoodsSystem.GoodsTypes[2];
		
		results[0] = GoodsSystem.GoodsTypes.lumber;
		results[1] = GoodsSystem.GoodsTypes.lumber;
		float bestBuyRatio = 0;
		float bestSellRatio = Mathf.Infinity;
		for(int i = 0; i < GoodsSystem.types; i++){
			GoodsSystem.GoodsTypes examinedType = (GoodsSystem.GoodsTypes) i;
			float buyPrice = targetCity.GetGoodsBuyPrice(examinedType);
			float sellPrice = targetCity.GetGoodsSellPrice(examinedType);
			if(buyPrice > bestBuyRatio){
				bestBuyRatio = buyPrice;
				results[0] = examinedType;
			}
			if(sellPrice < bestSellRatio){
				bestSellRatio = sellPrice;
				results[1] = examinedType;
			}
		}
		return results;
	}
}
