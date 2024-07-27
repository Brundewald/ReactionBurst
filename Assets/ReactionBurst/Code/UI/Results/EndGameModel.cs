using System.Collections.Generic;
using MyProject.ReactionBurst.UI;

namespace MyProject.ReactionBurst.UI.Results
{
    public class EndGameModel : BaseModel
    {
        public int CurrentScore;
        public int OldScore;
        public int BestScore;
        public int ScoreType;
        public int Percent;

        public int MaximumScore;
        public int StarMarkScore;
        
        public List<CharacteristicVisualData> CharacteristicsVisual;
        public List<CharacteristicData> NewCharacteristics;
        public List<CharacteristicData> PreviousCharacteristics;
    }
}