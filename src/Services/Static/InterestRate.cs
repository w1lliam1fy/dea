namespace DEA.Services.Static
{
    internal static class InterestRate
    {
        public static decimal Calculate (decimal wealth)
        {
            var interestRate = 0.01m + ((wealth / 100) * .00008m);
            if (interestRate > 0.05m)
            {
                interestRate = 0.05m;
            }

            if (interestRate * wealth > 2500)
            {
                interestRate = 2500 / wealth;
            }

            return interestRate;
        }
    }
}
