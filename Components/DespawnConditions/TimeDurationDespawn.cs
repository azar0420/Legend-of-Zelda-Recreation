namespace amongus3902.Components.DespawnConditions
{
    internal class TimeDurationDespawn : IDespawnCondition
    {
        //milliseconds
        public int DespawnTime { get; set; }

        public int TimeElapsed { get; set; }

        public TimeDurationDespawn(int DespawnTime)
        {
            this.TimeElapsed = 0;
            this.DespawnTime = DespawnTime;
        }
    }
}
