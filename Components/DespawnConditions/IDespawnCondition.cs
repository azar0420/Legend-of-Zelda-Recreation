namespace amongus3902.Components.DespawnConditions
{
    //here for typechecking
    internal abstract class IDespawnCondition {

        public IDespawnCondition Duplicate()
        {
            return (IDespawnCondition)this.MemberwiseClone();
        }
    }
}
