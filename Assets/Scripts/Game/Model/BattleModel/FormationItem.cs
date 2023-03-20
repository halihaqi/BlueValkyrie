using System;

namespace Game.Model.BattleModel
{
    [Serializable]
    public class FormationItem
    {
        public int formationId;
        public string formationName;
        public StudentItem[] students;
        
        public FormationItem(){}

        public FormationItem(int formationId)
        {
            this.formationId = formationId;
            formationName = $"队伍{formationId}";
            students = new StudentItem[4];
        }

        public void SwitchStudent(int index1, int index2)
            => (students[index1], students[index2]) = (students[index2], students[index1]);
    }
}