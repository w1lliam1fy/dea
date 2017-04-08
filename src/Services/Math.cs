namespace DEA.Services
{
    public static class Math
    {
        public static decimal CalculateIntrestRate(decimal wealth)
        {
            var InterestRate = 0.01m + ((wealth / 100) * .000015m);
            if (InterestRate > 0.025m) InterestRate = 0.025m;
            return InterestRate;
        }
    }
}
