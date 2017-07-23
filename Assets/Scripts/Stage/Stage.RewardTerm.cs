using System;

public abstract partial class Stage : Methods
{
    class RewardTerm
    {
        public Func<bool> term = () => true;
        public string explanation = "";
    }
}
