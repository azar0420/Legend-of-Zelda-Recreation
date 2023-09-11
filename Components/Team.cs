namespace amongus3902.Components
{
    enum TeamType
    {
        Friend,
        Foe,
        Neutral,
    }

    internal class Team : IComponent
    {
        public readonly TeamType AlliedTo;

        public Team(TeamType alliedTo)
        {
            AlliedTo = alliedTo;
        }
    }
}
