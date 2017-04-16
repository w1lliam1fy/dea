namespace DEA.Services
{
    public static class Math
    {
        public static decimal CalculateIntrestRate(decimal wealth)
        {
            var InterestRate = 0.01m + ((wealth / 100) * .00008m);
            if (InterestRate * wealth > 2500) InterestRate = 2500 / wealth;
            return InterestRate;
        }
    }
}
