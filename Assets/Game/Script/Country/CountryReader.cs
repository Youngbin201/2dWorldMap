using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CountryReader
{
	JsonTextReader reader;
	List<Dictionary<string, object>> countryCenterList;
	List<Sprite> countryFlags;

	public Country[] ReadCountries(TextAsset countryFile, TextAsset countryCenterFile, List<string> excludedCountryCodes , List<Sprite> countryFlags)
	{

		reader = new JsonTextReader(new System.IO.StringReader(countryFile.text));

		List<Country> countryList = new List<Country>();

		countryCenterList = CSVReader.Read(countryCenterFile);

		this.countryFlags = countryFlags;


		ReadAhead(5);

		int ii = 0;

		while (reader.Read())
		{
			ii++;

			if (reader.TokenType == JsonToken.StartObject)
			{
				Country country = ReadCountry();
				country.countryIndex = ii;

				bool isexcludedCountry = false;

				for (int i = 0; i < excludedCountryCodes.Count; i++)
				{
					if (country.alpha2Code == excludedCountryCodes[i] || country.name == excludedCountryCodes[i])
					{
						isexcludedCountry = true;
					}
				}

				if (!isexcludedCountry)
					countryList.Add(country);
			}
			else
			{
				break;
			}

		}

		reader.Close();

		//countryList.Sort((a, b) => (a.name.CompareTo(b.name)));
		return countryList.ToArray();
	}

	Country ReadCountry()
	{
		Country country = new Country();

		int startDepth = reader.Depth;

		// Read country name and code from properties array
		while (reader.TokenType != JsonToken.EndArray)
		{
			reader.Read();
			if (reader.TokenType == JsonToken.PropertyName)
			{
				switch ((string)reader.Value)
				{
					case "ADMIN":
						country.nameOfficial = reader.ReadAsString();
						break;
					case "NAME":
						country.name = reader.ReadAsString();
						break;
					case "NAME_LONG":
						country.name_long = reader.ReadAsString();
						break;
					case "NAME_SORT":
						country.name_sort = reader.ReadAsString();
						break;
					case "2_LETTER_CODE":
						country.alpha2Code = reader.ReadAsString();
						break;
					case "ADM0_A3":
						country.alpha3Code = reader.ReadAsString();
						break;
					case "ABBREV":
						country.abbreviation = reader.ReadAsString();
						break;
					case "CONTINENT":
						country.continent = reader.ReadAsString();
						break;
					case "POP_EST":
						country.population = (int)reader.ReadAsDouble();
						break;
				}
			}
		}

		List<Polygon> polygons = new List<Polygon>();
		List<Path> pathsInCurrentPolygon = new List<Path>();
		List<Coordinate> coordList = new List<Coordinate>();

		// Read shape data from geometry array
		while (reader.Read() && reader.Depth > startDepth)
		{
			if (reader.TokenType == JsonToken.Float)
			{
				double x = (double)reader.Value;
				double y = (double)reader.ReadAsDouble();
				Coordinate coord = new Coordinate((float)x * Mathf.Deg2Rad, (float)y * Mathf.Deg2Rad);
				coordList.Add(coord);
				ReadAhead(2);
			}

			if (reader.TokenType == JsonToken.EndArray)
			{
				//	Debug.Log("Finished path " + pointsList[0] + "  -> " + pointsList[pointsList.Count - 1]);
				coordList.Add(coordList[0]); // duplicate start point at end for conveniece in some other code
				Path path = new Path() { points = coordList.ToArray() };
				pathsInCurrentPolygon.Add(path);

				coordList.Clear();
				ReadAhead(1);
			}
			if (reader.TokenType == JsonToken.EndArray)
			{
				//Debug.Log("Finished polygon (" + pathsInCurrentPolygon.Count + " paths)");
				Polygon polygon = new Polygon(pathsInCurrentPolygon.ToArray());
				polygons.Add(polygon);

				pathsInCurrentPolygon.Clear();
				ReadAhead(1);
			}
		}

		country.shape = new Shape() { polygons = polygons.ToArray() };

		//동서남북 극점

		float ea = polygons[0].paths[0].points[0].longitude;
		float we = polygons[0].paths[0].points[0].longitude;
		float so = polygons[0].paths[0].points[0].latitude;
		float no = polygons[0].paths[0].points[0].latitude;

		for (int j = 0; j < polygons.Count; j++)
		{
			for (int i = 0; i < polygons[j].paths.Length; i++)
			{
				for (int ii = 0; ii < polygons[j].paths[i].points.Length; ii++)
				{
					if (polygons[j].paths[i].points[ii].longitude > ea)
						ea = polygons[j].paths[i].points[ii].longitude;

					if (polygons[j].paths[i].points[ii].longitude < we)
						we = polygons[j].paths[i].points[ii].longitude;

					if (polygons[j].paths[i].points[ii].latitude < so)
						so = polygons[j].paths[i].points[ii].latitude;

					if (polygons[j].paths[i].points[ii].latitude > no)
						no = polygons[j].paths[i].points[ii].latitude;
				}
			}
		}


		country.easternmost = ea;
		country.westernmost = we;
		country.southernmost = so;
		country.northernmost = no;

		Coordinate center = new Coordinate(999, 999);

		//중심
		for (int i = 0; i < countryCenterList.Count; i++)
		{
			if (countryCenterList[i]["ISO"].ToString() == country.alpha2Code)
			{
				string longitude = countryCenterList[i]["longitude"].ToString();
				string latitude = countryCenterList[i]["latitude"].ToString();
				center = new Coordinate(float.Parse(longitude), float.Parse(latitude));
				break;
			}
		}

		if (center.latitude == 999)
		{
			//Debug.Log($"{country.name}Missing Center");

			/* for (int i = 0; i < countryCenterList.Count; i++)
			{
				if (countryCenterList[i]["COUNTRY"].ToString() == country.name || countryCenterList[i]["COUNTRYAFF"].ToString() == country.name)
				{
					string longitude = countryCenterList[i]["longitude"].ToString();
					string latitude = countryCenterList[i]["latitude"].ToString();
					center = new Coordinate(float.Parse(longitude), float.Parse(latitude));
					country.alpha2Code = countryCenterList[i]["ISO"].ToString();
					break;
				}
			} */
		}


		country.center = center;

		//국기
		for (int i = 0; i < countryFlags.Count; i++)
		{
			if(countryFlags[i].name.ToUpper() == country.alpha2Code)
			{
				country.flag = countryFlags[i];
			}
		}

		return country;
	}

	void ReadAhead(int n)
	{
		for (int i = 0; i < n; i++)
		{
			reader.Read();
		}
	}

}
