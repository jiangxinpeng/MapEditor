using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LitJson;

namespace ArrowLegend.MapEditor
{

    public class MapEditorWeatherHandle : BaseHandle, IHandle
    {
        private LevelCorrespondWeather levelCorrespondWeather;

        public WeatherType weatherType = WeatherType.Sunny;

        public bool isDay = true;

        public void AfterChangeLevel()
        {
            BindWeatherInfo();
            InitWeatherData();
        }

        public void ChangeLevel(int level)
        {
         
        }

        public void CreateNewLevel()
        {
          
            BindWeatherInfo();
            InitWeatherData();
        }

        public void Destory()
        {
           
        }

        public void Init()
        {
            BindWeatherInfo();
            InitWeatherData();
        }

        //绑定天气信息
        private void BindWeatherInfo()
        {
            levelCorrespondWeather = MapGeneratorEditor.levelInfo.weatherInfo;
            if (levelCorrespondWeather==null)
            {
                levelCorrespondWeather = new LevelCorrespondWeather();
                MapGeneratorEditor.levelInfo.weatherInfo = levelCorrespondWeather;
            }
        }

        //初始化天气信息
        private void InitWeatherData()
        {
            weatherType = levelCorrespondWeather.weather;
            isDay = levelCorrespondWeather.IsDay;
        }

        ///// <summary>
        ///// 保存天气信息
        ///// </summary>
        //private void SaveWeatherData()
        //{
        //    levelCorrespondWeather.weather = weatherType;
        //    levelCorrespondWeather.IsDay = isDay;

        //    Debug.Log("保存天气信息  " + MapGeneratorEditor.levelInfo.levelId +"  "+ JsonMapper.ToJson(levelCorrespondWeather));

        //}

        public void SaveDay()
        {
            levelCorrespondWeather.IsDay = isDay;
        }

        public void SaveWeather()
        {
            levelCorrespondWeather.weather = weatherType;
        }
    }
}
