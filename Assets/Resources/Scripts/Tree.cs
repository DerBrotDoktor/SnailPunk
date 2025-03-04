namespace Resources
{
    public class Tree : Resource
    {
        
        public override int Harvest(int amount)
        {
            return base.Harvest(maxAmount);
        }
    }
}
