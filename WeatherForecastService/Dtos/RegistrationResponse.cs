﻿namespace WeatherForecastService.Dtos
{
    public class RegistrationResponse
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}