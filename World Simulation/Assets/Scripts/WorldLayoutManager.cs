using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldLayoutManager {

	public static WorldLayoutManager activeManager;
	public List<City> cities;
	List<City>[,] cityGrid;
	float gridSize;
	Vector2 lowerLeftCircleCentrum;
	float circleRadius;

	public WorldLayoutManager(){
		activeManager = this;
	}

	private struct Circle{
		public List<City> citiesInCircle;
		public List<City> northOfC;
		public List<City> eastOfC;

		public float radius;
		public Vector2 centrum;
		public Vector2 northPoint;
		public Vector2 eastPoint;

		public Circle(Vector2 circleCentrum, float circleRadius){
			centrum = circleCentrum;
			radius = circleRadius;
			northPoint = circleCentrum + new Vector2(0, circleRadius);
			eastPoint = circleCentrum + new Vector2(circleRadius, 0);

			citiesInCircle = new List<City>();
			northOfC = new List<City>();
			eastOfC = new List<City>();
		}
	}

	public List<City> CreateCitiesRandomly(int circlesx, int circlesy, int CitiesPerCircle, int maxPopulation, float gridSize){
		List<City> allCities = new List<City>();
		this.gridSize = gridSize;

		float circleRadius = City.MinimumDistanceFor (maxPopulation, maxPopulation);
		float radiusSqare = Mathf.Sqrt(circleRadius);
		Vector2 startPosition = new Vector2 (0, 0);
		this.circleRadius = circleRadius;
		lowerLeftCircleCentrum = startPosition;

		float areax = circleRadius * circlesx + circleRadius;
		float areay = circleRadius * circlesy + circleRadius;
		cityGrid = new List<City>[Mathf.CeilToInt(areax / gridSize),Mathf.CeilToInt(areay / gridSize)];
		for (int i = 0; i < cityGrid.GetLength(0); i++) {
			for(int j = 0; j < cityGrid.GetLength(1); j++){
				cityGrid[i,j] = new List<City>();
			}
		}
		List<Circle> circlesLastRow = null;
		for(int x = 0; x < circlesx; x++){

			List<Circle> circlesThisRow = new List<Circle>();
			for(int y = 0; y < circlesy; y++){

				//Establish circle
				Circle currentCircle = new Circle(startPosition + Vector2.up * x * circleRadius, circleRadius);

				for(int i = 0; i < CitiesPerCircle; i++){
					//Determine city position
					float randomDistance = circleRadius - (Random.Range(0f, radiusSqare) * Random.Range(0f, radiusSqare));
					float randomAngleDirection = Random.Range(0f, 2 * Mathf.PI);
					Vector2 direction = new Vector2(Mathf.Cos(randomAngleDirection), Mathf.Sin(randomAngleDirection));
					Vector2 position = currentCircle.centrum + direction * randomDistance;



					//Determine city population
					int highestPossiblePopulation = maxPopulation;
					foreach(City city in currentCircle.citiesInCircle){
						int limitedPopulation = City.MaxPopulationAt(city, position);
						if(limitedPopulation < highestPossiblePopulation) highestPossiblePopulation = limitedPopulation;
					}
					try{
						foreach(City city in circlesThisRow[y - 1].northOfC){
							int limitedPopulation = City.MaxPopulationAt(city, position);
							if(limitedPopulation < highestPossiblePopulation) highestPossiblePopulation = limitedPopulation;
						}
					}
					catch(System.ArgumentOutOfRangeException){}
					if(circlesLastRow != null){
						try{
							foreach(City city in circlesLastRow[y].eastOfC){
								int limitedPopulation = City.MaxPopulationAt(city, position);
								if(limitedPopulation < highestPossiblePopulation) highestPossiblePopulation = limitedPopulation;
							}
						}
						catch(System.ArgumentOutOfRangeException){}
					}
					int random1 = Random.Range(0, (int)(maxPopulation * 0.5));
					int random2 = Random.Range(0, (int)(maxPopulation * 0.5));
					highestPossiblePopulation = random1 + random2;

					//Create and add city
					City newCity = new City(position, highestPossiblePopulation);
					currentCircle.citiesInCircle.Add(newCity);
					allCities.Add(newCity);
					PositionToCityGrid(newCity.GetPosition()).Add(newCity);

					if(IsWithinCircle(position, currentCircle.northPoint, circleRadius)){
						currentCircle.northOfC.Add(newCity);
					}
					if(IsWithinCircle(position, currentCircle.eastPoint, circleRadius)){
						currentCircle.eastOfC.Add(newCity);
					}
					try{
						if(IsWithinCircle(position, circlesThisRow[y - 1].eastPoint, circleRadius)){
							circlesThisRow[y - 1].eastOfC.Add(newCity);
						}
					}
					catch(System.ArgumentOutOfRangeException){}
				}
			}
			circlesLastRow = circlesThisRow;
		}
		cities = allCities;
		return allCities;
	}

	public List<City> PositionToCityGrid(Vector2 position){
		Vector2 lowerLeftPosition = new Vector2(lowerLeftCircleCentrum.x - circleRadius, lowerLeftCircleCentrum.y - circleRadius);
		float dx = position.x - lowerLeftPosition.x;
		float dy = position.y - lowerLeftPosition.y;
		int posx = (int)(dx / gridSize);
		int posy = (int)(dy / gridSize);
		return cityGrid [posx, posy];
	}

	public static bool IsWithinCircle(Vector2 point, Vector2 circleCentrum, float circleRadius){
		return (point - circleCentrum).magnitude < circleRadius;
	}

	public List<List<City>> GetCitiesInRadius(Vector2 center, float radius){
		List<List<City>> result = new List<List<City>> ();

		Vector2 lowerLeftPosition = new Vector2(lowerLeftCircleCentrum.x - circleRadius, lowerLeftCircleCentrum.y - circleRadius);
		float dx = center.x - lowerLeftPosition.x;
		float dy = center.y - lowerLeftPosition.y;
		dx %= gridSize;
		dy %= gridSize;
		float dxInverse = gridSize - dx;
		float dyInverse = gridSize - dy;
		int placesN = Mathf.CeilToInt((radius - dyInverse) / gridSize);
		int placesE = Mathf.CeilToInt((radius - dxInverse) / gridSize);
		int placesS = Mathf.CeilToInt((radius - dy)		   / gridSize);
		int placesW = Mathf.CeilToInt((radius - dx) 	   / gridSize);
		int initialX = (int)(dx / gridSize) - placesW;
		int initialY = (int)(dy / gridSize) - placesS;
		int rangex = placesE + placesW + 1;
		int rangey = placesS + placesN + 1;
		int destinationx = initialX + rangex;
		int destinationy = initialY + rangey;
		if (initialX < 0) {
			rangex += initialX;
			initialX = 0;
		}
		if (initialY < 0){
			rangey += initialY;
			initialY = 0;
		}
		if (destinationx > cityGrid.GetLength (0))
			destinationx = cityGrid.GetLength (0);
		if (destinationy > cityGrid.GetLength (1))
			destinationy = cityGrid.GetLength (1);



		for (int x = initialX; x < destinationx; x++) {
			for(int y = initialY; y < destinationy; y++){
				result.Add(cityGrid[x,y]);
			}
  		}

		return result;
	}
}
