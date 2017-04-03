namespace DEA
{
    public class Program
    {
        public static void Main(string[] args)
            => new DEABot().RunAndBlockAsync(args).GetAwaiter().GetResult();
    }
}