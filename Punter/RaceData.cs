using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter
{
    public class RaceData
    {
        public RaceData(string trackCondition, string[] dataLines, StringBuilder jockeyDescriptions,
            StringBuilder trainerDescriptions, StringBuilder horseDescriptions)
        {
            TrackCondition = trackCondition;
            DataLines = dataLines;
            JockeyDescriptions = jockeyDescriptions;
            TrainerDescriptions = trainerDescriptions;
            HorseDescriptions = horseDescriptions;
        }

        public string TrackCondition { get; set; }
        public string[] DataLines { get; set; }
        public StringBuilder JockeyDescriptions { get; set; }
        public StringBuilder TrainerDescriptions { get; set; }
        public StringBuilder HorseDescriptions { get; set; }


    }
}
