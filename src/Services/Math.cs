namespace DEA.Services
{
    public static class Math
    {
        public static decimal CalculateIntrestRate(decimal wealth)
        {
            var InterestRate = 0.01m + ((wealth / 100) * .00004m);
            if (InterestRate > 0.05m) InterestRate = 0.05m;
            return InterestRate;
        }
    }
}
