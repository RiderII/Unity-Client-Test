using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float CaloriesBurned(float weight, float meterPerHour)
    {
        float MET = CalculateMET(meterPerHour);
        return ((MET*weight* 3.5f) / 200f)*0.0006f;
    }

    private static float CalculateMET(float meterPerHour)
    {
        float MET = 0;
        switch (meterPerHour)
        {
            case float n when (n >= 20):
                MET = Constants.vigorousRacingMET;
                break;

            case float n when (n < 20 && n >= 16):
                MET = Constants.racingMET;
                break;

            case float n when (n < 16 && n >= 14):
                MET = Constants.leisureVigorousEffortMET;
                break;

            case float n when (n < 14 && n >= 12):
                MET = Constants.leisureModerateEffortMET;
                break;

            case float n when (n < 12 && n >= 10):
                MET = Constants.leisureLightToModarateEffortMET;
                break;

            case float n when (n < 10 && n >= 9):
                MET = Constants.leisureLightEffortMET;
                break;

            case float n when (n < 9 && n > 5.5):
                MET = Constants.leisureSlightLightEffortMET;
                break;
            case float n when (n == 5.5):
                MET = Constants.leisureVeryLightEffortMET;
                break;
            default:
                MET = (meterPerHour * 3.5f) / 5.5f;
                break;
        }

        return MET;
    }

    public static int CalculatePoints(float distanceTimer)
    {
        int points = 0;
        switch (distanceTimer)
        {
            case var _ when distanceTimer <= 3: points += 250; break;
            case var _ when distanceTimer <= 5: points += 200; break;
            case var _ when distanceTimer <= 7: points += 150; break;
            case var _ when distanceTimer <= 10: points += 100; break;
        }

        return points;
    }
}

