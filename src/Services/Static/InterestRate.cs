namespace DEA.Services.Static
{
    public static class InterestRate
    {
        public static decimal Calculate (decimal wealth)
        {
            var InterestRate = 0.01m + ((wealth / 100) * .00008m);
            if (InterestRate > 0.05m) InterestRate = 0.05m;
            if (InterestRate * wealth > 2500) InterestRate = 2500 / wealth;
            return InterestRate;
        }
    }
}
