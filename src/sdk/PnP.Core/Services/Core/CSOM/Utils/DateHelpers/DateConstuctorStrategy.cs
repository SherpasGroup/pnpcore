﻿using System;

namespace PnP.Core.Services.Core.CSOM.Utils.DateHelpers
{
    internal sealed class DateConstuctorStrategy : IDateConversionStrategy
    {
        public DateTime? ConverDate(string dateValue)
        {
            DateTime? result = null;
            string[] constructorValues = dateValue.Replace("/Date(", "").Replace(")/", "").Split(',');
            if (constructorValues.Length == 7)
            {
                if (constructorValues[0] == "1" || constructorValues[0] == "0")
                {
                    // For dates returned as "/Date(1,0,1,0,0,0,0)/"
                    return DateTime.MinValue;
                }

                result = new DateTime(
                    Convert.ToInt32(constructorValues[0]),
                    Convert.ToInt32(constructorValues[1]),
                    Convert.ToInt32(constructorValues[2]),
                    Convert.ToInt32(constructorValues[3]),
                    Convert.ToInt32(constructorValues[4]),
                    Convert.ToInt32(constructorValues[5]),
                    Convert.ToInt32(constructorValues[6]));
            }
            return result;
        }
    }
}
