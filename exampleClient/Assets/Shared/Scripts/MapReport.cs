using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MapReport
{
    public int collisions = 0;
    public float traveled_kilometers = 0f;
    public float burned_calories = 0f;
    public float totalGameTime = 0f;
    public Medal medal;

    public MapReport() { }

    public MapReport(int collisions, float traveled_kilometers, 
        float burned_calories, float totalGameTime, Medal medal) {
        this.collisions = collisions;
        this.traveled_kilometers = traveled_kilometers;
        this.burned_calories = burned_calories;
        this.totalGameTime = totalGameTime;
        this.medal = medal;
    }
}
