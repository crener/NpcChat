namespace NpcChatSystem.Branching.Conditions
{
    public class StartCondition : AbstractCondition
    {
        public bool DefaultNode { get; set; }

        public override bool Evaluate()
        {
            return true;
        }
    }
}
