namespace amongus3902.Components.DespawnConditions
{
    internal class OnReturnDespawn : IDespawnCondition
    {
        public bool HasLeft { get; set; } = false;

        public bool Returning { get; set; } = false;
    }
}
